using ModuleSpaceShip.Runtime;

public abstract class ReactiveModule : Module
{
    protected ReactiveModuleThing reactiveModuleThing => (ReactiveModuleThing)moduleThing;

    public abstract void OnAttached(); // ReactiveModule이 도킹됨
    public abstract void OnDetached(); // ReactiveModule이 언도킹됨
    public abstract void OnModuleAttached(); // 다른 Module이 도킹됨
    public abstract void OnModuleDetached(); // 다른 Module이 언도킹됨
}