using Godot;
using System;
using System.Collections.Generic;

//Кодот за главната сцена/ниво
public partial class Map1 : Node2D
{
    //Во Godot секој тип на enemy во суштина е тип на node(во овој случај PathFollow2D Node) чија класа наследува од класата BaseEnemy и мора да биде имплементиран како сцена која е внатре во главната сцена a тоа е Map1. 

    //Enemies
    [Export] public PackedScene GreenSlimeScene;
    [Export] public PackedScene RedSlimeScene;
    [Export] public PackedScene SkeletonScene;
    [Export] public PackedScene NinjaScene;
    [Export] public PackedScene BlueSlimeScene;
    [Export] public PackedScene DarkKnightScene;
    [Export] public PackedScene BatScene;
    [Export] public PackedScene DarkChampionScene;


    //Исто и за кулите

    //Towers
    [Export] public PackedScene KnightTowerScene;
    [Export] public PackedScene ArcherTowerScene;
    [Export] public PackedScene BanditTowerScene;
    [Export] public PackedScene CavalierKnightTowerScene;
    [Export] public PackedScene FrostTowerScene;

    private PackedScene selectedTowerScene; //Oваа сцена се користи како променлива за кула со која вршиме некаква интеракција - Селектирање, Обновување итн.

    private Timer spawnTimer; // Taјмер кој се користи за појавување на enemies внатре во сцената
    private Timer waveDelayTimer; // Тајмер кој се користи за одложување на времето на појавување на следниот бран на enemies со цел да не се појавуваат брановите наеднаш
    private PackedScene enemyScene; // Сцена од enemy
    private int count = 0; // бројот на enemies во еден бран
    private int initialCount = 0; // оригиналниот број на enemies во еден бран
    private int activeEnemies = 0; // Бројач кој го брои бројот на активни enemies односно enemies кои се појавени внатре во главната сцена
    private int wave = 1; // бројач кој ја чува вредноста на бранот кој треба следно да се појави
    public int Coins; // Вредноста внатре во играта која се користи за купување и обновување на кулите
    private bool isWaitingForTowerPlacement = false; // Проверува дали сме во состојба на ставање на кула
    private TileMap towerExclusion; // Теренот од нивото каде не можеме да поставиме кула
    private Label coin_label; //node кој содржи икона за Coins
    private Label heart_label;//node кој содржи икона за животот на базата

    private Path2D enemies; // Контејнер node каде во него се чуваат тековните enemies кои се појавуваат во сцената

    private Base MainBase; // Node за базата

    private Control upgradePanel; // Панел кој се активира кога треба да обновиме кула
    private Button upgradeButton; // Копче за обновување
    private Tower selectedTower; // Ова ја чува вредноста на тековната кула која ја имаме селектирано

    public bool IsPlacingTower => isWaitingForTowerPlacement;


    
    private Label gameOverLabel;
    private Label nextRoundLabel;

    private TextEdit level2_Info;
    private TextEdit level3_Info;

    private Tower tmpTower;

    private bool allEnemiesSpawned = false;

    private Label WinMessageLabel;

    [Export] public string MainMenuPath = "res://Scenes/main_menu.tscn"; //Главното мени , се зема целосната патека на менито за да можеме после да го повикаме

