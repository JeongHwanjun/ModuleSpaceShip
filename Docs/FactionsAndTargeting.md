# 세력과 AI 목표 선정

## 역할

- `FactionRelationTable`: Inspector에서 편집하는 세력 관계 표.
- `ShipManager`: 함선 등록 시 양쪽 Ship의 관계 초기화, 제거 시 관계 정리.
- `Ship`: 소속 세력과 `Dictionary<Ship, RelationType> relations` 저장. 기존 조종 명령 실행.
- `ComputerShipController`: 관계별 목록 갱신, 적대 표적 선택. 이동/조준/공격 판단은 아직 없음.

## 초기 설정

`Assets/Settings/FactionRelations.asset`의 각 행은 양방향 관계다.

| 세력 | Independent | Player | Enemy |
| --- | --- | --- | --- |
| Independent | Friendly | Neutral | Neutral |
| Player | Neutral | Friendly | Hostile |
| Enemy | Neutral | Hostile | Friendly |

동일한 쌍을 여러 번 입력하면 마지막 행을 사용한다. 누락된 쌍은 같은 세력끼리 Friendly,
다른 세력끼리 Neutral이다. MainScene의 ShipManager가 이 에셋을 참조한다.
PlayerShip 프리팹은 Player, ComputerShip 프리팹은 Enemy 세력이다.
새 세력은 `FactionId`에 기존 숫자를 보존하며 항목을 추가하고 표에 관계를 지정한다.

소속 세력은 생성 전에 프리팹/Inspector에서 설정한다. 런타임 세력 변경과 외교 변경 전파는
이번 범위에 포함하지 않는다. 테이블은 새 함선과 기존 함선 사이의 관계만 초기화하며,
새 함선 등록이 이미 존재하는 두 함선의 개별 관계를 덮어쓰지는 않는다.

## 개별 관계

```csharp
RelationType relation = myShip.GetRelation(otherShip);
myShip.SetRelation(otherShip, RelationType.Hostile);
```

GetRelation은 기록이 없으면 Neutral을 반환한다. SetRelation은 등록된 살아 있는 상대에게만
적용되며 자기 자신은 제외한다. 이 변경은 myShip에서 otherShip을 보는 관계만 바꾼다.
상호 변경이 필요하면 `otherShip.SetRelation(myShip, ...)`도 호출한다.
`Relations`는 전체 사전의 읽기 전용 뷰이며, 변경 알림은 `RelationsChanged`이다.

## 표적 갱신

관계가 변경되면 Controller는 다음 LateUpdate에서 우호/중립/적대 목록을 다시 만든다.
자신, 파괴된 함선, Manager에 없는 함선은 제외한다.
현재 적대 표적이 유효하면 유지하고, 없으면 실행 중 ShipId가 가장 작은 적을 선택한다.
ShipId는 저장용 ID가 아니므로 실행 간 선택 순서를 보장하지 않는다.

- `CurrentTarget`: 선정한 표적. 관계 변경 직후에는 LateUpdate까지 이전 참조가 남을 수 있다.
- `HasTarget`: 현재 참조의 유효성까지 확인하므로, 후속 AI 행동의 실행 조건으로 사용한다.
- `RefreshTarget()`: 즉시 목록과 표적 갱신이 필요한 호출부에서 사용한다.
- 적이 없거나 Controller가 비활성화되면 이동/회전/사격 명령을 해제한다.
  Rigidbody2D의 속도는 변경하지 않으므로 관성에 의한 이동은 계속될 수 있다.

## Unity에서 확인

1. 디스크 변경을 다시 불러온 뒤 MainScene에 ShipManager가 하나만 있고 관계 에셋이 연결됐는지 확인한다.
2. Bootstrap 씬에서 Play한다. 이 프로젝트의 Def 로딩은 Bootstrap에서 실행된다.
3. MainScene의 ShipBuilder 컴포넌트 메뉴에서 `Deploy new 'ComputerShip'`을 실행한다.
4. 생성된 ComputerShip의 Controller에서 Hostile Ships에 플레이어가 있고 Current Target이 플레이어인지 확인한다.
5. 컴퓨터 함선을 하나 더 생성한다. 컴퓨터끼리는 Friendly Ships에 들어가야 한다.
6. 표적 플레이어 오브젝트를 삭제하면 다음 프레임에 Current Target이 None이 되어야 한다.
   ShipBuilder 메뉴에서 플레이어 설계도를 다시 배치하면 새 함선이 관계에 반영되는지도 확인한다.
   (현재 ShipBuilder는 두 번째 플레이어 설계도를 ComputerShip 프리팹으로 생성한다.)
7. 중립 확인은 Play 전에 테스트용 ComputerShip 프리팹 사본의 Faction을 Independent로 지정하고 생성한다.

## 자동 검사

Window > General > Test Runner의 EditMode에서 `ShipRelationsTests`를 실행한다.
세력 조합 9개와 함선 생명주기 시나리오를 검사한다. 생명주기 검사는 테스트용 빈 씬에서
Play Mode에 진입해 등록, 개별 관계 변경, 전투 파괴, 일반 Destroy, 표적 재선정,
Controller 재활성화, Manager 종료와 명령 해제를 검증한다.

### 확인 결과 (2026-09-16)

Unity 6000.3.4f1의 임시 프로젝트 사본에서 위 테스트 10개 모두 통과했다.
런타임 및 Editor 스크립트의 Unity 컴파일도 통과했다.
실제 전투 씬에서 모듈을 조작하는 수동 플레이 검사는 별도로 수행해야 한다.
