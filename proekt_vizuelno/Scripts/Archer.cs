using Godot;
using System;

public partial class Archer : Tower //Исто како Knight кулата но со различни stats
{

    public Archer() : base(30, 220f, 2.3f, 50) { }

    private AnimatedSprite2D SwordSlash;
    private Timer SlashTimer;

    public override void _Ready()
    {
        base._Ready();

        GetNode<AnimatedSprite2D>("AnimatedSprite2D2").Visible = false;

        SwordSlash = GetNode<AnimatedSprite2D>("Attack");
        SwordSlash.Visible = false;

        SlashTimer = GetNode<Timer>("SlashTimer");
        SlashTimer.Timeout += OnSlashTimeout;

    }

    public override void Attack(BaseEnemy enemy)
    {
        if (!canAttack) return;

        GD.Print($"{enemy.Name} is in range of {Name}");
        enemy.TakeDamage(TowerDamage);
        GD.Print($"{enemy.Name} health = {enemy.Health}");

        if (SwordSlash != null)
        {
            SwordSlash.Visible = true;
            SlashTimer.Start();
        }

        canAttack = false;
        attackCooldownTimer.Start();
    }

    private void OnSlashTimeout()
    {
        SwordSlash.Visible = false;
    }


    public override void UpgradeTowerLevel()
    {

        if (UpgradeLevel >= MaxUpgradeLevel)
        {
            GD.Print($"{Name} is already at max upgrade level.");
            return;
        }

        UpgradeLevel++;

        if (UpgradeLevel == 2)
        {
            GetNode<AnimatedSprite2D>("AnimatedSprite2D").Visible = false;
            GetNode<AnimatedSprite2D>("AnimatedSprite2D2").Visible = true;

            TowerDamage += 25;
            Range += 10.5f;
            if (Cooldown > 0)
            {
                Cooldown -= 0.8f;
            }


            QueueRedraw();
            GD.Print("Damage: " + TowerDamage + " Range: " + Range + " Cooldown: " + Cooldown);
        }
        if (UpgradeLevel == 3)
        {
            GetNode<AnimatedSprite2D>("AnimatedSprite2D2").Visible = false;
            GetNode<AnimatedSprite2D>("AnimatedSprite2D3").Visible = true;

            TowerDamage += 35;
            Range += 13.5f;
            if (Cooldown > 0)
            {
                Cooldown -= 1.5f;
            }


            GD.Print("Damage: " + TowerDamage + " Range: " + Range + " Cooldown: " + Cooldown);
            QueueRedraw();
        }

    }


}
