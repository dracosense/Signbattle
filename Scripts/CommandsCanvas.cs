using Godot;
using System;
using System.Collections.Generic;
using static Lib;

public class CommandsCanvas : Control
{

    public Color color = Colors.White;
    public float width;

    protected List<Line> lines;
    protected Root root;

    public virtual void AddLine(Line line)
    {
        if (lines == null)
        {
            return;
        }
        lines.Add(line);
    }

    public virtual void ClearLines()
    {
        if (lines == null)
        {
            return;
        }
        lines.Clear();
    }

    public override void _Ready()
    {
        root = (Root)GetNode("/root/root");    
        lines = new List<Line>();
        width = BASE_C_WIDTH;
    }

    public override void _Process(float delta)
    {
            Update();
    }

    public override void _Draw()
    {
        if (lines == null)
        {
            return;
        }
        for (int i = 0; i < lines.Count; i++)
        {
            DrawLine(lines[i].begin, lines[i].end, color, width, true);
        }
    }

}