    public override void _Ready() //при стартување на сцената
    {
        Coins = 250; //Почетната вредност со која корисникот започнува во играта
        coin_label = GetNode<Label>("UI/Stats");
        CoinUpdate(); //функција која ја ажурира вредноста на Coins

        heart_label = GetNode<Label>("UI/Health");


        enemies = GetNode<Path2D>("Enemies"); // Контејнерот за enemies
        spawnTimer = GetNode<Timer>("SpawnTimer"); // Spawn timer
        waveDelayTimer = GetNode<Timer>("WaveDelayTimer"); // Wave delay timer

        waveDelayTimer.OneShot = true; // кога тајмерот заврши тој запира
        waveDelayTimer.Autostart = false; // започнувањето на тајмерот не е автоматски
        waveDelayTimer.WaitTime = 8; //Чекај 8 секунди и потоа повикај го следниот бран на enemies


        waveDelayTimer.Timeout += StartNextWave; // Кога тајмерот ќе заврши повикај ја функцијата StartNextWave


        spawnTimer.OneShot = true;
        spawnTimer.Timeout += SpawnEnemy; //povrzuvame timer so SpawnEnemy funkcijata

        SetSpawnEnemy(GreenSlimeScene, 3, 2.0f); //go spanovame prviot bran

        towerExclusion = GetNode<TileMap>("TowerExclusion");

        MainBase = GetNode<Base>("Base"); //Главната база
        HealthUpdate(); // Ажурирање на животот на базата

        //For upgrading character

        upgradePanel = GetNode<Control>("UI/UpgradePanel");
        upgradeButton = upgradePanel.GetNode<Button>("UpgradeButton");

        level2_Info = upgradePanel.GetNode<TextEdit>("level2_info");
        level3_Info = upgradePanel.GetNode<TextEdit>("level3_info");

        upgradePanel.Visible = false;
        upgradeButton.Pressed += OnUpgradeButtonPressed;

        level2_Info.Visible = false;
        level3_Info.Visible = false;

        //Closing the upgrade panel

        var closeButton = upgradePanel.GetNode<Button>("CloseButton");
        closeButton.Pressed += OnCloseButtonPressed;

        gameOverLabel = GetNode<Label>("UI/GameOverLabel");
        gameOverLabel.Visible = false;

        nextRoundLabel = GetNode<Label>("UI/NextRoundLabel");

        //Копчиња кои се користат во tower menu за селектирање на кули
        var knightButton = GetNode<Button>("UI/Panel/KnightButton");
        var archerButton = GetNode<Button>("UI/Panel/ArcherButton");
        var banditButton = GetNode<Button>("UI/Panel/BanditButton");
        var cavalierButton = GetNode<Button>("UI/Panel/CavalierButton");
        var frostTowerButton = GetNode<Button>("UI/Panel/FrostTowerButton");

        //За секоја кула се повикува StartPlacingTower(сцена од типот на кула) кога ќе стисниме на соодветното копче 
        knightButton.Pressed += () => StartPlacingTower(KnightTowerScene);
        archerButton.Pressed += () => StartPlacingTower(ArcherTowerScene);
        banditButton.Pressed += () => StartPlacingTower(BanditTowerScene);
        cavalierButton.Pressed += () => StartPlacingTower(CavalierKnightTowerScene);
        frostTowerButton.Pressed += () => StartPlacingTower(FrostTowerScene);

        //Порака за кога корисникот ќе ги помини сите бранови - победува
        WinMessageLabel = GetNode<Label>("UI/WinMessageLabel"); 
        WinMessageLabel.Visible = false;
    }

    private void CoinUpdate() // Прикажува вредноста на coins
    {
        String coins = Coins.ToString();
        coin_label.Text = "COINS: " + coins;
    }

    private void HealthUpdate()// Прикажува вредноста на животот на базата
    {
        String hitpoints = MainBase.BaseHealth.ToString();
        heart_label.Text = "HEALTH: " + hitpoints;
    }

    private void SetSpawnEnemy(PackedScene scene, int c, float delay) //go deklarirame tipot na enemy, brojot na enemies i vremeto na zadocnuvanje na pojavuvanje pomegju predhodniot enemy i sledniot enemy
    {
        count = c;
        initialCount = c;
        enemyScene = scene;
        spawnTimer.WaitTime = delay;

        allEnemiesSpawned = false; // boolеаn Променлива која се користи за да провериме дали сите enemies се појавени внатре во сцената со цел ако во некој случај непријателите ќе бидат уништени и отстранети од сцената премногу брзо и главната сцена ќе нема време да го инкрементира wave бројачот правилно

        spawnTimer.Start(); // Го запоичнуваме тајмерот
    }

