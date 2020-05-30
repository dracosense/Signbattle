using Godot;
using System;
using static Lib;

public class GameGUI : Control
{

    private Root root;
    private Label mouseMode;
    private Label moneyP0;
    private Label moneyP1;

    public override void _Ready()
    {
        root = (Root)GetNode("/root/root");
        mouseMode = (Label)GetNode("MouseMode");
        moneyP0 = (Label)GetNode("MoneyP0");
        moneyP1 = (Label)GetNode("MoneyP1");
    }

    public override void _Process(float delta)
    {
        string s;
        switch (root.mouseMode)
        {
            case BUILD_M_MODE:
                s = "BUILD";
                break;
            case STRATEGY_M_MODE:
                s = "STRATEGY";
                break;
            case TARGET_M_MODE:
                s = "TARGET";
                break;
            default:
                s = "";
                break;
        }
        mouseMode.Text = "MOUSE MODE: " + s;
        moneyP0.Text = "MONEY: " + root.money[0].ToString();
        moneyP1.Text = "MONEY: " + root.money[1].ToString();
    }

}
