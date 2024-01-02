using ImGuiNET;
using KirboRotations.Custom.Configurations;
using KirboRotations.Custom.Configurations.Enums;
using KirboRotations.Custom.Data;
using KirboRotations.JobHelpers;
using KirboRotations.UI;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Attributes;
using RotationSolver.Basic.Configuration.RotationConfig;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Helpers;
using RotationSolver.Basic.Rotations.Basic;

namespace KirboRotations.PvE.Ranged;

[RotationDesc(ActionID.Wildfire)]
[LinkDescription("https://i.imgur.com/23r8kFK.png", "Early AA")]
[LinkDescription("https://i.imgur.com/vekKW2k.jpg", "Delayed Tools")]
internal class PvE_MCH_Kirbo : MCH_Base
{
    #region Rotation Info
    public override CombatType Type => CombatType.PvE;
    public override string GameVersion => "6.51";
    public override string RotationName => $"{USERNAME}'s {ClassJob.Abbreviation} [{Type}]";
    #endregion Rotation Info

    #region New PvE IBaseActions
    private new static IBaseAction Dismantle { get; } = new BaseAction(ActionID.Dismantle, ActionOption.None | ActionOption.Defense);

    private new static IBaseAction Drill { get; } = new BaseAction(ActionID.Drill)
    {
        ActionCheck = (b, m) => !IsOverheated,
    };

    private new static IBaseAction AirAnchor { get; } = new BaseAction(ActionID.AirAnchor)
    {
        ActionCheck = (b, m) => !IsOverheated,
    };

    private new static IBaseAction ChainSaw { get; } = new BaseAction(ActionID.ChainSaw)
    {
        ActionCheck = (b, m) => !IsOverheated,
    };

    private new static IBaseAction Wildfire { get; } = new BaseAction(ActionID.Wildfire)
    {
        ActionCheck = (b, m) => (Player.HasStatus(true, StatusID.Overheated) && MCHHelper.HeatStacks > 4 || Heat >= 45) && InCombat
    };

    private new static IBaseAction Reassemble { get; } = new BaseAction(ActionID.Reassemble)
    {
        StatusProvide = new StatusID[1] { StatusID.Reassemble },
        ActionCheck = (b, m) => !Player.HasStatus(true, StatusID.Reassemble),
    };

    private new static IBaseAction Hypercharge { get; } = new BaseAction(ActionID.Hypercharge, ActionOption.UseResources)
    {
        StatusProvide = new StatusID[1] { StatusID.Overheated },
        ActionCheck = (b, m) => !IsOverheated && Heat >= 50 && IsLongerThan(10f)
    };

    private new static IBaseAction BarrelStabilizer { get; } = new BaseAction(ActionID.BarrelStabilizer)
    {
        ActionCheck = (b, m) => Heat <= 45 && InCombat && Target.IsTargetable && Target != Player
    };

    #endregion New PvE IBaseActions

    #region Debug window

    /// <summary>
    /// Displays the debug status in the rotation status panel
    /// </summary>
    public override bool ShowStatus => true;
    public override void DisplayStatus()
    {
        RotationConfigs rotationData = new ();
        rotationData.AddUltimateCompatibility(UltimateCompatibility.UCoB);

        rotationData.AddContentCompatibility(ContentCompatibility.DutyRoulette);
        rotationData.AddContentCompatibility(ContentCompatibility.SavageRaids);
        rotationData.AddContentCompatibility(ContentCompatibility.ExtremeTrials);
        rotationData.AddContentCompatibility(ContentCompatibility.Criterion);
        rotationData.AddContentCompatibility(ContentCompatibility.Hunts);

        rotationData.AddFeatures(Features.UseTincture);
        rotationData.AddFeatures(Features.SavageOptimized);
        rotationData.AddFeatures(Features.HasUserConfig);

        rotationData.SetRotationOpeners("Early AA", "Delayed Tools");
        rotationData.CurrentRotationSelection = Configs.GetCombo("RotationSelection");

        DebugWindow.DisplayDebugWindow(RotationName, rotationData.RotationVersion, rotationData);
    }

    #endregion Debug window

    #region Action Related Properties

    // Check if the major tools will come off cooldown soon
    internal bool WillhaveTool { get; set; }

    /*// Holds the remaining amount of Heat stacks
    internal static byte HeatStacks
    {
        get
        {
            byte stacks = Player.StatusStack(true, StatusID.Overheated);
            return stacks == byte.MaxValue ? (byte)5 : stacks;
        }
    }*/

    #endregion Action Related Properties

    #region Rotation Config

