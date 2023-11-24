using Dalamud.Game.ClientState.Objects.SubKinds;

namespace KirboRotations.Melee;

[BetaRotation]
[RotationDesc(ActionID.DragonSight)]
public sealed class DRG_KirboPVP : DRG_Base
{
    #region Rotation Info
    public override string GameVersion => "6.51";

    public override string RotationName => "Kirbo's Dragoon";

    public override string Description => "Quickly build Dragoon Rotation for PvP";

    public override CombatType Type => CombatType.PvP;
    #endregion

    #region PvPDeclaration
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
        FilterForHostiles = tars => tars.Where(t => t is PlayerCharacter),
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
        ActionCheck = (BattleChara t, bool m) => CustomRotation.LimitBreakLevel >= 1
    };

    /// <summary>
    /// 
    /// </summary>
    private static IBaseAction PvP_SkyShatter { get; } = new BaseAction(ActionID.PvP_SkyShatter)
    {

    };
    #endregion


    protected override IRotationConfigSet CreateConfiguration() => base.CreateConfiguration()
        .SetBool(CombatType.PvP, "GuardCancel", true, "Turn on if you want to FORCE RS to use nothing while in guard in PvP")
        .SetBool(CombatType.PvP, "PreventActionWaste", true, "Turn on to prevent using actions on targets with invulns\n(For example: DRK with Undead Redemption)")
        .SetBool(CombatType.PvP, "SafetyCheck", true, "Turn on to prevent using actions on targets that have a dangerous status\n(For example a SAM with Chiten)");

    protected override bool GeneralGCD(out IAction act)
    {
        act = null;

        // Status checks
        bool TargetIsNotPlayer = Target != Player;
        BattleChara target = Target;
        bool hasGuard = TargetIsNotPlayer && target.HasStatus(false, StatusID.PvP_Guard);
        bool hasChiten = TargetIsNotPlayer && target.HasStatus(false, StatusID.PvP_Chiten);
        bool hasHallowedGround = TargetIsNotPlayer && target.HasStatus(false, StatusID.PvP_HallowedGround);
        bool hasUndeadRedemption = TargetIsNotPlayer && target.HasStatus(false, StatusID.PvP_UndeadRedemption);


        // Config checks
        bool guardCancel = Configs.GetBool("GuardCancel");
        bool preventActionWaste = Configs.GetBool("PreventActionWaste");
        bool safetyCheck = Configs.GetBool("SafetyCheck");
        if (Methods.InPvP())
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

            if (PvP_WyrmwindThrust.CanUse(out act, CanUseOption.MustUse) && HostileTarget.DistanceToPlayer() <= 5)
            {
                return true;
            }

            if (PvP_HeavensThrust.CanUse(out act, CanUseOption.MustUse) && HostileTarget.DistanceToPlayer() <= 5)
            {
                return true;
            }

            if (PvP_ChaoticSpring.CanUse(out act, CanUseOption.MustUse) && HostileTarget.DistanceToPlayer() <= 5)
            {
                return true;
            }

            // 3
            if (PvP_WheelingThrust.CanUse(out act, CanUseOption.MustUse))
            {
                return true;
            }
            // 2
            if (PvP_FangAndClaw.CanUse(out act, CanUseOption.MustUse))
            {
                return true;
            }
            // 1
            if (PvP_RaidenThrust.CanUse(out act, CanUseOption.MustUse))
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
            return true;
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