using Dalamud.Game.ClientState.Objects.Types;
using GameMain = FFXIVClientStructs.FFXIV.Client.Game.GameMain;

namespace KirboRotations.Extensions;

internal static class BattleCharaEx
{
    public static bool SaveAction { get; set; } = false;

    public static unsafe FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara* Struct(this BattleChara o)
    {
        return (FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara*)o.Address;
    }

    public static unsafe uint RawShieldValue(this BattleChara chara)
    {
        FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara* baseVal = (FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara*)chara.Address;
        var value = baseVal->Character.CharacterData.ShieldValue;
        var rawValue = chara.MaxHp / 100 * value;

        return rawValue;
    }

    public static unsafe byte ShieldPercentage(this BattleChara chara)
    {
        FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara* baseVal = (FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara*)chara.Address;
        var value = baseVal->Character.CharacterData.ShieldValue;

        return value;
    }

    public static bool HasShield(this BattleChara chara) => chara.RawShieldValue() > 0;

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