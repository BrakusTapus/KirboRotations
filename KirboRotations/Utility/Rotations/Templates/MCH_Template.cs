// Define the namespace for the class. Namespaces are used to organize code into logical groups and to prevent name clashes.

// Define the namespace for the class. Namespaces are used to organize code into logical groups and to prevent name clashes.
// The 'KirboRotations.Ranged' namespace is specifically designated for ranged DPS rotations like Machinist.
namespace KirboRotations.Utility.CombatRotations.Templates;

// Import necessary libraries and namespaces. Remove or add imports as needed for specific rotation logic.
using System.Net.NetworkInformation; // Used for network-related operations. Remove if not needed.
// Add other necessary imports here.

// Custom attributes to provide metadata about the class. These are likely specific to the application or framework being used.

// Custom attributes to provide metadata about the class. These are specific to the application or framework being used.
[BetaRotation] // Indicates that this rotation is in a beta state and may not be final. Modify as needed.
[SourceCode(Path = "main/KirboRotations/Ranged/'filename'.cs")] // Path to the source code file. Replace 'filename' with the actual file name.
[LinkDescription("'linkForImageRelatedToTheRotation'")] // URL to an image related to the job's rotation. Replace with an appropriate link.
[RotationDesc(ActionID.Wildfire)] // Specifies the action that determines the burst window in the rotation. Adjust according to job-specific burst actions.

// Definition of the MCH_Template class, inheriting from MCH_Base. 'sealed' keyword prevents other classes from inheriting from this one.

// Definition of the MCH_Template class, inheriting from MCH_Base. The 'sealed' keyword prevents other classes from inheriting from this one.
// This class serves as a template for creating new Machinist rotations. Customize the class name and logic as needed for specific rotations.
public sealed class MCH_Template : MCH_Base
{
    // The following code defines basic information for the rotation.
    #region Rotation Info

    // Overrides the 'Type' property from the base class to specify the intended content type (PvE) for the rotation.
    // Modify this value to match the type of content the rotation is designed for (e.g., PvP).
    public override CombatType Type => CombatType.PvE;

    // Overrides the 'GameVersion' property to specify the game version this rotation is based on.
    // Update this value to reflect the current or targeted game version for the rotation.
    public override string GameVersion => "6.51";

    // Overrides the 'RotationName' property to provide a standard name for the rotation.
    // Replace with a descriptive and unique name for the specific rotation.
    public override string RotationName => "Kirbo's 'FullNameOfJob' '(PvP/PvE)'";

    // Overrides the 'Description' property to provide an optional description of the rotation.
    // Use this space to describe the rotation's purpose, strategy, or any other relevant information.
    public override string Description => "Kirbo's 'FullNameOfJob'";

    // Specifies the current version of the rotation.
    // Follow a consistent versioning scheme (e.g., Major.Minor.Patch) and update as the rotation evolves.
    public string RotationVersion => "1.0.0.12";

    #endregion

    // ... (continuing with the rest of the class content)


    // The following code defines additional actions and their logic.    
    #region Action Related Properties

    // This section should define properties and methods related to the Machinist's actions and abilities.
    // Utilize the mechanics and properties provided by the MCH_Base class to implement effective rotation logic.

    // Example: Property for a Machinist action
    // Replace with actual action definitions and logic specific to the Machinist job.
    // public static IBaseAction ExampleAction { get; } = new BaseAction(ActionID.ExampleAction);

    // Example: Method to determine if an action should be used based on certain conditions
    // Adapt the logic to suit the specific needs and strategies of the rotation.
    // protected static bool ShouldUseExampleAction()
    // {
    //     return /* condition for using the action */;
    // }

    #endregion

    // ... (continuing with the rest of the class content)
    #region Action Related Properties

    // Check at every frame if 1 of our major tools will come off cooldown soon
    private bool WillhaveTool { get; set; }
    // Sets InBurst to true if player has the wildfire Buff
    private bool InBurst { get; set; }
    // Holds the remaining amount of Heat stacks
    private static byte HeatStacks
    {
        get
        {
            byte stacks = Player.StatusStack(true, StatusID.Overheated);
            return stacks == byte.MaxValue ? (byte)5 : stacks;
        }
    }


    // Static property 'Dismantle', defined as a new IBaseAction instance.
    // The action is configured with specific options, indicating its nature and usage.
    private static new IBaseAction Dismantle { get; } = new BaseAction(ActionID.Dismantle, ActionOption.None | ActionOption.Defense);

