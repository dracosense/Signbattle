using Godot;
using System;
using static Lib;

public class Tower : Building
{

    private Area aRange;

    public override void SetPlayer(int p)
    {
        base.SetPlayer(p);
        if (aRange != null)
        {
            aRange.SetCollisionMaskBit(((p == 0)?P1_MASK_BIT:P0_MASK_BIT), true); // ?? player == -1 ?? //
        }
    }

    public override void _Ready()
    {
        base._Ready();
        aRange = (Area)GetNode("ARange");
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        if (objType >= 0 && timeFromAttack >= T_ATTACK_TIMEOUT[objType])
        {
            var objects = aRange.GetOverlappingBodies();
            if (objects != null && objects.Count > 0)
            {
                GameObj obj = objects[0/*(int)(GD.Randi() % objects.Count)*/] as GameObj; // ?? rand ??
                if (obj != null)
                {
                    Attack(obj);
                }
                else
                {
                    GD.Print("Attack error (tower).");
                }
            }
        }
    }

}
