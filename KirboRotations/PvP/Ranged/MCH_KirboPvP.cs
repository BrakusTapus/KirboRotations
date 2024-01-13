using static KirboRotations.Extensions.BattleCharaEx;

namespace KirboRotations.PvP.Ranged;

[BetaRotation]
[RotationDesc(ActionID.Wildfire)]
internal class MCH_KirboPvP : MCH_Base
{
    #region Rotation Info

    public override string GameVersion => "6.51";

    public override string RotationName => $"{USERNAME}'s {ClassJob.Abbreviation} [{Type}]";

    public override CombatType Type => CombatType.PvP;

    #endregion Rotation Info

    #region IBaseActions

    private new static IBaseAction PvP_AirAnchor { get; } = new BaseAction(ActionID.PvP_AirAnchor)
    {
        StatusNeed = new StatusID[1] { StatusID.PvP_AirAnchorPrimed },
        StatusProvide = new StatusID[1] { StatusID.PvP_ChainSawPrimed },
    };

    private new static IBaseAction PvP_Analysis { get; } = new BaseAction(ActionID.PvP_Analysis, ActionOption.Friendly)
    {
        StatusProvide = new StatusID[1] { StatusID.PvP_Analysis },
        ActionCheck = (b, m) => !Player.HasStatus(true, StatusID.PvP_Analysis) && HasHostilesInRange && PvP_Analysis.CurrentCharges > 0,
    };

    private new static IBaseAction PvP_Bioblaster { get; } = new BaseAction(ActionID.PvP_Bioblaster)
    {
        ActionCheck = (b, m) => !Player.HasStatus(true, StatusID.PvP_Guard),
        StatusNeed = new StatusID[1] { StatusID.PvP_BioblasterPrimed },
        StatusProvide = new StatusID[1] { StatusID.PvP_AirAnchorPrimed },
    };

    private new static IBaseAction PvP_BishopAutoTurret { get; } = new BaseAction(ActionID.PvP_BishopAutoTurret, ActionOption.Attack)
    {
        ActionCheck = (b, m) => HasHostilesInRange && !Player.HasStatus(true, StatusID.PvP_Guard),
    };

    private new static IBaseAction PvP_BlastCharge { get; } = new BaseAction(ActionID.PvP_BlastCharge)
    {
        ActionCheck = (b, m) => !Player.HasStatus(true, StatusID.PvP_Guard),
    };

    private new static IBaseAction PvP_ChainSaw { get; } = new BaseAction(ActionID.PvP_ChainSaw)
    {
        StatusNeed = new StatusID[1] { StatusID.PvP_ChainSawPrimed },
        StatusProvide = new StatusID[1] { StatusID.PvP_DrillPrimed },
    };

    private new static IBaseAction PvP_Drill { get; } = new BaseAction(ActionID.PvP_Drill)
    {
        StatusNeed = new StatusID[1] { StatusID.PvP_DrillPrimed },
        StatusProvide = new StatusID[1] { StatusID.PvP_BioblasterPrimed },
    };

    private new static IBaseAction PvP_Scattergun { get; } = new BaseAction(ActionID.PvP_Scattergun)
    { };

    private new static IBaseAction PvP_Wildfire { get; } = new BaseAction(ActionID.PvP_Wildfire, ActionOption.Attack)
    {
        ActionCheck = (b, m) => Player.HasStatus(true, StatusID.PvP_Overheated) && !Player.HasStatus(true, StatusID.PvP_Guard),
    };

    #endregion IBaseActions

    #region Action Properties

    private static bool IsPvPOverheated => Player.HasStatus(true, StatusID.PvP_Overheated);

    private static byte PvP_HeatStacks
    {
        get
        {
            byte pvp_heatstacks = Player.StatusStack(true, StatusID.PvP_HeatStack);
            return pvp_heatstacks == byte.MaxValue ? (byte)5 : pvp_heatstacks;
        }
    }

    #endregion Action Properties

