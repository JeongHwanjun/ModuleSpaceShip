using UnityEngine;
using ModuleSpaceShip.Defs;
using System;

namespace ModuleSpaceShip.Runtime
{
    public abstract class ModuleThing : ThingBase<ModuleDef>
    {
        // State는 Thing 내부에서만 변경 가능, Monobehaviour에서는 입력과 출력만 담당.
        public enum State
        {
            Connected,
            Floating,
            Grabbed
        }
        public enum Direction
        {
            up,
            down,
            left,
            right
        }
        private State _state;
        public State state {get => _state;set => _state=value;}
        private Direction _direction;
        public Direction direction {get => _direction; set => _direction = value;}
        protected ModuleDef _def;
        public  ModuleDef def => _def;

        private float maxHullPoint;
        private float currentHullPoint;

        protected override void OnInitTyped()
        {
            // 초기화
            _def = TypedDef;

            maxHullPoint = _def.hullPoint;
            currentHullPoint = maxHullPoint;
            _state = State.Floating;
            _direction = Direction.up;
        }

        public float AddHullPoint(float delta)
        {
            currentHullPoint = Math.Min(currentHullPoint+delta, maxHullPoint);

            if(currentHullPoint <= 0)
            {
                // 체력이 0 이하일 경우 처리
                Debug.Log($"[HullThing] currentHullPoint : {currentHullPoint}");
            }

            return currentHullPoint;
        }
    }
}