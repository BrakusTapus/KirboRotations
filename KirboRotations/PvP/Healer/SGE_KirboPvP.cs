using Dalamud.Game.ClientState.Objects.Types;
using KirboRotations.Custom.JobHelpers;
using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Attributes;
using RotationSolver.Basic.Configuration.RotationConfig;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Rotations;
using RotationSolver.Basic.Rotations.Basic;

namespace KirboRotations.PvP.Healer;

[BetaRotation]
[RotationDesc(ActionID.Wildfire)]
internal class PvP_SGE_Kirbo : SGE_Base
{
    #region Rotation Info
    public override CombatType Type => CombatType.PvP;
    public override string GameVersion => "6.51";
    public override string RotationName => $"{GeneralHelpers.USERNAME}'s {ClassJob.Abbreviation} [{Type}]";
    public override string Description => $"{GeneralHelpers.USERNAME}'s {ClassJob.Name}";
    #endregion Rotation Info

    #region PvP

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Dosis { get; } = new BaseAction(ActionID.PvP_Dosis)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Phlegma3 { get; } = new BaseAction(ActionID.PvP_Phlegma3)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Pneuma { get; } = new BaseAction(ActionID.PvP_Pneuma)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Eukrasia { get; } = new BaseAction(ActionID.PvP_Eukrasia)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Icarus { get; } = new BaseAction(ActionID.PvP_Icarus)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Toxikon { get; } = new BaseAction(ActionID.PvP_Toxikon)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Kardia { get; } = new BaseAction(ActionID.PvP_Kardia)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_EukrasianDosis2 { get; } = new BaseAction(ActionID.PvP_EukrasianDosis2)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Toxikon2 { get; } = new BaseAction(ActionID.PvP_Toxikon2)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Mesotes { get; } = new BaseAction(ActionID.PvP_Mesotes)
    {
        ActionCheck = (BattleChara t, bool m) => CustomRotation.LimitBreakLevel >= 1
    };

    #endregion PvP

    #region Debug window
    public override bool ShowStatus => true;

    public override void DisplayStatus()
    {
        // WIP
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