using KirboRotations.Configurations;
using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Attributes;
using RotationSolver.Basic.Configuration.RotationConfig;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Helpers;
using RotationSolver.Basic.Rotations.Basic;

namespace KirboRotations.PvE.Healer;

//[RotationDesc(ActionID.Pneuma)]
[SourceCode(Path = "main/KirboRotations/Healer/SGE_Default.cs")]
internal sealed class SGE_KirboPvE : SGE_Base
{
    #region Rotation Info

    public override string GameVersion => "6.51";
    public override string RotationName => $"{RotationConfigs.USERNAME}'s {ClassJob.Abbreviation} [{Type}]";
    public override CombatType Type => CombatType.PvE;

    #endregion Rotation Info

    #region New IBaseActions

    private static BaseAction MEukrasianDiagnosis { get; } = new(ActionID.EukrasianDiagnosis, ActionOption.Heal)
    {
        ChoiceTarget = (Targets, mustUse) =>
        {
            var targets = Targets.GetJobCategory(JobRole.Tank);
            if (!targets.Any())
            {
                return null;
            }

            return targets.FirstOrDefault();
        },
        ActionCheck = (b, m) =>
        {
            if (InCombat || HasHostilesInRange)
            {
                return false;
            }

            if (b == Player)
            {
                return false;
            }

            if (b.HasStatus(false, StatusID.EukrasianDiagnosis, StatusID.EukrasianPrognosis, StatusID.Galvanize))
            {
                return false;
            }

            return true;
        }
    };

    #endregion New IBaseActions

    #region Debug window

    // Add your debug window-related logic and properties here.

    #endregion Debug window

    #region Action Related Properties

    public override bool CanHealSingleSpell => base.CanHealSingleSpell && (Configs.GetBool("GCDHeal") || PartyHealers.Count() < 2);
    public override bool CanHealAreaSpell => base.CanHealAreaSpell && (Configs.GetBool("GCDHeal") || PartyHealers.Count() < 2);

    #endregion Action Related Properties

    #region Rotation Config

    protected override IRotationConfigSet CreateConfiguration() => base.CreateConfiguration()
        .SetBool(CombatType.PvE, "GCDHeal", false, "Use spells with cast times to heal.")
        .SetBool(CombatType.PvE, "UseAddersgalWhenLowMP", true, "Use addersgall oGCD's if MP is low.")
        .SetInt(CombatType.PvE, "EmergencyMP", 3000, "MP Threshold for when to use MP restoring oGCD's", 0, 10000);

    #endregion Rotation Config

    #region Countdown Logic

    protected override IAction CountDownAction(float remainTime)
    {
        if (remainTime <= 1.5 && Dosis.CanUse(out var act))
        {
            return act;
        }

        if (remainTime <= 3 && UseBurstMedicine(out act))
        {
            return act;
        }

        return base.CountDownAction(remainTime);
    }

    #endregion Countdown Logic

    #region Opener Logic

    // Add your opener logic and methods here.

    #endregion Opener Logic

    #region GCD Logic

    [RotationDesc(ActionID.EukrasianDiagnosis)]
    protected override bool DefenseSingleGCD(out IAction act)
    {
        if (EukrasianDiagnosis.CanUse(out act))
        {
            if (EukrasianDiagnosis.Target.HasStatus(false,
                StatusID.EukrasianDiagnosis,
                StatusID.EukrasianPrognosis,
                StatusID.Galvanize
            ))
            {
                return false;
            }

            if (Eukrasia.CanUse(out act))
            {
                return true;
            }

            act = EukrasianDiagnosis;
            return true;
        }

        return base.DefenseSingleGCD(out act);
    }

    [RotationDesc(ActionID.EukrasianPrognosis)]
    protected override bool DefenseAreaGCD(out IAction act)
    {
        if (EukrasianPrognosis.CanUse(out act))
        {
            if (EukrasianDiagnosis.Target.HasStatus(false,
                StatusID.EukrasianDiagnosis,
                StatusID.EukrasianPrognosis,
                StatusID.Galvanize
            ))
            {
                return false;
            }

            if (Eukrasia.CanUse(out act))
            {
                return true;
            }

            act = EukrasianPrognosis;
            return true;
        }

        return base.DefenseAreaGCD(out act);
    }

