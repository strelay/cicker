using Godot;
using System;

public class NormalCircle : Node2D
{
    [Signal] public delegate void on_destroy();

    private Area2D collision;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        collision = GetNode<Area2D>("NormalCircleArea2D");
        collision.Connect("area_entered", this, "OnCollision");
    }

    public void OnCollision(Area2D area)
    {
        if (area.Name == "ClickCircleArea2D")
        {
            EmitSignal("on_destroy");
            this.QueueFree();
        }
    }
}
