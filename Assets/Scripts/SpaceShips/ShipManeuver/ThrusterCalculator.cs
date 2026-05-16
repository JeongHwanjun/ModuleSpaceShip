using System.Collections.Generic;
using Unity;
using UnityEngine;

public class ThrusterCalculator
{
    private const float MinScoreToFire = 0.1f;

    private readonly Dictionary<ThrustIntentKey, List<ThrusterCommand>> cache = new();

    public IReadOnlyList<ThrusterCommand> GetCommands(Vector2 movement, float torque)
    {
        ThrustIntentKey key = ToKey(movement, torque);

        if (cache.TryGetValue(key, out var commands))
            return commands;

        return System.Array.Empty<ThrusterCommand>();
    }

    public void Rebuild(
        IReadOnlyList<Thruster> thrusters,
        Vector2 centerOfMassLocal,
        float moveWeight = 1f,
        float turnWeight = 1f)
    {
        cache.Clear();

        for (int moveX = -1; moveX <= 1; moveX++)
        {
            for (int moveY = -1; moveY <= 1; moveY++)
            {
                for (int turn = -1; turn <= 1; turn++)
                {
                    ThrustIntentKey key = new(moveX, moveY, turn);

                    Vector2 movement = new Vector2(moveX, moveY);

                    if (movement.sqrMagnitude > 1f)
                        movement.Normalize();

                    float torque = turn;

                    List<ThrusterCommand> commands = BuildCommandsForIntent(
                        thrusters,
                        movement,
                        torque,
                        centerOfMassLocal,
                        moveWeight,
                        turnWeight
                    );

                    cache[key] = commands;
                }
            }
        }
    }

    private static List<ThrusterCommand> BuildCommandsForIntent(
        IReadOnlyList<Thruster> thrusters,
        Vector2 desiredMoveLocal,
        float desiredTurn,
        Vector2 centerOfMassLocal,
        float moveWeight,
        float turnWeight)
    {
        List<ThrusterCommand> commands = new();

        foreach (Thruster thruster in thrusters)
        {
            if (!thruster) continue;

            float score = CalculateScore(
                thruster,
                desiredMoveLocal,
                desiredTurn,
                centerOfMassLocal,
                moveWeight,
                turnWeight
            );

            if (score > MinScoreToFire)
            {
                float throttle = Mathf.Clamp01(score);

                commands.Add(new ThrusterCommand(thruster, throttle));
            }
        }

        return commands;
    }

    private static float CalculateScore(
        Thruster thruster,
        Vector2 desiredMoveLocal,
        float desiredTurn,
        Vector2 centerOfMassLocal,
        float moveWeight,
        float turnWeight)
    {
        Vector2 forceDir = thruster.localForceDirection.normalized;

        float moveScore = 0f;

        if (desiredMoveLocal.sqrMagnitude > 0.0001f)
        {
            moveScore = Vector2.Dot(forceDir, desiredMoveLocal.normalized);
        }

        Vector2 r = thruster.localPosition - centerOfMassLocal;
        float torque = Cross2D(r, forceDir);

        float turnScore = 0f;

        if (Mathf.Abs(desiredTurn) > 0.0001f)
        {
            // desiredTurn > 0 : 반시계
            // desiredTurn < 0 : 시계
            turnScore = torque * Mathf.Sign(desiredTurn);

            // 위치가 멀수록 torque가 커지는 문제를 완화.
            // 필요 없으면 이 줄은 제거해도 됨.
            turnScore /= Mathf.Max(1f, r.magnitude);
        }

        return moveScore * moveWeight + turnScore * turnWeight;
    }

    private static ThrustIntentKey ToKey(Vector2 movement, float torque)
    {
        int moveX = QuantizeAxis(movement.x);
        int moveY = QuantizeAxis(movement.y);
        int turn = QuantizeAxis(torque);

        return new ThrustIntentKey(moveX, moveY, turn);
    }

    private static int QuantizeAxis(float value)
    {
        if (value > 0.5f)
            return 1;

        if (value < -0.5f)
            return -1;

        return 0;
    }

    private static float Cross2D(Vector2 a, Vector2 b)
    {
        return a.x * b.y - a.y * b.x;
    }
}