using Godot;
using System;

public static class Lib
{

    public struct Vec2I
    {
        public int x;
        public int y;

        public Vec2I(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
    }

    public struct Line
    {

        public Vector2 begin;
        public Vector2 end;

        public Line(Vector2 _begin, Vector2 _end)
        {
            begin = _begin;
            end = _end;
        }

    }

    public struct StrPattern
    {

        public string p;
        public bool cycled;

        public StrPattern(string _p, bool _cycled = false)
        {
            p = _p;
            cycled = _cycled;
        }

        public bool IsEqual(string s) // ?? optimize with hash ??
        {
            bool ans = false;
            int k = 0;
            if (p == null || p.Length != s.Length)
            {
                return ans;
            }
            for (int i = 0; i < (cycled?p.Length:1) && !ans; i++)
            {
                ans = true;
                for (int j = 0; j < p.Length; j++)
                {
                    k = (i + j) % p.Length;
                    if (s[j] != p[k] && p[k] != '*')
                    {
                        ans = false;
                        break;
                    }
                }
            }
            return ans;
        }
    }

    public const float BASE_C_WIDTH = 2;
    public const float RAYCAST_LEN = 100.0f;
    public const float START_MONEY = 4.0f;
    public const float MINE_SPEED = 0.04f;
    public const float GOLD_MINE_SPEED = 0.09f;
    public const float UNIT_TARGET_DIST = 1.5f;
    public const float UNIT_S_TARGET_DIST = 2.0f; // strategy
    public const float UNIT_VIEW_RANGE = 4.0f;
    public const float ARCHER_VIEW_RANGE = 7.0f;
    public const float UNIT_STRATEGY_R = 1.5f;
    public const float MAX_CASTLE_HEALTH = 80.0f;
    public const float ARROW_SPEED = 6.0f;
    public const float AI_TIMEOUT = 3.6f;
    public const float FLY_Y_POS = 8.0f;
    public const int TYPES_NUM = 10;
    public const int FREE_TILE = 0;
    public const int GOLD_TILE = 1;
    public const int ACTIVE_TILE_C = 2;
    public const int WALL_TYPE = 0;
    public const int MINE_TYPE = 3;
    public const int DRAGON_TYPE = 9;
    public const int MAGE_TOWER_TYPE = 4;
    public const int PLAYERS_NUM = 2;
    public const int BUILDING_T = 0;
    public const int UNIT_T = 2;
    public const int P0_MASK_BIT = 4;
    public const int P1_MASK_BIT = 5;
    public const int BASE_TECH_LEVEL = 0;
    public const int BUILD_M_MODE = 0;
    public const int STRATEGY_M_MODE = 1;
    public const int TARGET_M_MODE = 2;
    public const int BUILDING_ZONE = 1;
    public const int MAX_AI_UNITS_GEN = 3;


