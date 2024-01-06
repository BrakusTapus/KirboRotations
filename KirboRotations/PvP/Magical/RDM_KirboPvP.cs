using Dalamud.Game.ClientState.Objects.Types;
using KirboRotations.Custom.JobHelpers;
using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Attributes;
using RotationSolver.Basic.Configuration.RotationConfig;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Rotations;
using RotationSolver.Basic.Rotations.Basic;

namespace KirboRotations.PvP.Magical;

[BetaRotation]
[RotationDesc(ActionID.Wildfire)]
internal class RDM_KirboPvP : RDM_Base
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
    public static IBaseAction PvP_Verstone { get; } = new BaseAction(ActionID.PvP_Verstone)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Enchantedriposte { get; } = new BaseAction(ActionID.PvP_Enchantedriposte)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Resolution { get; } = new BaseAction(ActionID.PvP_Resolution)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Magickbarrier { get; } = new BaseAction(ActionID.PvP_Magickbarrier)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Corpsacorps { get; } = new BaseAction(ActionID.PvP_Corpsacorps)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Displacement { get; } = new BaseAction(ActionID.PvP_Displacement)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Veraero3 { get; } = new BaseAction(ActionID.PvP_Veraero3)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Verholy { get; } = new BaseAction(ActionID.PvP_Verholy)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Verfire { get; } = new BaseAction(ActionID.PvP_Verfire)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Verthunder3 { get; } = new BaseAction(ActionID.PvP_Verthunder3)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Verflare { get; } = new BaseAction(ActionID.PvP_Verflare)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Enchantedzwerchhau { get; } = new BaseAction(ActionID.PvP_Enchantedzwerchhau)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Enchantedredoublement { get; } = new BaseAction(ActionID.PvP_Enchantedredoublement)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Frazzle { get; } = new BaseAction(ActionID.PvP_Frazzle)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Whiteshift { get; } = new BaseAction(ActionID.PvP_Whiteshift)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Blackshift { get; } = new BaseAction(ActionID.PvP_Blackshift)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Moulinet { get; } = new BaseAction(ActionID.PvP_Moulinet)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_SouthernCross { get; } = new BaseAction(ActionID.PvP_SouthernCross)
    {
        ActionCheck = (BattleChara t, bool m) => CustomRotation.LimitBreakLevel >= 1
    };

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