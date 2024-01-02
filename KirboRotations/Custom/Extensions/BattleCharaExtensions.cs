using Dalamud.Game.ClientState.Objects.Types;

namespace KirboRotations.Custom.Extensions;

internal static unsafe class BattleCharaExtensions
{
    public static FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara* Struct(this BattleChara o)
    {
        return (FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara*)o.Address;
    }
    public unsafe static uint RawShieldValue(this Dalamud.Game.ClientState.Objects.Types.BattleChara chara)
    {
        FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara* baseVal = (FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara*)chara.Address;
        var value = baseVal->Character.CharacterData.ShieldValue;
        var rawValue = chara.MaxHp / 100 * value;

        return rawValue;
    }

    public unsafe static byte ShieldPercentage(this Dalamud.Game.ClientState.Objects.Types.BattleChara chara)
    {
        FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara* baseVal = (FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara*)chara.Address;
        var value = baseVal->Character.CharacterData.ShieldValue;

        return value;
    }

    public static bool HasShield(this Dalamud.Game.ClientState.Objects.Types.BattleChara chara) => chara.RawShieldValue() > 0;
}