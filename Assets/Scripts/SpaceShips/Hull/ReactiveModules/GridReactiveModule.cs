using ModuleSpaceShip.Runtime;
public abstract class GridReactiveModule : ReactiveModule
{
    protected GridReactiveModuleThing gridReactiveModuleThing => (GridReactiveModuleThing)reactiveModuleThing;

    protected void GetModulesInGrid()
    {
        // ship 소식인지 확인
        if(ship == null) return; // ship 소속이 아니라면 스킵

        // ship 소속이라면 주어진 그리드대로 방문하며 Collider2D 조회
        GridPos[] grids = gridReactiveModuleThing.GetGrid();
        if(grids == null) return;
        targetModules = ship.GetModulesByGrid(grids);
    }
}