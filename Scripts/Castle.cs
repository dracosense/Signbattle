using Godot;
using System;

public class Castle : GameObj
{

    protected float maxHealth;

    public void SetHealth(float h)
    {
        health = maxHealth = h;
    }

    public override void Destroy()
    {
        root.EndGame(this);
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        healthBar.Scale = new Vector3((maxHealth == 0.0f?1.0f:(health / maxHealth)), 1.0f, 1.0f);
    }

}