    protected override IRotationConfigSet CreateConfiguration() => base.CreateConfiguration()
        .SetCombo(CombatType.PvE, "RotationSelection", 0, "Select which Rotation will be used. (Openers will only be followed at level 90)", "Early AA", "Delayed Tools"/*, "Early All"*/)
        .SetBool(CombatType.PvE, "BatteryStuck", false, "Battery overcap protection\n(Will try and use Rook AutoTurret if Battery is at 100 and next skill increases Battery)")
        .SetBool(CombatType.PvE, "HeatStuck", false, "Heat overcap protection\n(Will try and use HyperCharge if Heat is at 100 and next skill increases Heat)")
        .SetBool(CombatType.PvE, "DumpSkills", false, "Dump Skills when Target is dying\n(Will try and spend remaining resources before boss dies)")
        .SetBool(CombatType.PvE, "NoWaste", false, "(NO ACTUAL CODE IMPLEMENTED YET)\nTries to save important actions for Boss\n(Example: If Boss is out of reach and you're stuck killing adds)");

    #endregion Rotation Config

    #region Countdown Logic

    protected override IAction CountDownAction(float remainTime)
    {
        TerritoryContentType Content = TerritoryContentType;
        bool UltimateRaids = (int)Content == 28;
        bool UwUorUCoB = UltimateRaids && Player.Level == 70;
        bool TEA = UltimateRaids && Player.Level == 80;

        // If 'OpenerActionsAvailable' is true (see method 'HandleOpenerAvailability' for conditions) proceed to using Action logic during countdown
        if (OpenerHelpers.OpenerActionsAvailable)
        {
            // Selects action logic depending on which rotation has been selected (Default: Delayed Tool)
            switch (Configs.GetCombo("RotationSelection"))
            {
                case 0: // Early AA
                    // Use Drill when the remaining countdown time is less or equal to Drill's AnimationLock, also sets OpenerInProgress to 'True'
                    if (remainTime <= AirAnchor.AnimationLockTime && AirAnchor.CanUse(out _))
                    {
                        OpenerHelpers.OpenerInProgress = true;
                        return AirAnchor;
                    }
                    // Use Tincture if Tincture use is enabled and the countdown time is less or equal to SplitShot+Tincture animationlock (1.8s)
                    IAction act0;
                    if (remainTime <= AirAnchor.AnimationLockTime + TinctureOfDexterity8.AnimationLockTime && UseBurstMedicine(out act0, false))
                    {
                        return act0;
                    }
                    // Use Reassemble
                    if (remainTime <= 5f && Reassemble.CurrentCharges == 2)
                    {
                        return Reassemble;
                    }
                    break;

                case 1: // Delayed Tools
                    // Use SplitShot when the remaining countdown time is less or equal to SplitShot's AnimationLock, also sets OpenerInProgress to 'True'
                    if (remainTime <= SplitShot.AnimationLockTime && SplitShot.CanUse(out _))
                    {
                        OpenerHelpers.OpenerInProgress = true;
                        return SplitShot;
                    }
                    // Use Tincture if Tincture use is enabled and the countdown time is less or equal to SplitShot+Tincture animationlock (1.8s)
                    IAction act1;
                    if (remainTime <= SplitShot.AnimationLockTime + TinctureOfDexterity8.AnimationLockTime && UseBurstMedicine(out act1, false))
                    {
                        return act1;
                    }
                    break;

                    //case 2: // Early All
                    //    if (remainTime <= AirAnchor.AnimationLockTime && Player.HasStatus(true, StatusID.Reassemble) && AirAnchor.CanUse(out _))
                    //    {
                    //        OpenerInProgress = true;
                    //        return AirAnchor;
                    //    }
                    //    if (remainTime <= 5f && Reassemble.CurrentCharges > 1 && !Player.HasStatus(true, StatusID.Reassemble))
                    //    {
                    //        return Reassemble;
                    //    }
                    //    break;
            }
        }
        if (Player.Level < 90)
        {
            if (AirAnchor.EnoughLevel && remainTime <= 0.6 + CountDownAhead && AirAnchor.CanUse(out _))
            {
                return AirAnchor;
            }
            if (!AirAnchor.EnoughLevel && Drill.EnoughLevel && remainTime <= 0.6 + CountDownAhead && Drill.CanUse(out _))
            {
                return Drill;
            }
            if (!AirAnchor.EnoughLevel && !Drill.EnoughLevel && HotShot.EnoughLevel && remainTime <= 0.6 + CountDownAhead && HotShot.CanUse(out _))
            {
                return HotShot;
            }
            if (!AirAnchor.EnoughLevel && !Drill.EnoughLevel && !HotShot.EnoughLevel && remainTime <= 0.6 + CountDownAhead && CleanShot.CanUse(out _))
            {
                return CleanShot;
            }
            if (remainTime < 5f && Reassemble.CurrentCharges > 0 && !Player.HasStatus(true, StatusID.Reassemble))
            {
                return Reassemble;
            }
        }

        if (UltimateRaids)
        {
            if (UwUorUCoB)
            {
                if (remainTime <= Drill.AnimationLockTime && Player.HasStatus(true, StatusID.Reassemble) && Drill.CanUse(out _))
                {
                    return Drill;
                }
                if (remainTime < 5f && Reassemble.CurrentCharges > 0 && !Player.HasStatus(true, StatusID.Reassemble))
                {
                    return Reassemble;
                }
                return base.CountDownAction(remainTime);
            }
            if (TEA)
            {
                if (remainTime <= AirAnchor.AnimationLockTime && Player.HasStatus(true, StatusID.Reassemble) && AirAnchor.CanUse(out _))
                {
                    return AirAnchor;
                }
                if (remainTime < 5f && Reassemble.CurrentCharges > 0 && !Player.HasStatus(true, StatusID.Reassemble))
                {
                    return Reassemble;
                }
                return base.CountDownAction(remainTime);
            }
            return base.CountDownAction(remainTime);
        }
        return base.CountDownAction(remainTime);
    }

