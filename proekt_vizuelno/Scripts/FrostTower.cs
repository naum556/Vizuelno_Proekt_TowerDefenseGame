using Godot;
using System;

public partial class FrostTower : Tower //Исто како Knight кулата но со различни stats
{
    //Оваа кула има само една нова карактеристика која другите кули ја немаат, оваа кула може да ја намалува брзината на напаѓачите
    private float Slow;

    public FrostTower() : base(5, 250f, 6.5f, 250)
    { 
        Slow = 55.5f;
    }

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
        float minSpeed = enemy.OriginalSpeed * 0.4f; //Минималната брзина која може да ја имаат непријателит од кога ќе им биде намалена
        if (enemy.Speed > minSpeed) //Овој метод се користи за да не има шанса да се претвори брзината на напаѓачот во негативна вредност
        {
            enemy.Speed = Math.Max(minSpeed, enemy.Speed - Slow); // Осигурува дека брзината на непријателот никогаш не оди под дефинираниот минимум кога ќе биде успорен
            GD.Print($"Enemy slowed! Current speed: {enemy.Speed}");
        }
        else
        {
            GD.Print("Enemy is already slowed to minimum.");
        }
        GD.Print($"enemy has been slowed!");

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

            TowerDamage += 5;
            Range += 10.5f;
            Slow += 10.0f;
            if (Cooldown > 0)
            {
                Cooldown -= 0.8f;
            }


            QueueRedraw();
            GD.Print("Damage: " + TowerDamage + " Range: " + Range + " Cooldown: " + Cooldown + " Slow: " + Slow);
        }
        if (UpgradeLevel == 3)
        {
            GetNode<AnimatedSprite2D>("AnimatedSprite2D2").Visible = false;
            GetNode<AnimatedSprite2D>("AnimatedSprite2D3").Visible = true;

            TowerDamage += 5;
            Range += 13.5f;
            Slow += 15.5f;
            if (Cooldown > 0)
            {
                Cooldown -= 1.5f;
            }


            GD.Print("Damage: " + TowerDamage + " Range: " + Range + " Cooldown: " + Cooldown + " Slow: "+Slow);
            QueueRedraw();
        }

    }


}
