namespace ModuleSpaceShip.Utilities
{
    public static class GridRangeUtilities
    {
        // 인접 4타일, 상하좌우
        public static readonly GridPos[] Card4 =
        {
                                new GridPos( 0,  1), // Up
            new GridPos(-1,  0),                     new GridPos( 1,  0), // Left, Right
                                new GridPos( 0, -1), // Down
        };

        // 인접 8타일, 3*3 정사각형(자신 제외)
        public static readonly GridPos[] Card8 =
        {
            new GridPos(-1,  1), new GridPos( 0,  1), new GridPos( 1,  1),
            new GridPos(-1,  0),                      new GridPos( 1,  0),
            new GridPos(-1, -1), new GridPos( 0, -1), new GridPos( 1, -1),
        };

        // 인접 24타일, 5*5 정사각형(자신 제외)
        public static readonly GridPos[] Card24 =
        {
            new GridPos(-2,  2), new GridPos(-1,  2), new GridPos( 0,  2), new GridPos( 1,  2), new GridPos( 2,  2),
            new GridPos(-2,  1), new GridPos(-1,  1), new GridPos( 0,  1), new GridPos( 1,  1), new GridPos( 2,  1),
            new GridPos(-2,  0), new GridPos(-1,  0),                      new GridPos( 1,  0), new GridPos( 2,  0),
            new GridPos(-2, -1), new GridPos(-1, -1), new GridPos( 0, -1), new GridPos( 1, -1), new GridPos( 2, -1),
            new GridPos(-2, -2), new GridPos(-1, -2), new GridPos( 0, -2), new GridPos( 1, -2), new GridPos( 2, -2),
        };
    }
}
