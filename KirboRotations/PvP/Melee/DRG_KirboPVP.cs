using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using ImGuiNET;
using KirboRotations.Configurations;
using KirboRotations.Extensions;
using KirboRotations.UI;
using Lumina.Excel;
using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Attributes;
using RotationSolver.Basic.Configuration.RotationConfig;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Helpers;
using RotationSolver.Basic.Rotations.Basic;

namespace KirboRotations.PvP.Melee;

[BetaRotation]
[RotationDesc(ActionID.DragonSight)]
internal class DRG_KirboPvP : DRG_Base
{
    #region Rotation Info
    public override string GameVersion => "6.51";
    public override string RotationName => $"{RotationConfigs.USERNAME}'s {ClassJob.Abbreviation} [{Type}]";
    public override CombatType Type => CombatType.PvP;
    #endregion Rotation Info

    #region IBaseActions
/*
    internal ActionID[] ComboIdsNot { private get; init; }

    internal ActionID[] ComboIds { private get; init; }
    private bool CheckForCombo()
    {
        if (ComboIdsNot != null && ComboIdsNot.Contains(DataCenter.LastComboAction))
        {
            return false;
        }

        LazyRow<Lumina.Excel.GeneratedSheets.Action> actionCombo = _action.ActionCombo;
        ActionID[] array = ((actionCombo != null && actionCombo.Row == 0) ? Array.Empty<ActionID>() : new ActionID[1] { (ActionID)_action.ActionCombo.Row });
        if (ComboIds != null)
        {
            array = array.Union(ComboIds).ToArray();
        }

        if (array.Length != 0)
        {
            if (!array.Contains(DataCenter.LastComboAction))
            {
                return false;
            }

            if (DataCenter.ComboTime < DataCenter.WeaponRemain)
            {
                return false;
            }
        }

        return true;
    }
*/


    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_WheelingThrustCombo { get; } = new BaseAction(ActionID.PvP_WheelingThrustCombo);

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_RaidenThrust { get; } = new BaseAction(ActionID.PvP_RaidenThrust);


    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_FangAndClaw { get; } = new BaseAction(ActionID.PvP_FangAndClaw);

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_WheelingThrust { get; } = new BaseAction(ActionID.PvP_WheelingThrust);

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_ChaoticSpring { get; } = new BaseAction(ActionID.PvP_ChaoticSpring);

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Geirskogul { get; } = new BaseAction(ActionID.PvP_Geirskogul)
    {
        StatusProvide = [StatusID.PvP_LifeOfTheDragon]
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_HighJump { get; } = new BaseAction(ActionID.PvP_HighJump)
    {
        StatusProvide = [StatusID.PvP_Heavensent]
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_ElusiveJump { get; } = new BaseAction(ActionID.PvP_ElusiveJump);

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_WyrmwindThrust { get; } = new BaseAction(ActionID.PvP_WyrmwindThrust);

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_HorridRoar { get; } = new BaseAction(ActionID.PvP_HorridRoar, ActionOption.Friendly)
    {
        FilterForHostiles = tars => tars.Where(t => t is PlayerCharacter && Target != Player),
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_HeavensThrust { get; } = new BaseAction(ActionID.PvP_HeavensThrust)
    {
        StatusNeed = new StatusID[] { StatusID.PvP_Heavensent },
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_Nastrond { get; } = new BaseAction(ActionID.PvP_Nastrond)
    {
        StatusNeed = new StatusID[] { StatusID.PvP_LifeOfTheDragon },
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_SkyHigh { get; } = new BaseAction(ActionID.PvP_SkyHigh)
    {
        ActionCheck = (t, m) => LimitBreakLevel >= 1
    };

    /// <summary>
    ///
    /// </summary>
    private static IBaseAction PvP_SkyShatter { get; } = new BaseAction(ActionID.PvP_SkyShatter)
    {
    };
    #endregion PvP

    #region PvP Debug window
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

    protected override IRotationConfigSet CreateConfiguration() => base.CreateConfiguration()
        .SetBool(CombatType.PvP, "GuardCancel", true, "Turn on if you want to FORCE RS to use nothing while in guard in PvP")
        .SetBool(CombatType.PvP, "PreventActionWaste", true, "Turn on to prevent using actions on targets with invulns\n(For example: DRK with Undead Redemption)")
        .SetBool(CombatType.PvP, "SafetyCheck", true, "Turn on to prevent using actions on targets that have a dangerous status\n(For example a SAM with Chiten)");

    protected override bool GeneralGCD(out IAction act)
    {
        act = null;

        // Status checks
        bool TargetIsNotPlayer = Target == Target.IsOthersPlayers();
        BattleChara target = Target;
        bool hasGuard = TargetIsNotPlayer && target.HasStatus(false, StatusID.PvP_Guard);
        bool hasChiten = TargetIsNotPlayer && target.HasStatus(false, StatusID.PvP_Chiten);
        bool hasHallowedGround = TargetIsNotPlayer && target.HasStatus(false, StatusID.PvP_HallowedGround);
        bool hasUndeadRedemption = TargetIsNotPlayer && target.HasStatus(false, StatusID.PvP_UndeadRedemption);

        // Config checks
        bool guardCancel = Configs.GetBool("GuardCancel");
        bool preventActionWaste = Configs.GetBool("PreventActionWaste");
        bool safetyCheck = Configs.GetBool("SafetyCheck");
        if (BattleCharaEx.InPvP())
        {
            if (guardCancel && Player.HasStatus(true, StatusID.PvP_Guard))
            {
                return false;
            }

            if (safetyCheck && hasChiten)
            {
                return false;
            }

            if (preventActionWaste && (hasGuard || hasHallowedGround || hasUndeadRedemption))
            {
                return false;
            }

            if (PvP_WyrmwindThrust.CanUse(out act, CanUseOption.MustUse) && TargetIsNotPlayer && HostileTarget.DistanceToPlayer() <= 5)
            {
                return true;
            }

            if (PvP_HeavensThrust.CanUse(out act, CanUseOption.MustUse) && TargetIsNotPlayer && HostileTarget.DistanceToPlayer() <= 5)
            {
                return true;
            }

            if (PvP_ChaoticSpring.CanUse(out act, CanUseOption.MustUse) && TargetIsNotPlayer && HostileTarget.DistanceToPlayer() <= 5)
            {
                return true;
            }

            // 3
            if (PvP_WheelingThrustCombo.CanUse(out act))
            {
                return true;
            }
            // 3
            if (PvP_WheelingThrust.CanUse(out act))
            {
                return true;
            }
            // 2
            if (PvP_FangAndClaw.CanUse(out act))
            {
                return true;
            }
            // 1
            if (PvP_RaidenThrust.CanUse(out act))
            {
                return true;
            }
        }
        return base.GeneralGCD(out act);
    }

    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
    {
        act = null;

        if (PvP_HighJump.CanUse(out act, CanUseOption.MustUse) && HostileTarget.DistanceToPlayer() <= 20)
        {
            return true;
        }

        if (PvP_HorridRoar.CanUse(out act, CanUseOption.MustUse, 1))
        {
            if (IsLastAction(ActionID.HighJump) && HostileTarget.DistanceToPlayer() <= 3)
            {
                return true;
            }
            return false;
        }

        if (PvP_Geirskogul.CanUse(out act, CanUseOption.MustUse) && HostileTarget.DistanceToPlayer() <= 15)
        {
            return true;
        }

        if (PvP_Nastrond.CanUse(out act, CanUseOption.MustUse) && HostileTarget.DistanceToPlayer() <= 15)
        {
            return true;
        }

        return base.EmergencyAbility(nextGCD, out act);
    }
}