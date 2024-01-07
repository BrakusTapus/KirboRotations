using Dalamud.Game.ClientState.Objects.Types;
using RotationSolver.RotationBasics.Actions;
using RotationSolver.RotationBasics.Attributes;
using RotationSolver.RotationBasics.Configuration.RotationConfig;
using RotationSolver.RotationBasics.Data;
using RotationSolver.RotationBasics.Rotations;
using RotationSolver.RotationBasics.Rotations.Basic;

namespace KirboRotations.PvP.Tank;

[BetaRotation]
[RotationDesc(ActionID.Wildfire)]
internal class GNB_KirboPvP : GNB_Base
{
    #region Rotation Info
    public override string GameVersion => "6.51";
    public override string RotationName => $"{RotationConfigs.USERNAME}'s {ClassJob.Abbreviation} [{Type}]";
    public override CombatType Type => CombatType.PvP;
    #endregion Rotation Info

    #region PvP

    /// <summary>
    /// 1-2-3 combo
    /// </summary>
    private static IBaseAction PvP_SolidBarrelCombo { get; } = new BaseAction(ActionID.PvP_SolidBarrelCombo)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_KeenEdge { get; } = new BaseAction(ActionID.PvP_KeenEdge)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_BrutalShell { get; } = new BaseAction(ActionID.PvP_BrutalShell)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_SolidBarrel { get; } = new BaseAction(ActionID.PvP_SolidBarrel)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_GnashingFang { get; } = new BaseAction(ActionID.PvP_GnashingFang)
    {
    };

    /// <summary>
    ///
    /// </summary>
    public static IBaseAction PvP_Fountaincombo { get; } = new BaseAction(ActionID.PvP_Fountaincombo)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_SavageClaw { get; } = new BaseAction(ActionID.PvP_SavageClaw)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_WickedTalon { get; } = new BaseAction(ActionID.PvP_WickedTalon)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_DoubleDown { get; } = new BaseAction(ActionID.PvP_DoubleDown)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Nebula { get; } = new BaseAction(ActionID.PvP_Nebula)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_BlastingZone { get; } = new BaseAction(ActionID.PvP_BlastingZone)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Aurora { get; } = new BaseAction(ActionID.PvP_Aurora)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_HyperVelocity { get; } = new BaseAction(ActionID.PvP_HyperVelocity)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_JugularRip { get; } = new BaseAction(ActionID.PvP_JugularRip)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_RoughDivide { get; } = new BaseAction(ActionID.PvP_RoughDivide)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_DrawAndJunction { get; } = new BaseAction(ActionID.PvP_DrawAndJunction)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_JunctionCast { get; } = new BaseAction(ActionID.PvP_JunctionCast)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_AbdomenTear { get; } = new BaseAction(ActionID.PvP_AbdomenTear)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_HeartOfStone { get; } = new BaseAction(ActionID.PvP_HeartOfStone)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_BurstStrike { get; } = new BaseAction(ActionID.PvP_BurstStrike)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_EyeGouge { get; } = new BaseAction(ActionID.PvP_EyeGouge)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Hypervelocity { get; } = new BaseAction(ActionID.PvP_Hypervelocity)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_TerminalTrigger { get; } = new BaseAction(ActionID.PvP_TerminalTrigger)
    {
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_RelentlessRush { get; } = new BaseAction(ActionID.PvP_RelentlessRush)
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