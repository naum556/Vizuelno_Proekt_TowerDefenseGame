using Godot;
using System.Collections.Generic;

public abstract partial class BaseEnemy : PathFollow2D //Абстрактна функција која се користи како темплејт за сите видови на непријатели
{
    public float Speed { get; set; } //Брзината со која се движи непријателот
    public int Health { get; set; } //Колку живот има непријателот
    public bool isDefeated { get; set; } //Дали е убиен/поразен непријателот
    public int coinDrop { get; set; } //Колку пари/вредност добиваме ако го убиеме

    public int Damage { get; set; } //Колку живот ќе одземе на нашата база ако успешно влезе внатре во базата

    public bool HasDamagedBase { get; set; } = false; //Дали е успешно влезен во базата

    public float OriginalSpeed { get; set; } //Оригиналната брзина на непријателот

    [Signal]
    public delegate void EnemyDefeatedEventHandler(BaseEnemy enemy); //chatgpt

    public BaseEnemy(float speed, int health, bool isDefeated, int coinDrop, int damage)
    {
        Speed = speed;
        Health = health;
        this.isDefeated = isDefeated;
        this.coinDrop = coinDrop;
        Damage = damage;
    }

    public BaseEnemy() { }

    public override void _Ready()
    {
        OriginalSpeed = Speed;

        GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play(); //Анимацијата на непријателот

        var Healthbar = GetNode<ProgressBar>("Healthbar"); //Progress bar кој го прикажува животот на непријателот, ако тој прими damage тогаш Progress bar-от се намалува
        Healthbar.Value = Health; 
        Healthbar.MaxValue = Health;
        Healthbar.MinValue = 0;

    }

    public override void _Process(double delta)
    {
        Progress += Speed * (float)delta; //Движење на непријателот, delta е времето во секунди кое има поминато од последниот кадар/frame, ова покажува колку непријателот треба да се движи во еден кадар

        var healthBar = GetNode<ProgressBar>("Healthbar");

        if (Health <= 0) //Ако животот е помал или еднаков на нула тогаш непријателот е поразен и се емитува сигнал дека е поразен и се прави QueueFree() што го отстранува од главнат сцена
        {
            isDefeated = true;
            EmitSignal("EnemyDefeated", this); //chatgpt
            QueueFree();
        }
        else
        {
            healthBar.Value = Health; //Ако не е поразен готаш неговиот progress bar(healthbar) ја зема вредноста од неговиот Health
        }

    }

    public void TakeDamage(int amount) //Функција која се користи за намалување на Health/животот на непријателот
    {
        Health -= amount;

        if (Health < 0)
            Health = 0;

       
        var healthBar = GetNode<ProgressBar>("Healthbar"); 
        healthBar.Value = Health; //Се ажурира Healthbar-от на непријателот 

    }


}
