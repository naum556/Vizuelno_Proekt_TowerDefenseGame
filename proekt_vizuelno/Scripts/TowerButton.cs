using Godot;
using System;

public partial class TowerButton : Button //Копче кое се користи за селектирање на кула
{

    private Node2D tmpTower = null; // Привремено прикажување на кулата се додека не кликнеме на некој дел од теренот на нивото. Ова ни помогнува да знаеме каде точно ја поставуваме кулата кога кликаме не некој дел од теренот со курсорот
    private bool isPlacingTower = false; // Проверува дали поставуваме кула на теренот

    [Export]
    public PackedScene TowerScene; //Сцена од кулата која ја поставуваме внатре во главната сцена односно Map1
    public override void _Ready() // Ready() функцијата се повукува при стартување на играта/сцената
    {
        Pressed += () => //При кликање на копчето за кула се емитува lambda функција до Pressed сигналот од копчето, кога кочето ќе се кликен кодот од оваа функција ќе се изврши
        {
            GD.Print("Tower button clicked!");
            var map = GetNode<Map1>("/root/Map1"); //Ја земаме сцената
            map.StartPlacingTower(TowerScene); //На главната сцена ја повикуваме функција StartPlacingTower
        };
    }

}