    private void SpawnEnemy() // Функција за вметнување на enemy внатре во главната сцене/нивото
    {

        if (count > 0) // Ако бројот на enemies е поголем од 0:
        {

            var enemy = enemyScene.Instantiate<BaseEnemy>(); //go zemame enemy 
            float baseOffset = -80f;
            float spacing = 40f;
            float progress = baseOffset - ((initialCount - count) * spacing); // Со оваа пресметка се гарантира дека enemies секогаш ќе се појават на почетокот од нивниот предефиниран пат по кој тие треба да се движат и нема да се појават на некој случаен дел од патот

            // Clamp the minimum to avoid overshooting the path
            enemy.Progress = Mathf.Max(progress, 0f); // space

            enemy.Connect("EnemyDefeated", new Callable(this, nameof(OnEnemyDefeated))); //Овој дел е направен со ChatGPT со цел кога еден enemy е поразен тој повикува сигнал EnemyDefeated кој се поврзува и се повикува ОnEnemyDefeated функцијата

            enemies.AddChild(enemy); //go dodavame enemy vnatre vo Enemies container node

            activeEnemies++;
            enemy.TreeExited += OnEnemyRemoved; //ova go povikuva OnEnemyRemoved sekogas koga eden enemy ke bide izbrisan od igrata/ќе биде отстранет од главната сцена
            GD.Print("Currently active enemies: " + activeEnemies);

            GD.Print("Spawned enemy. Remaining: ", count - 1);
            count--; // Count се намалува бидејќи се појави веќе еден enemy од предефинираната група од enemies

            if (count > 0)
            {
                spawnTimer.Start(); // Ако count > 0 тогаш имаме уште enemies па се започнува spawnTimer Повторно да се вметне следниот enemy од бранот во сцената
            }
            else
            {
                allEnemiesSpawned = true; // Count == 0 значи сите enemies се појавија во сцената
            }
                
        }
    }

    private void OnEnemyDefeated(BaseEnemy enemy) // Кога еден enemy ќе биде поразен вредноста на Coins се зголемува со додавање на вредноста на coinDrop од тој enemy
    {
        Coins += enemy.coinDrop;
        GD.Print($"Enemy defeated! +{enemy.coinDrop} coins. Total: {Coins}");
        CoinUpdate(); //Се повикува повторно функцијата да се ажурира статусот на Coins
    }

    private void OnEnemyRemoved() // Оваа функција се повикува кога еден enemy ќе биде отстранет/поразен
    {

        activeEnemies--; // бројот на активни enemies се намалува


        GD.Print("Enemy removed. Remaining alive: ", activeEnemies);

        if (activeEnemies == 0 && allEnemiesSpawned) // Ако бројот на активни enemies е 0 и сите enemies се имет појавени внатре во сцената тогаш:
        {
            if(wave == 19) //Провери дали сме ги завршиле сите бранови
            {
                WinMessageLabel.Visible = true;

                Timer returnTimer = new Timer();
                returnTimer.OneShot = true;
                returnTimer.WaitTime = 3; // Show win message for 3 seconds
                AddChild(returnTimer);
                returnTimer.Timeout += () =>
                {
                    GetTree().ChangeSceneToFile(MainMenuPath); //Врати се назад во главното мени за да можеме повторно од почеток да ја започнеме играта
                };
                returnTimer.Start();
            }
            else
            {
                wave++; //Ако не сме на крајот зголеми го бројот на wave за да можеме да продолжеме со следниот бран

                GD.Print("Wave cleared! Waiting before next wave...");
                nextRoundLabel.Visible = true;
                waveDelayTimer.Start(); // Започни го одложувањето на појавувањето на следниот бран за 8 секунди
            }
        }
        else
        {
            nextRoundLabel.Visible = false;
        }
    }

