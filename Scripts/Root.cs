using Godot;
using System;
using System.Collections.Generic;
using static Lib;

public class Root : Node
{

    public Vector2[] unitTargetPos;
    public float[] money;
    public int[] techLevel;
    public int[,,] buildZone;
    public int guiInput;

    public GameMap map;
    public Spatial objects;
    public Vector3 mouseMapPos;
    public Vector2 mousePos;
    public int mouseMode;
    public int activePlayer;

    private Vec2I[] aiMines = {new Vec2I(MAP_SIZE.x - 1, 0), new Vec2I(MAP_SIZE.x - 1, -1), 
     new Vec2I(0, 0), new Vec2I(0, -1), new Vec2I(-1, 0), new Vec2I(-1, -1), 
    new Vec2I(-MAP_SIZE.x, 0), new Vec2I(-MAP_SIZE.x, -1)};
    private CommandsCanvas commands;
    private StrategyCCanvas sCommandsP0;
    private StrategyCCanvas sCommandsP1;
    private Label patternTypeLabel;
    private RayCast ray;
    private Camera camera;
    private Castle castleP0;
    private Castle castleP1;
    private Command c;
    private float timeFromAIStep; 
    private int aiStep;
    private int aiPlayer;

    public Vector3 GetMapPos(Vector2 pos)
    {
        Vector3 v;
        ray.CastTo = RAYCAST_LEN * (camera.ProjectRayNormal(pos).Normalized());
        ray.ForceRaycastUpdate();
        v = new Vector3(ray.GetCollisionPoint());
        ray.CastTo = Vector3.Zero;
        return v;
    }

    public Spatial CreateObj(Vector3 pos, PackedScene ps)
    {
        try
        {
            Spatial obj = (Spatial)ps.Instance();
            objects.AddChild(obj);
            obj.GlobalTransform = new Transform(obj.GlobalTransform.basis, pos);
            return obj;
        }
        catch
        {
            GD.Print("Can't create obj.");
            return null;
        }
    }

    public void EndGame(Castle c) // remake
    {
        int x = -1;
        if (c == castleP1)
        {
            x = 0;
        }
        if (c == castleP0)
        {
            x = 1;
        }
        if (activePlayer == x)
        {
            GD.Print("You win.");
            GetTree().Paused = true;
            ((Control)GetNode("/root/Game/GUI/WinPanel")).Visible = true;
        }
        else
        {
            GD.Print("You lose.");
            GetTree().Quit();
        }
    }

    public void MakeAIStep(int p) // only for player 1
    {
        int x = 0;
        unitTargetPos[p] = new Vector2(-(MAP_SIZE.x + 1) * map.CellSize.x, 0.0f);
        for (int i = 0; i < aiMines.Length; i++)
        {
            map.Build(aiMines[i], 3, p);
        }
        if (aiStep % 120 == 111)
        {
            map.Build(new Vec2I(MAP_SIZE.x - 1, -MAP_SIZE.y), 4, p);
            map.Build(new Vec2I(MAP_SIZE.x - 1, MAP_SIZE.y - 1), 4, p);
        }
        if (aiStep % 12 == 11)
        {
            x = 0;
            for (int i = MAP_SIZE.x - 1; i >= -MAP_SIZE.x && x <= 0; i--)
            {
                x += map.Build(new Vec2I(i, 1), -2, p)?1:0;
                x += map.Build(new Vec2I(i, 4), 1, p)?1:0;
            }
        }
        if (aiStep % 6 == 3 && aiStep % 120 < 90 && aiStep > 70)
        {
            x = 0;
            for (int i = MAP_SIZE.x - 1; i >= -MAP_SIZE.x && x < MAX_AI_UNITS_GEN; i--)
            {
                for (int j = MAP_SIZE.y - 1; j >= -MAP_SIZE.y && x < MAX_AI_UNITS_GEN; j--)
                {
                    x += map.Build(new Vec2I(i, j), 7, p)?1:0;
                    x += map.Build(new Vec2I(i, j), 5, p)?1:0;
                }
            }
        }
    }

    public void StartGame()
    {
        for (int i = 0; i < PLAYERS_NUM; i++)
        {
            unitTargetPos[i] = Vector2.Zero; //
            money[i] = START_MONEY; 
            techLevel[i] = BASE_TECH_LEVEL;
            for (int j = 0; j < 2 * MAP_SIZE.x; j++)
            {
                for (int p = 0; p < 2 * MAP_SIZE.y; p++)
                {
                    if((i == 0 && j == 0) || (i == 1 && j == 2 * MAP_SIZE.x - 1))
                    {
                        buildZone[i, j, p] = 1;
                    }
                    else
                    {
                        buildZone[i, j, p] = 0;
                    }
                }
            }
        }
        if (objects != null)
        {
            var c = objects.GetChildren();
            if (c != null)
            {
                for (int i = 0; i < c.Count; i++)
                {
                    if (c[i] is Spatial)
                    {
                        ((Spatial)c[i]).QueueFree();
                    }
                }
            }
        }
        sCommandsP1.ClearLines();
        sCommandsP0.ClearLines();
        castleP0.SetHealth(MAX_CASTLE_HEALTH);
        castleP1.SetHealth(MAX_CASTLE_HEALTH);
        map.StartGame();
        timeFromAIStep = 0;
        aiStep = 0;
    }

