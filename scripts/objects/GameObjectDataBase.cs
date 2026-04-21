using Godot;
using System.Linq;

public enum GameObjectState 
{ 
    NONE,
    IDLE,
    WALKING,
    WORKING,
    FARMING,
    FIGHTING,
    WAITING,
    DEAD
}

public class Mission
{
    public GameObjectState State;
    public Variant Target;

    public Mission(GameObjectState state, Variant target)
    {
        State = state;
        Target = target;
    }
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
    public Mission Mission { get; set; } = null;


    public static void RemoveFromTarget(Node2D remObject)
    {
        var animals = WorldMain.Instance.Map.GetChildren().Where(x => x is Animal).ToArray();
        foreach (Animal animal in animals)
        {
            if (animal.Target == remObject)
            {
                animal.Target = null;
                animal.State = GameObjectState.IDLE;
            }
        }
    }

}