    private void StartNextWave() //Во оваа функција се декларираат брановите и се вметнуваат во сцената за секој бран
    {
        GD.Print("Starting next wave...");

        if (wave == 2)
            SetSpawnEnemy(RedSlimeScene, 5, 3.5f);
        else if (wave == 3)
            SetSpawnEnemy(GreenSlimeScene, 7, 1.5f);
        else if (wave == 4)
            SetSpawnEnemy(SkeletonScene, 3, 2.5f);
        else if (wave == 5)
            SetSpawnEnemy(NinjaScene, 10, 0.5f);
        else if (wave == 6)
            SetSpawnEnemy(RedSlimeScene, 7, 2.5f);
        else if (wave == 7)
            SetSpawnEnemy(SkeletonScene, 8, 1.5f);
        else if (wave == 8)
            SetSpawnEnemy(GreenSlimeScene, 15, 1.0f);
        else if (wave == 9)
            SetSpawnEnemy(NinjaScene, 20, 1.0f);
        else if (wave == 10)
            SetSpawnEnemy(BlueSlimeScene, 1, 1.5f);
        else if (wave == 11)
            SetSpawnEnemy(GreenSlimeScene, 20, 1.0f);
        else if (wave == 12)
            SetSpawnEnemy(NinjaScene, 20, 1.2f);
        else if (wave == 13)
            SetSpawnEnemy(SkeletonScene, 8, 1.0f);
        else if (wave == 14)
            SetSpawnEnemy(NinjaScene, 25, 0.7f);
        else if (wave == 15)
            SetSpawnEnemy(DarkKnightScene, 5, 1.2f);
        else if (wave == 16)
            SetSpawnEnemy(DarkKnightScene, 8, 1.5f);
        else if (wave == 17)
            SetSpawnEnemy(BatScene, 50, 0.5f);
        else if (wave == 18)
            SetSpawnEnemy(DarkKnightScene, 15, 1.5f);
        else if (wave == 19)
            SetSpawnEnemy(DarkChampionScene, 1, 1.5f);



    }


    public override void _Process(double delta)
    {

        var enemiesNode = GetNode("Enemies"); // Контејнерoт за enemies
        var towersNode = GetNode("Towers"); //Контејнерот за towers



        if (isWaitingForTowerPlacement && tmpTower != null) 
        {
            Vector2 mousePos = GetGlobalMousePosition(); // Се зема вредноста на курсурот
            tmpTower.GlobalPosition = mousePos.Snapped(Vector2.One * 32); // Над курсурот се поставува привремената кула која ни покажува каде ќе биде сместена кулата
        }

        // Во овој for циклус изминуваме низ сите towers кои се внатре во контејнерот за towers
        foreach (Node towerNode in towersNode.GetChildren())
        {
            if (towerNode is Tower tower)
            {
                //Потоа внатре во вгнезден циклус ги изминуваме enemies во enemy контејнерот
                foreach (Node enemyNode in enemiesNode.GetChildren())
                {
                    if (enemyNode is BaseEnemy enemy)
                    {
                        //За секоја кула ако еден од овие enemies се наоѓа внатре во неговиот опсег (опсегот каде кулата може да напаѓа) тогаш повикај ја Attack функцијата која се извршува врз тој enemy кој го задоволува условот
                        if (tower.IsInRange(enemy) && tower.CanAttack())
                        {
                            GD.Print($"{enemy.Name} is in range of {tower.Name}");
                            tower.Attack(enemy);

                            GD.Print($"{enemy.Name} health = {enemy.Health}");
                            break;
                        }
                    }
                }
            }
        }

        //За секој enemy ако е внатре во опсегот на базата и нема одземено живот тогаш направи damage врз базата, отстрани го enemy од сцената и ажурирај го текстот кој го покажува статусот на базата
        foreach (Node enemyNode in enemiesNode.GetChildren())
        {
            if (enemyNode is BaseEnemy enemy)
            {
                if (MainBase.IsHit(enemy) && !enemy.HasDamagedBase)
                {
                    GD.Print($"{enemy.Name} has hit the base!");
                    MainBase.BaseHealth -= enemy.Damage;
                    HealthUpdate();
                    GD.Print($"{MainBase.BaseHealth} ---> Current Health");

                    enemy.HasDamagedBase = true;
                    enemy.QueueFree();

                    if (MainBase.BaseHealth <= 0) // Ако базата има живот со вредност 0 тогаш покажи го Game Over текстот и рестартирај ја сцената
                    {
                        GD.Print("Game Over! Restarting...");
                        gameOverLabel.Visible = true;
                        RestartGame();
                    }

                }
            }
        }

    }

    private void resetLevelStatus()
    {
        level2_Info.Visible = false;
        level3_Info.Visible = false;
    }


