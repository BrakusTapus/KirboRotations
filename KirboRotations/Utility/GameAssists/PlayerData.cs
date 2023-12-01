using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Statuses;
using KirboRotations.Utility.Core;
using BattleChara = FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara;
using Character = FFXIVClientStructs.FFXIV.Client.Game.Character.Character;
using GameObject = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject;

namespace KirboRotations.Utility.GameAssists;

public unsafe static class PlayerData
{
    /// <summary>Retrieves the current instance of the player character in the game.</summary>
    /// <value>A <see cref="PlayerCharacter"/> object, representing the player's character in the game.</value>
    /// <remarks>
    /// <br>Provides quick access to the player's character data as a static property.</br>
    /// <br>Utilizes <see cref="KirboSvc.ClientState.LocalPlayer"/> for fetching the player character data.</br>
    /// </remarks>
    /// Note: Used 90 times in RS
    public static PlayerCharacter Object => KirboSvc.ClientState.LocalPlayer;

    /// <summary>
    /// Checks if Player is not null
    /// </summary>
    public static bool Available => KirboSvc.ClientState.LocalPlayer != null;

    /// <summary>
    /// Note: Used 1 time in RS
    /// </summary>
    public static bool Interactable
    {
        get
        {
            if (Available)
            {
                return Object.IsTargetable;
            }

            return false;
        }
    }

    /// <summary>
    /// Note: Not used by RS
    /// </summary>
    public static ulong CID => KirboSvc.ClientState.LocalContentId;

    /// <summary>
    /// Note: Not used by RS
    /// </summary>
    public static StatusList Status => KirboSvc.ClientState.LocalPlayer.StatusList;

    /// <summary>
    /// Note: Not used by RS
    /// </summary>
    public static string Name => KirboSvc.ClientState.LocalPlayer?.Name.ToString();

    /// <summary>
    /// Note: Used 4 times in RS
    /// </summary>
    public static int Level => KirboSvc.ClientState.LocalPlayer?.Level ?? 0;

    /// <summary>
    /// Note: Not used by RS
    /// </summary>
    public static bool IsInHomeWorld => KirboSvc.ClientState.LocalPlayer.HomeWorld.Id == KirboSvc.ClientState.LocalPlayer.CurrentWorld.Id;

    /// <summary>
    /// Note: Not used by RS
    /// </summary>
    public static string HomeWorld => KirboSvc.ClientState.LocalPlayer?.HomeWorld.GameData.Name.ToString();

    /// <summary>
    /// Note: Not used by RS
    /// </summary>
    public static string CurrentWorld => KirboSvc.ClientState.LocalPlayer?.CurrentWorld.GameData.Name.ToString();

    /// <summary>Gets a pointer to the character data for the local player.</summary>
    /// <value>A pointer to a <see cref="Character"/> object.</value>
    /// <remarks>
    /// <br>It converts the address to a <see cref="Character"/> pointer, allowing for direct memory manipulation.</br>
    /// </remarks>
    /// <seealso cref="KirboSvc.ClientState.LocalPlayer"/>
    /// Note: Not important
    public static Character* Character => (Character*)KirboSvc.ClientState.LocalPlayer.Address;

    /// <summary>
    /// Used by RS for the Datacenter.HasCompanion
    /// Note: not important
    /// </summary>
    public static BattleChara* BattleChara => (BattleChara*)KirboSvc.ClientState.LocalPlayer.Address;

    /// <summary>
    /// Used by RS for the CanSee Method
    /// Note: not important
    /// </summary>
    public static GameObject* GameObject => (GameObject*)KirboSvc.ClientState.LocalPlayer.Address;
}
