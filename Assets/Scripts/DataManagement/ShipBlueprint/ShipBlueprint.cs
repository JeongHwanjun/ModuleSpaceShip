using System.Collections.Generic;
using ModuleSpaceShip.Defs;

public class ShipBlueprint // 함선 전체의 청사진
{
    public string id; // 각 청사진을 구분
    public string shipName; // 함선 이름
    public List<ModuleBlueprint> moduleBlueprints; // 함선을 구성하는 모듈의 청사진
}

public class ModuleBlueprint // 모듈의 청사진
{
    public string defName; // 이 모듈이 참조하는 defName
    public string prefabPath; // 이 모듈을 생성할 때 참조할 prefab
    public GridPos position; // 이 모듈의 위치
}