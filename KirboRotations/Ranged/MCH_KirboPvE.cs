using System.Collections;
using Dalamud.Interface.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using KirboRotations.Utility.Core;
using KirboRotations.Utility.GameAssists;
using KirboRotations.Utility.Rotations;
using KirboRotations.Utility.ImGuiEx;
using KirboRotations.Utility.Data.Actions;

namespace KirboRotations.Ranged;

[RotationDesc(ActionID.Wildfire)]
[LinkDescription("https://i.imgur.com/vekKW2k.jpg", "Delayed Tools")]
public class MCH_KirboPvE : MCH_Base
{
    #region Rotation Info
    public override CombatType Type => CombatType.PvE;
    public override string GameVersion => "6.51";
    public override string RotationName => "Kirbo's Machinist (PvE)";
    public override string Description => "Kirbo's Machinist, revived and modified by Incognito, Do Delayed Tools and Early AA. \n\n Should be optimised for Boss Level 90 content with 2.5 GCD.";
    public string RotationVersion => "1.0.0.12";
    #endregion

    #region New PvE IBaseActions
    private static new IBaseAction Dismantle { get; } = new BaseAction(ActionID.Dismantle, ActionOption.None | ActionOption.Defense);
    private static new IBaseAction Drill { get; } = new BaseAction(ActionID.Drill)
    {
        ActionCheck = (b, m) => !IsOverheated,
    };
    private static new IBaseAction AirAnchor { get; } = new BaseAction(ActionID.AirAnchor)
    {
        ActionCheck = (b, m) => !IsOverheated,
    };
    private static new IBaseAction ChainSaw { get; } = new BaseAction(ActionID.ChainSaw)
    {
        ActionCheck = (b, m) => !IsOverheated,
    };
    private static new IBaseAction Wildfire { get; } = new BaseAction(ActionID.Wildfire)
    {
        ActionCheck = (BattleChara b, bool m) => ((CustomRotation.Player.HasStatus(true, StatusID.Overheated) && HeatStacks > 4) || Heat >= 45) && CustomRotation.InCombat
    };
    private static new IBaseAction Reassemble { get; } = new BaseAction(ActionID.Reassemble)
    {
        StatusProvide = new StatusID[1] { StatusID.Reassemble },
        ActionCheck = (BattleChara b, bool m) => !CustomRotation.Player.HasStatus(true, StatusID.Reassemble) && CustomRotation.HasHostilesInRange
    };
    private static new IBaseAction Hypercharge { get; } = new BaseAction(ActionID.Hypercharge, ActionOption.UseResources)
    {
        StatusProvide = new StatusID[1] { StatusID.Overheated },
        ActionCheck = (BattleChara b, bool m) => !IsOverheated && Heat >= 50 && CustomRotation.IsLongerThan(10f)
    };
    private static new IBaseAction BarrelStabilizer { get; } = new BaseAction(ActionID.BarrelStabilizer)
    {
        ActionCheck = (BattleChara b, bool m) => Heat <= 45 && CustomRotation.InCombat && CustomRotation.Target.IsTargetable && CustomRotation.Target != Player
    };
    #endregion

    #region Debug window stuff
    // Displays our 'Debug' in the status tab
    //ImGui.Text($"CustomRotation Player: {CustomRotation.Player.Name}");
    //ImGui.Text($"UserRotations LocalPlayer: {UserRotations.LocalPlayer.Name}");
    //ImGui.Text($"PlayerData Character: {PlayerData.Character->Name}");

    public override bool ShowStatus => true;

