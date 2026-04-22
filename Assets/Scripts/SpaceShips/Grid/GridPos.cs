using System;
using UnityEngine;

[Serializable]
public readonly struct GridPos : IEquatable<GridPos>
{
    public readonly int x;
    public readonly int y;

    public GridPos(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static GridPos operator +(GridPos a, GridPos b) => new GridPos(a.x + b.x, a.y + b.y);

    public bool Equals(GridPos other) => x == other.x && y == other.y;
    public override bool Equals(object obj) => obj is GridPos other && Equals(other);

    public override int GetHashCode()
    {
        unchecked
        {
            return (x * 397) ^ y;
        }
    }

    public override string ToString() => $"({x},{y})";
}
