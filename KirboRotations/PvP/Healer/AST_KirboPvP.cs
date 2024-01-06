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
internal class AST_KirboPvP : AST_Base
{
    #region Rotation Info

    public override string GameVersion => "6.51";
    public override string RotationName => $"{RotationConfigs.USERNAME}'s {ClassJob.Abbreviation} [{Type}]";
    public override CombatType Type => CombatType.PvP;

    #endregion Rotation Info

    #region IBaseActions

    /// <summary>
    /// 1-2-3 combo
    /// </summary>
    private static IBaseAction PvP_FallMalefic { get; } = new BaseAction(ActionID.PvP_FallMalefic)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_AspectedBenefic { get; } = new BaseAction(ActionID.PvP_AspectedBenefic)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Gravity { get; } = new BaseAction(ActionID.PvP_Gravity)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_FallMalefic2 { get; } = new BaseAction(ActionID.PvP_FallMalefic2)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_AspectedBenefic2 { get; } = new BaseAction(ActionID.PvP_AspectedBenefic2)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Gravity2 { get; } = new BaseAction(ActionID.PvP_Gravity2)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Draw { get; } = new BaseAction(ActionID.PvP_Draw)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_DrawTheBalance { get; } = new BaseAction(ActionID.PvP_DrawTheBalance)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_DrawTheBole { get; } = new BaseAction(ActionID.PvP_DrawTheBole)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_DrawTheArrow { get; } = new BaseAction(ActionID.PvP_DrawTheArrow)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Macrocosmos { get; } = new BaseAction(ActionID.PvP_Macrocosmos)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Microcosmos { get; } = new BaseAction(ActionID.PvP_Microcosmos)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_CelestialRiver { get; } = new BaseAction(ActionID.PvP_CelestialRiver, ActionOption.Buff)
    {
        ActionCheck = (BattleChara t, bool m) => CustomRotation.LimitBreakLevel >= 1
    };

    #endregion IBaseActions

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