    public override void DisplayStatus()
    {
        try
        {
            try
            {
                /*if (actionMethods != null)
                {
                    ImGui.Text("GetSpecificActionRecastTime: " + actionMethods.GetSpecificActionRecastTime(ActionID.Peloton));
                    ImGui.Text("GetSpecificActionRecastTime: " + actionMethods.GetSpecificActionRecastTime(ActionID.Peloton));
                    ImGui.Text("GetSpecificActionRecastTime: " + actionMethods.GetSpecificActionRecastTime(ActionID.Peloton));
                }*/

                ImGuiEx.TripleSpacing();
                ImGuiEx.CollapsingHeaderWithContent("General Info", () =>
                {
                    ImGui.Text($"Rotation: {RotationName} - v{RotationVersion}");
                    ImGuiEx.ImGuiColoredText("Rotation  Job: ", ClassJob.Abbreviation, EColor.LightBlue);
                    ImGuiEx.SeperatorWithSpacing();
                    ImGui.Text($"Player Name: {Player.Name}");
                    ImGui.Text($"Player HP: {Player.GetHealthRatio() * 100:F2}%%");
                    ImGuiEx.ImGuiColoredText("Player MP: ", (int)Player.CurrentMp, EColor.Blue);
                    ImGuiEx.SeperatorWithSpacing();
                    ImGui.Text("In Combat: " + InCombat);
                    // ... other general info ...
                });
                ImGuiEx.Tooltip("Displays General information like:\n-Rotation Name\n-Player's Health\n-InCombat Status");
            }
            catch { Serilog.Log.Error($"Error Caught at: {RotationName} between line   65 - 80"); }

            ImGuiEx.TripleSpacing();

            try
            {
                ImGuiEx.CollapsingHeaderWithContent("Rotation Status", () =>
                {
                    string rotationText = GetRotationText(Configs.GetCombo("RotationSelection"));
                    ImGui.Text($"Rotation Selection: {rotationText}");
                    ImGui.Text("Openerstep: " + Methods.OpenerStep);
                    ImGui.Text("OpenerActionsAvailable: " + Methods.OpenerActionsAvailable);
                    ImGui.Text("OpenerInProgress: " + Methods.OpenerInProgress);
                    ImGui.Text("OpenerHasFailed: " + Methods.OpenerHasFailed);
                    ImGui.Text("OpenerHasFinished: " + Methods.OpenerHasFinished);
                    // ... other rotation status ...
                });
                ImGuiEx.Tooltip("Displays Rotation information like:\n-Selected Rotation\n-Opener Status");
            }
            catch { Serilog.Log.Error($"Error Caught at: {RotationName} between line   89 - 100"); }

            ImGuiEx.TripleSpacing();

            try
            {
                ImGuiEx.CollapsingHeaderWithContent("Burst Status", () =>
                {
                    string rotationText = GetRotationText(Configs.GetCombo("RotationSelection"));
                    ImGui.Text($"Rotation Selection: {rotationText}");
                    ImGui.Text("BurstStep: " + Methods.BurstStep);
                    ImGui.Text("BurstActionsAvailable: " + Methods.BurstActionsAvailable);
                    ImGui.Text("BurstInProgress: " + Methods.BurstInProgress);
                    ImGui.Text("BurstHasFailed: " + Methods.BurstHasFailed);
                    ImGui.Text("BurstHasFinished: " + Methods.BurstHasFinished);
                    // ... other Burst status ...
                });
                ImGuiEx.Tooltip("Displays Burst information like:\n-Burst Available\n-Burst HasFailed");
            }
            catch { Serilog.Log.Error($"Error Caught at: {RotationName} between line   89 - 100"); }

            ImGuiEx.TripleSpacing();

            try
            {
                ImGuiEx.CollapsingHeaderWithContent("Action Details", () =>
                {
                    if (ImGui.BeginTable("gcdTable", 2))
                    {
                        ImGui.TableSetupColumn("Description"); ImGui.TableSetupColumn("Value"); ImGui.TableHeadersRow();
                        ImGui.TableNextRow();
                        ImGui.TableNextColumn(); ImGui.Text("GCD Remain:"); ImGui.TableNextColumn(); ImGui.Text(WeaponRemain.ToString());
                        ImGui.TableNextRow();

                        ImGui.TableNextColumn(); ImGui.Text($"LastAction: {DataBase.LastAction}  {(uint)DataBase.LastAction}"); ImGui.TableNextColumn(); ImGui.Text(WeaponRemain.ToString());
                        //ImGui.TableNextRow();
                        //ImGui.TableNextColumn(); ImGui.Text($"Remain: "); ImGui.TableNextColumn(); ImGui.Text(WeaponRemain.ToString());
                        //ImGui.TableNextRow();
                        //ImGui.TableNextColumn(); ImGui.Text($"Remain: "); ImGui.TableNextColumn(); ImGui.Text(WeaponRemain.ToString());

                        // Add more rows as needed...

                        ImGui.EndTable();
                    }
                });
                ImGuiEx.Tooltip("Displays action information like:\n-LastAction Used\n-LastGCD Used\n-LastAbility Used");
            }
            catch { Serilog.Log.Error($"Error Caught at: {RotationName} between line   109 - 128"); }

            float remainingSpace = ImGuiEx.CalculateRemainingVerticalSpace();
            ImGui.Text($"Remaining Vertical Space: {remainingSpace} pixels");

            // Calculate the remaining vertical space in the window
            // Subtracting button height with spacing
            float remainingSpace2 = ImGui.GetContentRegionAvail().Y - ImGui.GetFrameHeightWithSpacing();
            if (remainingSpace2 > 0)
            { ImGui.SetCursorPosY(ImGui.GetCursorPosY() + remainingSpace2); }
            ImGuiEx.DisplayResetButton("Reset Properties");

        }
        catch { Serilog.Log.Warning("Something wrong with DisplayStatus"); }
    }

