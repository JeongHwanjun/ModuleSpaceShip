public readonly struct ThrusterCommand
{
    public readonly Thruster thruster;
    public readonly float throttle;

    public ThrusterCommand(Thruster thruster, float throttle)
    {
        this.thruster = thruster;
        this.throttle = throttle;
    }
}