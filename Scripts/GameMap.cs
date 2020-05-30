using Godot;
using System;
using System.Collections.Generic;
using static Lib;

public class GameMap : GridMap
{

    public List<WeakRef>[] buildings;
    public List<WeakRef>[] units;
    public int[,] unitsPos;
    public bool redraw;

    private Root root;

    public void ChangeBuildZone(Vec2I pos, uint player, int num)
    {
        int x = 0;
        int y = 0;
        for (int i = -BUILDING_ZONE; i <= BUILDING_ZONE; i++)
        {
            for (int j = -BUILDING_ZONE; j <= BUILDING_ZONE; j++)
            {
                x = i + pos.x + MAP_SIZE.x;
                y = j + pos.y + MAP_SIZE.y;
                if (x >= 0 && y >= 0 && x < 2 * MAP_SIZE.x && y < 2 * MAP_SIZE.y)
                {
                    root.buildZone[player, x, y] += num;
                }
            }
        }
        redraw = true;
    }

    public bool Build(Vec2I pos, int type, int player)
    {
        int x = 0;
        GameObj obj;
        Vector3 v = MapToWorld(pos.x, 0, pos.y);
        if (type < 0 || player < 0 || pos.x < -MAP_SIZE.x || pos.y < -MAP_SIZE.y || pos.x >= MAP_SIZE.x ||
         pos.y >= MAP_SIZE.y || root.buildZone[player, pos.x + MAP_SIZE.x, pos.y + MAP_SIZE.y] <= 0 ||
          root.techLevel[player] < T_NEEDED_TECH[type] || root.money[player] < T_BUILD_COST[type] || 
          unitsPos[pos.x + MAP_SIZE.x, pos.y + MAP_SIZE.y] > 0 || GetCellItem(pos.x, 0, pos.y) < 0)
        {
            return false;
        }
        root.money[player] -= T_BUILD_COST[type];
        x = T_SUBTYPE[type];
        obj = root.CreateObj(new Vector3(v.x, 0.0f, v.z) + ((x == BUILDING_T)?Vector3.Zero:
        (new Vector3(CellSize.x * (GD.Randf() - 0.5f), 0.0f, CellSize.z * (GD.Randf() - 0.5f)))),
         ((x == BUILDING_T)?((T_ARROW[type] != null)?towerPS:buildingPS):((T_ARROW[type] != null)?archerPS:unitPS))) as GameObj;
        if (obj == null || ((!(obj is Building)) && x == BUILDING_T) ||
         ((!(obj is Unit)) && x != BUILDING_T)) // ??
        { 
            GD.Print("Create building error.");
            return false;
        }
        if (T_SUBTYPE[type] == BUILDING_T)
        {
            if (type != WALL_TYPE)
            {
                ChangeBuildZone(pos, (uint)player, 1);
            }
            ((Building)obj).Init((uint)type, player, pos);
            SetCellItem(pos.x, 0, pos.y, -1);
            buildings[player].Add(WeakRef(obj));
        }
        else // ??
        {
            ((Unit)obj).Init((uint)type, player);
            ((Unit)obj).target = Vec3ToVec2(v);
            units[player].Add(WeakRef(obj));
        }
        return true;
    }

    public bool Build(Vector3 pos, int type, int player)
    {
        Vector3 v = WorldToMap(pos);
        return Build(new Vec2I((int)v.x, (int)v.z), type, player);
    }

    public bool IsGoldTile(Vec2I pos)
    {
        return ((pos.x == -MAP_SIZE.x || pos.x == MAP_SIZE.x - 1 || (pos.x >= -1 && pos.x <= 0)) && pos.y >= -1 && pos.y <= 0);
    }

    public void StartGame()
    {
        if (buildings == null)
        {
            buildings = new List<WeakRef>[PLAYERS_NUM];
            for (int i = 0; i < PLAYERS_NUM; i++)
            {
                buildings[i] = new List<WeakRef>();
            }
        }
        if (units == null)
        {
            units = new List<WeakRef>[PLAYERS_NUM];
            for (int i = 0; i < PLAYERS_NUM; i++)
            {
                units[i] = new List<WeakRef>();
            }
        }
        if (unitsPos == null)
        {
            unitsPos = new int[2 * MAP_SIZE.x, 2 * MAP_SIZE.y];
        }
        Clear();
        for (int i = 0; i < PLAYERS_NUM; i++)
        {
            buildings[i].Clear();
            units[i].Clear();
        }
        for (int i = -MAP_SIZE.x; i < MAP_SIZE.x; i++)
        {
            for (int j = -MAP_SIZE.y; j < MAP_SIZE.y; j++)
            {
                if (IsGoldTile(new Vec2I(i, j)))
                {
                    SetCellItem(i, 0, j, GOLD_TILE);
                }
                else
                {
                    SetCellItem(i, 0, j, FREE_TILE);
                }
                unitsPos[i + MAP_SIZE.x, j + MAP_SIZE.y] = 0;
            }
        }
        redraw = true;
    }

    public override void _Ready()
    {
        root = (Root)GetNode("/root/root");
        redraw = true;
        //StartGame();
    }

    public override void _PhysicsProcess(float delta)
    {
        Vector2 v;
        int x = 0;
        int y = 0;
        if (redraw)
        {
            for (int i = 0; i < 2 * MAP_SIZE.x; i++)
            {
                for (int j = 0; j < 2 * MAP_SIZE.y; j++)
                {
                    x = i - MAP_SIZE.x;
                    y = j - MAP_SIZE.y;
                    if (GetCellItem(x, 0, y) >= 0)
                    {
                        SetCellItem(x ,0, y, (IsGoldTile(new Vec2I(x, y))?GOLD_TILE:FREE_TILE) + ((root.buildZone[root.activePlayer, i, j] > 0)?ACTIVE_TILE_C:0));
                    }
                }
            }
        }
        for (int i = 0; i < 2 * MAP_SIZE.x; i++)
        {
            for (int j = 0; j < 2 * MAP_SIZE.y; j++)
            {
                unitsPos[i, j] = 0;
            }
        }
        for (int i = 0; i < PLAYERS_NUM; i++)
        {
            for (int j = 0; j < buildings[i].Count;)
            {
                if (buildings[i][j].GetRef() == null)
                {
                    buildings[i][j] = buildings[i][buildings[i].Count - 1];
                    buildings[i].RemoveAt(buildings[i].Count - 1);
                }
                else
                {
                    j++;
                }
            }
            for (int j = 0; j < units[i].Count;)
            {
                if (units[i][j].GetRef() == null)
                {
                    units[i][j] = units[i][units[i].Count - 1];
                    units[i].RemoveAt(units[i].Count - 1);
                }
                else
                {
                    Unit u = units[i][j].GetRef() as Unit;
                    if (u != null && u.GetObjType() != DRAGON_TYPE)
                    {
                        v = Vec3ToVec2(WorldToMap(u.GlobalTransform.origin)) + new Vector2(MAP_SIZE.x, MAP_SIZE.y);
                        if (v.x >= 0.0f && v.y >= 0.0f && v.x < 2 * MAP_SIZE.x && v.y < 2 * MAP_SIZE.y)
                        {
                            unitsPos[(int)v.x, (int)v.y]++;
                        }
                    }
                    j++;
                }
            }
        }
    }

}
