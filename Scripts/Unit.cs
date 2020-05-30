using Godot;
using System;
using static Lib;

public class Unit : GameObj
{

    public Vector2 target;

    protected Area aRange;
    protected Area vRange;
    protected AnimationPlayer aPlayer;
    protected bool waitAttack;

    public bool OnTarget()
    {
        return (target - Vec3ToVec2(this.GlobalTransform.origin)).Length() <= UNIT_TARGET_DIST;
    }

    public void PlayIdleAnim()
    {
        aPlayer.Play(TYPE_NAME[objType] + ((objType == DRAGON_TYPE)?"":"_idle"));
    }

    public void PlayMoveAnim()
    {
        aPlayer.Play(TYPE_NAME[objType] + ((objType == DRAGON_TYPE)?"":"_move"));
    }

    public void PlayAttackAnim()
    {
        aPlayer.Play(TYPE_NAME[objType] + ((objType == DRAGON_TYPE)?"":"_attack"));
    }

    public override void SetPlayer(int p)
    {
        base.SetPlayer(p);
        if (aRange != null)
        {
            aRange.SetCollisionMaskBit(((player == 0)?P1_MASK_BIT:P0_MASK_BIT), true);
        }
        if (vRange != null)
        {
            vRange.SetCollisionMaskBit(((player == 0)?P1_MASK_BIT:P0_MASK_BIT), true);
        }
    }

    public override bool Attack(GameObj obj)
    {
        bool b = false;
        b = base.Attack(obj);
        if (obj == null || objType < 0 || player < 0 || !b)
        {
            return false;
        }
        this.Rotation = new Vector3(0.0f, -(Vec3ToVec2(obj.GlobalTransform.origin - this.GlobalTransform.origin)).Angle() + Mathf.Pi / 2.0f, 0.0f);
        PlayAttackAnim();
        return b;
    }

    public override void _Ready()
    {
        base._Ready();
        aRange = (Area)GetNode("ARange");
        vRange = (Area)GetNode("VRange");
        aPlayer = (AnimationPlayer)GetNode("APlayer");
        attacker = null;
        timeFromAttack = 0.0f;
        waitAttack = false;
    }

    public override void _PhysicsProcess(float delta) // ?? attack dragons for non-archers ??
    {
        base._PhysicsProcess(delta);
        if (objType == DRAGON_TYPE)
        {
            this.Translation = new Vector3(this.Translation.x, FLY_Y_POS, this.Translation.z);
            model.Scale = new Vector3(1.2f, 1.2f, 1.2f);
        }
        if (!Builded())
        {
            if (objType >= 0)
            {
                PlayIdleAnim();
            }
            return;
        }
        Vector2 pos = Vec3ToVec2(this.GlobalTransform.origin);
        Vector2 v;
        if (attacker != null && attacker.GetRef() != null && attacker.GetRef() is Spatial)
        {
            Spatial obj = (attacker.GetRef() as Spatial);
            v = Vec3ToVec2(obj.GlobalTransform.origin);
            if ((pos - v).Length() <= UNIT_VIEW_RANGE || ((pos - v).Length() <= ARCHER_VIEW_RANGE && objType >= 0 && T_ARROW[objType] != null))
            {
                target = v;
            }
        }
        if (OnTarget() && player >= 0)
        {
            v = root.unitTargetPos[player];
            if ((v - pos).Length() > UNIT_TARGET_DIST)
            {
                target = v;
            }
        }
        if (OnTarget())
        {
            var objects = vRange.GetOverlappingBodies();
            if (objects != null && objects.Count > 0)
            {
                for (int i = 0; i < objects.Count; i++)
                {
                    GameObj obj = objects[i] as GameObj;
                    if (obj != null)
                    {
                        target = Vec3ToVec2(obj.GlobalTransform.origin);
                        break;
                    }
                    else
                    {
                        GD.Print("Set attack target error (unit).");
                    }
                }
            }
        }
        if (objType < 0)
        {
            return;
        }
        v = target - pos;
        if (timeFromAttack > T_ATTACK_TIMEOUT[objType]) // ?? 
        {
            var objects = aRange.GetOverlappingBodies();
            if (objects != null && objects.Count > 0)
            {
                GameObj obj = objects[(int)(GD.Randi() % objects.Count)] as GameObj;
                if (obj != null)
                {
                    Attack(obj);
                    waitAttack = true;
                }
                else
                {
                    waitAttack = false;
                    GD.Print("Attack error (unit).");
                }
            }
            else
            {
                waitAttack = false;
            }
        }
        /*else
        {
            waitAttack = false;
        }*/
        if (timeFromAttack > T_ATTACK_TIME[objType])
        {
            if (!OnTarget() && !waitAttack)
            {
                PlayMoveAnim();
                this.Rotation = new Vector3(0.0f, -v.Angle() + Mathf.Pi / 2.0f, 0.0f);
                MoveAndSlide(TYPE_SPEED[objType]* (new Vector3(v.x, 0.0f, v.y)).Normalized());
            }
            else
            {
                PlayIdleAnim();
            }
        }
    }

}
