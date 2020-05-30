using Godot;
using System;
using static Lib;

public class GameObj : KinematicBody
{

    public WeakRef attacker;
    public int player;

    protected Root root;
    protected MeshInstance model;
    protected MeshInstance healthBar;
    protected GameMap gameMap;
    protected Spatial archPos;
    protected float health;
    protected float buildProgress;
    protected float timeFromAttack;
    protected int objType;

    public void Init(uint _objType, int _player)
    {
        if (!IsInsideTree())
        {
            return;
        }
        objType = (int)_objType;
        SetPlayer(_player);
        health = T_MAX_HEALTH[objType];
        model.Mesh = TYPE_MESH[objType];
        buildProgress = 0.0f;
        healthBar.Translation = new Vector3(0.0f, T_HEALTH_BAR_Y_POS[objType], 0.0f);
    }

    public virtual bool Attack(GameObj obj)
    {
        if (objType < 0 || player < 0)
        {
            return false;
        }
        if (T_ARROW[objType] == null)
        {
            obj.Damage(TYPE_DAMAGE[objType]);
            obj.attacker = WeakRef(this);
        }
        else
        {
            if (archPos == null)
            {
                GD.Print("Can't find arch pos.");
                return false;
            }
            Arrow arrow = root.CreateObj(archPos.GlobalTransform.origin, T_ARROW[objType]) as Arrow;
            if (arrow != null)
            {
                arrow.damage = TYPE_DAMAGE[objType];
                arrow.player = this.player;
                arrow.speed = ARROW_SPEED * ((obj.GlobalTransform.origin - archPos.GlobalTransform.origin).Normalized());
                arrow.SetCollisionMaskBit(((player == 0)?P1_MASK_BIT:P0_MASK_BIT), true); // ?? player == -1 ??
            }
            else
            {
                GD.Print("Create arrow error.");
            }
        }
        timeFromAttack = 0;
        return true;
    }

    public void Damage(float d)
    {
        health = Mathf.Max(health - d, 0.0f);    
    }

    public bool Builded()
    {
        return (buildProgress >= 1.0f);
    }

    public int GetObjType()
    {
        return objType;
    }

    public int GetPlayer()
    {
        return player;
    }

    public virtual void SetPlayer(int p)
    {
        player = p;
        this.SetCollisionMaskBit(((player == 0)?P1_MASK_BIT:P0_MASK_BIT), true);
        this.SetCollisionLayerBit(((player == 0)?P0_MASK_BIT:P1_MASK_BIT), true);
    }

    public virtual void Destroy()
    {
        QueueFree();
    }

    public override void _Ready()
    {
        root = (Root)GetNode("/root/root");
        model = (MeshInstance)GetNode("Model");
        healthBar = (MeshInstance)GetNode("HealthBar");
        gameMap = (GameMap)GetNode("/root/Game/Map");
        archPos = (Spatial)GetNodeOrNull("ArchPos");
        buildProgress = 1.0f; // ??
        objType = -1;
    }

    public override void _PhysicsProcess(float delta)
    {
        if (!Builded())
        {
            return;
        }
        timeFromAttack += delta;
    }

    public override void _Process(float delta)
    {
        model.MaterialOverride = (Builded()?((player == 0)?player0M:player1M):((player == 0?player0TempM:player1TempM))); //
        healthBar.MaterialOverride = ((player == root.activePlayer)?aPlayerHBarM:hBarM);
        if (objType >= 0)
        {
            healthBar.Scale = new Vector3(health / T_MAX_HEALTH[objType], 1.0f, 1.0f);
            healthBar.Rotation = -this.Rotation;
            buildProgress = Mathf.Min(buildProgress + T_BUILD_SPEED[objType] * delta, 1.0f);
        }
        if (health <= 0.0f)
        {
            Destroy();
        }
    }

}
