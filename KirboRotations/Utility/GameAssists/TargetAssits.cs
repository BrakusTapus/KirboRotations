using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using static FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject;

namespace KirboRotations.Utility.GameAssists;

public static class TargetingUtilities
{
    private static unsafe TargetSystem* targetSystem;

    static unsafe TargetingUtilities()
    {
        // Initialize TargetSystem
        targetSystem = TargetSystem.Instance();

        // Optionally log the initialization
        Serilog.Log.Debug("Initialized TargetSystem...");
    }

    /// <summary>
    /// Gets the current target of the player.
    /// </summary>
    /// <returns></returns>
    internal static unsafe FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject* GetCurrentTarget()
    {
        return targetSystem->GetCurrentTarget();
    }

    /// <summary>
    /// Retrieves the unique ID of the current target.
    /// </summary>
    /// <returns></returns>
    internal static unsafe ulong GetCurrentTargetID()
    {
        return targetSystem->GetCurrentTargetID();
    }

    /// <summary>
    /// Checks if a specified game object is within the player's view range. Requires the game object as a parameter.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static unsafe bool IsObjectInViewRange(FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject* obj)
    {
        return targetSystem->IsObjectInViewRange(obj);
    }

    /// <summary>
    /// Determines if a specified game object is currently on the player's screen. Requires the game object as a parameter.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static unsafe bool IsObjectOnScreen(FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject* obj)
    {
        return targetSystem->IsObjectOnScreen(obj);
    }

    /// <summary>
    /// Interacts with a specified game object, with an option to check line of sight. Requires the game object and a boolean for line of sight as parameters.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="checkLineOfSight"></param>
    /// <returns></returns>
    public static unsafe ulong InteractWithObject(FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject* obj, bool checkLineOfSight = true)
    {
        return targetSystem->InteractWithObject(obj, checkLineOfSight);
    }

    /// <summary>
    /// Opens interaction with a specified game object. Requires the game object as a parameter.
    /// </summary>
    /// <param name="obj"></param>
    public static unsafe void OpenObjectInteraction(FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject* obj)
    {
        targetSystem->OpenObjectInteraction(obj);
    }

    /// <summary>
    /// Gets the game object that the mouse is currently hovering over, based on screen coordinates. Requires the screen coordinates as parameters.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static unsafe FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject* GetMouseOverObject(int x, int y)
    {
        return targetSystem->GetMouseOverObject(x, y);
    }


    // Method to find a target based on certain criteria
    /*public static unsafe FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject* FindTarget(Func<FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*, bool> criteria)
    {
        GameObjectArray objectArray = targetSystem->ObjectFilterArray1; // Example array, adjust as needed

        for (int i = 0; i < objectArray.Length; i++)
        {
            var gameObject = objectArray[i];
            if (gameObject != null && criteria(gameObject))
            {
                return gameObject;
            }
        }

        return null; // No suitable target found
    }*/
}