    private string GetRotationText(int rotationSelection)
    {
        return rotationSelection switch
        {
            0 => "Early AA",
            1 => "Delayed Tools",
            2 => "Early All",
            _ => "Unknown",
        };
    }
    #endregion

    /*#region Opener Related Properties
    // Displays the current opener step during the Opener
    private int Openerstep { get; set; }
    // Indicates wether or not the opener was finished succesfully
    private bool OpenerHasFinished { get; set; }
    // Indicates wether or not the opener has failed
    private bool OpenerHasFailed { get; set; }
    // Indicates wether or not the actions needed for the opener are available
    private bool OpenerActionsAvailable { get; set; }
    // Indicates wether or not the opener is currently in progress
    private bool OpenerInProgress { get; set; }
    // I have no clue what this did
    private bool Flag { get; set; }
    #endregion*/

    #region Action Related Properties
    // Check at every frame if 1 of our major tools will come off cooldown soon
    private bool WillhaveTool { get; set; }
    // Sets InBurst to true if player has the wildfire Buff
    //private bool InBurst { get; set; }
    // Holds the remaining amount of Heat stacks
    private static byte HeatStacks
    {
        get
        {
            byte stacks = Player.StatusStack(true, StatusID.Overheated);
            return stacks == byte.MaxValue ? (byte)5 : stacks;
        }
    }
    #endregion

    #region Rotation Config
    protected override IRotationConfigSet CreateConfiguration() => base.CreateConfiguration()
        .SetCombo(CombatType.PvE, "RotationSelection", 0, "Select which Rotation will be used. (Openers will only be followed at level 90)", "Early AA", "Delayed Tools"/*, "Early All"*/)
        .SetBool(CombatType.PvE, "BatteryStuck", false, "Battery overcap protection\n(Will try and use Rook AutoTurret if Battery is at 100 and next skill increases Battery)")
        .SetBool(CombatType.PvE, "HeatStuck", false, "Heat overcap protection\n(Will try and use HyperCharge if Heat is at 100 and next skill increases Heat)")
        .SetBool(CombatType.PvE, "DumpSkills", false, "Dump Skills when Target is dying\n(Will try and spend remaining resources before boss dies)");
    #endregion

