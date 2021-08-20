using Godot;

public class ClickCircle : Area2D
{
    private float selfDestroyTime = 0.1f;
    private Timer selfDestroyTimer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        selfDestroyTimer = GetNode<Timer>("Timer");
        selfDestroyTimer.WaitTime = selfDestroyTime;
        selfDestroyTimer.Connect("timeout", this, "SelfDestroy");
        selfDestroyTimer.Start();
    }

    public void SelfDestroy()
    {
        this.QueueFree();
    }
}
