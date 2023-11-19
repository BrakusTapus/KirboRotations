using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.IoC;
using Dalamud.Logging;
using Dalamud.Plugin.Services;
using KirboRotations.Utility;

namespace KirboRotations.Ranged;

[RotationDesc(ActionID.Wildfire)]
[LinkDescription("https://i.imgur.com/vekKW2k.jpg", "Delayed Tools")]
public class MCH_KirboComplete : MCH_Base
{
    public override string GameVersion => "6.51";

    public override string RotationName => "Kirbo's Machinist";

    public override string Description => "Kirbo's Machinist, revived and modified by Incognito, Do Delayed Tools and Early AA. \n\n Should be optimised for Boss Level 90 content with 2.5 GCD.";

    public override CombatType Type => CombatType.Both;

    private bool InBurst { get; set; }

    public override bool ShowStatus => true;

    public override void DisplayStatus()
    {
        bool inPvP = Methods.InPvP();

        ImGui.Separator();
        ImGui.Text("GCD remain: " + WeaponRemain);
        ImGui.Text("HeatStacks: " + HeatStacks);
        //ImGui.Text($"Player.HealthRatio: {Player.GetHealthRatio() * 100:F2}%%");
        ImGui.Text($"Player.HealthRatio: {Player.CurrentHp}");
        ImGui.Separator();
        ImGui.Spacing();
        if (!inPvP)
        {
            int rotationSelection = Configs.GetCombo("RotationSelection");
            string rotationText = "Unknown";

            switch (rotationSelection)
            {
                case 0:
                    rotationText = "Early AA";
                    break;
                case 1:
                    rotationText = "Delayed Tools";
                    break;
                case 2:
                    rotationText = "Early All";
                    break;
            }
            ImGui.Text($"Rotation Selection: {rotationText}");
            ImGui.Text("Openerstep: " + Openerstep);
            ImGui.Text("OpenerActionsAvailable: " + OpenerActionsAvailable);
            ImGui.Text("OpenerInProgress: " + OpenerInProgress);
            ImGui.Text("OpenerHasFailed: " + OpenerHasFailed);
            ImGui.Text("OpenerHasFinished: " + OpenerHasFinished);
            ImGui.Text("Flag: " + Flag);
        }
        else
        {
            ImGui.Text("IsPvPOverheated: " + IsPvPOverheated);
            ImGui.Text("PvP_HeatStacks: " + PvP_HeatStacks);
            ImGui.Text("PvP_Analysis CurrentCharges: " + PvP_Analysis.CurrentCharges);
        }
        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();

        // Calculate the remaining vertical space in the window
        float remainingSpace = ImGui.GetContentRegionAvail().Y - ImGui.GetFrameHeightWithSpacing(); // Subtracting button height with spacing
        if (remainingSpace > 0)
        {
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + remainingSpace);
        }

