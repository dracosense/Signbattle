using Godot;
using System;
using System.Collections.Generic;
using static Lib;

public class StrategyCCanvas : CommandsCanvas
{

    public int player = -1;

    private List<Line> mapLines;
    private PhysicsDirectSpaceState space;

    public override void AddLine(Line line)
    {
        base.AddLine(line);
        if (mapLines == null)
        {
            return;
        }
        Line mapLine = new Line();
        mapLine.begin = Vec3ToVec2(root.GetMapPos(line.begin));
        mapLine.end = Vec3ToVec2(root.GetMapPos(line.end)); 
        mapLines.Add(mapLine);
    }

    public override void ClearLines()
    {
        base.ClearLines();
        if (mapLines == null)
        {
            return;
        }
        mapLines.Clear();
    }

    public override void _Ready()
    {
        base._Ready();
        mapLines = new List<Line>();
    }

    public override void _Process(float delta)
    {
        Unit unit;
        Vector2 v;
        base._Process(delta);
        if (player >= 0)
        {
            for (int i = 0; i < mapLines.Count; i++)
            {
                for (int j = 0; j < root.map.units[player].Count; j++)
                {
                    unit = root.map.units[player][j].GetRef() as Unit;
                    if (unit != null)
                    {
                        v = Vec3ToVec2(unit.GlobalTransform.origin);
                        if ((mapLines[i].end - v).Length() > UNIT_S_TARGET_DIST && unit.attacker == null &&
                        Geometry.SegmentIntersectsCircle(mapLines[i].begin, mapLines[i].end, v, UNIT_STRATEGY_R) >= 0)
                        {
                            unit.target = mapLines[i].end;
                        }
                    }
                }
            }
        }
    }

}
