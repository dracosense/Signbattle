using Godot;
using System;
using static Lib;

public class Building : GameObj
{
    protected Vec2I pos;

    public void Init(uint _objType, int _player, Vec2I _pos)
    {
        Init(_objType, _player);
        if (!IsInsideTree())
        {
            return;
        }
        pos = _pos;
        if (this.objType == MAGE_TOWER_TYPE && player >= 0)
        {
            root.techLevel[player]++;
        }
    }

    public override void Destroy()
    {
        if (this.objType == MAGE_TOWER_TYPE && player >= 0)
        {
            root.techLevel[player]--;
        }
        if (player >= 0 && objType != WALL_TYPE)
        {
            gameMap.ChangeBuildZone(pos, (uint)player, -1);
        }
        gameMap.SetCellItem(pos.x, 0, pos.y, (gameMap.IsGoldTile(pos)?GOLD_TILE:FREE_TILE));
        QueueFree();
    }

    public override void _Ready()
    {
        base._Ready();
        pos = new Vec2I();
    }

    public override void _Process(float delta)
    {
        if (Builded() && objType == MINE_TYPE)
        {
            root.money[player] += (gameMap.IsGoldTile(pos)?GOLD_MINE_SPEED:MINE_SPEED) * delta;
        }
        base._Process(delta);
    }

}
