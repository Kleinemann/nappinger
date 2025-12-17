using Godot;
using System;

public partial class CameraScroll : Camera2D
{
    float EdgeMargin = 15f;
    float CameraSpeed = 200f;
    Vector2 UnZoomedViewportSize = new Vector2(1152, 648);
    float ZoomLevel = 1f;

    public override void _Input(InputEvent @event)
    {
        return;
        if(@event is InputEventMouseButton mbe)
        {
            if(mbe.IsPressed())
            {
                Vector2 mousePos = GetViewport().GetMousePosition();

                if (mbe.ButtonIndex == MouseButton.WheelUp)
                {
                    if (ZoomLevel > 1f)
                    {
                        Vector2 preZoomValue = Zoom;
                        ZoomLevel -= 0.25f;
                        Zoom = new Vector2(ZoomLevel, ZoomLevel);
                        Position += (mousePos - Position) * (new Vector2(1, 1) - preZoomValue / Zoom);
                    }
                }
                if(mbe.ButtonIndex == MouseButton.WheelDown)
                {
                    if (ZoomLevel < 4f)
                    {
                        Vector2 preZoomValue = Zoom;
                        ZoomLevel += 0.25f;
                        Zoom = new Vector2(ZoomLevel, ZoomLevel);
                        Position += (mousePos - Position) * (new Vector2(1, 1) - preZoomValue / Zoom);
                    }
                }
            }
        }
    }

    public override void _Process(double delta)
    {
        return;
        Vector2 mousePos = GetViewport().GetMousePosition();
        Vector2 moveVector = Vector2.Zero;
        if (mousePos.X <= EdgeMargin)
            moveVector.X = -CameraSpeed * (float)delta;
        else if(mousePos.X >= UnZoomedViewportSize.X - EdgeMargin)
            moveVector.X = CameraSpeed * (float)delta;

        if (mousePos.Y <= EdgeMargin)
            moveVector.Y = -CameraSpeed * (float)delta;
        else if(mousePos.Y >= UnZoomedViewportSize.Y - EdgeMargin)
            moveVector.Y = CameraSpeed * (float)delta;

        if(moveVector == Vector2.Zero)
            return;

        Position += moveVector;

        //WorldMain.Instance.Map.UpdateMap();
    }
}
