using Godot;
using System;

public partial class Knight : Tower //Класа која наследува од класата Tower
{
    //Oва е Tower од тип Knight
    public Knight() : base(55, 150f, 2.5f, 35) { }

    private AnimatedSprite2D SwordSlash; //Анимација која се активира кога ќе напаѓа
    private Timer SlashTimer; //Тајмер за анимацијата

    public override void _Ready()
    {
        base._Ready();

        GetNode<AnimatedSprite2D>("AnimatedSprite2D2").Visible = false; //Анимација за кога кулата е ниво 2

        SwordSlash = GetNode<AnimatedSprite2D>("Attack"); //Анимација за напад
        SwordSlash.Visible = false; //Од првин Visible го поставуваме на false

        SlashTimer = GetNode<Timer>("SlashTimer");
        SlashTimer.Timeout += OnSlashTimeout; // Koга тајмерот ќе заврши се повикува функцијата OnSlashTimeout

    }

    public override void Attack(BaseEnemy enemy)
    {
        if (!canAttack) return; //Ако не може да нападне тогаш излези

        GD.Print($"{enemy.Name} is in range of {Name}");
        enemy.TakeDamage(TowerDamage); //Се повикува функција од класата BaseEnemy за непријателот да прими damage
        GD.Print($"{enemy.Name} health = {enemy.Health}");

        if(SwordSlash != null)
        {
            SwordSlash.Visible = true; // Се прави видлива анимацијата
            SlashTimer.Start(); // Се стартува тајмерот за анимацијата
        }

        canAttack = false; // Повторно не може да напаѓа
        attackCooldownTimer.Start(); // Се стартува тајмерот повторно да чека кулата за да нападне
    }

    private void OnSlashTimeout() //Анимацијата за напад се прави невидлива
    {
        SwordSlash.Visible = false;
    }


    public override void UpgradeTowerLevel() // Функција која служи да се зајакне кулата
    {

        if (UpgradeLevel >= MaxUpgradeLevel)
        {
            GD.Print($"{Name} is already at max upgrade level.");
            return;
        }

        UpgradeLevel++;

        if(UpgradeLevel == 2) //Ако кулата е ниво 2 тогаш се менува анимацијата односно изгледот на кулата
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
        if(UpgradeLevel == 3) //Ако кулата е ниво 3 тогаш се менува анимацијата односно изгледот на кулата
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
