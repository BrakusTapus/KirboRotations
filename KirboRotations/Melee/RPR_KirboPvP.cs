using Dalamud.Game.ClientState.Objects.Types;
using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Attributes;
using RotationSolver.Basic.Configuration.RotationConfig;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Helpers;
using RotationSolver.Basic.Rotations;
using RotationSolver.Basic.Rotations.Basic;
using KirboRotations.Custom.ExtraHelpers;

namespace KirboRotations.Melee;

[BetaRotation]
[RotationDesc(ActionID.Wildfire)]
public class RPR_KirboPvP : RPR_Base
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
    private static IBaseAction PvP_InferSliceCombo { get; } = new BaseAction(ActionID.PvP_InferSliceCombo)
    {

    };

    /// <summary>
    /// 
    /// </summary>
    private static IBaseAction PvP_Slice { get; } = new BaseAction(ActionID.PvP_Slice)
    {

    };

    /// <summary>
    /// 
    /// </summary>
    private static IBaseAction PvP_WaxingSlice { get; } = new BaseAction(ActionID.PvP_WaxingSlice)
    {

    };

    /// <summary>
    /// 
    /// </summary>
    private static IBaseAction PvP_InfernalSlice { get; } = new BaseAction(ActionID.PvP_InfernalSlice)
    {

    };

    /// <summary>
    /// 
    /// </summary>
    private static IBaseAction PvP_SoulSlice { get; } = new BaseAction(ActionID.PvP_SoulSlice)
    {

    };

    /// <summary>
    /// 
    /// </summary>
    private static IBaseAction PvP_HarvestMoon { get; } = new BaseAction(ActionID.PvP_HarvestMoon)
    {

    };

    /// <summary>
    /// 
    /// </summary>
    private static IBaseAction PvP_PlentifulHarvest { get; } = new BaseAction(ActionID.PvP_PlentifulHarvest)
    {

    };

    /// <summary>
    /// 
    /// </summary>
    private static IBaseAction PvP_GrimSwathe { get; } = new BaseAction(ActionID.PvP_GrimSwathe)
    {

    };

    /// <summary>
    /// 
    /// </summary>
    private static IBaseAction PvP_DeathWarrant { get; } = new BaseAction(ActionID.PvP_DeathWarrant)
    {

    };

    /// <summary>
    /// 
    /// </summary>
    private static IBaseAction PvP_ArcaneCrest { get; } = new BaseAction(ActionID.PvP_ArcaneCrest)
    {

    };

    /// <summary>
    /// 
    /// </summary>
    private static IBaseAction PvP_HellsIngress { get; } = new BaseAction(ActionID.PvP_HellsIngress)
    {

    };

    /// <summary>
    /// 
    /// </summary>
    private static IBaseAction PvP_Regress { get; } = new BaseAction(ActionID.PvP_Regress)
    {

    };

    /// <summary>
    /// 
    /// </summary>
    private static IBaseAction PvP_Communio { get; } = new BaseAction(ActionID.PvP_Communio)
    {

    };

    /// <summary>
    /// 
    /// </summary>
    private static IBaseAction PvP_Guillotine { get; } = new BaseAction(ActionID.PvP_Guillotine)
    {

    };

    /// <summary>
    /// 
    /// </summary>
    private static IBaseAction PvP_VoidReaping { get; } = new BaseAction(ActionID.PvP_VoidReaping)
    {

    };

    /// <summary>
    /// 
    /// </summary>
    private static IBaseAction PvP_CrossReaping { get; } = new BaseAction(ActionID.PvP_CrossReaping)
    {

    };

    /// <summary>
    /// 
    /// </summary>
    private static IBaseAction PvP_LemuresSlice { get; } = new BaseAction(ActionID.PvP_LemuresSlice)
    {

    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_TenebraeLemurum { get; } = new BaseAction(ActionID.PvP_TenebraeLemurum)
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
