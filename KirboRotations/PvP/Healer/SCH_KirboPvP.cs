﻿using KirboRotations.Custom.JobHelpers;
using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Attributes;
using RotationSolver.Basic.Configuration.RotationConfig;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Helpers;
using RotationSolver.Basic.Rotations;
using RotationSolver.Basic.Rotations.Basic;

namespace KirboRotations.PvP.Healer;

[BetaRotation]
[RotationDesc(ActionID.Wildfire)]
internal class SCH_KirboPvP : SCH_Base
{
    #region Rotation Info
    public override string GameVersion => "6.51";
    public override string RotationName => $"{RotationConfigs.USERNAME}'s {ClassJob.Abbreviation} [{Type}]";
    public override CombatType Type => CombatType.PvP;
    #endregion Rotation Info

    #region PvP

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Broil { get; } = new BaseAction(ActionID.PvP_Broil);

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Adloquilum { get; } = new BaseAction(ActionID.PvP_Adloquilum, ActionOption.Heal)
    {
        ActionCheck = (b, m) => !b.HasStatus(
            false,
            StatusID.PvP_Galvanize),
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Biolysis { get; } = new BaseAction(ActionID.PvP_Biolysis, ActionOption.Dot)
    {
        TargetStatus = new StatusID[] { StatusID.PvP_Biolytic, StatusID.PvP_Biolysis },
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_DeploymentTactics { get; } = new BaseAction(ActionID.PvP_DeploymentTactics, ActionOption.Heal)
    {
        ChoiceTarget = (friends, mustUse) =>
        {
            foreach (var friend in from friend in friends
                                   where friend.HasStatus(true, StatusID.Galvanize)
                                   select friend)
            {
                return friend;
            }

            return null;
        },
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Mummification { get; } = new BaseAction(ActionID.PvP_Mummification);

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Expedient { get; } = new BaseAction(ActionID.PvP_Expedient);

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_SummonSeraph { get; } = new BaseAction(ActionID.PvP_SummonSeraph, ActionOption.Heal)
    {
        //ActionCheck = (b, m) => RotationSolver.Basic.DataCenter.HasPet && LimitBreakLevel >= 1,
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Consolation { get; } = new BaseAction(ActionID.PvP_Consolation, ActionOption.Heal)
    {
        ActionCheck = (b, m) => b.HasStatus(
            true,
            StatusID.PvP_SummonSeraph),
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_SeraphFlight { get; } = new BaseAction(ActionID.PvP_SeraphFlight);

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_SeraphicVeil { get; } = new BaseAction(ActionID.PvP_SeraphicVeil);

    #endregion PvP

    #region Debug window
    public override bool ShowStatus => true;
    public override void DisplayStatus()
    {
        RotationConfigs CompatibilityAndFeatures = new();
        CompatibilityAndFeatures.AddContentCompatibilityForPvP(PvPContentCompatibility.Frontlines);
        CompatibilityAndFeatures.AddContentCompatibilityForPvP(PvPContentCompatibility.CrystalineConflict);
        CompatibilityAndFeatures.AddFeaturesForPvP(PvPFeatures.HasUserConfig);
        try
        {
            PvPDebugWindow.DisplayPvPTab();
            ImGui.SameLine();
            PvPDebugWindow.DisplayPvPRotationTabs(RotationName, CompatibilityAndFeatures);
        }
        catch (Exception ex)
        {
            Serilog.Log.Warning($"{ex}");
        }
    }
    #endregion Debug window

    #region Action Properties

    // WIP

    #endregion Action Properties

    #region Rotation Config

    protected override IRotationConfigSet CreateConfiguration() => base.CreateConfiguration()
        .SetInt(CombatType.PvP, "Recuperate", 37500, "HP Threshold for Recuperate", 0, 52500)
        .SetInt(CombatType.PvP, "Guard", 27500, "HP Threshold for Guard", 0, 52500)
        .SetBool(CombatType.PvP, "GuardCancel", true, "Turn on if you want to FORCE RS to use nothing while in guard in PvP")
        .SetBool(CombatType.PvP, "PreventActionWaste", true, "Turn on to prevent using actions on targets with invulns\n(For example: DRK with Undead Redemption)")
        .SetBool(CombatType.PvP, "SafetyCheck", true, "Turn on to prevent using actions on targets that have a dangerous status\n(For example a SAM with Chiten)");

    #endregion Rotation Config

    #region GCD Logic

    protected override bool GeneralGCD(out IAction act)
    {
        act = null;

        return base.GeneralGCD(out act);
    }

    #endregion GCD Logic

    #region oGCD Logic

    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
    {
        act = null;

        return base.EmergencyAbility(nextGCD, out act);
    }

    #endregion oGCD Logic
}