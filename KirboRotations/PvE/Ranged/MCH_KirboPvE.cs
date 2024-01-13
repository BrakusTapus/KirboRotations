using KirboRotations.Extensions;
using Lumina.Excel.GeneratedSheets;
using static KirboRotations.Extensions.BattleCharaEx;

namespace KirboRotations.PvE.Ranged;

[BetaRotation]
[RotationDesc(ActionID.Wildfire)]
internal class MCH_KirboPvE : MCH_Base
{
    #region Rotation Info

    public override string GameVersion => "6.51";

    public override string RotationName => $"{USERNAME}'s {ClassJob.Abbreviation} [{Type}]";

    public override CombatType Type => CombatType.PvE;

    #endregion Rotation Info

    #region New PvE IBaseActions

    internal bool WillhaveTool { get; private set; }

    private new static IBaseAction AirAnchor => new BaseAction(ActionID.AirAnchor)
    {
        ActionCheck = (b, m) => !IsOverheated,
    };

    private new static IBaseAction BarrelStabilizer => new BaseAction(ActionID.BarrelStabilizer)
    {
        ActionCheck = (b, m) => Heat <= 45 && InCombat
    };

    private new static IBaseAction ChainSaw => new BaseAction(ActionID.ChainSaw)
    {
        ActionCheck = (b, m) => !IsOverheated,
    };

    private new static IBaseAction Drill => new BaseAction(ActionID.Drill)
    {
        ActionCheck = (b, m) => !IsOverheated,
    };

    private new static IBaseAction Hypercharge => new BaseAction(ActionID.Hypercharge, ActionOption.UseResources)
    {
        StatusProvide = new StatusID[1] { StatusID.Overheated },
        ActionCheck = (b, m) => !IsOverheated && Heat >= 50 && IsLongerThan(10f)
    };

    private new static IBaseAction Reassemble => new BaseAction(ActionID.Reassemble)
    {
        StatusProvide = new StatusID[1] { StatusID.Reassemble },
        ActionCheck = (b, m) => !Player.HasStatus(true, StatusID.Reassemble),
    };

    private new static IBaseAction Wildfire => new BaseAction(ActionID.Wildfire)
    {
        ActionCheck = (b, m) => (Player.HasStatus(true, StatusID.Overheated) && Heat >= 50) && InCombat
    };

    #endregion New PvE IBaseActions

    #region GCD Logic

    protected override bool GeneralGCD(out IAction act)
    {
        if (AutoCrossbow.CanUse(out act, (CanUseOption)1, 2) && HostileTarget.DistanceToPlayer() <= 12f)
        {
            return true;
        }
        if (HeatBlast.CanUse(out act, CanUseOption.MustUse))
        {
            return true;
        }
        if (HostileTarget.GetHealthRatio() > 0.6 && IsLongerThan(15) && BioBlaster.CanUse(out act, (CanUseOption)1, 2) && HostileTarget.DistanceToPlayer() <= 12f)
        {
            return true;
        }
        if (Drill.CanUse(out act, CanUseOption.MustUse) && !IsOverheated)
        {
            return true;
        }
        if (AirAnchor.CanUse(out act, CanUseOption.MustUse) && !IsOverheated)
        {
            return true;
        }
        if (!AirAnchor.EnoughLevel && HotShot.CanUse(out act, CanUseOption.MustUse) && !IsOverheated)
        {
            return true;
        }
        if (ChainSaw.CanUse(out act, CanUseOption.MustUse) && !IsOverheated)
        {
            return true;
        }
        if (SpreadShot.CanUse(out act, (CanUseOption)1, 2))
        {
            return true;
        }
        if (CleanShot.CanUse(out act, CanUseOption.MustUse))
        {
            return true;
        }
        if (SlugShot.CanUse(out act, CanUseOption.MustUse))
        {
            return true;
        }
        if (SplitShot.CanUse(out act, CanUseOption.MustUse))
        {
            return true;
        }
        return base.GeneralGCD(out act);
    }

    #endregion GCD Logic

