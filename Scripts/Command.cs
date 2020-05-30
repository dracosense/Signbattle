using Godot;
using System;
using System.Collections.Generic;
using static Lib;

public class Command
{

    public const int MIN_C_NUM = 2;
    public const int MIN_C_LEN = 20;
    public const int D_NUM = 4;

    protected List<Vector2> points;
    protected Vector2 lastPoint;
    protected Vector2 dStartPoint;
    protected string result;
    protected int lastD;
    protected int lastDNum;
    protected int pointsNum;
    protected bool lastDAdded;
    protected bool ended;

    public Command()
    {
        points = new List<Vector2>();
        lastD = -1;
        lastDNum = 0;
        pointsNum = 0;
        lastDAdded = true;
        ended = false;
    }

    public int GetD(Vector2 v)
    {
        float a = 0;
        if (v == Vector2.Zero)
        {
            return 0;
        }
        a = D_NUM * (v.Angle() / (2.0f * Mathf.Pi));
        /*if (a < 0.0f) // ?? more ??
        {
            a += D_NUM;
        }*/
        for (; a < 0.0f;) // ?? never more then one ??
        {
            a += D_NUM;
        }
        return (Mathf.RoundToInt(a) % D_NUM);
    }

    public void AddPoint(Vector2 v)
    {
        if ((pointsNum > 0 && v == lastPoint) || ended)
        {
            return;
        }
        int d = 0;
        switch (pointsNum)
        {
            case 0:
                dStartPoint = v;
                points.Add(v);
                break;
            case 1:
                lastD = GetD(v - lastPoint);
                lastDNum++;
                lastDAdded = false;
                break;
            default:
                d = GetD(v - lastPoint);
                if (lastD == d)
                {
                    lastDNum++;
                }
                else
                {
                    lastD = d;
                    dStartPoint = lastPoint;
                    lastDNum = 1;
                    lastDAdded = false;
                }
                break;
        }
        if (result != null && result.Length > 0 && lastD == (int)result[result.Length - 1] - '0')
        {
            lastDAdded = true;
        }
        if ((v - dStartPoint).Length() >= MIN_C_LEN && lastDNum >= MIN_C_NUM && !lastDAdded)
        {
            points.Add(dStartPoint);
            result += (char)(lastD + '0');
            lastDAdded = true;
        }
        lastPoint = v;
        pointsNum++;
    }

    public void End()
    {
        points.Add(lastPoint);
        ended = true;
    }

    public Vector2 GetCenter() // ??
    {
        Vector2 center = Vector2.Zero;
        for (int i = 0; i < points.Count; i++)
        {
            center += points[i];
        }
        center /= points.Count;
        return center;
    }

    public List<Vector2> GetPoints()
    {
        return new List<Vector2>(points);
    }


    public string GetResult()
    {
        return result;
    }

}
