using UnityEngine;
using ModuleSpaceShip.Runtime;

public class DoTEffect : EffectBase
{
    private DoTEffectThing _DoTEffectThing => (DoTEffectThing)effectThingBase;

    public DoTEffect(HealingEffectThing thing) : base((EffectThingBase)thing)
    {
        Debug.Log($"[HealingEffect] new HealingEffect");
    }

    public override void WorkOnUpdate(Module targetModule)
    {
        // 업데이트마다 적용되는 효과
        targetModule.DeliverDamage(_DoTEffectThing.damagePerSec * Time.deltaTime);

    }

    public override void WorkOnAttach(Module targetModule)
    {
        // 부착시 적용되는 효과
    }

    public override void WorkOnDetach(Module targetModule)
    {
        // 탈착시 적용되는 효과, 보통 부착시 효과를 제거하는 것으로 기대됨
    }
}