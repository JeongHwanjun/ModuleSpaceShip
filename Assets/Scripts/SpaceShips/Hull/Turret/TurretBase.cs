using ModuleSpaceShip.Runtime;
using ModuleSpaceShip.Defs;
using UnityEngine;

namespace ModuleSpaceShip.Runtime
{
    public abstract class TurretBase : Module
    {
        protected TurretThingBase turretThing => (TurretThingBase)moduleThing;

        // 표신. 포신의 방향에 따른 시각적 효과와 발사 실행시 방향 지정
        [SerializeField] protected GameObject Gun;
        private bool firing = false;
        protected bool isRotationComplete = false;
        protected override void Awake()
        {
            base.Awake();
        }
        protected override void Start()
        {
            base.Start();
        }

        protected virtual void Update()
        {
            if(Gun != null && turretThing.state == ModuleThing.State.Connected) RotateGunTowardMouse();
            if(firing) TryFire();
        }

        public override void OnAttached(Transform parent, Vector3 position)
        {
            base.OnAttached(parent, position);
            // 이 시점에 ship은 지정된 상태
            ship.OnTryFireStart += StartFire;
            ship.OnTryFireStop += StopFire;
        }

        public override void OnDetached(bool isChained)
        {
            ship.OnTryFireStart -= StartFire;
            ship.OnTryFireStop -= StopFire;
            base.OnDetached(isChained);
            // 이 시점에 ship은 null
        }

        protected void StartFire()
        {
            // turretBase단의 조건은 이곳에서 검사
            // 사격 트리거 ON
            Debug.Log($"[TurretBase] Start Fire");
            firing = true;
        }

        protected void StopFire()
        {
            // 사격 트리거 OFF
            Debug.Log($"[TurretBase] Stop Fire");
            firing = false;
        }

        // 발사 함수, 질량병기와 에너지병기에서 각각 구현
        protected abstract void TryFire();

        // 지정된 각도로 포구 회전
        private void RotateGunTowardMouse()
        {
            Vector2 mousePos = inputManager.mousePos;
            Vector2 desiredDirection = mousePos - (Vector2)Gun.transform.position;
            float desiredAngle = Mathf.Atan2(desiredDirection.y, desiredDirection.x) * Mathf.Rad2Deg - 90f;
            float currentAngle = Gun.transform.eulerAngles.z;

            float newAngle = Mathf.MoveTowardsAngle(currentAngle, desiredAngle, turretThing.TurretDefBase.rotationSpeed * Time.deltaTime);
            Gun.transform.rotation = Quaternion.Euler(0f, 0f, newAngle);

            if(Mathf.Abs(newAngle - desiredAngle) <= turretThing.AngleThreshold) isRotationComplete = true;
            else isRotationComplete = false;
        }
    }
}