    private void OnTowerClicked(Tower tower) // Се повикува кога ќе кликнеме на кула
    {
        GD.Print($"{tower.Name} was clicked.");
        upgradePanel.Visible = true; // Панелот за Upgrade се појавува
        selectedTower = tower; 

        var towersNode = GetNode("Towers");
        foreach (Node towerNode in towersNode.GetChildren()) // Сите други кули го кријат индикаторот на опсег (кругот)
        {
            if (towerNode is Tower t)
            {
                t.HideRangeCircle();
            }
        }

        tower.ShowRangeCircle(); // Индикаторот на опсег се црта само кај селектираната кула

        level2_Info.Visible = false;
        level3_Info.Visible = false;

        int nextLevel = tower.UpgradeLevel + 1; // Се чува вредноста на следното ниво која кулата треба да биде ако направиме Upgrade
        string info = $"Level: {tower.UpgradeLevel}";

        // Enable upgrade button only if not maxed out
        if (tower.UpgradeLevel >= tower.MaxUpgradeLevel)
        {
            upgradeButton.Disabled = true;
        }
        else
        {
            int cost; // Цената за Upgrade
            if (nextLevel == 2)
            {
                level2_Info.Visible = true; // Прикажи го панелот за Upgrade level 2 ако кулате е ниво 1
                cost = 50; // цената е 50
            }
            else
            {
                level3_Info.Visible = true; // Прикажи го панелот за Upgrade level 3 ако кулате е ниво 2
                cost = 100; //цената е 100
            }
            info += $"\nUpgrade to level {nextLevel} for {cost} coins";
            upgradeButton.Disabled = false; // Ако кулата е максимално ниво тогаш копчето за Upgrade се оневозможува

        }

    }


    private void ConnectTowerClick(Tower tower) //chatgpt
    {
        tower.Connect("TowerClicked", new Callable(this, nameof(OnTowerClicked))); // Го поврзува TowerClicked сигналот од кулата на  OnTowerClicked методот
    }


    public override void _Input(InputEvent @event) // При кликање со лев клик
    {
        if (@event is InputEventMouseButton mouseEvent &&
            mouseEvent.Pressed &&
            mouseEvent.ButtonIndex == MouseButton.Left &&
            isWaitingForTowerPlacement)
        {
            Vector2 clickPosition = mouseEvent.Position;
            Vector2 snappedPosition = clickPosition.Snapped(Vector2.One * 32); // snap to tile grid, оваа линија е генерирана со chatgpt, Ja мести позицијата на курсорот на позиција до најблиската 32x32 мрежна ќелија од теренот на нивото.

            GD.Print("Tower placed at ", snappedPosition);

            var tower = selectedTowerScene.Instantiate<Tower>();

            if (IsValidTowerPlacement(snappedPosition) && !IsTowerAlreadyPlaced(snappedPosition) && !IsTowerTooClose(snappedPosition)) //Проверуваме дали позицијата на кулата која ја вметнуваме е валидна
            {

                if (Coins >= tower.Price) // Ако имаме доволно пари за да ја купиме кулата тогаш кулата ќе биде поставена
                {
                    Coins -= tower.Price; // Се намалува вредноста на Coins
                    CoinUpdate();
                    GD.Print($"{Coins} --> Coin value.");

                    tower.GlobalPosition = snappedPosition;
                    GetNode("Towers").AddChild(tower);

                    //  Delay connection to the next frame, after placement click
                    CallDeferred(nameof(ConnectTowerClick), tower);

                    GD.Print("Tower placed at ", snappedPosition);
                }
                else
                {
                    GD.Print("Not enough coins!");
                }
            }
            else
            {
                GD.Print("Invalid placement: blocked by TowerExclusion tile.");
            }

            isWaitingForTowerPlacement = false; // Повеќе не сме во состјба на поставување на кула бидејќи сега или ја сместивме или не успеавме со сместување ( немавме доволно пари или пробувавме да ја сместиме на дел од теренот кој не дозволува сместување)
            if (tmpTower != null)
            {
                tmpTower.QueueFree();
                tmpTower = null;
            }
            tower.HideRangeCircle();
        }
    }

