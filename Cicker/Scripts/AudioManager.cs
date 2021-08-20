using Godot;

public class AudioManager : Node
{
    private AudioStreamPlayer normalHitCircleSFX;
    private AudioStreamPlayer dragHitCircleSFX;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        normalHitCircleSFX = GetNode<AudioStreamPlayer>("SFX/NormalHitCircleSFX");
        dragHitCircleSFX = GetNode<AudioStreamPlayer>("SFX/DragHitCircleSFX");
    }

    public void PlayHitCircleSFX(HitCircleType circleType)
    {
        switch (circleType)
        {
            case HitCircleType.Normal:
                normalHitCircleSFX.Play();
            break;
            case HitCircleType.Drag:
                dragHitCircleSFX.Play();
            break;
            default:
                return;
        }
    }
}