        // Add a button for resetting rotation properties
        if (ImGui.Button("Reset Rotation"))
        {
            ResetRotationProperties();
        }
    }
    private int Openerstep { get; set; }

    private bool OpenerHasFinished { get; set; }

    private bool OpenerHasFailed { get; set; }

    private bool OpenerActionsAvailable { get; set; }

    private bool OpenerInProgress { get; set; }

    private bool WillhaveTool { get; set; }

    private bool Flag { get; set; }

    private static byte HeatStacks
    {
        get
        {
            byte stacks = Player.StatusStack(true, StatusID.Overheated);
            return stacks == byte.MaxValue ? (byte)5 : stacks;
        }
    }

    private static byte PvP_HeatStacks
    {
        get
        {
            byte pvp_heatstacks = Player.StatusStack(true, StatusID.PvP_HeatStack);
            return pvp_heatstacks == byte.MaxValue ? (byte)5 : pvp_heatstacks;
        }
    }

    private bool IsPvPOverheated => Player.HasStatus(true, StatusID.PvP_Overheated);

    // Thank you Rabbs!
    // private static BaseAction PvP_MarksmansSpite { get; } = new(ActionID.PvP_MarksmansSpite)
    // {
    //
    //     ChoiceTarget = (Targets, mustUse) =>
    //     {
    //        Targets = Targets.Where(b => b.YalmDistanceX < 50 &&
    //       (b.CurrentHp + b.CurrentMp * 6) < 50000 &&
    //      b.HasStatus(false, (StatusID)3054)).ToArray();
    //
    //    if (Targets.Any())
    //  {
    //    return Targets.OrderBy(ObjectHelper.GetHealthRatio).Last();
    //   }
    //
    //          return null;
    //    },
    //  ActionCheck = (BattleChara b, bool m) => LimitBreakLevel >= 1
    // };

    protected override IRotationConfigSet CreateConfiguration() => base.CreateConfiguration()
        .SetCombo(CombatType.PvE, "RotationSelection", 1, "Select which Rotation will be used. (Openers will only be followed at level 90)", "Early AA", "Delayed Tools", "Early All")
        .SetBool(CombatType.PvE, "BatteryStuck", false, "Battery overcap protection\n(Will try and use Rook AutoTurret if Battery is at 100 and next skill increases Battery)")
        .SetBool(CombatType.PvE, "HeatStuck", false, "Heat overcap protection\n(Will try and use HyperCharge if Heat is at 100 and next skill increases Heat)")
        .SetBool(CombatType.PvE, "DumpSkills", false, "Dump Skills when Target is dying\n(Will try and spend remaining resources before boss dies)")
        //.SetBool(CombatType.PvP, "LBInPvP", true, "Use the LB in PvP when Target is killable by it")
        .SetInt(CombatType.PvP, "MarksmanRifleThreshold", 32000, "Marksman Rifle HP Threshold\n(Doule click or hold CTRL and click to set value)", 0, 75000)
        .SetBool(CombatType.PvP, "GuardCancel", true, "Turn on if you want to FORCE RS to use nothing while in guard in PvP")
        .SetBool(CombatType.PvP, "PreventActionWaste", true, "Turn on to prevent using actions on targets with invulns\n(For example: DRK with Undead Redemption)")
        .SetBool(CombatType.PvP, "SafetyCheck", true, "Turn on to prevent using actions on targets that have a dangerous status\n(For example a SAM with Chiten)");

    protected override IAction CountDownAction(float remainTime)
    {
        TerritoryContentType Content = TerritoryContentType;
        bool UltimateRaids = (int)Content == 28;

        if (OpenerActionsAvailable)
        {
            switch (Configs.GetCombo("RotationSelection")) // Select CountDownAction Depending on which Rotation will be used
            {

                case 0: // Early AA
                    if (remainTime <= AirAnchor.AnimationLockTime && Player.HasStatus(true, StatusID.Reassemble) && AirAnchor.CanUse(out _))
                    {
                        OpenerInProgress = true;
                        return AirAnchor;
                    }
                    IAction act0;
                    if (remainTime <= TinctureOfDexterity8.AnimationLockTime + AirAnchor.AnimationLockTime && UseBurstMedicine(out act0, false))
                    {
                        return act0;
                    }
                    if (remainTime <= 5f && Reassemble.CurrentCharges > 1 && !Player.HasStatus(true, StatusID.Reassemble))
                    {
                        return Reassemble;
                    }
                    break;

                case 1: // Delayed Tools
                    if (remainTime <= SplitShot.AnimationLockTime && SplitShot.CanUse(out _))
                    {
                        OpenerInProgress = true;
                        return SplitShot;
                    }
                    IAction act1;
                    if (remainTime <= SplitShot.AnimationLockTime + TinctureOfDexterity8.AnimationLockTime + 0.2 && UseBurstMedicine(out act1, false))
                    {
                        return act1;
                    }
                    break;

                case 2: // Early All
                    if (remainTime <= AirAnchor.AnimationLockTime && Player.HasStatus(true, StatusID.Reassemble) && AirAnchor.CanUse(out _))
                    {
                        OpenerInProgress = true;
                        return AirAnchor;
                    }
                    if (remainTime <= 5f && Reassemble.CurrentCharges > 1 && !Player.HasStatus(true, StatusID.Reassemble))
                    {
                        return Reassemble;
                    }
                    break;
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
            if (Player.Level != 70)
            {
                return base.CountDownAction(remainTime);
            }
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
        return base.CountDownAction(remainTime);
    }

    private bool Opener(out IAction act)
    {
        act = default(IAction);
        while (OpenerInProgress /* && (!OpenerHasFinished || !OpenerHasFailed)*/)
        {
            if (TimeSinceLastAction.TotalSeconds > 3.0 && !Flag)
            {
                OpenerHasFailed = true;
                OpenerInProgress = false;
                Openerstep = 0;
                Flag = true;
            }
            if (Player.IsDead && !Flag)
            {
                OpenerHasFailed = true;
                OpenerInProgress = false;
                Openerstep = 0;
                Flag = true;
            }
            switch (Configs.GetCombo("RotationSelection"))
            {
                case 0: // Early AA
                    switch (Openerstep)
                    {
                        case 0:
                            return OpenerStep(IsLastGCD(false, AirAnchor), AirAnchor.CanUse(out act, CanUseOption.MustUse));
                        case 1:
                            return OpenerStep(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 2:
                            return OpenerStep(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 3:
                            return OpenerStep(IsLastGCD(false, Drill), Drill.CanUse(out act, CanUseOption.MustUse));
                        case 4:
                            return OpenerStep(IsLastAbility(false, BarrelStabilizer), BarrelStabilizer.CanUse(out act, CanUseOption.MustUse));
                        case 5:
                            return OpenerStep(IsLastGCD(true, SplitShot), SplitShot.CanUse(out act, CanUseOption.MustUse));
                        case 6:
                            return OpenerStep(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 7:
                            return OpenerStep(IsLastGCD(true, SlugShot), SlugShot.CanUse(out act, CanUseOption.MustUse));
                        case 8:
                            return OpenerStep(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 9:
                            return OpenerStep(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 10:
                            return OpenerStep(IsLastGCD(true, CleanShot), CleanShot.CanUse(out act, CanUseOption.MustUse));
                        case 11:
                            return OpenerStep(IsLastAbility(false, Reassemble), Reassemble.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 12:
                            return OpenerStep(IsLastAbility(false, Wildfire), Wildfire.CanUse(out act, (CanUseOption)17));
                        case 13:
                            return OpenerStep(IsLastGCD(false, ChainSaw), ChainSaw.CanUse(out act, CanUseOption.MustUse));
                        case 14:
                            return OpenerStep(IsLastAbility(true, RookAutoturret), RookAutoturret.CanUse(out act, CanUseOption.MustUse));
                        case 15:
                            return OpenerStep(IsLastAbility(false, Hypercharge), Hypercharge.CanUse(out act, (CanUseOption)51));
                        case 16:
                            return OpenerStep(IsLastGCD(false, HeatBlast) && HeatStacks == 4, HeatBlast.CanUse(out act, CanUseOption.MustUse));
                        case 17:
                            return OpenerStep(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 18:
                            return OpenerStep(IsLastGCD(false, HeatBlast) && HeatStacks == 3, HeatBlast.CanUse(out act, CanUseOption.MustUse));
                        case 19:
                            return OpenerStep(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 20:
                            return OpenerStep(IsLastGCD(false, HeatBlast) && HeatStacks == 2, HeatBlast.CanUse(out act, CanUseOption.MustUse));
                        case 21:
                            return OpenerStep(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 22:
                            return OpenerStep(IsLastGCD(false, HeatBlast) && HeatStacks == 1, HeatBlast.CanUse(out act, CanUseOption.MustUse));
                        case 23:
                            return OpenerStep(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 24:
                            return OpenerStep(IsLastGCD(false, HeatBlast) && HeatStacks == 0, HeatBlast.CanUse(out act, CanUseOption.MustUse));
                        case 25:
                            return OpenerStep(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 26:
                            return OpenerStep(IsLastGCD(false, Drill), Drill.CanUse(out act, CanUseOption.MustUse));
                        case 27:
                            OpenerHasFinished = true;
                            OpenerInProgress = false;
                            // Finished Early AA
                            break;
                    }
                    break;
                case 1: // Delayed Tools
                    switch (Openerstep)
                    {
                        case 0:
                            return OpenerStep(IsLastGCD(true, SplitShot), SplitShot.CanUse(out act, CanUseOption.MustUse));
                        case 1:
                            return OpenerStep(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 2:
                            return OpenerStep(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 3:
                            //if (Drill.IsCoolingDown)
                            //{
                            //OpenerHasFailed = true;
                            //}
                            return OpenerStep(IsLastGCD(false, Drill), Drill.CanUse(out act, CanUseOption.MustUse));
                        case 4:
                            return OpenerStep(IsLastAbility(false, BarrelStabilizer), BarrelStabilizer.CanUse(out act, CanUseOption.MustUse));
                        case 5:
                            return OpenerStep(IsLastGCD(true, SlugShot), SlugShot.CanUse(out act, CanUseOption.MustUse));
                        case 6:
                            return OpenerStep(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 7:
                            return OpenerStep(IsLastGCD(true, CleanShot), CleanShot.CanUse(out act, CanUseOption.MustUse));
                        case 8:
                            return OpenerStep(IsLastAbility(false, Reassemble), Reassemble.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 9:
                            return OpenerStep(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 10:
                            return OpenerStep(IsLastGCD(false, AirAnchor), AirAnchor.CanUse(out act, CanUseOption.MustUse));
                        case 11:
                            return OpenerStep(IsLastAbility(false, Reassemble), Reassemble.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 12:
                            return OpenerStep(IsLastAbility(false, Wildfire), Wildfire.CanUse(out act, (CanUseOption)17));
                        case 13:
                            return OpenerStep(IsLastGCD(false, ChainSaw), ChainSaw.CanUse(out act, CanUseOption.MustUse));
                        case 14:
                            return OpenerStep(IsLastAbility(true, RookAutoturret), RookAutoturret.CanUse(out act, CanUseOption.MustUse));
                        case 15:
                            return OpenerStep(IsLastAbility(false, Hypercharge), Hypercharge.CanUse(out act, (CanUseOption)51));
                        case 16:
                            return OpenerStep(IsLastGCD(false, HeatBlast) && HeatStacks == 4, HeatBlast.CanUse(out act, CanUseOption.MustUse));
                        case 17:
                            return OpenerStep(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 18:
                            return OpenerStep(IsLastGCD(false, HeatBlast) && HeatStacks == 3, HeatBlast.CanUse(out act, CanUseOption.MustUse));
                        case 19:
                            return OpenerStep(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 20:
                            return OpenerStep(IsLastGCD(false, HeatBlast) && HeatStacks == 2, HeatBlast.CanUse(out act, CanUseOption.MustUse));
                        case 21:
                            return OpenerStep(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 22:
                            return OpenerStep(IsLastGCD(false, HeatBlast) && HeatStacks == 1, HeatBlast.CanUse(out act, CanUseOption.MustUse));
                        case 23:
                            return OpenerStep(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 24:
                            return OpenerStep(IsLastGCD(false, HeatBlast) && HeatStacks == 0, HeatBlast.CanUse(out act, CanUseOption.MustUse));
                        case 25:
                            return OpenerStep(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 26:
                            return OpenerStep(IsLastGCD(false, Drill), Drill.CanUse(out act, CanUseOption.MustUse));
                        case 27:
                            OpenerHasFinished = true;
                            OpenerInProgress = false;
                            // Finished Delayed Tools
                            break;
                    }
                    break;
                case 2: // Early All
                    switch (Openerstep)
                    {
                        case 0:
                            return OpenerStep(IsLastGCD(false, AirAnchor), AirAnchor.CanUse(out act, CanUseOption.MustUse));
                        case 1:
                            return OpenerStep(IsLastAbility(false, BarrelStabilizer), BarrelStabilizer.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 2:
                            return OpenerStep(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty | CanUseOption.OnLastAbility));
                        case 3:
                            return OpenerStep(IsLastGCD(false, Drill), Drill.CanUse(out act, CanUseOption.MustUse));
                        case 4:
                            return OpenerStep(IsLastAbility(false, Reassemble), Reassemble.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 5:
                            return OpenerStep(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty | CanUseOption.OnLastAbility));
                        case 6:
                            return OpenerStep(IsLastGCD(false, ChainSaw), ChainSaw.CanUse(out act, CanUseOption.MustUse));
                        case 7:
                            return OpenerStep(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 8:
                            return OpenerStep(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty | CanUseOption.OnLastAbility));
                        case 9:
                            return OpenerStep(IsLastGCD(true, SplitShot), SplitShot.CanUse(out act, CanUseOption.MustUse));
                        case 10:
                            return OpenerStep(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 11:
                            return OpenerStep(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty | CanUseOption.OnLastAbility));
                        case 12:
                            return OpenerStep(IsLastGCD(true, SlugShot), SlugShot.CanUse(out act, CanUseOption.MustUse));
                        case 13:
                            return OpenerStep(IsLastAbility(false, Tactician), Tactician.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 14:
                            return OpenerStep(IsLastAbility(false, Wildfire), Wildfire.CanUse(out act, CanUseOption.OnLastAbility));
                        case 15:
                            return OpenerStep(IsLastGCD(true, CleanShot), CleanShot.CanUse(out act, CanUseOption.MustUse));
                        case 16:
                            return OpenerStep(IsLastAbility(true, RookAutoturret), RookAutoturret.CanUse(out act, CanUseOption.MustUse));
                        case 17:
                            return OpenerStep(IsLastAbility(false, Hypercharge), Hypercharge.CanUse(out act, CanUseOption.OnLastAbility));
                        case 18:
                            OpenerHasFinished = true;
                            OpenerInProgress = false;
                            // Finished Early All
                            break;
                    }
                    break;

            }
        }
        act = null;
        return false;
    }

    private bool OpenerStep(bool condition, bool result)
    {
        if (condition)
        {
            Openerstep++;
            return false;
        }
        return result;
    }

    protected override bool GeneralGCD(out IAction act)
    {
        act = null;

        #region PvP
        // Status checks
        bool hasGuard = HostileTarget.HasStatus(false, StatusID.PvP_Guard);
        bool hasChiten = HostileTarget.HasStatus(false, StatusID.PvP_Chiten);
        bool hasHallowedGround = HostileTarget.HasStatus(false, StatusID.PvP_HallowedGround);
        bool hasUndeadRedemption = HostileTarget.HasStatus(false, StatusID.PvP_UndeadRedemption);
        bool TargetIsNotPlayer = Target != Player;

        // Config checks
        bool guardCancel = Configs.GetBool("GuardCancel");
        bool preventActionWaste = Configs.GetBool("PreventActionWaste");
        bool safetyCheck = Configs.GetBool("SafetyCheck");
        int marksmanRifleThreshold = Configs.GetInt("MarksmanRifleThreshold");

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

            if (!IsPvPOverheated)
            {
                //if (HostileTarget && Configs.GetBool("LBInPvP") && HostileTarget.CurrentHp < 32000 && PvP_MarksmansSpite.CanUse(out act, CanUseOption.MustUse))
                //{
                //return true;
                //}
                if (Target.CurrentHp <= marksmanRifleThreshold && TargetIsNotPlayer && !hasGuard && !hasHallowedGround && !hasUndeadRedemption
                    && PvP_MarksmansSpite.CanUse(out act, CanUseOption.MustUse))
                {
                    return true;
                }
                if (Player.HasStatus(true, StatusID.PvP_DrillPrimed))
                {
                    if (PvP_Drill.CanUse(out act, CanUseOption.MustUseEmpty)) return true;
                }
                else if (Player.HasStatus(true, StatusID.PvP_BioblasterPrimed) && HostileTarget && HostileTarget.DistanceToPlayer() < 12)
                {
                    if (PvP_Bioblaster.CanUse(out act, CanUseOption.MustUseEmpty)) return true;
                }
                else if (Player.HasStatus(true, StatusID.PvP_AirAnchorPrimed))
                {
                    if (PvP_AirAnchor.CanUse(out act, CanUseOption.MustUseEmpty)) return true;
                }
                else if (Player.HasStatus(true, StatusID.PvP_ChainSawPrimed))
                {
                    if (PvP_ChainSaw.CanUse(out act, CanUseOption.MustUseEmpty, 1)) return true;
                }
            }

            if (PvP_Scattergun.CanUse(out act, CanUseOption.MustUseEmpty, 1) && HostileTarget.DistanceToPlayer() < 12 && !IsPvPOverheated)
            {
                return true;
            }

            if (PvP_BlastCharge.CanUse(out act, CanUseOption.IgnoreCastCheck))
            {
                return true;
            }
        }
        #endregion

        #region PVE
        if (OpenerInProgress)
        {
            return Opener(out act);
        }
        if (!OpenerInProgress /*|| OpenerHasFailed || OpenerHasFinished*/)
        {
            if (AutoCrossbow.CanUse(out act, (CanUseOption)1, 2) && ObjectHelper.DistanceToPlayer(HostileTarget) <= 12f)
            {
                return true;
            }
            if (HeatBlast.CanUse(out act, CanUseOption.MustUse))
            {
                return true;
            }
            if (HostileTarget.GetHealthRatio() > 0.6 && IsLongerThan(15) && BioBlaster.CanUse(out act, (CanUseOption)1, 2) && ObjectHelper.DistanceToPlayer(HostileTarget) <= 12f)
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
                if (Drill.WillHaveOneCharge(0.1f))
                {
                    return false;
                }
                return true;
            }
            if (SlugShot.CanUse(out act, CanUseOption.MustUse))
            {
                if (Drill.WillHaveOneCharge(0.1f))
                {
                    return false;
                }
                return true;
            }
            if (SplitShot.CanUse(out act, CanUseOption.MustUse))
            {
                if (Drill.WillHaveOneCharge(0.1f))
                {
                    return false;
                }
                return true;
            }
        }
        return base.GeneralGCD(out act);
        #endregion
    }

    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
    {
        act = null;

        #region PvP
        // Status checks
        bool hasGuard = HostileTarget.HasStatus(false, StatusID.PvP_Guard);
        bool hasChiten = HostileTarget.HasStatus(false, StatusID.PvP_Chiten);
        bool hasHallowedGround = HostileTarget.HasStatus(false, StatusID.PvP_HallowedGround);
        bool hasUndeadRedemption = HostileTarget.HasStatus(false, StatusID.PvP_UndeadRedemption);

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

            if (Configs.GetBool("SafetyCheck") && hasChiten)
            {
                return false;
            }

            if (Configs.GetBool("PreventActionWaste") && (hasGuard || hasHallowedGround || hasUndeadRedemption))
            {
                return false;
            }

            if (IsPvPOverheated && !Player.WillStatusEnd(3.5f, true, StatusID.PvP_Overheated) && HostileTarget.DistanceToPlayer() < 20 && PvP_Wildfire.CanUse(out act, CanUseOption.MustUse))
            {
                return true;
            }

            if (!Player.HasStatus(true, StatusID.PvP_Analysis) && PvP_Analysis.CurrentCharges >= 1)
            {
                if (Player.HasStatus(true, StatusID.PvP_DrillPrimed) && NumberOfHostilesInRange > 0)
                {
                    return true;
                }

                if (nextGCD == PvP_Drill || nextGCD == PvP_ChainSaw)
                {
                    return PvP_Analysis.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (Player.HasStatus(true, StatusID.PvP_DrillPrimed) && IsPvPOverheated)
                {
                    return true;
                }

                if (Player.HasStatus(true, StatusID.PvP_ChainSawPrimed) && IsPvPOverheated)
                {
                    return true;
                }
            }

            if (PvP_BishopAutoTurret.CanUse(out act, CanUseOption.MustUse) && HasHostilesInRange)
            {
                return true;
            }
        }
        #endregion

        #region PVE
        //TerritoryContentType Content = TerritoryContentType;
        //bool Dungeon = (int)Content == 2;
        //bool Roulette = (int)Content == 1;
        //bool Deepdungeon = (int)Content == 21;
        //bool VCDungeonFinder = (int)Content == 30;
        //bool FATEs = (int)Content == 8;
        //bool Eureka = (int)Content == 26;
        //bool UltimateRaids = (int)Content == 28;

        if (ShouldUseBurstMedicine(out act))
        {
            return true;
        }
        if (OpenerInProgress /*&& !OpenerHasFailed && !OpenerHasFinished*/)
        {
            return Opener(out act);
        }
        if (Configs.GetBool("BatteryStuck") && /*!OpenerInProgress &&*/ Battery == 100 && RookAutoturret.CanUse(out act, CanUseOption.MustUseEmpty) && (nextGCD == ChainSaw || nextGCD == AirAnchor || nextGCD == CleanShot))
        {
            return true;
        }
        if (Configs.GetBool("HeatStuck") && /*!OpenerInProgress &&*/ Heat == 100 && Hypercharge.CanUse(out act, CanUseOption.MustUseEmpty) && (nextGCD == SplitShot || nextGCD == SlugShot || nextGCD == CleanShot))
        {
            return true;
        }
        if (Configs.GetBool("DumpSkills") && HostileTarget.IsDying() && HostileTarget.IsBossFromIcon())
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
            if (HostileTarget.GetHealthRatio() < 0.02 && ((Player.HasStatus(true, StatusID.Wildfire)) || InBurst) && Wildfire.ElapsedAfter(5f) && Detonator.CanUse(out act))
            {
                return true;
            }
        }

        // LvL 90+
        if (/*(*/!OpenerInProgress /*|| OpenerHasFailed || OpenerHasFinished) && Player.Level >= 90*/)
        {
            if (Wildfire.CanUse(out act, (CanUseOption)16) && nextGCD == ChainSaw && Heat >= 50)
            {
                return true;
            }
            if (BarrelStabilizer.CanUse(out act, CanUseOption.MustUseEmpty))
            {
                if (Wildfire.IsCoolingDown && IsLastGCD((ActionID)16498))
                {
                    return true;
                }
                return true;
            }
            if (Reassemble.CanUse(out act, CanUseOption.MustUseEmpty) && !Player.HasStatus(true, StatusID.Reassemble))
            {
                if (IActionHelper.IsTheSameTo(nextGCD, true, ChainSaw))
                {
                    return true;
                }
                if ((IActionHelper.IsTheSameTo(nextGCD, true, AirAnchor) || IActionHelper.IsTheSameTo(nextGCD, true, Drill)) && !Wildfire.WillHaveOneCharge(55f))
                {
                    return true;
                }
            }
            if (RookAutoturret.CanUse(out act, (CanUseOption)16) && HostileTarget && HostileTarget.IsTargetable && InCombat)
            {
                if (CombatElapsedLess(60f) && !CombatElapsedLess(45f) && Battery >= 50)
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
            if (Hypercharge.CanUse(out act) && !WillhaveTool)
            {
                if (InBurst && IsLastGCD((ActionID)25788))
                {
                    return true;
                }
                if (Heat >= 100 && Wildfire.WillHaveOneCharge(10f))
                {
                    return true;
                }
                if (Heat >= 90 && Wildfire.WillHaveOneCharge(40f))
                {
                    return true;
                }
                if (Heat >= 50 && !Wildfire.WillHaveOneCharge(40f))
                {
                    return true;
                }
            }

            if (ShouldUseGaussroundOrRicochet(out act) && NextAbilityToNextGCD > GaussRound.AnimationLockTime + Ping)
            {
                return true;
            }
        }

        // LvL 30-89 and Casual Content
        if (/*Deepdungeon || Eureka || Roulette || Dungeon || VCDungeonFinder || FATEs || */Player.Level < 90)
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
        #endregion
    }

    #region PvE Methods
    private bool ShouldUseBurstMedicine(out IAction act)
    {
        act = null; // Default to null if Burst Medicine cannot be used.

        if (Player.HasStatus(true, StatusID.Weakness))
        {
            return false;
        }

        // Check if the conditions for using Burst Medicine are met.
        if (Wildfire.WillHaveOneCharge(20) && CombatTime > 60 && NextAbilityToNextGCD > 1.2 && !Player.HasStatus(true, StatusID.Weakness)
            && !TinctureOfDexterity6.IsCoolingDown && !TinctureOfDexterity7.IsCoolingDown && !TinctureOfDexterity8.IsCoolingDown && Drill.WillHaveOneCharge(3))
        {
            // Attempt to use Burst Medicine.
            return UseBurstMedicine(out act, false);
        }

        // If the conditions are not met, return false.
        return false;
    }

    private bool ShouldUseReassemble(IAction nextGCD, out IAction act)
    {
        act = null; // Default to null if Reassemble cannot be used.

        // Common checks before considering rotation variants
        bool hasReassemble = Player.HasStatus(true, StatusID.Reassemble);
        bool isPlayerLevelTooLowForDrill = !Drill.EnoughLevel;
        bool isNextGCDEligibleForDefault =
            nextGCD == Drill ||
            nextGCD == AirAnchor ||
            nextGCD == ChainSaw ||
            (isPlayerLevelTooLowForDrill && nextGCD == CleanShot);

        // If the player already has Reassemble, return false.
        if (hasReassemble)
        {
            return false;
        }

        // If none of the conditions are met for any rotation variant, return false.
        return false;
    }
    private bool ShouldUseHypercharge(out IAction act)
    {
        act = null; // Default to null if Hypercharge cannot be used.

        // Check if currently overheated, which would make using Hypercharge unnecessary or impossible.
        if (IsOverheated)
        {
            return false;
        }

        // Check if the target has the Wildfire status.
        bool hasWildfire = HostileTarget.HasStatus(true, StatusID.Wildfire) || Player.HasStatus(true, StatusID.Wildfire);

        // Check if the Wildfire cooldown is greater than 30 seconds.
        bool isWildfireCooldownLong = !Wildfire.WillHaveOneCharge(60);

        // Check if the Wildfire cooldown is less than 30 seconds.
        bool isWildfireCooldownShort = Wildfire.WillHaveOneCharge(60);

        // Check if the Heat gauge is at least 50.
        bool isHeatAtLeast50 = Heat >= 50;

        // Check if the Heat gauge is 95 or more.
        bool isHeatFullAlmostFull = Heat >= 100;

        // Check the cooldowns of your main abilities to see if they will be ready soon.
        bool isAnyMainAbilityReadySoon = Drill.WillHaveOneCharge(7.5f) ||
                                         AirAnchor.WillHaveOneCharge(7.5f) ||
                                         ChainSaw.WillHaveOneCharge(7.5f);

        // Check if the last ability used was Wildfire.
        bool isLastAbilityWildfire = IsLastAbility(ActionID.Wildfire);

        // Determine if Hypercharge should be used based on the presence of Wildfire status,
        // the Wildfire's cooldown, the current heat, not being overheated, and all main abilities not being ready soon,
        // with an exception if the last ability used was Wildfire.
        bool shouldUseHypercharge = hasWildfire ||
                                    ((!isAnyMainAbilityReadySoon || isLastAbilityWildfire) &&
                                     ((isWildfireCooldownLong && isHeatAtLeast50) ||
                                      (isWildfireCooldownShort && isHeatFullAlmostFull)));

        // If the conditions are met, attempt to use Hypercharge.
        if (shouldUseHypercharge)
        {
            return Hypercharge.CanUse(out act, CanUseOption.MustUse);
        }

        // If the conditions are not met, return false.
        return false;
    }
    private bool ShouldUseWildfire(out IAction act)
    {
        act = null; // Default to null if Wildfire cannot be used.

        // Check if the target is a boss. If not, return false immediately.
        if (!HostileTarget.IsBossFromTTK() && !HostileTarget.IsDummy())
        {
            return false;
        }


        // Check the cooldowns of your main abilities.
        bool isDrillReadySoon = Drill.WillHaveOneCharge(7.5f);
        bool isAirAnchorReadySoon = AirAnchor.WillHaveOneCharge(7.5f);
        bool isChainSawReadySoon = ChainSaw.WillHaveOneCharge(7.5f);

        // Check if the combat time is less than 15 seconds and the last action was AirAnchor.
        bool isEarlyCombatAndLastActionAirAnchor = CombatTime < 15 && IsLastGCD(ActionID.AirAnchor);

        // Determine if Wildfire should be used based on the conditions provided.
        bool shouldUseWildfire = !isDrillReadySoon && !isAirAnchorReadySoon && !isChainSawReadySoon ||
                                 isEarlyCombatAndLastActionAirAnchor;

        // If the conditions are met, attempt to use Wildfire.
        if (shouldUseWildfire)
        {
            return Wildfire.CanUse(out act, CanUseOption.OnLastAbility);
        }

        // If the conditions are not met, return false.
        return false;
    }
    private bool ShouldUseWildfire(IAction nextGCD, out IAction act)
    {
        if (Wildfire.CanUse(out act, CanUseOption.OnLastAbility))
        {
            if (ChainSaw.EnoughLevel && nextGCD == ChainSaw && Heat >= 50)
            {
                return true;
            }

            if (Drill.IsCoolingDown && AirAnchor.IsCoolingDown && ChainSaw.IsCoolingDown && Heat >= 45)
            {
                return true;
            }

            if (!CombatElapsedLessGCD(2) && Heat >= 50)
            {
                return true;
            }

            if (IsOverheated && HeatStacks > 4)
            {
                return true;
            }

            return false;
        }

        return false;
    }
    private bool ShouldUseBarrelStabilizer(out IAction act)
    {
        act = null; // Default to null if Barrel Stabilizer cannot be used.

        // Check if the target is not a boss or a dummy.
        if (!HostileTarget.IsBossFromTTK() || !HostileTarget.IsDummy())
        {
            return BarrelStabilizer.CanUse(out act, CanUseOption.MustUse);
        }

        // Check if the combat time is less than 10 seconds and the last action was Drill.
        bool isEarlyCombatAndLastActionDrill = CombatTime < 10 && IsLastAction(ActionID.Drill);

        // Check the relative cooldowns of Wildfire and Barrel Stabilizer.
        bool isWildfireCooldownShorter = Wildfire.WillHaveOneCharge(30) && Heat >= 50;
        bool isWildfireCooldownLonger = !Wildfire.WillHaveOneCharge(30);

        // Determine if Barrel Stabilizer should be used based on the conditions provided.
        bool shouldUseBarrelStabilizer = isEarlyCombatAndLastActionDrill ||
                                         isWildfireCooldownShorter ||
                                         isWildfireCooldownLonger;

        // If the conditions are met, attempt to use Barrel Stabilizer.
        if (shouldUseBarrelStabilizer)
        {
            return BarrelStabilizer.CanUse(out act, CanUseOption.MustUse);
        }

        // If the conditions are not met, return false.
        return false;
    }
    private bool ShouldUseRookAutoturret(IAction nextGCD, out IAction act)
    {
        act = null; // Default to null if Rook Autoturret cannot be used.

        // Logic when the target is a boss.
        if (HostileTarget.IsBossFromTTK() || HostileTarget.IsDummy())
        {
            // If combat time is less than 80 seconds and last summon battery power was at least 50.
            if (CombatTime < 80 && Battery >= 50 && !RookAutoturret.IsCoolingDown)
            {
                return RookAutoturret.CanUse(out act, CanUseOption.MustUse);
            }
            // If combat time is more than 80 seconds and additional conditions are met, use Rook Autoturret.
            else if (CombatTime >= 80)
            {
                bool hasWildfireStatus = HostileTarget.HasStatus(true, StatusID.Wildfire);
                bool isWildfireCooldownLong = !Wildfire.WillHaveOneCharge(30);
                bool isBatteryHighEnough = Battery >= 80;
                bool isAirAnchorOrChainSawSoon = AirAnchor.WillHaveOneCharge(2.5f) || ChainSaw.WillHaveOneCharge(2.5f);
                bool isNextGCDCleanShot = nextGCD == CleanShot;

                if ((isWildfireCooldownLong && isBatteryHighEnough) ||
                    (hasWildfireStatus) ||
                    (!hasWildfireStatus && Wildfire.WillHaveOneCharge(30) && (isBatteryHighEnough && isAirAnchorOrChainSawSoon)) ||
                    (isBatteryHighEnough && (isAirAnchorOrChainSawSoon || isNextGCDCleanShot)))
                {
                    return RookAutoturret.CanUse(out act, CanUseOption.MustUse);
                }
            }
        }
        else // Logic when the target is not a boss.
        {
            // If the target's time to kill is 17 seconds or more and battery is full.
            bool isAirAnchorOrChainSawSoon = AirAnchor.WillHaveOneCharge(2.5f) || ChainSaw.WillHaveOneCharge(2.5f);
            if (HostileTarget.GetTimeToKill(false) >= 17 && Battery == 100)
            {
                // If the next GCD is Clean Shot or if Air Anchor or Chain Saw are about to be ready.
                if (nextGCD == CleanShot || isAirAnchorOrChainSawSoon)
                {
                    return RookAutoturret.CanUse(out act, CanUseOption.MustUse);
                }
            }
            // If the target's time to kill is 17 seconds or more, use Rook Autoturret.
            else if (HostileTarget.GetTimeToKill(false) >= 17)
            {
                return RookAutoturret.CanUse(out act, CanUseOption.MustUse);
            }
        }

        // If none of the conditions are met, return false.
        return false;
    }
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
    #endregion

    //  protected override bool AttackAbility(out IAction act)
    //	{
    //		#region PVE
    //		if(OpenerInProgress)
    //		{
    //			return Opener(out act);
    //		}
    //		if(GaussRound.CurrentCharges >= Ricochet.CurrentCharges)
    //		{
    //			if(GaussRound.CanUse(out act, CanUseOption.MustUseEmpty))
    //			{
    //				return true;
    //			}
    //		}
    //		else if(Ricochet.CanUse(out act, (CanUseOption)19))
    //		{
    //			return true;
    //		}
    //		act = null;
    //		return false;
    //		#endregion
    //	}

    protected override void UpdateInfo()
    {
        HandleOpenerAvailability();
        ToolKitCheck();
        StateOfOpener();
    }

    private void ToolKitCheck()
    {
        bool WillHaveDrill = Drill.WillHaveOneCharge(5f);
        bool WillHaveAirAnchor = AirAnchor.WillHaveOneCharge(5f);
        bool WillHaveChainSaw = ChainSaw.WillHaveOneCharge(5f);
        if (Player.Level >= 90)
        {
            WillhaveTool = WillHaveDrill || WillHaveAirAnchor || WillHaveChainSaw;
        }

        InBurst = Player.HasStatus(true, StatusID.Wildfire);
    }

    public void StateOfOpener()
    {
        if (Player.IsDead)
        {
            OpenerHasFailed = false;
            OpenerHasFinished = false;
            Openerstep = 0;
        }
        if (!InCombat)
        {
            OpenerHasFailed = false;
            OpenerHasFinished = false;
            Openerstep = 0;
        }
        if (OpenerHasFailed)
        {
            OpenerInProgress = false;
        }
        if (OpenerHasFinished)
        {
            OpenerInProgress = false;
        }
    }

    // Used by Reset button to in Displaystatus
    private void ResetRotationProperties()
    {
        Openerstep = 0;
        OpenerHasFinished = false;
        OpenerHasFailed = false;
        OpenerActionsAvailable = false;
        OpenerInProgress = false;
        Serilog.Log.Warning($"Openerstep = {Openerstep}");
        Serilog.Log.Warning($"OpenerHasFinished = {OpenerHasFinished}");
        Serilog.Log.Warning($"OpenerHasFailed = {OpenerHasFailed}");
        Serilog.Log.Warning($"OpenerActionsAvailable = {OpenerActionsAvailable}");
        Serilog.Log.Warning($"OpenerInProgress = {OpenerInProgress}");
    }

    public void HandleOpenerAvailability()
    {
        bool Lvl90 = Player.Level >= 90;
        bool HasChainSaw = !ChainSaw.IsCoolingDown;
        bool HasAirAnchor = !AirAnchor.IsCoolingDown;
        bool HasDrill = !Drill.IsCoolingDown;
        bool HasBarrelStabilizer = !BarrelStabilizer.IsCoolingDown;
        bool HasRicochet = Ricochet.CurrentCharges == 3;
        bool HasWildfire = !Wildfire.IsCoolingDown;
        bool HasGaussRound = GaussRound.CurrentCharges == 3;
        bool ReassembleOneCharge = Reassemble.CurrentCharges >= 1;
        bool NoHeat = Heat == 0;
        bool NoBattery = Battery == 0;
        bool Openerstep0 = Openerstep == 0;
        OpenerActionsAvailable = ReassembleOneCharge && HasChainSaw && HasAirAnchor && HasDrill && HasBarrelStabilizer && HasRicochet && HasWildfire && HasGaussRound && Lvl90 && NoBattery && NoHeat && Openerstep0;
    }

}