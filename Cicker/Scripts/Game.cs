using Godot;
using System;

public class Game : Node2D
{
    private float spawnTime = 1f;
    private Timer spawnTimer;
    private Label comboLabel;
    private int combo = 0;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        comboLabel = GetNode<Label>("UIContainer/ComboLabel");
        spawnTimer = GetNode<Timer>("SpawnTimer");
        spawnTimer.Connect("timeout", this, "OnSpawnTimerTimeout");
        spawnTimer.Start(spawnTime);
    }

    public void OnSpawnTimerTimeout()
    {
        var randX = GD.Randf() * 950f;
        var randY = GD.Randf() * 550f;
        var normalCircleScene = GD.Load<PackedScene>("res://Scenes/Circles/NormalCircle.tscn");
        var normalCircle = (Node2D)normalCircleScene.Instance();
        normalCircle.GlobalPosition = new Vector2(randX, randY);
        normalCircle.Connect("on_destroy", this, "OnNormalCircleDestroyed");
        AddChild(normalCircle);
    }

    public override void _Input(InputEvent @event)
    {
        bool isDesktop =
        (OS.GetName() == "Windows") ||
        (OS.GetName() == "X11") ||
        (OS.GetName() == "OSX");

        if (isDesktop)
        {
            if (@event is InputEventMouseButton eventbtn && @event.IsPressed())
            {
                SpawnClickCircle(eventbtn.GlobalPosition);
            }
        }
        else
        {
            if (@event is InputEventScreenTouch eventScreenTouch && @event.IsPressed())
            {
                var globalTouchPosition = GetCanvasTransform().AffineInverse().XformInv(eventScreenTouch.Position);
                SpawnClickCircle(globalTouchPosition);
            }
        }
    }

    public void SpawnClickCircle(Vector2 position)
    {
        var clickCircleScene = GD.Load<PackedScene>("res://Scenes/Circles/ClickCircle.tscn");
        var clickCircle = (Area2D)clickCircleScene.Instance();
        clickCircle.GlobalPosition = position;
        AddChild(clickCircle);
    }

    public void OnNormalCircleDestroyed()
    {
        combo++;
        comboLabel.Text = combo.ToString();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {

    }
}
