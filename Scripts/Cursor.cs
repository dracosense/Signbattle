using Godot;
using System;
using static Lib;

public class Cursor : Spatial
{

    private Root root;

    public override void _Ready()
    {
        root = (Root)GetNode("/root/root");
    }

    public override void _PhysicsProcess(float delta)
    {
        this.Translation = Vec2ToVec3(root.unitTargetPos[0]);
    }

}
