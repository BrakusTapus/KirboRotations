using Dalamud.Game.ClientState.Objects.Types;

namespace KirboRotations.Custom.ExtraHelpers;

public static unsafe class Unsafe
{
    public static FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara* Struct(this BattleChara o)
    {
        return (FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara*)o.Address;
    }
}