using Godot;
using System;

public abstract partial class Tower : Area2D //Наследува од тип node Area2D
{
    public Timer attackCooldownTimer {  get; set; } //Тајмер кој се користи за контролирање на нападот на кулата
    protected bool canAttack { get; set; } = true;

    protected  int TowerDamage { get; set; }
    protected  float Range { get; set; } //Опсегот на кулата, до каде може да ги напаѓа непријателите
    protected  float Cooldown { get; set; } //Времето за која кулата чека за повторно да нападне

    public int Price { get; } //Цена на кулата

    public int UpgradeLevel { get; set; } = 1; //Моменталното ниво на кулата

    public int MaxUpgradeLevel { get; } = 3; //Максималното ниво која може да го има кулата

    public bool ShowRange = false; //Bool променлива која се користи за прикажување на range-от/опсегот на кулата

    public bool ShowBlockCircle = false; //Bool променлива која се користи за прикажување колку место завзема кулата на стазата со цел да може да се прикажува каде не може да се смести друга кула и да не се сместуваат кулите една до друга за еден пиксел разлика


    [Signal]
    public delegate void TowerClickedEventHandler(Tower tower);


    public Tower(int towerDamage, float range, float cooldown, int price)
    {
        TowerDamage = towerDamage;
        Range = range;
        Cooldown = cooldown;
        Price = price;
    }

    public override void _Ready()
    {
       
        GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play(); // Анимација на кулата кога е ниво 1
        GetNode<AnimatedSprite2D>("AnimatedSprite2D2").Play(); // Анимација на кулата кога е ниво 2
        GetNode<AnimatedSprite2D>("AnimatedSprite2D3").Play(); // Анимација на кулата кога е ниво 3
        GetNode<AnimatedSprite2D>("Attack").Play(); // Анимација кога кулата напаѓа непријател

        attackCooldownTimer = GetNode<Timer>("AttackCooldownTimer"); // Тајмерот кој се коирсити за контролирање на кулата за повторно да нападне

        attackCooldownTimer.OneShot = true; //Тајмерот прекинува кога ќе го пристигне својот крај
        attackCooldownTimer.Autostart = false;
        attackCooldownTimer.WaitTime = Cooldown; //Waittime односно чекањето на тајмерот е вредноста на Cooldown

        attackCooldownTimer.Timeout += () => canAttack = true; // Кога тајмерот ќе заврши canAttack се поставува на true, во друг случај кулата чека за повторно да нападне, ова е потребно за кулата да не е премногу силна и затоа се прави да не напаѓа многу брзо
    }


    public override void _Process(double delta)
    {

    }

    public bool IsInRange(BaseEnemy enemy) // Со користење на евклидово растојание се пресметува дали непријателот е влезен во оспегот на кулата
    {
        float distance = GlobalPosition.DistanceTo(enemy.GlobalPosition); //растојанието помеѓу кулата и непријателот
        return distance < Range; //Ако растојанието е помало од опсегот на кулата тогаш непријателот е внатре во опсегот на кулата
    }

    public virtual bool CanAttack()
    {
        return canAttack;
    }


    public abstract void Attack(BaseEnemy enemy); //Абстрактна функција која ја користат другите tower класи кои наследуваат од оваа класа
    

    public override void _Draw() //Метод за цртање на опсегот на кулата(range) и опсегот на кулата кој прикажува колку простор зафаќа
    {
        if (ShowRange)
        {
            DrawCircle(Vector2.Zero, Range, new Color(0, 1, 0, 0.3f)); //Го црта опсегот со зелена боја

            DrawCircle(Vector2.Zero, 75, new Color(1, 0, 0, 0.2f)); // Го црта опсегот кој прикажува колку простор зафаќа кулата со црвена боја
        }    
    }

    public void ShowRangeCircle() //Го прикажува опсегот
    {
        ShowRange = true;
        QueueRedraw();
    }

    public void HideRangeCircle() //Го крие опсегот
    {
        ShowRange = false;
        QueueRedraw();
    }

    public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx) // Овој метод е генериран со ChatGPT, целта на методот е да се детектира дали корисникот клика на кулата со лев клик
    {
        if (@event is InputEventMouseButton mouseEvent &&
            mouseEvent.Pressed &&
            mouseEvent.ButtonIndex == MouseButton.Left)
        {
            Node parent = this; // ова е кулата на која кликаме
            while (parent != null && parent is not Map1) //Преку while loop се качуваме по Scene tree почнувајќи од this, односно кулата на која кликнавме и оди нагоре со користење на .GetParent() се додека не стигне до root node или Null.
            {
                parent = parent.GetParent(); // Ја земаме вредноста на Towers Node кој служи како контејнер за чување на кулите
            }

            if (parent is Map1 map && !map.IsPlacingTower) //Проверува дали кулата припаѓа внатре во сцената односно Map1 а !map.IsPlacingTower проверува дали во моментот поставуваме кула
            {
                
                GD.Print($"{Name} was clicked and emitting signal.");
                EmitSignal("TowerClicked", this); //Емитира сигнал дека кулата е кликната и го праќа сигналот до Map1 класата
            }
        }
    }


    public virtual void UpgradeTowerLevel() //Апстрактна класа која се користи за зајакнување на кулата
    {
        
    }

}