using ModuleSpaceShip.Runtime;
using ModuleSpaceShip.Defs;
using UnityEngine;
using System.Collections.Generic;


namespace ModuleSpaceShip.Runtime
{
    class ShipThing : ThingBase<ShipDef>
    {
        private float _mass; // 전체 질량
        public float mass{get => _mass;}
        private float _thrust; // 추력 총합
        public float thrust{get => _thrust;}

        private ShipDef _def;
        private GameObject coreHullObject; // 코어 선체 오브젝트
        private Dictionary<Transform, GameObject> hullDB;

        protected override void OnInitTyped()
        {
            _def = TypedDef;

            // 코어 선체에서 구할 수 있는 모듈의 수, 전체 질량, 추력 총합 등 구하기
        }

        private void CheckCoreReachable(){}

        public bool EnlistNewHull(Transform DockingTransform, GameObject newHull)
        {
            if(!hullDB.TryGetValue(DockingTransform, out GameObject DockedObject))
            {
                // 해당 Transform이 등록되지 않았을 경우 새로이 등록
                hullDB[DockingTransform] = newHull;
                return true;
            }
            else return false; // 등록되었을 경우 false 반환
        }
    }
}