    protected override bool GeneralGCD(out IAction act)
    {
        if ((HostileTarget?.IsBossFromTTK() ?? false) && EukrasianDosis.CanUse(out _, CanUseOption.IgnoreCastCheck))
        {
            if (Eukrasia.CanUse(out act, CanUseOption.IgnoreCastCheck))
            {
                return true;
            }

            act = EukrasianDosis;
            return true;
        }

        var option = CanUseOption.MustUse;
        if (IsMoving || Dyskrasia.CanUse(out _) || InTwoMIsBurst())
        {
            option |= CanUseOption.EmptyOrSkipCombo;
        }

        if (Phlegma3.CanUse(out act, option))
        {
            return true;
        }

        if (!Phlegma3.EnoughLevel && Phlegma2.CanUse(out act, option))
        {
            return true;
        }

        if (!Phlegma2.EnoughLevel && Phlegma.CanUse(out act, option))
        {
            return true;
        }

        if ((PartyMembers.Any(b => b.GetHealthRatio() < 0.20f) || PartyTanks.Any(t => t.GetHealthRatio() < 0.6f)) && Pneuma.CanUse(out act, CanUseOption.MustUse))
        {
            return true;
        }

        if (IsMoving && Toxikon.CanUse(out act, CanUseOption.MustUse))
        {
            return true;
        }

        if (Dyskrasia.CanUse(out act))
        {
            return true;
        }

        if (EukrasianDosis.CanUse(out _, CanUseOption.IgnoreCastCheck))
        {
            if (Eukrasia.CanUse(out act, CanUseOption.IgnoreCastCheck))
            {
                return true;
            }

            act = EukrasianDosis;
            return true;
        }

        if (Dosis.CanUse(out act))
        {
            return true;
        }

        if (MEukrasianDiagnosis.CanUse(out _))
        {
            if (Eukrasia.CanUse(out act))
            {
                return true;
            }

            act = MEukrasianDiagnosis;
            return true;
        }

        return base.GeneralGCD(out act);
    }

    [RotationDesc(ActionID.Diagnosis)]
    protected override bool HealSingleGCD(out IAction act)
    {
        if (Diagnosis.CanUse(out act))
        {
            return true;
        }

        return base.HealSingleGCD(out act);
    }

    [RotationDesc(ActionID.Pneuma, ActionID.Prognosis, ActionID.EukrasianPrognosis)]
    protected override bool HealAreaGCD(out IAction act)
    {
        if ((PartyMembersAverHP < 0.65f || Dyskrasia.CanUse(out _) && PartyTanks.Any(t => t.GetHealthRatio() < 0.6f)) && Pneuma.CanUse(out act, CanUseOption.MustUse))
        {
            return true;
        }

        if (Player.HasStatus(false, StatusID.EukrasianDiagnosis, StatusID.EukrasianPrognosis, StatusID.Galvanize) && Prognosis.CanUse(out act))
        {
            return true;
        }

        if (EukrasianPrognosis.CanUse(out _))
        {
            if (Eukrasia.CanUse(out act))
            {
                return true;
            }

            act = EukrasianPrognosis;
            return true;
        }

        return base.HealAreaGCD(out act);
    }

    #endregion GCD Logic

    #region oGCD Logic

    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
    {
        if (base.EmergencyAbility(nextGCD, out act))
        {
            return true;
        }

        if (nextGCD.IsTheSameTo(false, Pneuma, EukrasianDiagnosis, EukrasianPrognosis, Diagnosis, Prognosis) && Zoe.CanUse(out act))
        {
            return true;
        }

        if (nextGCD.IsTheSameTo(false, Pneuma))
        {
            if (Krasis.CanUse(out act))
            {
                return true;
            }
            else if (Zoe.CanUse(out act))
            {
                return true;
            }
            else if (Soteria.CanUse(out act))
            {
                return true;
            }
        }

        if (Configs.GetBool("UseAddersgalWhenLowMP") && Player.CurrentMp < Configs.GetInt("EmergencyMP"))
        {
            //if (Addersgall == 2 && (!Rhizomata.IsCoolingDown  || Rhizomata.WillHaveOneCharge(20)))
            //{
            if (Druochole.CanUse(out act, CanUseOption.MustUse | CanUseOption.IgnoreTarget))
            {
                return true;
            }
            //}
        }

        int lucidDreamingThreshold = Configs.GetInt("LucidDreaming");
        if (Player.CurrentMp < lucidDreamingThreshold && LucidDreaming.CanUse(out act))
        {
            return true;
        }

        return base.EmergencyAbility(nextGCD, out act);
    }

    [RotationDesc(ActionID.Haima, ActionID.Taurochole, ActionID.Panhaima, ActionID.Kerachole, ActionID.Holos)]
    protected override bool DefenseSingleAbility(out IAction act)
    {
        if (Addersgall <= 1)
        {
            if (Haima.CanUse(out act, CanUseOption.OnLastAbility))
            {
                return true;
            }
        }

        if (Taurochole.CanUse(out act, CanUseOption.OnLastAbility) && Taurochole.Target.GetHealthRatio() < 0.8)
        {
            return true;
        }

        if (Addersgall <= 1)
        {
            if ((!Haima.EnoughLevel || Haima.ElapsedAfter(20)) && Panhaima.CanUse(out act, CanUseOption.OnLastAbility))
            {
                return true;
            }
        }

        if ((!Taurochole.EnoughLevel || Taurochole.ElapsedAfter(20)) && Kerachole.CanUse(out act, CanUseOption.OnLastAbility))
        {
            return true;
        }

        if (Holos.CanUse(out act, CanUseOption.OnLastAbility))
        {
            return true;
        }

        return base.DefenseSingleAbility(out act);
    }

