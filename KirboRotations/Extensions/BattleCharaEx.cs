using Dalamud.Game.ClientState.Objects.Types;
using GameMain = FFXIVClientStructs.FFXIV.Client.Game.GameMain;

namespace KirboRotations.Extensions;

internal static class BattleCharaEx
{
    internal const string USERNAME = "Kirbo";

    internal static unsafe bool IsInCombat(this BattleChara obj)
    {
        return obj.Struct()->Character.InCombat;
    }

    internal static unsafe FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara* Struct(this BattleChara o)
    {
        return (FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara*)o.Address;
    }

    internal static bool InPvP() => GameMain.IsInPvPArea() || GameMain.IsInPvPInstance();
}