    // Here, 'Dismantle' is redefined as a BaseAction with custom target selection logic.
    // The new keyword is used to hide the inherited member.
    private static new BaseAction Dismantle { get; } = new(ActionID.Dismantle)
    {
        // The ChoiceTarget is a lambda expression that determines the target for the action.
        ChoiceTarget = (Targets, mustUse) =>
        {
            // The lambda checks conditions like distance and status effects on potential targets.
            if (Targets != null &&
                Targets.YalmDistanceX < 25 &&
                !Targets.HasStatus(false, StatusID.Dismantled, StatusID.Reprisal))
            {
                // If conditions are met, it returns the target.
                return Targets;
            }
            // Otherwise, it returns null, indicating no valid target.
            return null;
        },
    };

    #endregion

    // Displays our 'Debug' in the status tab
    #region Debug window stuff

    public override bool ShowStatus => true;
    public override void DisplayStatus()
    {
        try
        {
            ImGuiEx.CollapsingHeaderWithContent("General Info", () =>
            {
                ImGui.Text($"Rotation: {RotationName} - v{RotationVersion}");
                ImGuiColoredText("Rotation  Job: ", ClassJob.Abbreviation, KirboColor.LightBlue);
                ImGuiEx.SeperatorWithSpacing();
                ImGui.Text($"Player Name: {Player.Name}");
                ImGui.Text($"Player Health: {Player.GetHealthRatio() * 100:F2}%%");
                ImGui.Text($"Player Health: {Player.CurrentMp}");
                ImGuiEx.SeperatorWithSpacing();
                ImGui.Text("In Combat: " + InCombat);
                // ... other general info ...
            });
            ImGuiEx.Tooltip("Displays General information like:\n-Rotation Name\n-Player's Health\n-InCombat Status");

            ImGuiEx.CollapsingHeaderWithContent("Rotation Status", () =>
            {
                string rotationText = GetRotationText(Configs.GetCombo("RotationSelection"));
                ImGui.Text($"Rotation Selection: {rotationText}");
                ImGui.Text("Openerstep: " + Methods.Openerstep);
                ImGui.Text("OpenerActionsAvailable: " + OpenerActionsAvailable);
                ImGui.Text("OpenerInProgress: " + Methods.OpenerInProgress);
                ImGui.Text("OpenerHasFailed: " + Methods.OpenerHasFailed);
                ImGui.Text("OpenerHasFinished: " + Methods.OpenerHasFinished);
                // ... other rotation status ...
            });
            ImGuiEx.Tooltip("Displays Rotation information like:\n-Selected Rotation\n-Opener Status");

            try
            {
                ImGuiEx.CollapsingHeaderWithContent("GCD Details", () =>
                {
                    if (ImGui.BeginTable("gcdTable", 2))
                    {
                        ImGui.TableSetupColumn("Description");
                        ImGui.TableSetupColumn("Value");
                        ImGui.TableHeadersRow();

                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                        ImGui.Text("GCD Remain:");
                        ImGui.TableNextColumn();
                        ImGui.Text(WeaponRemain.ToString());

                        // Add more rows as needed...

                        ImGui.EndTable();
                    }
                });
            }
            catch
            {
                Serilog.Log.Warning("Error: Collapsing Header - GCD Details");
            }

            // Calculate the remaining vertical space in the window
            float remainingSpace = ImGui.GetContentRegionAvail().Y - ImGui.GetFrameHeightWithSpacing(); // Subtracting button height with spacing
            if (remainingSpace > 0)
            {
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + remainingSpace);
            }

            // Reset Button at the bottom
            DisplayResetButton();
        }
        catch
        {
            Serilog.Log.Warning("Something wrong with DisplayStatus");
        }
    }

    private void DisplayResetButton()
    {
        if (ImGui.Button("Reset Rotation"))
        {
            Methods.ResetOpenerProperties();
        }
        ImGuiEx.Tooltip("Resets Opener properties\nUse this is Opener gets stuck");
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

    // Method 'CreateConfiguration' to set up configuration for the rotation.
    #region Rotation Config

    // It uses a base implementation and extends it with specific settings.
    protected override IRotationConfigSet CreateConfiguration() => base.CreateConfiguration()
        .SetCombo(CombatType.PvE, "RotationSelection", 1, "Select which Rotation will be used. (Openers will only be followed at level 90)", "Early AA", "Delayed Tools"/*, "Early All"*/)
        .SetBool(CombatType.PvE, "BatteryStuck", false, "Battery overcap protection\n(Will try and use Rook AutoTurret if Battery is at 100 and next skill increases Battery)")
        .SetBool(CombatType.PvE, "HeatStuck", false, "Heat overcap protection\n(Will try and use HyperCharge if Heat is at 100 and next skill increases Heat)")
        .SetBool(CombatType.PvE, "DumpSkills", false, "Dump Skills when Target is dying\n(Will try and spend remaining resources before boss dies)");

    #endregion

    // A protected method 'CountDownAction' that determines actions based on the remaining time before an event.
    #region Countdown Logic
    protected override IAction CountDownAction(float remainTime)
    {
        TerritoryContentType Content = TerritoryContentType;
        bool UltimateRaids = (int)Content == 28;
        bool UwUorUCoB = UltimateRaids && Player.Level == 70;
        bool TEA = UltimateRaids && Player.Level == 80;

        // If 'OpenerActionsAvailable' is true (see method 'HandleOpenerAvailability' for conditions) proceed to using Action logic during countdown
        if (OpenerActionsAvailable)
        {
            // Selects action logic depending on which rotation has been selected (Default: Delayed Tool)
            switch (Configs.GetCombo("RotationSelection"))
            {

                case 0: // Early AA
                    // Use AirAnchor when the remaining countdown time is less or equal to AirAnchor's AnimationLock AND player has the Reassemble Status, also sets OpenerInProgress to 'True'
                    if (remainTime <= AirAnchor.AnimationLockTime && Player.HasStatus(true, StatusID.Reassemble) && AirAnchor.CanUse(out _))
                    {
                        Methods.OpenerInProgress = true;
                        return AirAnchor;
                    }
                    // Use Tincture if Tincture use is enabled and the countdown time is less or equal to AirAnchor+Tincture animationlock (1.8s)
                    IAction act0;
                    if (remainTime <= TinctureOfDexterity8.AnimationLockTime + AirAnchor.AnimationLockTime && UseBurstMedicine(out act0, false))
                    {
                        return act0;
                    }
                    // Use Reassemble if countdown timer is 5s or less and Player has more then 1 Reassemble Charges AND does not already have the Reassemble Status
                    if (remainTime <= 5f && Reassemble.CurrentCharges > 1 && !Player.HasStatus(true, StatusID.Reassemble))
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

    // Logic for the opener
    #region Opener Logic

    /// <summary>
    /// <br>The first step should allign with the last action that was used during the countdown</br>
    /// <br>Case0 returns true and then OpenerStep increases the step number</br>
    /// <br>Case1 first checks the last action which is going to return false, which then triggers 'nextaction' to be true</br>
    /// <br>'nextaction' will then use the action that 'lastaction' is checking for</br>
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool Opener(out IAction act)
    {
        act = default(IAction);
        while (Methods.OpenerInProgress)
        {
            if (TimeSinceLastAction.TotalSeconds > 3.0 && !Methods.Flag)
            {
                Methods.OpenerHasFailed = true;
                Methods.OpenerInProgress = false;
                Methods.Openerstep = 0;
                Methods.Flag = true;
            }
            if (Player.IsDead && !Methods.Flag)
            {
                Methods.OpenerHasFailed = true;
                Methods.OpenerInProgress = false;
                Methods.Openerstep = 0;
                Methods.Flag = true;
            }
            switch (Configs.GetCombo("RotationSelection"))
            {
                case 0: // Early AA
                    switch (Methods.Openerstep)
                    {
                        case 0:
                            return Methods.OpenerStep(IsLastGCD(false, AirAnchor), AirAnchor.CanUse(out act, CanUseOption.MustUse));
                        case 1:
                            return Methods.OpenerStep(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));

                        // More steps

                        case 26:
                            return Methods.OpenerStep(IsLastGCD(false, Drill), Drill.CanUse(out act, CanUseOption.MustUse));
                        case 27:
                            Methods.OpenerHasFinished = true;
                            Methods.OpenerInProgress = false;
                            // Finished Early AA
                            break;
                    }
                    break;
                case 1: // Delayed Tools
                    switch (Methods.Openerstep)
                    {
                        case 0:
                            return Methods.OpenerStep(IsLastGCD(true, SplitShot), SplitShot.CanUse(out act, CanUseOption.MustUse));
                        case 1:
                            return Methods.OpenerStep(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 2:
                            return Methods.OpenerStep(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 3:
                            return Methods.OpenerStep(IsLastGCD(false, Drill), Drill.CanUse(out act, CanUseOption.MustUse));
                        case 4:
                            return Methods.OpenerStep(IsLastAbility(false, BarrelStabilizer), BarrelStabilizer.CanUse(out act, CanUseOption.MustUse));
                        case 5:
                            return Methods.OpenerStep(IsLastGCD(true, SlugShot), SlugShot.CanUse(out act, CanUseOption.MustUse));
                        case 6:
                            return Methods.OpenerStep(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 7:
                            return Methods.OpenerStep(IsLastGCD(true, CleanShot), CleanShot.CanUse(out act, CanUseOption.MustUse));
                        case 8:
                            return Methods.OpenerStep(IsLastAbility(false, Reassemble), Reassemble.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 9:
                            return Methods.OpenerStep(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 10:
                            return Methods.OpenerStep(IsLastGCD(false, AirAnchor), AirAnchor.CanUse(out act, CanUseOption.MustUse));
                        case 11:
                            return Methods.OpenerStep(IsLastAbility(false, Reassemble), Reassemble.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 12:
                            return Methods.OpenerStep(IsLastAbility(false, Wildfire), Wildfire.CanUse(out act, (CanUseOption)17));
                        case 13:
                            return Methods.OpenerStep(IsLastGCD(false, ChainSaw), ChainSaw.CanUse(out act, CanUseOption.MustUse));
                        case 14:
                            return Methods.OpenerStep(IsLastAbility(true, RookAutoturret), RookAutoturret.CanUse(out act, CanUseOption.MustUse));
                        case 15:
                            return Methods.OpenerStep(IsLastAbility(false, Hypercharge), Hypercharge.CanUse(out act, (CanUseOption)51));
                        case 16:
                            return Methods.OpenerStep(IsLastGCD(false, HeatBlast) && HeatStacks == 4, HeatBlast.CanUse(out act, CanUseOption.MustUse));
                        case 17:
                            return Methods.OpenerStep(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 18:
                            return Methods.OpenerStep(IsLastGCD(false, HeatBlast) && HeatStacks == 3, HeatBlast.CanUse(out act, CanUseOption.MustUse));
                        case 19:
                            return Methods.OpenerStep(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 20:
                            return Methods.OpenerStep(IsLastGCD(false, HeatBlast) && HeatStacks == 2, HeatBlast.CanUse(out act, CanUseOption.MustUse));
                        case 21:
                            return Methods.OpenerStep(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 22:
                            return Methods.OpenerStep(IsLastGCD(false, HeatBlast) && HeatStacks == 1, HeatBlast.CanUse(out act, CanUseOption.MustUse));
                        case 23:
                            return Methods.OpenerStep(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 24:
                            return Methods.OpenerStep(IsLastGCD(false, HeatBlast) && HeatStacks == 0, HeatBlast.CanUse(out act, CanUseOption.MustUse));
                        case 25:
                            return Methods.OpenerStep(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));
                        case 26:
                            return Methods.OpenerStep(IsLastGCD(false, Drill), Drill.CanUse(out act, CanUseOption.MustUse));
                        case 27:
                            Methods.OpenerHasFinished = true;
                            Methods.OpenerInProgress = false;
                            // Finished Delayed Tools
                            break;
                    }
                    break;
                case 2: // Early All
                    switch (Methods.Openerstep)
                    {
                        case 0:
                            return Methods.OpenerStep(IsLastGCD(false, AirAnchor), AirAnchor.CanUse(out act, CanUseOption.MustUse));
                        case 1:
                            return Methods.OpenerStep(IsLastAbility(false, BarrelStabilizer), BarrelStabilizer.CanUse(out act, CanUseOption.MustUseEmpty));

                        // More steps

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

    // The following methods are examples of how Global Cooldoown actions are implemented in the rotation.
    // This Code is called by the main plugin to handle GCD actions
    #region GCD Logic

    // Override of the 'GeneralGCD' method, which is likely related to global cooldown actions.
    // The method currently defers to the base implementation, indicating it can be customized further if needed.

    protected override bool GeneralGCD(out IAction act)
    {
        act = null;

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
        return base.GeneralGCD(out act);

    }
    #endregion

    // The following methods are examples of how off Global Cooldoown actions are implemented in the rotation.
    // This Code is called by the main plugin to handle oGCD actions
    #region oGCD Logic
    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
    {
        act = null;

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
            if (HostileTarget.GetHealthRatio() < 0.02 && ((Player.HasStatus(true, StatusID.Wildfire)) || InBurst) && Wildfire.ElapsedAfter(5f) && Detonator.CanUse(out act))
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
        return base.EmergencyAbility(nextGCD, out act);

    }
    #endregion

    // The following methods are examples of helper methods that can be used in the GCD or oGCD logic
    #region Helper Methods
    // Tincture Conditions
    private bool ShouldUseBurstMedicine(out IAction act)
    {
        act = null; // Default to null if Tincture cannot be used.

        // Don't use Tincture if player has the 'Weakness' status 
        if (Player.HasStatus(true, StatusID.Weakness))
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

    // The Following methods are examples of extra helper methods
    #region Extra Helper Methods

    // Updates Status of other extra helper methods on every frame
    protected override void UpdateInfo()
    {
        HandleOpenerAvailability();
        ToolKitCheck();
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

        InBurst = Player.HasStatus(true, StatusID.Wildfire);
    }

    // Used to check OpenerAvailability
    private void HandleOpenerAvailability()
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
        bool Openerstep0 = Methods.Openerstep == 0;
        OpenerActionsAvailable = ReassembleOneCharge && HasChainSaw && HasAirAnchor && HasDrill && HasBarrelStabilizer && HasRicochet && HasWildfire && HasGaussRound && Lvl90 && NoBattery && NoHeat && Openerstep0;
    }

    #endregion
}
