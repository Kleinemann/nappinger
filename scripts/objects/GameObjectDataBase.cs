using Godot;

public enum GameObjectState { 
    IDLE,
    WALKING,
    WORKING,
    FIGHTING,
    DEAD
}


public class GameObjectDataBase
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Texture2D Icon { get; set; }
    public int Healt { get; set; } = int.MaxValue;
    public int MaxHealt { get; set; } = int.MaxValue;
    public Inventory Inventory;
}


public class GameObjectDataMoveable : GameObjectDataBase
{
    public float Speed { get; set; } = 100f;
    public GameObjectState State { get; set; } = GameObjectState.IDLE;
}