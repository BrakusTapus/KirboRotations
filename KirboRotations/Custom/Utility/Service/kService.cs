//using ECommons.DalamudServices;
using FFXIVClientStructs.Attributes;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel;

namespace KirboRotations.Custom.Utility.Service;

internal class kService : IDisposable
{
    // Constants and fields from the original Service class
    public const string USERNAME = "Kirbo";

    // private static nint forceDisableMovementPtr = IntPtr.Zero;

    // private static bool _canMove = true;
    // private unsafe static ref int ForceDisableMovement => ref *(int*)(forceDisableMovementPtr + 4);

    // internal static bool CanMove
    // {
    //     get { return ForceDisableMovement == 0; }
    //     set
    //     {
    //         bool flag = value || DataCenter.NoPoslock;
    //         if (_canMove != flag)
    //         {
    //             _canMove = flag;
    //             if (!flag)
    //             {
    //                 ForceDisableMovement++;
    //             }
    //             else if (ForceDisableMovement > 0)
    //             {
    //                 ForceDisableMovement--;
    //             }
    //         }
    //     }
    // }

    // Properties from the original Service class

    public static float CountDownTime => Countdown.TimeRemaining;

    // public static PluginConfig Config { get; set; } = new PluginConfig();

/*
    // Check RS disposing code in RotationSolverPlugin 
    public kService()
    {
        Svc.Hook.InitializeFromAttributes(this);
    }
*/

    public static ActionID GetAdjustedActionId(ActionID id)
    {
        return (ActionID)GetAdjustedActionId((uint)id);
    }

    public unsafe static uint GetAdjustedActionId(uint id)
    {
        return ActionManager.Instance()->GetAdjustedActionId(id);
    }

/*
    public static IEnumerable<nint> GetAddons<T>() where T : struct
    {
        Addon customAttribute = typeof(T).GetCustomAttribute<Addon>();
        if (customAttribute == null)
        {
            return Array.Empty<nint>();
        }

        return from str in customAttribute.AddonIdentifiers
               select Svc.GameGui.GetAddonByName(str) into ptr
               where ptr != IntPtr.Zero
               select ptr;
    }

    public static ExcelSheet<T> GetSheet<T>() where T : ExcelRow
    {
        return Svc.Data.GetExcelSheet<T>();
    }
*/
    public void Dispose()
    {
        //if (!_canMove && ForceDisableMovement > 0)
        //{
        //    ForceDisableMovement--;
        //}
    }

}