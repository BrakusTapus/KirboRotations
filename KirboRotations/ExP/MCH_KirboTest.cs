using ImGuiNET;
using KirboRotations.Custom.Configurations;
using KirboRotations.Custom.Configurations.Enums;
using KirboRotations.JobHelpers;
using KirboRotations.JobHelpers.Openers;
using KirboRotations.UI;
using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Attributes;
using RotationSolver.Basic.Configuration.RotationConfig;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Helpers;
using RotationSolver.Basic.Rotations.Basic;

namespace KirboRotations.ExP;

[RotationDesc(ActionID.Wildfire)]
[LinkDescription("https://i.imgur.com/23r8kFK.png", "Early AA")]
[LinkDescription("https://i.imgur.com/vekKW2k.jpg", "Delayed Tools")]
internal class ExP_MCH_Kirbo : MCH_Base
{
    #region Rotation Info
    public override CombatType Type => CombatType.PvE;
    public override string GameVersion => "0";
    public override string RotationName => $"{USERNAME}'s Test_MCH]";
    #endregion Rotation Info

    internal static MCHOpenerLogic MCHOpener = new();

    #region New PvE IBaseActions

    private static IBaseAction HeatedCleanShot { get; } = new BaseAction((ActionID)7413)
    {
    };

    private new static IBaseAction Dismantle { get; } = new BaseAction(ActionID.Dismantle, ActionOption.None | ActionOption.Defense);

    private new static IBaseAction Drill { get; } = new BaseAction(ActionID.Drill)
    {
        ActionCheck = (b, m) => !IsOverheated,
    };

    private new static IBaseAction AirAnchor { get; } = new BaseAction(ActionID.AirAnchor)
    {
        ChoiceTarget = (Targets, mustUse) =>
        {
            // If no target with PvP_WildfireDebuff, use existing logic
            Targets = Targets.Where(b => b.YalmDistanceX < 25);
            if (Targets.Any())
            {
                return Targets.OrderBy(ObjectHelper.GetHealthRatio).First();
            }
            return null;
        },
        ActionCheck = (b, m) => !IsOverheated,
    };

    private new static IBaseAction ChainSaw { get; } = new BaseAction(ActionID.ChainSaw)
    {
        ActionCheck = (b, m) => !IsOverheated,
    };

    private new static IBaseAction Wildfire { get; } = new BaseAction(ActionID.Wildfire)
    {
        ActionCheck = (b, m) => (Player.HasStatus(true, StatusID.Overheated) && HeatStacks > 4 || Heat >= 45) && InCombat
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
        //MCHOpenerLogic CurrentState = new();
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

        ImGuiExtra.TripleSpacing();
        ImGui.Text($"CurrentState: {MCHOpener.CurrentState}");
        ImGui.Text($"PrePullStep: {MCHOpener.PrePullStep}");
        ImGui.Text($"OpenerStep: {MCHOpener.OpenerStep}");
        ImGui.Text($"TimeSinceLastAction: {TimeSinceLastAction.TotalSeconds}");

        ImGuiExtra.TripleSpacing();
        ImGui.Text($"Your character combat: {Player.IsInCombat()}");
        ImGui.Text($"SaveAction: {SaveAction}");
        ImGui.Text($"WillhaveTool: {WillhaveTool}");

        DebugWindow.DisplayDebugWindow(RotationName, rotationData.RotationVersion, rotationData);
    }

    #endregion Debug window

    #region Action Related Properties

    // Check if the major tools will come off cooldown soon
    private bool WillhaveTool { get; set; }

    // Holds the remaining amount of Heat stacks
    private static byte HeatStacks
    {
        get
        {
            byte stacks = Player.StatusStack(true, StatusID.Overheated);
            return stacks == byte.MaxValue ? (byte)5 : stacks;
        }
    }

    #endregion Action Related Properties

    #region Rotation Config

    protected override IRotationConfigSet CreateConfiguration() => base.CreateConfiguration()
        .SetCombo(CombatType.PvE, "RotationSelection", 0, "Select which Rotation will be used. (Openers will only be followed at level 90)", "Early AA", "Delayed Tools");

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
                            return OpenerHelpers.OpenerController(IsLastGCD(false, HeatBlast) && HeatStacks == 4, HeatBlast.CanUse(out act, CanUseOption.MustUse));

                        case 17:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));

                        case 18:
                            return OpenerHelpers.OpenerController(IsLastGCD(false, HeatBlast) && HeatStacks == 3, HeatBlast.CanUse(out act, CanUseOption.MustUse));

                        case 19:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));

                        case 20:
                            return OpenerHelpers.OpenerController(IsLastGCD(false, HeatBlast) && HeatStacks == 2, HeatBlast.CanUse(out act, CanUseOption.MustUse));

                        case 21:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));

                        case 22:
                            return OpenerHelpers.OpenerController(IsLastGCD(false, HeatBlast) && HeatStacks == 1, HeatBlast.CanUse(out act, CanUseOption.MustUse));

                        case 23:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));

                        case 24:
                            return OpenerHelpers.OpenerController(IsLastGCD(false, HeatBlast) && HeatStacks == 0, HeatBlast.CanUse(out act, CanUseOption.MustUse));

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
                            return OpenerHelpers.OpenerController(IsLastGCD(false, HeatBlast) && HeatStacks == 4, HeatBlast.CanUse(out act, CanUseOption.MustUse));

                        case 17:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));

                        case 18:
                            return OpenerHelpers.OpenerController(IsLastGCD(false, HeatBlast) && HeatStacks == 3, HeatBlast.CanUse(out act, CanUseOption.MustUse));

                        case 19:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));

                        case 20:
                            return OpenerHelpers.OpenerController(IsLastGCD(false, HeatBlast) && HeatStacks == 2, HeatBlast.CanUse(out act, CanUseOption.MustUse));

                        case 21:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));

                        case 22:
                            return OpenerHelpers.OpenerController(IsLastGCD(false, HeatBlast) && HeatStacks == 1, HeatBlast.CanUse(out act, CanUseOption.MustUse));

                        case 23:
                            return OpenerHelpers.OpenerController(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));

                        case 24:
                            return OpenerHelpers.OpenerController(IsLastGCD(false, HeatBlast) && HeatStacks == 0, HeatBlast.CanUse(out act, CanUseOption.MustUse));

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

        // Opener for MCH
        if (MCHOpener.DoFullOpener(out act))
            return true;

        return base.GeneralGCD(out act);
    }
    #endregion GCD Logic

    #region oGCD Logic
    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
    {
        act = null;

        // Opener for MCH
        if (MCHOpener.DoFullOpener(out act))
            return true;

        return base.EmergencyAbility(nextGCD, out act);
    }
    #endregion oGCD Logic

    #region Job Helper Methods

    // This should be relevant to the Action shown in the [RotationDesc(ActionID.Action]
    private void BurstActionCheck()
    {
        BurstHelpers.InBurst = Player.HasStatus(true, StatusID.Wildfire);
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

    #endregion Job Helper Methods

    #region Miscellaneous Helper Methods

    // Updates Status of other extra helper methods on every frame draw.
    protected override void UpdateInfo()
    {
        if (Player.IsInCombat())
        {
            ToolKitCheck();
            BurstActionCheck();
        }
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

    #endregion Miscellaneous Helper Methods
}