using Dalamud.Plugin.Services;
//using ECommons.DalamudServices;
//using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using RotationSolver.Basic.Configuration;
using RotationSolver.Basic.Helpers;

namespace KirboRotations.Custom.Data;

public class Cooldown
{
    private ActionID _actionID;
    public Cooldown(ActionID actionID)
    {
        _actionID = actionID;
    }
    private byte CoolDownGroup { get; }
    private unsafe RecastDetail* CoolDownDetail => ActionManager.Instance()->GetRecastGroupDetail(CoolDownGroup - 1);

    private unsafe float RecastTime => CoolDownDetail == null ? 0 : CoolDownDetail->Total;

    /// <summary>
    /// Calculates the remaining cooldown time by subtracting the elapsed time from the total recast time.
    /// </summary>
    public float CooldownRemaining => RecastTime;
}
