using Godot;
using System;

public partial class CavalierKnight : Tower //Исто како Knight кулата но со различни stats
{

    public CavalierKnight() : base(200, 165f, 8.5f, 150) { }

    private AnimatedSprite2D Idle;
    private AnimatedSprite2D SwordSlash;
    private AnimatedSprite2D SwordSlash2;
    private AnimatedSprite2D SwordSlash3;
    private Timer SlashTimer;

    public override void _Ready()
    {
        base._Ready();

        GetNode<AnimatedSprite2D>("AnimatedSprite2D2").Visible = false;

        Idle = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        SwordSlash = GetNode<AnimatedSprite2D>("Attack");
        SwordSlash.Visible = false;
        SwordSlash2 = GetNode<AnimatedSprite2D>("Attack2");
        SwordSlash2.Visible = false;
        SwordSlash3 = GetNode<AnimatedSprite2D>("Attack3");
        SwordSlash3.Visible = false;

        SlashTimer = GetNode<Timer>("SlashTimer");
        SlashTimer.Timeout += OnSlashTimeout;

    }

    public override void Attack(BaseEnemy enemy)
    {
        if (!canAttack) return;

        GD.Print($"{enemy.Name} is in range of {Name}");
        enemy.TakeDamage(TowerDamage);
        GD.Print($"{enemy.Name} health = {enemy.Health}");

        if (SwordSlash != null && UpgradeLevel == 1)
        {
            Idle.Visible = false;
            SwordSlash.Visible = true;
            SwordSlash.Play();
            SlashTimer.Start();
        }
        else if(SwordSlash2!=null && UpgradeLevel == 2)
        {
            Idle.Visible = false;
            SwordSlash.Visible = false;
            SwordSlash2.Visible = true;
            SwordSlash2.Play();
            SlashTimer.Start();
        }
        else if (SwordSlash3 != null && UpgradeLevel == 3)
        {
            Idle.Visible = false;
            SwordSlash.Visible = false;
            SwordSlash2.Visible = false;
            SwordSlash3.Visible = true;
            SwordSlash3.Play();
            SlashTimer.Start();
        }


        canAttack = false;
        attackCooldownTimer.Start();
    }

    private void OnSlashTimeout()
    {
        Idle.Visible = true;
        SwordSlash.Visible = false;
        SwordSlash2.Visible = false;
        SwordSlash3.Visible = false;
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
