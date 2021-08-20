using Godot;
using System.Collections.Generic;

public class Game : Node2D
{
    private float spawnTime = 1f;
    private Timer spawnTimer;
    private Label comboLabel;
    private int combo = 0;
    private VBoxContainer gameOverUIContainer;
    private VBoxContainer pauseUIContainer;
    private TextureButton restartButton;
    private TextureButton pauseButton;
    private List<Node2D> circles = new List<Node2D>();
    private bool isGameOver = false;
    private bool isGamePaused = false;
    private AudioManager audioManager;
    private ConfigSystem configSystem;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        configSystem = GetNode<ConfigSystem>("/root/ConfigSystem");
        audioManager = GetNode<AudioManager>("AudioManager");
        restartButton = GetNode<TextureButton>("CanvasLayer/RestartButton");
        pauseButton = GetNode<TextureButton>("CanvasLayer/PauseButton");
        pauseButton.Connect("pressed", this, "PauseGame");
        gameOverUIContainer = GetNode<VBoxContainer>("CanvasLayer/GameOverUIContainer");
        pauseUIContainer = GetNode<VBoxContainer>("CanvasLayer/PauseUIContainer");
        comboLabel = GetNode<Label>("CanvasLayer/UIContainer/ComboLabel");
        spawnTimer = GetNode<Timer>("SpawnTimer");
        spawnTimer.Connect("timeout", this, "OnSpawnTimerTimeout");
        spawnTimer.Start(spawnTime);
    }

    public void OnSpawnTimerTimeout()
    {
        var randX = GD.Randf() * 850f + 80;
        var randY = GD.Randf() * 460f + 70;

        HitCircleType circleType = HitCircleType.Normal;
        PackedScene circleScene = null;
        uint randomCircleTypeInt = GD.Randi() % 2;
        switch (randomCircleTypeInt)
        {
            case 0:
                circleType = HitCircleType.Normal;
                circleScene = GD.Load<PackedScene>("res://Scenes/Circles/NormalCircle.tscn");
            break;
            case 1:
                circleType = HitCircleType.Drag;
                circleScene = GD.Load<PackedScene>("res://Scenes/Circles/DragCircle.tscn");
            break;
        }
        var circle = (Node2D)circleScene.Instance();
        (circle as EnemyHitCircle).circleType = circleType;
        circle.GlobalPosition = new Vector2(randX, randY);
        circle.Connect("on_destroy", this, "OnHitCircleDestroyed");

        AddChild(circle);
        circles.Add(circle);
    }

    public override void _Input(InputEvent @event)
    {
        if (!isGameOver || isGamePaused)
        {
            bool isDesktop =
            (OS.GetName() == "Windows") ||
            (OS.GetName() == "X11") ||
            (OS.GetName() == "OSX");

            if (isDesktop)
            {
                if (@event is InputEventMouseButton eventbtn && @event.IsPressed())
                {
                    HandleCollisions(eventbtn.Position);
                }

                if (@event is InputEventMouseMotion dragEvent)
                {
                    if (dragEvent.Pressure > 0.5)
                    {
                        HandleCollisions(dragEvent.Position, dragEvent.Speed, true);
                    }
                }
            }
            else
            {
                if (@event is InputEventScreenTouch eventScreenTouch && @event.IsPressed())
                {
                    var globalTouchPosition = GetCanvasTransform().AffineInverse().XformInv(eventScreenTouch.Position);
                    HandleCollisions(globalTouchPosition);
                }

                if (@event is InputEventScreenDrag dragEvent)
                {
                    HandleCollisions(dragEvent.Position, dragEvent.Speed, true);
                }
            }
        }
    }

    public void HandleCollisions(Vector2 inputPosition, Vector2 dragSpeed = new Vector2(), bool isDragging = false)
    {
        for (int i = 0; i < circles.Count; i++)
        {
            if (isDragging)
            {
                HandleDragCollisions(inputPosition, dragSpeed, i);
            }
            else
            {
                HandleClickCollisions(inputPosition, i);
            }
        }
    }

    public void HandleClickCollisions(Vector2 clickPosition, int circleIndex)
    {
        var circle = circles[circleIndex];
        var circleScript = circle as EnemyHitCircle;
        if (circleScript.circleType == HitCircleType.Normal)
        {
            var circleCollissionShape = circle.GetNode<CollisionShape2D>("EnemyCircleArea2D/CollisionShape2D");
            var circleHitbox = new Rect2(circleCollissionShape.GlobalPosition, circleCollissionShape.GlobalScale);
            var clickHitbox = new Rect2(clickPosition.x - 50, clickPosition.y - 50, 100, 100);
            if (circleHitbox.Intersects(clickHitbox))
            {
                circleScript.Destroy();
                circles.RemoveAt(circleIndex);
            }
        }
    }

    public void HandleDragCollisions(Vector2 clickPosition, Vector2 speed, int circleIndex)
    {
        var circle = circles[circleIndex];
        var circleScript = circle as EnemyHitCircle;
        int speedThreshold = 20;
        if (circleScript.circleType == HitCircleType.Drag)
        {
            var circleCollissionShape = circle.GetNode<CollisionShape2D>("EnemyCircleArea2D/CollisionShape2D");
            var circleHitbox = new Rect2(circleCollissionShape.GlobalPosition, circleCollissionShape.GlobalScale);
            var clickHitbox = new Rect2(clickPosition.x - 50, clickPosition.y - 50, 100, 100);
            if (circleHitbox.Intersects(clickHitbox) &&
                (Mathf.Abs(speed.x) > speedThreshold || Mathf.Abs(speed.y) > speedThreshold))
            {
                circleScript.Destroy();
                circles.RemoveAt(circleIndex);
            }
        }
    }

    public void OnHitCircleDestroyed(HitCircleType circleType)
    {
        string currentHighScore = configSystem.LoadValue("Scores", "HighScore");
        string highScoreStr = "";
        if (currentHighScore.IsValidInteger())
        {
            highScoreStr = "/" + currentHighScore;
        }
        combo++;
        comboLabel.Text = combo.ToString() + highScoreStr;
        audioManager.PlayHitCircleSFX(circleType);
    }

    public void GameOver()
    {
        isGameOver = true;
        spawnTimer.Stop();

        comboLabel.Visible = false;
        pauseButton.Disabled = true;
        pauseButton.Visible = false;
        gameOverUIContainer.Visible = true;
        restartButton.Disabled = false;
        
        string currentHighScore = configSystem.LoadValue("Scores", "HighScore");
        if ((currentHighScore.IsValidInteger() && int.Parse(currentHighScore) < combo) 
            || currentHighScore == "")
        {
            configSystem.SaveValue("Scores", "HighScore", combo.ToString());
        }
    }

    public void PauseGame()
    {
        isGamePaused = !isGamePaused;
        spawnTimer.Paused = isGamePaused;
        pauseUIContainer.Visible = isGamePaused;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (combo > 0 && spawnTime > 0.3f)
        {
            spawnTime = 1f - (combo / 250f);
            spawnTimer.WaitTime = spawnTime;
        }

        if (circles.Count > 5)
        {
            GameOver();
        }
    }
}
