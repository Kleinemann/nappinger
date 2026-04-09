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
    public Texture2D Texture { get; set; }
    public int Healt { get; set; } = 10;
    public int MaxHealt { get; set; } = 10;
}

public class GameObjectDataMoveable : GameObjectDataBase
{
    public GameObjectState State { get; set; } = GameObjectState.IDLE;
}