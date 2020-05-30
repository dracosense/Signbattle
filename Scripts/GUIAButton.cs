using Godot;
using System;
using static Lib;

public class GUIAButton : Button
{

    protected Root root;
    protected uint releaseAction = 0;

    public void _on_button_down()
    {
        root.guiInput++;
        Input.ActionPress(this.Name);
    }

    public void _on_button_up()
    {
        Input.ActionRelease(this.Name);    
        releaseAction++;    
    }

    public override void _Ready()
    {
        root = (Root)GetNode("/root/root");
    }

    public override void _Process(float delta)
    {
        int x = 0;
        switch (this.Name)
        {
            case "build_m_mode":
                x = BUILD_M_MODE;
                break;
            case "strategy_m_mode":
                x = STRATEGY_M_MODE;
                break;
            case "target_m_mode":
                x = TARGET_M_MODE;
                break;
            default:
                x = -1;
                break;
        }
        //this.Disabled = !(root.mouseMode == x);
        this.SelfModulate = (root.mouseMode == x)?(new Color(1.0f, 1.0f, 1.0f, 0.4f)):(new Color(1.0f, 1.0f, 1.0f, 0.8f));
        if (releaseAction > 0)
        {
            root.guiInput -= (int)releaseAction;
            releaseAction = 0;
        }
    }

}
