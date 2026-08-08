using UnityEngine;

public readonly struct ShipControlIntent
{
    public readonly Vector2 movement;
    public readonly float turn;
    public readonly bool fire;

    public ShipControlIntent(Vector2 movement, float turn, bool fire)
    {
        this.movement = Vector2.ClampMagnitude(movement, 1f);
        this.turn = Mathf.Clamp(turn, -1f, 1f);
        this.fire = fire;
    }
}

public readonly struct CombatContext
{
    public readonly Ship self;
    public readonly Ship target;
    public readonly Vector2 selfPosition;
    public readonly Vector2 targetPosition;
    public readonly Vector2 selfVelocity;
    public readonly Vector2 targetVelocity;
    public readonly Vector2 selfHeading;

    public float Distance =>
        Vector2.Distance(selfPosition, targetPosition);

    public Vector2 DirectionToTarget =>
        (targetPosition - selfPosition).normalized;
}

public enum TravelMode
{
    PreserveCombatFacing, // 전투 방향을 유지하며 횡이동
    FaceMovement,         // 이동 방향으로 선회한 후 전진
    Auto
}

public readonly struct ManeuverGoal
{
    public readonly Vector2 anchorPosition;
    public readonly Vector2 feedForwardVelocity;
    public readonly Vector2 preferredFacing;

    public readonly float maxSpeed;
    public readonly float positionGain;
    public readonly float facingPriority;
    public readonly TravelMode travelMode;
}