    private bool IsTowerAlreadyPlaced(Vector2 position) // Проверува дали веќе постои кула на одредена позиција со цел да не можеме да редеме кули една над друга
    {
        var towersNode = GetNode("Towers");

        foreach (Node towerNode in towersNode.GetChildren()) // Ги проверуваме сите веќе сместени кули
        {
            if (towerNode is Tower tower)
            {
                // Snap both positions to same grid to ensure accuracy
                if (tower.GlobalPosition.Snapped(Vector2.One * 32) == position) // Ако постои кула на позицијата на која сакаме да ја сместиме новата кула тогаш врати true
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool IsTowerTooClose(Vector2 position) // Проверува дали кулата која сакаме да ја сместиме е во опсегот на кулата кој служи за опфаќање на простор
    {
        var towersNode = GetNode("Towers");
        foreach (Node towerNode in towersNode.GetChildren())
        {
            if (towerNode is Tower tower)
            {
                float distance = position.DistanceTo(tower.GlobalPosition);
                if (distance < 75)
                {
                    return true;
                }
            }
        }
        return false;
    }


    public void StartPlacingTower(PackedScene towerScene)
    {
        GD.Print("Placing tower...");
        isWaitingForTowerPlacement = true; // Сме во состојба на сместување на кула
        selectedTowerScene = towerScene; // Селектираната кула ја зема вредноста од сцена од кула

        if (tmpTower == null)
        {
            tmpTower = selectedTowerScene.Instantiate<Tower>(); 
            tmpTower.Modulate = new Color(1, 1, 1, 0.5f); // semi-transparent
            tmpTower.SetProcess(false); // stop any tower logic
            AddChild(tmpTower);
            tmpTower.QueueRedraw();
        }
    }

    private bool IsValidTowerPlacement(Vector2 globalPosition) // Проверува дали ја сместуваме кулата на терен кој припаѓа од towerExclusion која е TileMap каде на секој пиксел не може да се постави кула
    {
        Vector2I tileCoords = towerExclusion.LocalToMap(globalPosition);
        int cellId = towerExclusion.GetCellSourceId(0, tileCoords); // Layer 0

        return cellId == -1; // -1 means no tile exists => valid spot
    }

    //Upgrade button
    private void OnUpgradeButtonPressed() // При кликнување на Upgrade дугмето
    {
        if (selectedTower != null)
        {
            if (selectedTower.UpgradeLevel < selectedTower.MaxUpgradeLevel) // Ако нивото не е со максимална вредност
            {
                int nextLevel = selectedTower.UpgradeLevel + 1; // Следното ниво
                int cost = 0;

                if (nextLevel == 2) // Ако следното ниво е 2 тогаш трошокот ќе биде 50
                    cost = 50;
                else if (nextLevel == 3)// Ако следното ниво е 3 тогаш трошокот ќе биде 100
                    cost = 100;

                if (Coins >= cost) // Ако имаме доволно пари
                {
                    Coins -= cost; // Правиме трошок
                    CoinUpdate();

                    selectedTower.UpgradeTowerLevel(); // Кулата се ажурира
                    selectedTower.HideRangeCircle(); // Се крие кругот кој го прикажува опсегот
                    GD.Print($"{selectedTower.Name} upgraded to level {selectedTower.UpgradeLevel}!");

                    if (selectedTower.UpgradeLevel >= selectedTower.MaxUpgradeLevel) // Ако веќе сме го достигнале максималното ниво на кулата
                    {
                        upgradeButton.Disabled = true;
                        GD.Print("Max upgrade level reached, button disabled.");
                    }

                    upgradePanel.Visible = false;
                }
                else
                {
                    GD.Print($"Not enough coins to upgrade to level {nextLevel}. Required: {cost}, You have: {Coins}");
                }
            }
            else
            {
                GD.Print("Cannot upgrade, tower is at max level.");
                upgradeButton.Disabled = true;
            }
        }
    }



    private void OnCloseButtonPressed() // Кога сакаме да го исклучиме панелот за Upgrade
    {
        upgradePanel.Visible = false;
        GD.Print("Upgrade panel closed.");
    }


    //Restart the game
    private void RestartGame() // Функција која служи за ресетирање на играта од почеток
    {
        Timer timer = new Timer();
        timer.OneShot = true;
        timer.WaitTime = 2; // 2-second delay
        AddChild(timer);
        timer.Timeout += () => GetTree().ReloadCurrentScene();
        timer.Start();
    }

}