    public override void _Ready()
    {
        commands = (CommandsCanvas)GetNode("/root/Game/Commands");
        sCommandsP0 = (StrategyCCanvas)GetNode("/root/Game/StrategyCommandsP0");
        sCommandsP1 = (StrategyCCanvas)GetNode("/root/Game/StrategyCommandsP1");
        patternTypeLabel = (Label)GetNode("/root/Game/GUI/Game/PatternName");
        ray = (RayCast)GetNode("/root/Game/Ray");
        camera = (Camera)GetNode("/root/Game/Camera");
        map = (GameMap)GetNode("/root/Game/Map");
        objects = (Spatial)GetNode("/root/Game/Objects");
        castleP0 = (Castle)GetNode("/root/Game/CastleP0");
        castleP1 = (Castle)GetNode("/root/Game/CastleP1");
        commands.color = Colors.Green; 
        sCommandsP1.color = sCommandsP0.color = Colors.Red;
        sCommandsP0.player = 0;
        sCommandsP1.player = 1;
        castleP0.SetPlayer(0);
        castleP1.SetPlayer(1);
        ray.Translation = camera.Translation;
        unitTargetPos = new Vector2[PLAYERS_NUM];
        money = new float[PLAYERS_NUM];
        techLevel = new int[PLAYERS_NUM];
        buildZone = new int[PLAYERS_NUM, 2 * MAP_SIZE.x, 2 * MAP_SIZE.y];
        activePlayer = 0;
        aiPlayer = 1;
        StartGame();
        guiInput = 0;
    }

    public override void _Process(float delta)
    {
        List<Vector2> points;
        Vector3 u;
        Vector3 v;
        if (GetTree().Paused)
        {
            return;
        }
        mousePos = GetViewport().GetMousePosition();
        mouseMapPos = GetMapPos(mousePos);
        timeFromAIStep += delta;
        if (timeFromAIStep >= AI_TIMEOUT)
        {
            MakeAIStep(aiPlayer);
            aiStep++;
            timeFromAIStep = 0;
        }
        if (Input.IsActionJustPressed("build_m_mode"))
        {
            mouseMode = BUILD_M_MODE;
        }
        if (Input.IsActionJustPressed("strategy_m_mode"))
        {
            ((activePlayer == 0) ? sCommandsP0 : sCommandsP1).ClearLines();
            mouseMode = STRATEGY_M_MODE;
        }
        if (Input.IsActionJustPressed("target_m_mode"))
        {
            mouseMode = TARGET_M_MODE;
        }
        if (guiInput <= 0)
        {
            if (mouseMode == TARGET_M_MODE)
            {
                unitTargetPos[activePlayer] = Vec3ToVec2(mouseMapPos);
            }
            if (Input.IsActionJustPressed("left_m_click"))
            {
                commands.ClearLines();
                if (mouseMode != TARGET_M_MODE)
                {
                    c = new Command();
                }
                else
                {
                    c = null;
                }
            }
            if (Input.IsActionPressed("left_m_click"))
            {
                if (c != null && mouseMode != TARGET_M_MODE)
                {
                    c.AddPoint(mousePos);
                    if (mouseMode == BUILD_M_MODE)
                    {
                        points = c.GetPoints();
                        if (points != null)
                        {
                            points.Add(mousePos);
                            commands.ClearLines();
                            for (int i = 1; i < points.Count; i++)
                            {
                                // GD.Print(points[i]);
                                commands.AddLine(new Line(points[i - 1], points[i]));
                            }
                        }
                    }
                }
            }
            if (Input.IsActionJustReleased("left_m_click"))
            {
                if (mouseMode == TARGET_M_MODE)
                {
                    mouseMode = BUILD_M_MODE;
                }
                else
                {
                    if (c != null)
                    {
                        c.End();
                        string result = c.GetResult();
                        int p = 0;
                        if (mouseMode == BUILD_M_MODE)
                        {
                            commands.ClearLines();
                            if (result != null)
                            { 
                                points = c.GetPoints();
                                p = FindPattern(result);
                                patternTypeLabel.Text = "PATTERN TYPE: "+ ((p == -1)?"NO_PATTERN":TYPE_NAME[p]);
                                if (p != -1)
                                {
                                    if (p == WALL_TYPE && points != null && points.Count > 1)
                                    {
                                        u = map.WorldToMap(GetMapPos(points[0]));
                                        v = map.WorldToMap(GetMapPos(points[points.Count - 1]));
                                        for (int i = (int)Mathf.Min(u.x, v.x); i <= (int)Mathf.Max(u.x, v.x); i++)
                                        {
                                            for (int j = (int)Mathf.Min(u.z, v.z); j <= (int)Mathf.Max(u.z, v.z); j++)
                                            {
                                                map.Build(new Vec2I(i, j), p, activePlayer);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        map.Build(GetMapPos(c.GetCenter()), p, activePlayer);
                                    }
                                }
                                // GD.Print(result);
                            }
                        }
                        if (mouseMode == STRATEGY_M_MODE)
                        {
                            points = c.GetPoints();
                            if (points != null)
                            {
                                for (int i = 1; i < points.Count; i++)
                                {
                                    ((activePlayer == 0)?sCommandsP0:sCommandsP1).AddLine(new Line(points[i - 1], points[i]));
                                }
                            }
                        }
                        c = null;
                    }
                }
            }
        }
    }

}