    #region oGCD Logic

    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
    {
        act = null;
        if (Wildfire.CanUse(out act, (CanUseOption)16))
        {
            if (nextGCD == ChainSaw && Heat >= 50 ||
                IsLastAbility(ActionID.Hypercharge) &&
                Heat >= 45 && !Drill.WillHaveOneCharge(5) &&
                    !AirAnchor.WillHaveOneCharge(7.5f) &&
                    !ChainSaw.WillHaveOneCharge(7.5f))
            {
                return true;
            }
        }
        if (BarrelStabilizer.CanUse(out act, CanUseOption.MustUseEmpty))
        {
            if (Wildfire.IsCoolingDown && IsLastGCD((ActionID)16498))
            {
                return true;
            }
            return true;
        }
        if (Reassemble.CanUse(out act, CanUseOption.MustUseEmpty))
        {
            if (Reassemble.CurrentCharges > 1)
            {
                if (nextGCD == ChainSaw || nextGCD == AirAnchor ||
                    nextGCD == Drill)
                {
                    return true;
                }
            }

            if (nextGCD.IsTheSameTo(false, ChainSaw))
            {
                return true;
            }
            if (nextGCD.IsTheSameTo(true, AirAnchor) &&
                !Wildfire.WillHaveOneCharge(55f))
            {
                return true;
            }
            if (nextGCD.IsTheSameTo(false, Drill) &&
                !Wildfire.WillHaveOneCharge(55f))
            {
                return true;
            }
        }
        if (RookAutoturret.CanUse(out act, (CanUseOption)16) && HostileTarget && HostileTarget.IsTargetable && InCombat)
        {
            if (CombatElapsedLess(60f) && !CombatElapsedLess(48f) &&
                Battery >= 50)
            {
                return true;
            }
            if (Wildfire.IsCoolingDown && Wildfire.WillHaveOneCharge(15f))
            {
                return true;
            }
            if (Battery >= 90 && !Wildfire.ElapsedAfter(70f))
            {
                return true;
            }
            if (Battery >= 80 && !Wildfire.ElapsedAfter(77.5f) &&
                IsLastGCD((ActionID)16500))
            {
                return true;
            }
        }
        if (Hypercharge.CanUse(out act) && (IsLastAbility(ActionID.Wildfire) || !WillhaveTool && IsLastGCD(ActionID.ChainSaw, ActionID.AirAnchor, ActionID.Drill, ActionID.SplitShot, ActionID.SlugShot, ActionID.CleanShot,
            ActionID.HeatedSplitShot, ActionID.HeatedSlugShot) || Heat >= 100 && Wildfire.WillHaveOneCharge(10f) || Heat >= 100 && Wildfire.WillHaveOneCharge(40f) || Heat >= 50 && !Wildfire.WillHaveOneCharge(40f)))
        {
            return true;
        }
        if (Player.Level < 90)
        {
            if ((IsLastAbility(false, Hypercharge) || Heat >= 50) &&
                HostileTarget.IsBossFromIcon() &&
                Wildfire.CanUse(out act, CanUseOption.OnLastAbility))
            {
                return true;
            }
            if (Reassemble.CurrentCharges > 0 &&
                Reassemble.CanUse(out act, CanUseOption.MustUseEmpty))
            {
                if (ChainSaw.EnoughLevel &&
                    (nextGCD == ChainSaw || nextGCD == Drill ||
                     nextGCD == AirAnchor))
                {
                    return true;
                }
                if (!Drill.EnoughLevel && nextGCD == CleanShot)
                {
                    return true;
                }
            }
            if (BarrelStabilizer.CanUse(out act) && HostileTarget &&
                HostileTarget.IsTargetable && InCombat)
            {
                return true;
            }
            if (Hypercharge.CanUse(out act))
            {
                return true;
            }
            if (HostileTarget.GetTimeToKill() > 10 && Heat >= 100)
            {
                return true;
            }
            if (HostileTarget.GetHealthRatio() > 0.25)
            {
                return true;
            }
            if (HostileTarget.IsBossFromIcon())
            {
                return true;
            }
            if (RookAutoturret.CanUse(out act) && HostileTarget &&
                HostileTarget.IsTargetable && InCombat)
            {
                if (!HostileTarget.IsBossFromIcon() && CombatElapsedLess(30f))
                {
                    return true;
                }
                if (HostileTarget.IsBossFromIcon())
                {
                    return true;
                }
            }
            if (ShouldUseGaussroundOrRicochet(out act) &&
                NextAbilityToNextGCD > GaussRound.AnimationLockTime + Ping)
            {
                return true;
            }
        }
        return base.EmergencyAbility(nextGCD, out act);
    }

    #endregion oGCD Logic

    #region Job Helper Methods

    private bool ShouldUseGaussroundOrRicochet(out IAction act)
    {
        act = null;
        if (!GaussRound.HasOneCharge && !Ricochet.HasOneCharge)
        {
            return false;
        }
        if (!GaussRound.HasOneCharge && !Ricochet.EnoughLevel)
        {
            return false;
        }
        if (!Ricochet.EnoughLevel)
        {
            return GaussRound.CanUse(out act, CanUseOption.MustUseEmpty);
        }
        if (GaussRound.CurrentCharges >= Ricochet.CurrentCharges)
        {
            return GaussRound.CanUse(out act, CanUseOption.MustUseEmpty);
        }
        if (Ricochet.CurrentCharges >= GaussRound.CurrentCharges)
        {
            return Ricochet.HasOneCharge &&
                   Ricochet.CanUse(out act, CanUseOption.MustUseEmpty);
        }
        return GaussRound.CanUse(out act, CanUseOption.MustUseEmpty);
    }

    private void ToolKitCheck()
    {
        bool WillHaveDrill = !Drill.IsCoolingDown || Drill.WillHaveOneCharge(5f);
        bool WillHaveAirAnchor =
        !AirAnchor.IsCoolingDown || AirAnchor.WillHaveOneCharge(5f);
        bool WillHaveChainSaw =
        !ChainSaw.IsCoolingDown || ChainSaw.WillHaveOneCharge(5f);

        if (Player.Level >= 90)
        {
            WillhaveTool =
                WillHaveDrill || WillHaveAirAnchor || WillHaveChainSaw;
        }
        else if (Player.Level >= 76)
        {
            WillhaveTool = WillHaveDrill || WillHaveAirAnchor;
        }
        else if (Player.Level >= 58)
        {
            WillhaveTool = WillHaveDrill;
        }
    }

    #endregion Job Helper Methods

    #region Miscellaneous Helper Methods

    protected override void UpdateInfo()
    {
        if (Player.IsInCombat())
        {
            ToolKitCheck();
        }
    }

    #endregion Miscellaneous Helper Methods
}
