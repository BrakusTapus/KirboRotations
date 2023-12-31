using Dalamud.Game.ClientState.Objects.Types;
using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Attributes;
using RotationSolver.Basic.Configuration.RotationConfig;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Rotations;
using RotationSolver.Basic.Rotations.Basic;
using KirboRotations.Custom.ExtraHelpers;

namespace KirboRotations.Ranged;

[BetaRotation]
[RotationDesc(ActionID.Wildfire)]
public class DNC_KirboPvP : DNC_Base
{
    #region Rotation Info
    public override CombatType Type => CombatType.PvP;
    public override string GameVersion => "6.51";
    public override string RotationName => $"{GeneralHelpers.USERNAME}'s {ClassJob.Abbreviation} [{Type}]";
    public override string Description => $"{GeneralHelpers.USERNAME}'s {ClassJob.Name}";
    #endregion

    #region PvP
    /// <summary>
    /// 1-2-3 combo
    /// </summary>
    private static IBaseAction PvP_Fountaincombo { get; } = new BaseAction(ActionID.PvP_Fountaincombo)
    {

    };

    /// <summary>
    /// 
    /// </summary>
    private static IBaseAction PvP_Cascade { get; } = new BaseAction(ActionID.PvP_Cascade)
    {

    };

    /// <summary>
    /// 
    /// </summary>
    private static IBaseAction PvP_Fountain { get; } = new BaseAction(ActionID.PvP_Fountain)
    {

    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Reversecascade { get; } = new BaseAction(ActionID.PvP_Reversecascade)
    {

    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Fountainfall { get; } = new BaseAction(ActionID.PvP_Fountainfall)
    {

    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Saberdance { get; } = new BaseAction(ActionID.PvP_Saberdance)
    {

    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Starfalldance { get; } = new BaseAction(ActionID.PvP_Starfalldance)
    {

    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Honingdance { get; } = new BaseAction(ActionID.PvP_Honingdance)
    {

    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Honingovation { get; } = new BaseAction(ActionID.PvP_Honingovation)
    {

    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Fandance { get; } = new BaseAction(ActionID.PvP_Fandance)
    {

    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Curingwaltz { get; } = new BaseAction(ActionID.PvP_Curingwaltz)
    {

    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Closedposition { get; } = new BaseAction(ActionID.PvP_Closedposition)
    {

    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Enavant { get; } = new BaseAction(ActionID.PvP_Enavant)
    {

    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Contradance { get; } = new BaseAction(ActionID.PvP_Contradance)
    {
        ActionCheck = (BattleChara t, bool m) => CustomRotation.LimitBreakLevel >= 1
    };
    #endregion

    #region Debug window
    public override bool ShowStatus => true;
    public override void DisplayStatus()
    {
        // WIP
    }
    #endregion

    #region Action Properties
    // WIP    
    #endregion

    #region Rotation Config
    protected override IRotationConfigSet CreateConfiguration() => base.CreateConfiguration()
        .SetInt(CombatType.PvP, "Recuperate", 37500, "HP Threshold for Recuperate", 0, 52500)
        .SetInt(CombatType.PvP, "Guard", 27500, "HP Threshold for Guard", 0, 52500)
        .SetBool(CombatType.PvP, "GuardCancel", true, "Turn on if you want to FORCE RS to use nothing while in guard in PvP")
        .SetBool(CombatType.PvP, "PreventActionWaste", true, "Turn on to prevent using actions on targets with invulns\n(For example: DRK with Undead Redemption)")
        .SetBool(CombatType.PvP, "SafetyCheck", true, "Turn on to prevent using actions on targets that have a dangerous status\n(For example a SAM with Chiten)");
    #endregion

    #region GCD Logic
    protected override bool GeneralGCD(out IAction act)
    {
        act = null;

        return base.GeneralGCD(out act);
    }
    #endregion

    #region oGCD Logic
    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
    {
        act = null;
       
        return base.EmergencyAbility(nextGCD, out act);
    }
    #endregion
}
