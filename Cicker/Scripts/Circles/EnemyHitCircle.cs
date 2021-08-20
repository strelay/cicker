using Godot;

public class EnemyHitCircle : Node2D
{
    [Signal] public delegate void on_destroy(HitCircleType circleType);
    public HitCircleType circleType;
    private Area2D collision;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        collision = GetNode<Area2D>("EnemyCircleArea2D");
    }

    public void Destroy()
    {
        EmitSignal("on_destroy", circleType);
        this.QueueFree();
    }
}