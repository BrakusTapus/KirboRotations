using Dalamud.Game.ClientState.Objects.Types;
using KirboRotations.Custom.JobHelpers;
using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Attributes;
using RotationSolver.Basic.Configuration.RotationConfig;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Rotations;
using RotationSolver.Basic.Rotations.Basic;

namespace KirboRotations.PvP.Melee;

[BetaRotation]
[RotationDesc(ActionID.Wildfire)]
internal class PvP_SAM_Kirbo : SAM_Base
{
    #region Rotation Info
    public override CombatType Type => CombatType.PvP;
    public override string GameVersion => "6.51";
    public override string RotationName => $"{GeneralHelpers.USERNAME}'s {ClassJob.Abbreviation} [{Type}]";
    public override string Description => $"{GeneralHelpers.USERNAME}'s {ClassJob.Name}";
    #endregion Rotation Info

    #region PvP

    /// <summary>
    /// 1-2-3 combo
    /// </summary>
    private static IBaseAction PvP_KashakCombo { get; } = new BaseAction(ActionID.PvP_KashakCombo)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Yukikaze { get; } = new BaseAction(ActionID.PvP_Yukikaze)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Gekko { get; } = new BaseAction(ActionID.PvP_Gekko)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Kasha { get; } = new BaseAction(ActionID.PvP_Kasha)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Hyosetsu { get; } = new BaseAction(ActionID.PvP_Hyosetsu)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Mangetsu { get; } = new BaseAction(ActionID.PvP_Mangetsu)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Oka { get; } = new BaseAction(ActionID.PvP_Oka)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_OgiNamikiri { get; } = new BaseAction(ActionID.PvP_OgiNamikiri)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Soten { get; } = new BaseAction(ActionID.PvP_Soten)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Chiten { get; } = new BaseAction(ActionID.PvP_Chiten)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Mineuchi { get; } = new BaseAction(ActionID.PvP_Mineuchi)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_MeikyoShisui { get; } = new BaseAction(ActionID.PvP_MeikyoShisui)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Midare { get; } = new BaseAction(ActionID.PvP_Midare)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Kaeshi { get; } = new BaseAction(ActionID.PvP_Kaeshi)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Zantetsuken { get; } = new BaseAction(ActionID.PvP_Zantetsuken)
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