using Dalamud.IoC;
using Dalamud.Plugin.Services;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.Attributes;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel;

namespace KirboRotations.Utility.Core;

internal class KirboService : IDisposable
{
    [PluginService] public static IObjectTable Objects { get; private set; }

    // From https://GitHub.com/PunishXIV/Orbwalker/blame/master/Orbwalker/Memory.cs#L85-L87
    [Signature("F3 0F 10 05 ?? ?? ?? ?? 0F 2E C6 0F 8A", ScanType = ScanType.StaticAddress, Fallibility = Fallibility.Infallible)]
    static nint forceDisableMovementPtr = nint.Zero;

    private static unsafe ref int ForceDisableMovement => ref *(int*)(forceDisableMovementPtr + 4);

    static bool _canMove = true;
    internal static unsafe bool CanMove
    {
        get => ForceDisableMovement == 0;
        set
        {
            var realCanMove = value || DataBase.NoPoslock;
            if (_canMove == realCanMove) return;
            _canMove = realCanMove;

            if (!realCanMove)
            {
                ForceDisableMovement++;
            }
            else if (ForceDisableMovement > 0)
            {
                ForceDisableMovement--;
            }
        }
    }

    public static float CountDownTime => Countdown.TimeRemaining;

    public KirboService()
    {
        KirboSvc.Hook.InitializeFromAttributes(this);
    }

    public static ActionID GetAdjustedActionId(ActionID id) => (ActionID)GetAdjustedActionId((uint)id);

    public static unsafe uint GetAdjustedActionId(uint id) => ActionManager.Instance()->GetAdjustedActionId(id);

    public unsafe static IEnumerable<nint> GetAddons<T>() where T : struct
    {
        if (typeof(T).GetCustomAttribute<Addon>() is not Addon on) return Array.Empty<nint>();

        return on.AddonIdentifiers
            .Select(str => KirboSvc.GameGui.GetAddonByName(str, 1))
            .Where(ptr => ptr != nint.Zero);
    }

    public static ExcelSheet<T> GetSheet<T>() where T : ExcelRow => KirboSvc.Data.GetExcelSheet<T>();

    public void Dispose()
    {
        if (!_canMove && ForceDisableMovement > 0)
        {
            ForceDisableMovement--;
        }
    }

}
