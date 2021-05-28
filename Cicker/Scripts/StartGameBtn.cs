using Godot;
using System;

public class StartGameBtn : TextureButton
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.Connect("pressed", this, "StartGame");
    }

    public void StartGame()
    {
        GD.Print("Start btn clicked");
    }
}
