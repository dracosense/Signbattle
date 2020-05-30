using Godot;
using System;
using static Lib;

public class Arrow : Area
{

    public Vector3 speed = Vector3.Zero;
    public float damage = 0;
    public int player = -1;

    private Root root;

    public void _on_body_entered(Spatial body)
    {
        if (body is GameObj)
        {
            ((GameObj)body).Damage(damage);
        }
        QueueFree();
    }

    public override void _Ready()
    {
        root = (Root)GetNode("/root/root");
    }

    public override void _PhysicsProcess(float delta)
    {
        Vector2 v = Vec3ToVec2(speed);
        if (speed != Vector3.Zero)
        {
            this.LookAt(this.GlobalTransform.origin + this.speed, Vector3.Up);
        }
        //this.Rotation = new Vector3(0.0f, -v.Angle() + Mathf.Pi / 2.0f, 0.0f); // remake (up / down rotation)
        this.GlobalTranslate(delta * speed);
    }

}