    public static readonly StrPattern[] TYPE_PATTERN = {new StrPattern("*"), 
    new StrPattern("312", true), new StrPattern("3012", true), new StrPattern("301"), new StrPattern("30123012"), 
    new StrPattern("02"), new StrPattern("02032"), new StrPattern("012"), new StrPattern("30103"), new StrPattern("020202")}; 
    public static readonly string[] TYPE_NAME = {"wall", "tower", "stone_tower", "mine", "mage_tower",
     "solder", "knight", "archer", "mage", "dragon"};
    public static readonly float[] T_MAX_HEALTH = {12.0f, 24.0f, 36.0f, 24.0f, 40.0f, 4.0f, 8.0f, 3.0f, 6.0f, 16.0f};
    public static readonly float[] T_BUILD_SPEED = {0.5f, 0.3f, 0.2f, 0.3f, 0.1f, 0.5f, 0.3f, 0.5f, 0.2f, 0.1f};
    public static readonly float[] T_BUILD_COST = {1.0f, 3.0f, 4.0f, 3.0f, 8.0f, 2.0f, 5.0f, 3.0f, 5.0f, 6.0f};
    public static readonly float[] T_ATTACK_TIMEOUT = {0.0f, 1.8f, 1.2f, 0.0f, 0.0f, 1.0f, 1.0f, 2.0f, 2.0f, 3.0f};
    public static readonly float[] T_ATTACK_TIME = {0.0f, 0.5f, 0.5f, 0.0f, 0.0f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f};
    public static readonly float[] TYPE_SPEED = {0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 2.4f, 1.8f, 2.4f, 1.8f, 1.2f};
    public static readonly float[] TYPE_DAMAGE = {0.0f, 0.3f, 0.3f, 0.0f, 0.0f, 0.7f, 1.4f, 0.5f, 0.9f, 2.1f};
    public static readonly float[] T_HEALTH_BAR_Y_POS = {4.0f, 6.5f, 6.0f, 4.0f, 10.0f, 3.0f, 3.0f, 3.0f, 3.0f, 3.0f}; 
    public static readonly int[] T_SUBTYPE = {BUILDING_T, BUILDING_T, BUILDING_T, BUILDING_T, BUILDING_T, 
    UNIT_T, UNIT_T, UNIT_T, UNIT_T, UNIT_T};
    public static readonly int[] T_NEEDED_TECH = {0, 0, 1, 0, 0, 0, 1, 1, 2, 3}; 
    public static readonly Vec2I MAP_SIZE = new Vec2I(5, 3);

    public static readonly PackedScene ARROW_PS = LoadPS("arrow");
    public static readonly Mesh BASIC_UNIT = LoadMesh("solder/idle/0"); 

    public static readonly PackedScene[] T_ARROW = {null, ARROW_PS, ARROW_PS, null, null, null, null, ARROW_PS, LoadPS("energy_ball"), LoadPS("fire_ball")};
    public static readonly Mesh[] TYPE_MESH = {LoadMesh("wall"), LoadMesh("tower"), 
    LoadMesh("stone_tower"), LoadMesh("mine"), LoadMesh("mage_tower"), 
    BASIC_UNIT, BASIC_UNIT, BASIC_UNIT, BASIC_UNIT, BASIC_UNIT};
    public static readonly PackedScene buildingPS = LoadPS("building");
    public static readonly PackedScene towerPS = LoadPS("tower");
    public static readonly PackedScene unitPS = LoadPS("unit");
    public static readonly PackedScene archerPS = LoadPS("archer");
    public static readonly Material player0M = LoadM("blue_m");
    public static readonly Material player0TempM = LoadM("blue_temp_m");
    public static readonly Material player1M = LoadM("red_m");
    public static readonly Material player1TempM = LoadM("red_temp_m");
    public static readonly Material hBarM = LoadM("health_bar_m");
    public static readonly Material aPlayerHBarM = LoadM("a_player_health_bar_m");

    public static PackedScene LoadPS(string name)
    {
        return (PackedScene)ResourceLoader.Load("res://Scenes/" + name + ".tscn");
    }

    public static Material LoadM(string name)
    {
        return (Material)ResourceLoader.Load("res://Materials/" + name + ".tres");
    }

    public static Mesh LoadMesh(string name)
    {
        return (Mesh)ResourceLoader.Load("res://Models/" + name + ".obj");
    }

    public static int FindPattern(string s)
    {
        int ans = -1;
        bool b = false;
        if (s == null)
        {
            return ans;
        }
        for (int i = 0; i < TYPES_NUM; i++)
        {
            b = TYPE_PATTERN[i].IsEqual(s);
            if (b)
            {
                ans = i;
                break;
            }
        }
        return ans;
    }

    public static Vector2 Vec3ToVec2(Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }

    public static Vector3 Vec2ToVec3(Vector2 v)
    {
        return new Vector3(v.x, 0.0f, v.y);
    }

}
