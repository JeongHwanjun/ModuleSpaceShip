using UnityEngine;
using ModuleSpaceShip.Runtime;

public abstract class EffectBase
{
    protected EffectThingBase effectThingBase;

    public EffectBase(EffectThingBase thing)
    {
        effectThingBase = thing;
    }

    public abstract void WorkOnUpdate(Module targetModule);

    public abstract void WorkOnAttach(Module targetModule);

    public abstract void WorkOnDetach(Module targetModule);
}