    #endregion Countdown Logic

    #region Opener Logic

    private bool Opener(out IAction act)
    {
        act = default;
        while (OpenerHelpers.OpenerInProgress)
        {
            //if (/*!OpenerHelpers.OpenerFlag && */(Player.IsDead) || (TimeSinceLastAction.TotalSeconds > 3.0))
            //{
            //OpenerHelpers.OpenerHasFailed = true;
            /* OpenerHelpers.OpenerFlag = true; */
            //}
            switch (Configs.GetCombo("RotationSelection"))
            {
                case 0: // Early AA
                    switch (OpenerHelpers.OpenerStep)
                    {
                        case 0:
                            return OpenerHelpers.OpenerController(IsLastGCD(false, AirAnchor), AirAnchor.CanUse(out act, CanUseOption.MustUse));

                        case 1:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));

                        case 2:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));

                        case 3:
                            return OpenerHelpers.OpenerController(IsLastGCD(false, Drill), Drill.CanUse(out act, CanUseOption.MustUse));

                        case 4:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, BarrelStabilizer), BarrelStabilizer.CanUse(out act, CanUseOption.MustUse));

                        case 5:
                            return OpenerHelpers.OpenerController(IsLastGCD(true, SplitShot), SplitShot.CanUse(out act, CanUseOption.MustUse));

                        case 6:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));

                        case 7:
                            return OpenerHelpers.OpenerController(IsLastGCD(true, SlugShot), SlugShot.CanUse(out act, CanUseOption.MustUse));

                        case 8:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));

                        case 9:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));

                        case 10:
                            return OpenerHelpers.OpenerController(IsLastGCD(true, CleanShot), CleanShot.CanUse(out act, CanUseOption.MustUse));

                        case 11:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, Reassemble), Reassemble.CanUse(out act, CanUseOption.MustUseEmpty));

                        case 12:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, Wildfire), Wildfire.CanUse(out act, (CanUseOption)17));

                        case 13:
                            return OpenerHelpers.OpenerController(IsLastGCD(false, ChainSaw), ChainSaw.CanUse(out act, CanUseOption.MustUse));

                        case 14:
                            return OpenerHelpers.OpenerController(IsLastAbility(true, RookAutoturret), RookAutoturret.CanUse(out act, CanUseOption.MustUse));

                        case 15:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, Hypercharge), Hypercharge.CanUse(out act, (CanUseOption)51));

                        case 16:
                            return OpenerHelpers.OpenerController(IsLastGCD(false, HeatBlast) && MCHHelper.HeatStacks == 4, HeatBlast.CanUse(out act, CanUseOption.MustUse));

                        case 17:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));

                        case 18:
                            return OpenerHelpers.OpenerController(IsLastGCD(false, HeatBlast) && MCHHelper.HeatStacks == 3, HeatBlast.CanUse(out act, CanUseOption.MustUse));

                        case 19:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));

                        case 20:
                            return OpenerHelpers.OpenerController(IsLastGCD(false, HeatBlast) && MCHHelper.HeatStacks == 2, HeatBlast.CanUse(out act, CanUseOption.MustUse));

                        case 21:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));

                        case 22:
                            return OpenerHelpers.OpenerController(IsLastGCD(false, HeatBlast) && MCHHelper.HeatStacks == 1, HeatBlast.CanUse(out act, CanUseOption.MustUse));

                        case 23:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));

                        case 24:
                            return OpenerHelpers.OpenerController(IsLastGCD(false, HeatBlast) && MCHHelper.HeatStacks == 0, HeatBlast.CanUse(out act, CanUseOption.MustUse));

                        case 25:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));

                        case 26:
                            return OpenerHelpers.OpenerController(IsLastGCD(false, Drill), Drill.CanUse(out act, CanUseOption.MustUse));

                        case 27:
                            OpenerHelpers.OpenerHasFinished = true;
                            OpenerHelpers.OpenerInProgress = false;
                            //Serilog.Log.Information($"{v} {OpenerHelpers.OpenerComplete} - Early AA");
                            // Finished Early AA
                            break;
                    }
                    break;

                case 1: // Delayed Tools
                    switch (OpenerHelpers.OpenerStep)
                    {
                        case 0:
                            return OpenerHelpers.OpenerController(IsLastGCD(true, SplitShot), SplitShot.CanUse(out act, CanUseOption.MustUse));

                        case 1:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));

                        case 2:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));

                        case 3:
                            return OpenerHelpers.OpenerController(IsLastGCD(false, Drill), Drill.CanUse(out act, CanUseOption.MustUse));

                        case 4:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, BarrelStabilizer), BarrelStabilizer.CanUse(out act, CanUseOption.MustUse));

                        case 5:
                            return OpenerHelpers.OpenerController(IsLastGCD(true, SlugShot), SlugShot.CanUse(out act, CanUseOption.MustUse));

                        case 6:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));

                        case 7:
                            return OpenerHelpers.OpenerController(IsLastGCD(true, CleanShot), CleanShot.CanUse(out act, CanUseOption.MustUse));

                        case 8:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, Reassemble), Reassemble.CanUse(out act, CanUseOption.MustUseEmpty));

                        case 9:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));

                        case 10:
                            return OpenerHelpers.OpenerController(IsLastGCD(false, AirAnchor), AirAnchor.CanUse(out act, CanUseOption.MustUse));

                        case 11:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, Reassemble), Reassemble.CanUse(out act, CanUseOption.MustUseEmpty));

                        case 12:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, Wildfire), Wildfire.CanUse(out act, (CanUseOption)17));

                        case 13:
                            return OpenerHelpers.OpenerController(IsLastGCD(false, ChainSaw), ChainSaw.CanUse(out act, CanUseOption.MustUse));

                        case 14:
                            return OpenerHelpers.OpenerController(IsLastAbility(true, RookAutoturret), RookAutoturret.CanUse(out act, CanUseOption.MustUse));

                        case 15:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, Hypercharge), Hypercharge.CanUse(out act, (CanUseOption)51));

                        case 16:
                            return OpenerHelpers.OpenerController(IsLastGCD(false, HeatBlast) && MCHHelper.HeatStacks == 4, HeatBlast.CanUse(out act, CanUseOption.MustUse));

                        case 17:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));

                        case 18:
                            return OpenerHelpers.OpenerController(IsLastGCD(false, HeatBlast) && MCHHelper.HeatStacks == 3, HeatBlast.CanUse(out act, CanUseOption.MustUse));

                        case 19:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));

                        case 20:
                            return OpenerHelpers.OpenerController(IsLastGCD(false, HeatBlast) && MCHHelper.HeatStacks == 2, HeatBlast.CanUse(out act, CanUseOption.MustUse));

                        case 21:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));

                        case 22:
                            return OpenerHelpers.OpenerController(IsLastGCD(false, HeatBlast) && MCHHelper.HeatStacks == 1, HeatBlast.CanUse(out act, CanUseOption.MustUse));

                        case 23:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));

                        case 24:
                            return OpenerHelpers.OpenerController(IsLastGCD(false, HeatBlast) && MCHHelper.HeatStacks == 0, HeatBlast.CanUse(out act, CanUseOption.MustUse));

                        case 25:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));

                        case 26:
                            return OpenerHelpers.OpenerController(IsLastGCD(false, Drill), Drill.CanUse(out act, CanUseOption.MustUse));

                        case 27:
                            OpenerHelpers.OpenerHasFinished = true;
                            //OpenerHelpers.OpenerInProgress = false;
                            //Serilog.Log.Information($"{v} {OpenerHelpers.OpenerComplete} - Delayed Tools");
                            // Finished Delayed Tools
                            break;
                    }
                    break;

                case 2: // Early All
                    switch (OpenerHelpers.OpenerStep)
                    {
                        case 0:
                            return OpenerHelpers.OpenerController(IsLastGCD(false, AirAnchor), AirAnchor.CanUse(out act, CanUseOption.MustUse));

                        case 1:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, BarrelStabilizer), BarrelStabilizer.CanUse(out act, CanUseOption.MustUseEmpty));

                        case 2:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty | CanUseOption.OnLastAbility));

                        case 3:
                            return OpenerHelpers.OpenerController(IsLastGCD(false, Drill), Drill.CanUse(out act, CanUseOption.MustUse));

                        case 4:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, Reassemble), Reassemble.CanUse(out act, CanUseOption.MustUseEmpty));

                        case 5:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty | CanUseOption.OnLastAbility));

                        case 6:
                            return OpenerHelpers.OpenerController(IsLastGCD(false, ChainSaw), ChainSaw.CanUse(out act, CanUseOption.MustUse));

                        case 7:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));

                        case 8:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty | CanUseOption.OnLastAbility));

                        case 9:
                            return OpenerHelpers.OpenerController(IsLastGCD(true, SplitShot), SplitShot.CanUse(out act, CanUseOption.MustUse));

                        case 10:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));

                        case 11:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty | CanUseOption.OnLastAbility));

                        case 12:
                            return OpenerHelpers.OpenerController(IsLastGCD(true, SlugShot), SlugShot.CanUse(out act, CanUseOption.MustUse));

                        case 13:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, Tactician), Tactician.CanUse(out act, CanUseOption.MustUseEmpty));

                        case 14:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, Wildfire), Wildfire.CanUse(out act, CanUseOption.OnLastAbility));

                        case 15:
                            return OpenerHelpers.OpenerController(IsLastGCD(true, CleanShot), CleanShot.CanUse(out act, CanUseOption.MustUse));

                        case 16:
                            return OpenerHelpers.OpenerController(IsLastAbility(true, RookAutoturret), RookAutoturret.CanUse(out act, CanUseOption.MustUse));

                        case 17:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, Hypercharge), Hypercharge.CanUse(out act, CanUseOption.OnLastAbility));

                        case 18:
                            OpenerHelpers.OpenerHasFinished = true;
                            //OpenerHelpers.OpenerInProgress = false;
                            // Finished Early All
                            break;
                    }
                    break;
            }
        }
        act = null;
        return false;
    }

    #endregion Opener Logic

    #region GCD Logic

    protected override bool GeneralGCD(out IAction act)
    {
        act = null;

        if (Player.HasStatus(true, (StatusID)Buffs.Transcendent) || !Player.InCombat())
        {
            // Logic when the player is recently revived
            return false;
        }

        if (OpenerHelpers.OpenerInProgress)
        {
            return Opener(out act);
        }

        if (!OpenerHelpers.OpenerInProgress)
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
                if (Drill.EnoughLevel && Drill.WillHaveOneCharge(0.1f))
                {
                    return false;
                }
                return true;
            }
            if (SlugShot.CanUse(out act, CanUseOption.MustUse))
            {
                if (Drill.EnoughLevel && Drill.WillHaveOneCharge(0.1f))
                {
                    return false;
                }
                return true;
            }
            if (SplitShot.CanUse(out act, CanUseOption.MustUse))
            {
                if (Drill.EnoughLevel && Drill.WillHaveOneCharge(0.1f))
                {
                    return false;
                }
                return true;
            }
        }

        return base.GeneralGCD(out act);
    }

    #endregion GCD Logic

    #region oGCD Logic

    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
    {
        act = null;

        if (Player.HasStatus(true, (StatusID)Buffs.Transcendent) || !Player.InCombat())
        {
            // Logic when the player is recently revived
            return false;
        }
        if (ShouldUseBurstMedicine(out act))
        {
            return true;
        }
        if (OpenerHelpers.OpenerInProgress)
        {
            return Opener(out act);
        }
        if (Configs.GetBool("BatteryStuck") && Battery == 100 && RookAutoturret.CanUse(out act, CanUseOption.MustUseEmpty) && (nextGCD == ChainSaw || nextGCD == AirAnchor || nextGCD == CleanShot))
        {
            return true;
        }
        if (Configs.GetBool("HeatStuck") && Heat == 100 && Hypercharge.CanUse(out act, CanUseOption.MustUseEmpty) && (nextGCD == SplitShot || nextGCD == SlugShot || nextGCD == CleanShot))
        {
            return true;
        }
        if (DumpSkills(nextGCD, out act) && Configs.GetBool("DumpSkills") && HostileTarget.IsDying() && HostileTarget.IsBossFromIcon())
        {
            return true;
        }

        // LvL 90+
        if (!OpenerHelpers.OpenerInProgress)
        {
            if (Wildfire.CanUse(out act, (CanUseOption)16))
            {
                if (nextGCD == ChainSaw && Heat >= 50 ||
                    IsLastAbility(ActionID.Hypercharge) && MCHHelper.HeatStacks > 4 ||
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
                    if (nextGCD == ChainSaw || nextGCD == AirAnchor || nextGCD == Drill)
                    {
                        Serilog.Log.Information($"{v} More then 1 Reassemble Charge. using Reassemble on any major action");
                        return true;
                    }
                }

                if (nextGCD.IsTheSameTo(false, ChainSaw))
                {
                    Serilog.Log.Information($"{v} Next GCD is ChainSaw.  Using Reassemble");
                    return true;
                }
                if (nextGCD.IsTheSameTo(true, AirAnchor) && !Wildfire.WillHaveOneCharge(55f))
                {
                    Serilog.Log.Information($"{v} Next GCD is AA.  And Wildfire will not be ready within the next 55 seconds.   Using Reassemble");
                    return true;
                }
                if (nextGCD.IsTheSameTo(false, Drill) && !Wildfire.WillHaveOneCharge(55f))
                {
                    Serilog.Log.Information($"{v} Next GCD is Drill.  And Wildfire will not be ready within the next 55 seconds.   Using Reassemble");
                    return true;
                }
            }

            if (RookAutoturret.CanUse(out act, (CanUseOption)16) && HostileTarget && HostileTarget.IsTargetable && InCombat)
            {
                if (CombatElapsedLess(60f) && !CombatElapsedLess(48f) && Battery >= 50)
                {
                    return true;
                }
                if (Wildfire.IsCoolingDown && Wildfire.ElapsedAfter(105f) && Battery == 100 && (nextGCD == AirAnchor || nextGCD == CleanShot))
                {
                    return true;
                }
                if (Battery >= 90 && !Wildfire.ElapsedAfter(70f))
                {
                    return true;
                }
                if (Battery >= 80 && !Wildfire.ElapsedAfter(77.5f) && IsLastGCD((ActionID)16500))
                {
                    return true;
                }
            }

            // when using delayed tools, hypercharge should be used 3 GCD's before dril comes up at the 2min mark. Currently drifts barrel stabilizer and overcaps 20 heat because of it
            // link 'https://xivanalysis.com/fflogs/a:K6Jnpwbv4z3WRyGL/1/1' 12.230dps 10min test
            // when using Early AA, misses crowned colider during 2nd tincture, loses 30 battery due to overcap
            // at 2min mark loses 10 battery as queen is not used at 90 battery which then gets followed by AA
            // at 4min 15s loses another 20 battery when gauge is at 100 shouldve used queen after the clean shot at 4min 7s, 8s before wildfire came off cooldown
            // link 'https://xivanalysis.com/fflogs/a:K6Jnpwbv4z3WRyGL/3/1' 12.100dps 10min test
            // in both tests there's a weaving issue at 8min 46 (either gauss or rico's problem, maybe implement oGCD counter or something)
            if (Hypercharge.CanUse(out act) && (IsLastAbility(ActionID.Wildfire) || !WillhaveTool && (
                BurstHelpers.InBurst && IsLastGCD(ActionID.ChainSaw, ActionID.AirAnchor, ActionID.Drill, ActionID.SplitShot, ActionID.SlugShot, ActionID.CleanShot, ActionID.HeatedSplitShot, ActionID.HeatedSlugShot) ||
                Heat >= 100 && Wildfire.WillHaveOneCharge(10f) ||
                Heat >= 100 && Wildfire.WillHaveOneCharge(40f) || // was 90 (causes issues with 2min early aa burst)
                Heat >= 50 && !Wildfire.WillHaveOneCharge(40f)
            )))
            {
                return true;
            }

            if (ShouldUseGaussroundOrRicochet(out act) && WeaponRemain > GaussRound.AnimationLockTime + Ping)
            {
                return true;
            }
        }

        // LvL 30-89 and Casual Content
        if (Player.Level < 90)
        {
            if ((IsLastAbility(false, Hypercharge) || Heat >= 50) && HostileTarget.IsBossFromIcon()
                && Wildfire.CanUse(out act, CanUseOption.OnLastAbility)) return true;

            if (Reassemble.CurrentCharges > 0 && Reassemble.CanUse(out act, CanUseOption.MustUseEmpty))
            {
                if (ChainSaw.EnoughLevel && (nextGCD == ChainSaw || nextGCD == Drill || nextGCD == AirAnchor))
                {
                    return true;
                }
                if (!Drill.EnoughLevel && nextGCD == CleanShot)
                {
                    return true;
                }
            }
            if (BarrelStabilizer.CanUse(out act) && HostileTarget && HostileTarget.IsTargetable && InCombat)
            {
                return true;
            }
            if (Hypercharge.CanUse(out act) && InCombat && HostileTarget && HostileTarget.IsTargetable)
            {
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
            }
            if (RookAutoturret.CanUse(out act) && HostileTarget && HostileTarget.IsTargetable && InCombat)
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

            if (ShouldUseGaussroundOrRicochet(out act) && NextAbilityToNextGCD > GaussRound.AnimationLockTime + Ping)
            {
                return true;
            }
        }

        return base.EmergencyAbility(nextGCD, out act);
    }

    #endregion oGCD Logic

    private bool TestRotation(IAction nextGCD, out IAction act)
    {
        act = null;

        return false;
    }

    #region Job Helper Methods
    // This should be relevant to the Action shown in the [RotationDesc(ActionID.Action]
    private void BurstActionCheck()
    {
        BurstHelpers.InBurst = Player.HasStatus(true, StatusID.Wildfire);
    }

    // This should be relevant to the Action shown in the [RotationDesc(ActionID.Action]
    private void SaveAction()
    {
        TerritoryContentType Content = TerritoryContentType;
        bool UltimateRaids = (int)Content == 28;
        bool UwUorUCoB = UltimateRaids && Player.Level == 70;
        bool saveneeded = Target.GetHealthRatio() <= 0.25 && CombatTime < 150;

        if (!saveneeded)
        {
            GeneralHelpers.SaveAction = false;
        }

        if (UwUorUCoB && saveneeded)
        {
            GeneralHelpers.SaveAction = true;
        }
    }

    // Tincture Conditions
    private bool ShouldUseBurstMedicine(out IAction act)
    {
        act = null; // Default to null if Tincture cannot be used.

        // Don't use Tincture if player has the 'Weakness' status
        if (Player.HasStatus(true, StatusID.Weakness) || Player.HasStatus(true, (StatusID)Buffs.Transcendent))
        {
            return false;
        }

        // Check if the conditions for using Burst Medicine are met:
        // Wildfire's CD is less then 20s
        // Combat has been ongoing for atleast 60s
        // Atleast 1.2s left in oGCD window
        // Again as a double fail safe, Player does not have the weakness debuff
        // TinctureTier 6/7/8 are NOT on cooldown (Should be fine as when either 1 is on cooldown the others are as well, might remove lower tier tinctures at some point)
        // Drill's CD is 3s or less
        if (Wildfire.WillHaveOneCharge(20) && CombatTime > 60 && NextAbilityToNextGCD > 1.2 && !Player.HasStatus(true, StatusID.Weakness)
            && !TinctureOfDexterity6.IsCoolingDown && !TinctureOfDexterity7.IsCoolingDown && !TinctureOfDexterity8.IsCoolingDown && Drill.WillHaveOneCharge(3))
        {
            // Attempt to use Burst Medicine.
            return UseBurstMedicine(out act, false);
        }
        // If the conditions are not met, return false.
        return false;
    }

    // GaussRound & Ricochet Condition
    private bool ShouldUseGaussroundOrRicochet(out IAction act)
    {
        act = null; // Initialize the action as null.

        // First, check if both GaussRound and Ricochet do not have at least one charge.
        // If neither has a charge, we cannot use either, so return false.
        if (!GaussRound.HasOneCharge && !Ricochet.HasOneCharge)
        {
            return false;
        }

        if (!GaussRound.HasOneCharge && !Ricochet.EnoughLevel)
        {
            return false;
        }

        // Second, check if Ricochet is not at a sufficient level to be used.
        // If not, default to GaussRound (if it can be used).
        if (!Ricochet.EnoughLevel)
        {
            return GaussRound.CanUse(out act, CanUseOption.MustUseEmpty);
        }

        // Third, check if GaussRound and Ricochet have the same number of charges.
        // If they do, prefer using GaussRound.
        if (GaussRound.CurrentCharges >= Ricochet.CurrentCharges)
        {
            return GaussRound.CanUse(out act, CanUseOption.MustUseEmpty);
        }

        // Fourth, check if Ricochet has more or an equal number of charges compared to GaussRound.
        // If so, prefer using Ricochet.
        if (Ricochet.CurrentCharges >= GaussRound.CurrentCharges)
        {
            return Ricochet.HasOneCharge && Ricochet.CanUse(out act, CanUseOption.MustUseEmpty);
        }
        // If none of the above conditions are met, default to using GaussRound.
        // This is a fallback in case other conditions fail to determine a clear action.
        return GaussRound.CanUse(out act, CanUseOption.MustUseEmpty);
    }

    // 
    private bool DumpSkills(IAction nextGCD, out IAction act)
    {
        if (!Player.HasStatus(true, StatusID.Reassemble) && Reassemble.CanUse(out act, (CanUseOption)2) && Reassemble.CurrentCharges > 0 && (nextGCD == ChainSaw || nextGCD == AirAnchor || nextGCD == Drill))
        {
            return true;
        }
        if (BarrelStabilizer.CanUse(out act, CanUseOption.MustUse))
        {
            return true;
        }
        if (AirAnchor.CanUse(out act, CanUseOption.MustUse))
        {
            return true;
        }
        if (ChainSaw.CanUse(out act, CanUseOption.MustUse))
        {
            return true;
        }
        if (Drill.CanUse(out act, CanUseOption.MustUse))
        {
            return true;
        }
        if (RookAutoturret.CanUse(out act, CanUseOption.MustUse) && Battery >= 50)
        {
            return true;
        }
        if (Hypercharge.CanUse(out act) && !WillhaveTool && Heat >= 50)
        {
            return true;
        }
        if (HostileTarget.GetHealthRatio() < 0.03 && nextGCD == CleanShot && Reassemble.CurrentCharges > 0 && Reassemble.CanUse(out act, CanUseOption.IgnoreClippingCheck))
        {
            return true;
        }
        if (HostileTarget.GetHealthRatio() < 0.03 && RookAutoturret.ElapsedAfter(5f) && QueenOverdrive.CanUse(out act))
        {
            return true;
        }
        if (HostileTarget.GetHealthRatio() < 0.02 && (Player.HasStatus(true, StatusID.Wildfire) || BurstHelpers.InBurst) && Wildfire.ElapsedAfter(5f) && Detonator.CanUse(out act))
        {
            return true;
        }
        return false;
    }

    // Checks if any major tool skill will almost come off CD.
    private void ToolKitCheck()
    {
        bool WillHaveDrill = !Drill.IsCoolingDown || Drill.WillHaveOneCharge(5f);
        bool WillHaveAirAnchor = !AirAnchor.IsCoolingDown || AirAnchor.WillHaveOneCharge(5f);
        bool WillHaveChainSaw = !ChainSaw.IsCoolingDown || ChainSaw.WillHaveOneCharge(5f);

        if (Player.Level >= 90)
        {
            // Player is level 90 or higher, check all tools
            WillhaveTool = WillHaveDrill || WillHaveAirAnchor || WillHaveChainSaw;
        }
        else if (Player.Level >= 76)
        {
            // Player is level 76 or higher but lower than 90, check Drill and Air Anchor
            WillhaveTool = WillHaveDrill || WillHaveAirAnchor;
        }
        else if (Player.Level >= 58)
        {
            // Player is level 58 or higher but lower than 76, check only Drill
            WillhaveTool = WillHaveDrill;
        }
        // Optionally, add an else clause for levels lower than 58 if needed
    }

    // Used to check OpenerAvailability
    private void HandleOpenerAvailability()
    {
        bool Lvl90 = Player.Level >= 90;
        bool HasChainSaw = !ChainSaw.IsCoolingDown;
        bool HasAirAnchor = !AirAnchor.IsCoolingDown;
        bool HasDrill = !Drill.IsCoolingDown;
        bool HasBarrelStabilizer = !BarrelStabilizer.IsCoolingDown;
        var RCcharges = Ricochet.CurrentCharges;
        bool HasWildfire = !Wildfire.IsCoolingDown;
        var GRcharges = GaussRound.CurrentCharges;
        bool ReassembleOneCharge = Reassemble.CurrentCharges >= 1;
        bool NoHeat = Heat == 0;
        bool NoBattery = Battery == 0;
        bool NoResources = NoHeat && NoBattery;
        bool Openerstep0 = OpenerHelpers.OpenerStep == 0;

        OpenerHelpers.OpenerActionsAvailable = ReassembleOneCharge && HasChainSaw && HasAirAnchor && HasDrill && HasBarrelStabilizer && RCcharges == 3 && HasWildfire && GRcharges == 3 && Lvl90 && NoBattery && NoHeat && Openerstep0;

        // Future Opener conditions for ULTS
        TerritoryContentType Content = TerritoryContentType;
        bool UltimateRaids = (int)Content == 28;
        bool UwUorUCoB = UltimateRaids && Player.Level == 70;
        bool TEA = UltimateRaids && Player.Level == 80;

        OpenerHelpers.LvL70_Ultimate_OpenerActionsAvailable = UwUorUCoB && NoResources && ReassembleOneCharge && HasDrill && HasWildfire && HasBarrelStabilizer;

        OpenerHelpers.LvL80_Ultimate_OpenerActionsAvailable = TEA && NoResources && ReassembleOneCharge && HasDrill && HasAirAnchor && HasWildfire && HasBarrelStabilizer;
    }

    #endregion Job Helper Methods

    #region Miscellaneous Helper Methods
    // Updates Status of other extra helper methods on every frame draw.
    protected override void UpdateInfo()
    {
        if (Player.IsInCombat())
        {
            ToolKitCheck();
        }
        HandleOpenerAvailability();
        BurstActionCheck();
        //ToolKitCheck();
        StateOfOpener(); // Call StateOfOpener to update opener properties
                         //OpenerHelpers.StateOfOpener();
        SaveAction();
    }

    // This method triggers when the game loads an instance or whenever the screen fades to black
    public override void OnTerritoryChanged()
    {
        base.OnTerritoryChanged();
    }

    // Controls the opener flow
    private static void StateOfOpener()
    {
        if (Player.InCombat() && OpenerHelpers.OpenerActionsAvailable)
        {
            OpenerHelpers.OpenerInProgress = true;
        }

        if (Player.InCombat() && OpenerHelpers.OpenerHasFailed)
        {
            OpenerHelpers.OpenerInProgress = false;
        }

        if (Player.InCombat() && OpenerHelpers.OpenerHasFinished)
        {
            OpenerHelpers.OpenerInProgress = false;
        }

        if (Player.IsDead)
        {
            OpenerHelpers.OpenerHasFailed = false;
            OpenerHelpers.OpenerHasFinished = false;
            OpenerHelpers.OpenerStep = 0;
        }

        if (!Player.InCombat())
        {
            OpenerHelpers.OpenerHasFailed = false;
            OpenerHelpers.OpenerHasFinished = false;
            OpenerHelpers.OpenerStep = 0;
        }
    }
    #endregion Miscellaneous Helper Methods
}