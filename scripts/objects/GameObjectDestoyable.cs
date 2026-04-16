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

    public Inventory Inventory; 
}

public class GameObjectDestoyable: GameObjectDataBase
{
    public int Healt { get; set; } = int.MaxValue;
    public int MaxHealt { get; set; } = int.MaxValue;
}


public class GameObjectDataMoveable : GameObjectDestoyable
{
    public string Direction = "d";
    public object Target;
    public float Speed { get; set; } = 100f;
    public GameObjectState State { get; set; } = GameObjectState.IDLE;
}