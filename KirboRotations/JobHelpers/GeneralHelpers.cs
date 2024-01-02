using Dalamud.Game.ClientState.Objects.Types;
using KirboRotations.Custom.Extensions;
using GameMain = FFXIVClientStructs.FFXIV.Client.Game.GameMain;

namespace KirboRotations.JobHelpers;

internal static class GeneralHelpers
{
    public const string USERNAME = "Kirbo";

    private const string K = "[KirboRotations]";

    // Used so i can filter XLLog on rotation name
    public const string v = K;

    //
    public static bool SaveAction { get; set; } = false;

    /// <summary> Checks if the player is in a PVP enabled zone. </summary>
    /// <returns> A value indicating whether the player is in a PVP enabled zone. </returns>
    internal static bool InPvP() => GameMain.IsInPvPArea() || GameMain.IsInPvPInstance();

    /// <summary>
    /// Whether the character is in combat.  This is an extension method of the class BattleChara
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static unsafe bool IsInCombat(this BattleChara obj)
    {
        return obj.Struct()->Character.InCombat;
    }
}