    #region Rotation Config

    protected override IRotationConfigSet CreateConfiguration() => base.CreateConfiguration()
        .SetBool(CombatType.PvP, "GuardCancel", true, "Turn on if you want to FORCE RS to use nothing while in guard in PvP");

    #endregion Rotation Config

    #region GCD Logic

    protected override bool GeneralGCD(out IAction act)
    {
        act = null;
        bool guardCancel = Configs.GetBool("GuardCancel");
        if (guardCancel && Player.HasStatus(true, StatusID.PvP_Guard))
        {
            return false;
        }
        if (PvP_Drill.CanUse(out act, CanUseOption.MustUseEmpty))
        {
            if (PvP_Analysis.CurrentCharges > 0 && !Player.HasStatus(true, StatusID.PvP_Analysis))
            {
                return PvP_Analysis.CanUse(out act, CanUseOption.MustUseEmpty);
            }
            if (Player.HasStatus(true, StatusID.PvP_Analysis))
            {
                return PvP_Drill.CanUse(out act, CanUseOption.MustUseEmpty);
            }
        }
        if (PvP_Drill.CanUse(out act, CanUseOption.MustUseEmpty) && Target != Player && Target.GetHealthRatio() < 0.75)
        {
            if (PvP_Analysis.CurrentCharges > 0 && !Player.HasStatus(true, StatusID.PvP_Analysis))
            {
                return PvP_Analysis.CanUse(out act, CanUseOption.MustUseEmpty);
            }
            if (Player.HasStatus(true, StatusID.PvP_Analysis))
            {
                return true;
            }
            if (PvP_Analysis.CurrentCharges == 0)
            {
                return true;
            }
        }
        if (PvP_Bioblaster.CanUse(out act, CanUseOption.MustUseEmpty, 1))
        {
            return true;
        }
        if (PvP_AirAnchor.CanUse(out act, CanUseOption.MustUseEmpty))
        {
            return true;
        }
        if (PvP_ChainSaw.CanUse(out act, CanUseOption.MustUseEmpty, 1))
        {
            return true;
        }
        if (PvP_Scattergun.CanUse(out act, CanUseOption.MustUseEmpty, 1) && HostileTarget.DistanceToPlayer() <= 12)
        {
            return true;
        }
        if (PvP_BlastCharge.CanUse(out act, CanUseOption.IgnoreCastCheck))
        {
            if (guardCancel)
            {
                return false;
            }
            return true;
        }
        return base.GeneralGCD(out act);
    }

    #endregion GCD Logic

    #region oGCD Logic

    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
    {
        act = null;
        bool guardCancel = Configs.GetBool("GuardCancel");
        if (guardCancel)
        {
            return false;
        }
        if (Player.HasStatus(true, StatusID.PvP_DrillPrimed) && (PvP_Drill.CanUse(out act, CanUseOption.MustUseEmpty) || PvP_Drill.WillHaveOneCharge(5)))
        {
            return PvP_Analysis.CanUse(out act, CanUseOption.MustUseEmpty);
        }
        if (IsPvPOverheated && !Player.WillStatusEnd(3.5f, true, StatusID.PvP_Overheated) && PvP_Wildfire.CanUse(out act, CanUseOption.MustUse))
        {
            return true;
        }
        if (PvP_BishopAutoTurret.CanUse(out act, CanUseOption.MustUse))
        {
            return true;
        }
        if (PvP_Analysis.CanUse(out act, CanUseOption.MustUseEmpty) && NumberOfAllHostilesInRange > 0 && !IsPvPOverheated)
        {
            if (PvP_Analysis.CurrentCharges > 0 && Player.HasStatus(true, StatusID.PvP_DrillPrimed) && PvP_HeatStacks <= 4 && !Wildfire.WillHaveOneCharge(10))
            {
                return true;
            }
        }
        return base.EmergencyAbility(nextGCD, out act);
    }

    #endregion oGCD Logic
}