    [RotationDesc(ActionID.Panhaima, ActionID.Kerachole, ActionID.Holos)]
    protected override bool DefenseAreaAbility(out IAction act)
    {
        if (Addersgall <= 1)
        {
            if (Panhaima.CanUse(out act, CanUseOption.OnLastAbility))
            {
                return true;
            }
        }

        if (Kerachole.CanUse(out act, CanUseOption.OnLastAbility))
        {
            return true;
        }

        if (Holos.CanUse(out act, CanUseOption.OnLastAbility))
        {
            return true;
        }

        return base.DefenseAreaAbility(out act);
    }

    protected override bool GeneralAbility(out IAction act)
    {
        if (Kardia.CanUse(out act))
        {
            return true;
        }

        if (Addersgall <= 1 && Rhizomata.CanUse(out act))
        {
            return true;
        }

        //if (Soteria.CanUse(out act) && PartyMembers.Any(b => b.HasStatus(true, StatusID.Kardion) && b.GetHealthRatio() < HealthSingleAbility)) return true;         // Returns an error not sure what to do just yet

        if (Pepsis.CanUse(out act))
        {
            return true;
        }

        return base.GeneralAbility(out act);
    }

    [RotationDesc(ActionID.Taurochole, ActionID.Kerachole, ActionID.Druochole, ActionID.Holos, ActionID.Physis, ActionID.Panhaima)]
    protected override bool HealSingleAbility(out IAction act)
    {
        if (Taurochole.CanUse(out act))
        {
            return true;
        }

        if (Kerachole.CanUse(out act) && EnhancedKerachole.EnoughLevel)
        {
            return true;
        }

        if ((!Taurochole.EnoughLevel || Taurochole.IsCoolingDown) && Druochole.CanUse(out act))
        {
            return true;
        }

        if (Soteria.CanUse(out act) && PartyMembers.Any(b => b.HasStatus(true, StatusID.Kardion) && b.GetHealthRatio() < HealthSingleAbility))
        {
            return true;
        }

        var tank = PartyTanks;
        if (Addersgall < 1 && (tank.Any(t => t.GetHealthRatio() < 0.65f) || PartyMembers.Any(b => b.GetHealthRatio() < 0.20f)))
        {
            if (Haima.CanUse(out act, CanUseOption.OnLastAbility))
            {
                return true;
            }

            if (Physis2.CanUse(out act))
            {
                return true;
            }

            if (!Physis2.EnoughLevel && Physis.CanUse(out act))
            {
                return true;
            }

            if (Holos.CanUse(out act, CanUseOption.OnLastAbility))
            {
                return true;
            }

            if ((!Haima.EnoughLevel || Haima.ElapsedAfter(20)) && Panhaima.CanUse(out act, CanUseOption.OnLastAbility))
            {
                return true;
            }
        }

        if (PartyTanks.Any(t => t.GetHealthRatio() < 0.60f))
        {
            if (Zoe.CanUse(out act))
            {
                return true;
            }
        }

        if (PartyTanks.Any(t => t.GetHealthRatio() < 0.70f) || PartyMembers.Any(b => b.GetHealthRatio() < 0.30f))
        {
            if (Krasis.CanUse(out act))
            {
                return true;
            }
        }

        if (Kerachole.CanUse(out act))
        {
            return true;
        }

        return base.HealSingleAbility(out act);
    }

    [RotationDesc(ActionID.Kerachole, ActionID.Physis, ActionID.Holos, ActionID.Ixochole)]
    protected override bool HealAreaAbility(out IAction act)
    {
        if (Physis2.CanUse(out act))
        {
            return true;
        }

        if (!Physis2.EnoughLevel && Physis.CanUse(out act))
        {
            return true;
        }

        if (Kerachole.CanUse(out act, CanUseOption.OnLastAbility) && EnhancedKerachole.EnoughLevel)
        {
            return true;
        }

        if (Holos.CanUse(out act, CanUseOption.OnLastAbility) && PartyMembersAverHP < 0.50f)
        {
            return true;
        }

        if (Ixochole.CanUse(out act, CanUseOption.OnLastAbility))
        {
            return true;
        }

        if (Kerachole.CanUse(out act, CanUseOption.OnLastAbility))
        {
            return true;
        }

        return base.HealAreaAbility(out act);
    }

    #endregion oGCD Logic

    #region Job Helper Methods

    private static bool InTwoMIsBurst()
    {
        if (RatioOfMembersIn2minsBurst >= 0.5)
        {
            return true;
        }

        if (RatioOfMembersIn2minsBurst == -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion Job Helper Methods

    #region Miscellaneous Helper Methods

    // Updates Status of other extra helper methods on every frame draw.
    protected override void UpdateInfo()
    {
        /* OpenerHelpers.StateOfOpener(); */
    }

    #endregion Miscellaneous Helper Methods
}