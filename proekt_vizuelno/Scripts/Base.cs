using Godot;
using System;

public partial class Base : Area2D //Базата од играчот
{

    public int BaseHealth { get; set; } = 150; //Животот на базата
    public float BaseRange { get; set; } = 30f; //Опсегот кој се користи да се регистрира дали непријателот успешно влегов внатре во базата(дали успешно ја нападнал односно направил damage)

    public override void _Ready()
	{
        QueueRedraw(); //Се црта базата при стартување на играта
    }

	
	public override void _Process(double delta)
	{
	}

    public bool IsHit(BaseEnemy enemy) // Со евклидово растојание пресметуваме дали непријателот е внатре во опсегот на базата исто како што пресметуваме дали непријателот е внатре во опсегот на кулата
    {
        float distance = GlobalPosition.DistanceTo(enemy.GlobalPosition);
        return distance < BaseRange;
    }

    public override void _Draw()
    {
        DrawCircle(Vector2.Zero, BaseRange, new Color(0, 1, 0, 0.3f)); //Го цртаме опсегот на базата
    }
}
