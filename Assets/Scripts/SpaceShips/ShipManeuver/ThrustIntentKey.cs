using System;

public readonly struct ThrustIntentKey : IEquatable<ThrustIntentKey>
{
    public readonly int moveX;
    public readonly int moveY;
    public readonly int turn;

    public ThrustIntentKey(int moveX, int moveY, int turn)
    {
        this.moveX = moveX;
        this.moveY = moveY;
        this.turn = turn;
    }

    public bool Equals(ThrustIntentKey other)
    {
        return moveX == other.moveX &&
               moveY == other.moveY &&
               turn == other.turn;
    }

    public override bool Equals(object obj)
    {
        return obj is ThrustIntentKey other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(moveX, moveY, turn);
    }
}