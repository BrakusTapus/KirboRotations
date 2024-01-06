using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Utility;
using KirboRotations.PvE.Beta;
using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Helpers;

namespace KirboRotations.Extensions;

internal class BaseEx : MCH_KirboPvEBeta
{
    public static bool LoggedIn { get; set; }

    public static void CheckPlayerStatus()
    {
        if (Player == null)
        {
            LoggedIn = false;
        }
        else
        {
            LoggedIn = true;
        }
    }

    /// <summary>
    /// Gets the last attacked Target
    /// </summary>
    internal BattleChara LastAttackedTarget { get; private set; }

    public  BaseEx()
    {
        LastAttackedTarget = HostileTarget;
    }

    /// <summary>
    /// Add a method to check if the last attacked target meets a certain condition.
    /// </summary>
    /// <returns></returns>
    internal bool IsLastAttackedTargetValid()
    {
        return LastAttackedTarget != null && LastAttackedTarget.GetBattleNPCSubKind() == Dalamud.Game.ClientState.Objects.Enums.BattleNpcSubKind.Enemy;
    }

    /// <summary>
    /// Check if the last attacked target has a specific status effect or condition.
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    internal bool HasLastAttackedTargetStatus(StatusID status)
    {
        return LastAttackedTarget != null && LastAttackedTarget.HasStatus(false, status);
    }

    /// <summary>
    /// Verify if the last attacked target is a hostile enemy rather than a friendly entity.
    /// </summary>
    /// <returns></returns>
    internal bool IsLastAttackedTargetHostile()
    {
        return LastAttackedTarget != null && LastAttackedTarget.IsEnemy();
    }


    /// <summary>
    ///  Check if the last attacked target is currently alive (has positive health points).
    /// </summary>
    /// <returns></returns>
    internal bool IsLastAttackedTargetAlive()
    {
        return LastAttackedTarget != null && LastAttackedTarget.CurrentHp > 0;
    }

    /// <summary>
    /// Determine if the last attacked target has low health and may be a good target for finishing off.
    /// </summary>
    /// <param name="threshold"></param>
    /// <returns></returns>
    internal bool IsLastAttackedTargetLowHealth(float threshold)
    {
        return LastAttackedTarget != null && LastAttackedTarget.CurrentHp < threshold;
    }

    /// <summary>
    /// Determine if the last attacked target has low health and may be a good target for finishing off.
    /// </summary>
    /// <param name="threshold"></param>
    /// <returns></returns>
    internal bool IsLastAttackedTargetHPLowEnough(float threshold, IBaseAction act, IAction action)
    {
        if (LastAttackedTarget != null)
        {
            // Check if the target's health is below a certain threshold
            if (LastAttackedTarget.GetHealthRatio() < threshold && act.CanUse(out action))
            {
                return true;
            }
        }
        return false;
    }

}