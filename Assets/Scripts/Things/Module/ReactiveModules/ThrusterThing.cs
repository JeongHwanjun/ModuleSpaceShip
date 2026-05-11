using ModuleSpaceShip.Defs;
using UnityEngine;

namespace ModuleSpaceShip.Runtime
{
    public class ThrusterThing : ColliderReactiveModuleThing
    {
        private ThrusterDef _thrusterDef => (ThrusterDef)_def;
        public ThrusterDef thrusterDef => _thrusterDef;

        public float thrust = 0f;
        public float modifiedThrust = 0f;

        protected override void OnInitTyped()
        {
            base.OnInitTyped();
            thrust = thrusterDef.thrust;
            modifiedThrust = thrust;
        }

        public void AddThrust(float delta)
        {
            if(modifiedThrust + delta < 0)
            {
                Debug.LogWarning($"[ThrusterThing] thrust Cannot less than 0.");
                return;
            }
            modifiedThrust = thrust + delta;
        }

        public void MultiplyThrust(float coefficient)
        {
            if(coefficient < 0)
            {
                Debug.LogWarning($"[ThrusterThing] thrust Cannot less than 0");
                return;
            }
            modifiedThrust = thrust * coefficient;
        }
    }
}