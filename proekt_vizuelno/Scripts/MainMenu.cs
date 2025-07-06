using Godot;
using System;

public partial class MainMenu : Control // Main Menu кое се прикажува при стартовање на играта
{
    private Button startButton; //Дугме за стартовање на играта
    private Button howToPlayButton; //Дугме кој прикажува нов панел каде се објаснуваат главните функционалности на играта
    private Button quitButton; // Дугме кое не исклучува од играта
    private Panel howToPlayPanel; // Панел
    private Button exitButton; //Дугме за излегување од панелот


    public override void _Ready() 
	{
        //Ги земаме вредностите на сите елементи од сцената
        startButton = GetNode<Button>("VBoxContainer/StartGameButton");
        howToPlayButton = GetNode<Button>("VBoxContainer/HowToPlayButton");
        quitButton = GetNode<Button>("VBoxContainer/QuitGameButton");

        howToPlayPanel = GetNode<Panel>("HowToPlayPanel");
        exitButton = GetNode<Button>("HowToPlayPanel/ExitButton");

        //При клик на копчињата се повикуваат соодветните функции за секое копче
        startButton.Pressed += OnStartGamePressed;
        howToPlayButton.Pressed += OnHowToPlayPressed;
        quitButton.Pressed += OnQuitGamePressed;
        exitButton.Pressed += ExitHowToPlayPanel;
    }

    private void OnStartGamePressed()
    {
        
        GetTree().ChangeSceneToFile("res://Scenes/map_1.tscn"); 
    }

    private void OnHowToPlayPressed()
    {
        howToPlayPanel.Visible = true;
    }

    private void ExitHowToPlayPanel()
    {
        howToPlayPanel.Visible = false;
    }

    private void OnQuitGamePressed()
    {
        
        GetTree().Quit();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