    #region Countdown Logic
    protected override IAction CountDownAction(float remainTime)
    {
        TerritoryContentType Content = TerritoryContentType;
        bool UltimateRaids = (int)Content == 28;
        bool UwUorUCoB = UltimateRaids && Player.Level == 70;
        bool TEA = UltimateRaids && Player.Level == 80; 

        // If 'OpenerActionsAvailable' is true (see method 'HandleOpenerAvailability' for conditions) proceed to using Action logic during countdown
        if (Methods.OpenerActionsAvailable)
        {
            // Selects action logic depending on which rotation has been selected (Default: Delayed Tool)
            switch (Configs.GetCombo("RotationSelection"))
            {
                case 0: // Early AA
                    // Use Drill when the remaining countdown time is less or equal to Drill's AnimationLock, also sets OpenerInProgress to 'True'
                    if (remainTime <= Drill.AnimationLockTime && Drill.CanUse(out _))
                    {
                        Methods.OpenerInProgress = true;
                        return Drill;
                    }
                    // Use Tincture if Tincture use is enabled and the countdown time is less or equal to SplitShot+Tincture animationlock (1.8s)
                    IAction act0;
                    if (remainTime <= Drill.AnimationLockTime + TinctureOfDexterity8.AnimationLockTime && UseBurstMedicine(out act0, false))
                    {
                        return act0;
                    }
                    // Use Reassemble 
                    IAction act2;
                    if (remainTime <= 5f && !Player.HasStatus(true, StatusID.Reassemble) && Reassemble.CanUse(out act2, CanUseOption.MustUseEmpty))
                    {
                        return Reassemble;
                    }
                    break;

                case 1: // Delayed Tools
                    // Use SplitShot when the remaining countdown time is less or equal to SplitShot's AnimationLock, also sets OpenerInProgress to 'True'
                    if (remainTime <= SplitShot.AnimationLockTime && SplitShot.CanUse(out _))
                    {
                        Methods.OpenerInProgress = true;
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
    #endregion

    #region Opener Logic
    private bool Opener(out IAction act)
    {
        act = default(IAction);
        while (Methods.OpenerInProgress)
        {
            if (TimeSinceLastAction.TotalSeconds > 3.0 && !Methods._openerFlag)
            {
                Methods.OpenerHasFailed = true;
                Methods.OpenerInProgress = false;
                Methods.OpenerStep = 0;
                Methods._openerFlag = true;
            }
            if (Player.IsDead && !Methods._openerFlag)
            {
                Methods.OpenerHasFailed = true;
                Methods.OpenerInProgress = false;
                Methods.OpenerStep = 0;
                Methods._openerFlag = true;
            }
            switch (Configs.GetCombo("RotationSelection"))
            {
                case 0: // Early AA
                    switch (Methods.OpenerStep)
                    {
                        case 0:
                            return Methods.OpenerController(IsLastGCD(false, AirAnchor), AirAnchor.CanUse(out act, CanUseOption.MustUse));
                        case 1:
                            return Methods.OpenerController(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 2:
                            return Methods.OpenerController(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 3:
                            return Methods.OpenerController(IsLastGCD(false, Drill), Drill.CanUse(out act, CanUseOption.MustUse));
                        case 4:
                            return Methods.OpenerController(IsLastAbility(false, BarrelStabilizer), BarrelStabilizer.CanUse(out act, CanUseOption.MustUse));
                        case 5:
                            return Methods.OpenerController(IsLastGCD(true, SplitShot), SplitShot.CanUse(out act, CanUseOption.MustUse));
                        case 6:
                            return Methods.OpenerController(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 7:
                            return Methods.OpenerController(IsLastGCD(true, SlugShot), SlugShot.CanUse(out act, CanUseOption.MustUse));
                        case 8:
                            return Methods.OpenerController(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 9:
                            return Methods.OpenerController(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 10:
                            return Methods.OpenerController(IsLastGCD(true, CleanShot), CleanShot.CanUse(out act, CanUseOption.MustUse));
                        case 11:
                            return Methods.OpenerController(IsLastAbility(false, Reassemble), Reassemble.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 12:
                            return Methods.OpenerController(IsLastAbility(false, Wildfire), Wildfire.CanUse(out act, (CanUseOption)17));
                        case 13:
                            return Methods.OpenerController(IsLastGCD(false, ChainSaw), ChainSaw.CanUse(out act, CanUseOption.MustUse));
                        case 14:
                            return Methods.OpenerController(IsLastAbility(true, RookAutoturret), RookAutoturret.CanUse(out act, CanUseOption.MustUse));
                        case 15:
                            return Methods.OpenerController(IsLastAbility(false, Hypercharge), Hypercharge.CanUse(out act, (CanUseOption)51));
                        case 16:
                            return Methods.OpenerController(IsLastGCD(false, HeatBlast) && HeatStacks == 4, HeatBlast.CanUse(out act, CanUseOption.MustUse));
                        case 17:
                            return Methods.OpenerController(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 18:
                            return Methods.OpenerController(IsLastGCD(false, HeatBlast) && HeatStacks == 3, HeatBlast.CanUse(out act, CanUseOption.MustUse));
                        case 19:
                            return Methods.OpenerController(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 20:
                            return Methods.OpenerController(IsLastGCD(false, HeatBlast) && HeatStacks == 2, HeatBlast.CanUse(out act, CanUseOption.MustUse));
                        case 21:
                            return Methods.OpenerController(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 22:
                            return Methods.OpenerController(IsLastGCD(false, HeatBlast) && HeatStacks == 1, HeatBlast.CanUse(out act, CanUseOption.MustUse));
                        case 23:
                            return Methods.OpenerController(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 24:
                            return Methods.OpenerController(IsLastGCD(false, HeatBlast) && HeatStacks == 0, HeatBlast.CanUse(out act, CanUseOption.MustUse));
                        case 25:
                            return Methods.OpenerController(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 26:
                            return Methods.OpenerController(IsLastGCD(false, Drill), Drill.CanUse(out act, CanUseOption.MustUse));
                        case 27:
                            Methods.OpenerHasFinished = true;
                            Methods.OpenerInProgress = false;
                            // Finished Early AA
                            break;
                    }
                    break;
                case 1: // Delayed Tools
                    switch (Methods.OpenerStep)
                    {
                        case 0:
                            return Methods.OpenerController(IsLastGCD(true, SplitShot), SplitShot.CanUse(out act, CanUseOption.MustUse));
                        case 1:
                            return Methods.OpenerController(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 2:
                            return Methods.OpenerController(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 3:
                            return Methods.OpenerController(IsLastGCD(false, Drill), Drill.CanUse(out act, CanUseOption.MustUse));
                        case 4:
                            return Methods.OpenerController(IsLastAbility(false, BarrelStabilizer), BarrelStabilizer.CanUse(out act, CanUseOption.MustUse));
                        case 5:
                            return Methods.OpenerController(IsLastGCD(true, SlugShot), SlugShot.CanUse(out act, CanUseOption.MustUse));
                        case 6:
                            return Methods.OpenerController(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 7:
                            return Methods.OpenerController(IsLastGCD(true, CleanShot), CleanShot.CanUse(out act, CanUseOption.MustUse));
                        case 8:
                            return Methods.OpenerController(IsLastAbility(false, Reassemble), Reassemble.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 9:
                            return Methods.OpenerController(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 10:
                            return Methods.OpenerController(IsLastGCD(false, AirAnchor), AirAnchor.CanUse(out act, CanUseOption.MustUse));
                        case 11:
                            return Methods.OpenerController(IsLastAbility(false, Reassemble), Reassemble.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 12:
                            return Methods.OpenerController(IsLastAbility(false, Wildfire), Wildfire.CanUse(out act, (CanUseOption)17));
                        case 13:
                            return Methods.OpenerController(IsLastGCD(false, ChainSaw), ChainSaw.CanUse(out act, CanUseOption.MustUse));
                        case 14:
                            return Methods.OpenerController(IsLastAbility(true, RookAutoturret), RookAutoturret.CanUse(out act, CanUseOption.MustUse));
                        case 15:
                            return Methods.OpenerController(IsLastAbility(false, Hypercharge), Hypercharge.CanUse(out act, (CanUseOption)51));
                        case 16:
                            return Methods.OpenerController(IsLastGCD(false, HeatBlast) && HeatStacks == 4, HeatBlast.CanUse(out act, CanUseOption.MustUse));
                        case 17:
                            return Methods.OpenerController(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 18:
                            return Methods.OpenerController(IsLastGCD(false, HeatBlast) && HeatStacks == 3, HeatBlast.CanUse(out act, CanUseOption.MustUse));
                        case 19:
                            return Methods.OpenerController(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 20:
                            return Methods.OpenerController(IsLastGCD(false, HeatBlast) && HeatStacks == 2, HeatBlast.CanUse(out act, CanUseOption.MustUse));
                        case 21:
                            return Methods.OpenerController(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 22:
                            return Methods.OpenerController(IsLastGCD(false, HeatBlast) && HeatStacks == 1, HeatBlast.CanUse(out act, CanUseOption.MustUse));
                        case 23:
                            return Methods.OpenerController(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 24:
                            return Methods.OpenerController(IsLastGCD(false, HeatBlast) && HeatStacks == 0, HeatBlast.CanUse(out act, CanUseOption.MustUse));
                        case 25:
                            return Methods.OpenerController(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 26:
                            return Methods.OpenerController(IsLastGCD(false, Drill), Drill.CanUse(out act, CanUseOption.MustUse));
                        case 27:
                            Methods.OpenerHasFinished = true;
                            Methods.OpenerInProgress = false;
                            // Finished Delayed Tools
                            break;
                    }
                    break;
                case 2: // Early All
                    switch (Methods.OpenerStep)
                    {
                        case 0:
                            return Methods.OpenerController(IsLastGCD(false, AirAnchor), AirAnchor.CanUse(out act, CanUseOption.MustUse));
                        case 1:
                            return Methods.OpenerController(IsLastAbility(false, BarrelStabilizer), BarrelStabilizer.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 2:
                            return Methods.OpenerController(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty | CanUseOption.OnLastAbility));
                        case 3:
                            return Methods.OpenerController(IsLastGCD(false, Drill), Drill.CanUse(out act, CanUseOption.MustUse));
                        case 4:
                            return Methods.OpenerController(IsLastAbility(false, Reassemble), Reassemble.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 5:
                            return Methods.OpenerController(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty | CanUseOption.OnLastAbility));
                        case 6:
                            return Methods.OpenerController(IsLastGCD(false, ChainSaw), ChainSaw.CanUse(out act, CanUseOption.MustUse));
                        case 7:
                            return Methods.OpenerController(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 8:
                            return Methods.OpenerController(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty | CanUseOption.OnLastAbility));
                        case 9:
                            return Methods.OpenerController(IsLastGCD(true, SplitShot), SplitShot.CanUse(out act, CanUseOption.MustUse));
                        case 10:
                            return Methods.OpenerController(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 11:
                            return Methods.OpenerController(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty | CanUseOption.OnLastAbility));
                        case 12:
                            return Methods.OpenerController(IsLastGCD(true, SlugShot), SlugShot.CanUse(out act, CanUseOption.MustUse));
                        case 13:
                            return Methods.OpenerController(IsLastAbility(false, Tactician), Tactician.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 14:
                            return Methods.OpenerController(IsLastAbility(false, Wildfire), Wildfire.CanUse(out act, CanUseOption.OnLastAbility));
                        case 15:
                            return Methods.OpenerController(IsLastGCD(true, CleanShot), CleanShot.CanUse(out act, CanUseOption.MustUse));
                        case 16:
                            return Methods.OpenerController(IsLastAbility(true, RookAutoturret), RookAutoturret.CanUse(out act, CanUseOption.MustUse));
                        case 17:
                            return Methods.OpenerController(IsLastAbility(false, Hypercharge), Hypercharge.CanUse(out act, CanUseOption.OnLastAbility));
                        case 18:
                            Methods.OpenerHasFinished = true;
                            Methods.OpenerInProgress = false;
                            // Finished Early All
                            break;
                    }
                    break;

            }
        }
        act = null;
        return false;
    }
    #endregion

    #region GCD Logic
    protected override bool GeneralGCD(out IAction act)
    {
        act = null;

        #region PVE
        if (Methods.OpenerInProgress)
        {
            return Opener(out act);
        }
        if (!Methods.OpenerInProgress)
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
        #endregion
        return base.GeneralGCD(out act);

    }
    #endregion

    #region oGCD Logic
    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
    {
        //TerritoryContentType Content = TerritoryContentType;
        //bool Dungeon = (int)Content == 2;

        if (ShouldUseBurstMedicine(out act))
        {
            return true;
        }
        if (Methods.OpenerInProgress)
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
            if (HostileTarget.GetHealthRatio() < 0.02 && ((Player.HasStatus(true, StatusID.Wildfire)) || Methods.InBurst) && Wildfire.ElapsedAfter(5f) && Detonator.CanUse(out act))
            {
                return true;
            }
        }

        // LvL 90+
        if (!Methods.OpenerInProgress)
        {
            if (Wildfire.CanUse(out act, (CanUseOption)16))
            {
                if ((nextGCD == ChainSaw && Heat >= 50) ||
                    (IsLastAbility(ActionID.Hypercharge) && HeatStacks > 4) ||
                    (Heat >= 45 && !Drill.WillHaveOneCharge(5) &&
                     !AirAnchor.WillHaveOneCharge(7.5f) &&
                     !ChainSaw.WillHaveOneCharge(7.5f)))
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
                if (Methods.InBurst && IsLastGCD(ActionID.ChainSaw, ActionID.AirAnchor, ActionID.Drill, ActionID.SplitShot, ActionID.SlugShot, ActionID.CleanShot, ActionID.HeatedSplitShot, ActionID.HeatedSlugShot))
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

    }
    #endregion

    #region Helper Methods
    // Tincture Conditions
    private bool ShouldUseBurstMedicine(out IAction act)
    {
        act = null; // Default to null if Tincture cannot be used.

        // Don't use Tincture if player has the 'Weakness' status 
        if (Player.HasStatus(true, StatusID.Weakness) || Player.HasStatus(true, StatusID.Transcendent))
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
    #endregion

    #region Extra Helper Methods
    // Updates Status of other extra helper methods on every frame
    protected override void UpdateInfo()
    {
        HandleOpenerAvailability();
        ToolKitCheck();
        Methods.StateOfOpener();
    }

    // Checks if any major tool skill will almost come off CD (only at lvl 90), and sets "InBurst" to true if Player has Wildfire active
    private void ToolKitCheck()
    {
        bool WillHaveDrill = Drill.WillHaveOneCharge(5f);
        bool WillHaveAirAnchor = AirAnchor.WillHaveOneCharge(5f);
        bool WillHaveChainSaw = ChainSaw.WillHaveOneCharge(5f);
        if (Player.Level >= 90)
        {
            WillhaveTool = WillHaveDrill || WillHaveAirAnchor || WillHaveChainSaw;
        }

        Methods.InBurst = Player.HasStatus(true, StatusID.Wildfire);
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
        bool Openerstep0 = Methods.OpenerStep == 0;
        Methods.OpenerActionsAvailable = ReassembleOneCharge && HasChainSaw && HasAirAnchor && HasDrill && HasBarrelStabilizer && RCcharges == 3 && HasWildfire && GRcharges == 3 && Lvl90 && NoBattery && NoHeat && Openerstep0;

        // Future Opener conditions for ULTS

        TerritoryContentType Content = TerritoryContentType;    // Not implemented yet
        bool UltimateRaids = (int)Content == 28;                // Not implemented yet
        bool UwUorUCoB = UltimateRaids && Player.Level == 70;   // Not implemented yet
        bool TEA = UltimateRaids && Player.Level == 80;         // Not implemented yet



        Methods.LvL70_Ultimate_OpenerActionsAvailable = UwUorUCoB && NoResources && ReassembleOneCharge && HasDrill && HasWildfire && HasBarrelStabilizer;

        Methods.LvL80_Ultimate_OpenerActionsAvailable = TEA && NoResources && ReassembleOneCharge && HasDrill && HasAirAnchor && HasWildfire && HasBarrelStabilizer;

    }
    #endregion

}