using KirboRotations.Configurations;
using KirboRotations.Data;
using KirboRotations.Extensions;
using KirboRotations.Helpers.JobHelpers.Enums;
using KirboRotations.PvE.Beta;
using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Helpers;

namespace KirboRotations.Helpers.JobHelpers;

internal class MCHLogic : MCH_KirboPvEBeta
{
    // Check if the major tools will come off cooldown soon
    internal static bool WillhaveTool { get; set; }

    // Holds the remaining amount of Heat stacks
    internal static byte HeatStacks
    {
        get
        {
            byte stacks = Player.StatusStack(true, StatusID.Overheated);
            return stacks == byte.MaxValue ? (byte)5 : stacks;
        }
    }

    // Property to check if we're in a lvl 70 Ultimate
    internal static bool InLvL70Ultimate { get; set; }

    #region Opener

    // Checks if the opener sequence is available to be executed.
    // It verifies if certain abilities are not on cooldown and have enough charges.
    private static bool OpenerAvailable()
    {
        if (Ricochet.CurrentCharges < 3)
        {
            return false;
        }

        if (GaussRound.CurrentCharges < 3)
        {
            return false;
        }

        if (Drill.IsCoolingDown)
        {
            return false;
        }

        if (Wildfire.IsCoolingDown)
        {
            return false;
        }

        if (BarrelStabilizer.IsCoolingDown)
        {
            return false;
        }

        // If all checks pass, the opener is available.
        return true;
    }

    // Checks if pre-pull Actions are ready.
    private static bool PrePullActionsAvailable()
    {
        if (Reassemble.CurrentCharges < 2)
        {
            return false;
        }

        // If the check passes, pre-pull Actions are ready.
        return true;
    }

    // The level required to execute the opener.
    private static uint OpenerLevel => 90;

    // Tracks the current step in the pre-pull phase.
    internal uint PrePullStep = 0;

    // Tracks the current step in the opener sequence.
    internal uint OpenerStep = 1;

    // Checks if the player's level is high enough to execute the opener.
    internal static bool LevelChecked => Player.Level >= OpenerLevel;

    // Determines if the opener can be executed based on various conditions.
    internal static bool CanOpener => OpenerAvailable() && PrePullActionsAvailable() && LevelChecked;

    // Current state of the opener (e.g., PrePull, InOpener, etc.).
    private OpenerState currentState = OpenerState.PrePull;

    public OpenerState CurrentState
    {
        get
        {
            // Returns the current state of the opener.
            return currentState;
        }

        set
        {
            // Only change state if the new value is different from the current state.
            // This prevents unnecessary state transitions and redundant actions.
            if (value != currentState)
            {
                // When entering the PrePull state, log the transition for debugging.
                if (value == OpenerState.PrePull)
                {
                    Serilog.Log.Information($"{RotationConfigs.v} Entered PrePull Opener");
                }

                // When entering the InOpener state, reset the OpenerStep to 1.
                // This indicates the beginning of the opener sequence.
                if (value == OpenerState.InOpener)
                {
                    OpenerStep = 1;
                }

                // Handle the completion or failure of the opener.
                // Both Finished and Failed states lead to resetting the opener.
                if (value == OpenerState.OpenerFinished || value == OpenerState.FailedOpener)
                {
                    // Log a failure message if the opener failed.
                    if (value == OpenerState.FailedOpener)
                    {
                        Serilog.Log.Information($"{RotationConfigs.v} Opener Failed");
                    }

                    // Reset the opener to its initial state.
                    // This is common for both Finished and Failed states.
                    ResetOpener();
                }
                // If the opener finished successfully, log this information.
                if (value == OpenerState.OpenerFinished)
                {
                    Serilog.Log.Information($"{RotationConfigs.v} Opener Finished");
                }

                // Update the current state to the new value.
                currentState = value;
            }
        }
    }

    // Executes steps before the actual opener. Returns null if the level check fails or no action is determined.
    internal bool DoPrePullSteps(out IAction act)
    {
        act = null;

        // Check if the player's level is high enough to perform the opener.
        if (!LevelChecked)
        {
            return false;
        }

        // If opener conditions are met and it's the first step, advance the pre-pull step.
        if (CanOpener && PrePullStep == 0)
        {
            PrePullStep = 1;
        }

        // If opener is not available, reset the pre-pull step.
        if (!OpenerAvailable())
        {
            PrePullStep = 0;
        }

        // Check if the current state is PrePull and if any pre-pull steps are pending.
        if (CurrentState == OpenerState.PrePull && PrePullStep > 0)
        {
            // Handle different rotation selections based on configuration.
            if (Configs.GetCombo("RotationSelection") == 0)
            {
                // If the player has the Reassemble status and it's the first pre-pull step, transition to InOpener state.
                if (Player.HasStatus(true, StatusID.Reassemble) && PrePullStep == 1)
                {
                    CurrentState = OpenerState.InOpener;
                }
                // Otherwise, if it's the first pre-pull step, set the action to Reassemble.
                else if (PrePullStep == 1)
                {
                    Reassemble.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                // If it's the second pre-pull step and the player does not have the Reassemble status, mark the opener as failed.
                if (PrePullStep == 2 && !Player.HasStatus(true, StatusID.Reassemble))
                {
                    CurrentState = OpenerState.FailedOpener;
                }
            }

            if (Configs.GetCombo("RotationSelection") == 1)
            {
                // If the last action was HeatedSplitShot and it's the first pre-pull step, transition to InOpener state.
                if (IsLastGCD(ActionID.HeatedSplitShot) && PrePullStep == 1)
                {
                    CurrentState = OpenerState.InOpener;
                }
                // Otherwise, if it's the first pre-pull step, set the action to SplitShot.
                else if (PrePullStep == 1)
                {
                    SplitShot.CanUse(out act, CanUseOption.MustUseEmpty);
                }
            }

            // Additional conditions and logic as per your rotation requirements can be added here.
            return true;
        }

        // Resets the opener steps to their initial state if no pre-pull steps are pending.
        PrePullStep = 0;
        return false;
    }

    private bool DoOpener(out IAction act)
    {
        act = null;
        if (!LevelChecked)
        {
            return false;
        }

        if (!LevelChecked)
        {
            CurrentState = OpenerState.FailedOpener;
        }

        if (currentState == OpenerState.InOpener)
        {
            if (Configs.GetCombo("RotationSelection") == 0)
            {
                if (IsLastAction(ActionID.AirAnchor) && OpenerStep == 1)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 1)
                {
                    AirAnchor.CanUse(out act);
                }

                if (IsLastAction(ActionID.GaussRound) && OpenerStep == 2)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 2)
                {
                    GaussRound.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (IsLastAction(ActionID.Ricochet) && OpenerStep == 3)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 3)
                {
                    Ricochet.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (IsLastAction(ActionID.Drill) && OpenerStep == 4)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 4)
                {
                    Drill.CanUse(out act);
                }

                if (IsLastAction(ActionID.BarrelStabilizer) && OpenerStep == 5)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 5)
                {
                    BarrelStabilizer.CanUse(out act);
                }

                if (IsLastAction(ActionID.HeatedSplitShot) && OpenerStep == 6)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 6)
                {
                    SplitShot.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (IsLastAction(ActionID.HeatedSlugShot) && OpenerStep == 7)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 7)
                {
                    SlugShot.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (IsLastAction(ActionID.GaussRound) && OpenerStep == 8)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 8)
                {
                    GaussRound.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (IsLastAction(ActionID.Ricochet) && OpenerStep == 9)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 9)
                {
                    Ricochet.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (IsLastAction((ActionID)ActionIDs.HeatedCleanShot) && OpenerStep == 10)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 10)
                {
                    CleanShot.CanUse(out act);
                }

                if (IsLastAction(ActionID.Reassemble) && OpenerStep == 11)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 11)
                {
                    Reassemble.CanUse(out act);
                }

                if (IsLastAction((ActionID)ActionIDs.WildFiree) && OpenerStep == 12)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 12)
                {
                    Wildfire.CanUse(out act);
                }

                if (IsLastAction(ActionID.ChainSaw) && OpenerStep == 12)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 12)
                {
                    ChainSaw.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (IsLastAction((ActionID)ActionIDs.AutomatonQueen) && OpenerStep == 13)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 13)
                {
                    RookAutoturret.CanUse(out act);
                }

                if (IsLastAction(ActionID.Hypercharge) && OpenerStep == 14)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 14)
                {
                    Hypercharge.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (IsLastAction(ActionID.HeatBlast) && HeatStacks == 4 && OpenerStep == 15)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 15)
                {
                    HeatBlast.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (IsLastAction(ActionID.Ricochet) && OpenerStep == 16)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 16)
                {
                    Ricochet.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (IsLastAction(ActionID.HeatBlast) && HeatStacks == 3 && OpenerStep == 17)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 17)
                {
                    HeatBlast.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (IsLastAction(ActionID.GaussRound) && OpenerStep == 18)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 18)
                {
                    GaussRound.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (IsLastAction(ActionID.HeatBlast) && HeatStacks == 2 && OpenerStep == 19)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 19)
                {
                    HeatBlast.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (IsLastAction(ActionID.Ricochet) && OpenerStep == 20)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 20)
                {
                    Ricochet.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (IsLastAction(ActionID.HeatBlast) && HeatStacks == 1 && OpenerStep == 21)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 21)
                {
                    HeatBlast.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (IsLastAction(ActionID.GaussRound) && OpenerStep == 22)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 22)
                {
                    GaussRound.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (IsLastAction(ActionID.HeatBlast) && HeatStacks == 0 && OpenerStep == 23)
                {
                    CurrentState = OpenerState.OpenerFinished;
                }
                else if (OpenerStep == 23)
                {
                    HeatBlast.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (IsLastAction(ActionID.Ricochet) && OpenerStep == 24)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 24)
                {
                    Ricochet.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (IsLastAction(ActionID.Drill) && OpenerStep == 25)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 25)
                {
                    Drill.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (IsLastAction(ActionID.GaussRound) && OpenerStep == 26)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 26)
                {
                    GaussRound.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (IsLastAction(ActionID.Ricochet) && OpenerStep == 27)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 27)
                {
                    Ricochet.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (TimeSinceLastAction.TotalSeconds >= 5 && currentState == OpenerState.InOpener)
                {
                    CurrentState = OpenerState.FailedOpener;
                }

                if ((act == Ricochet && Ricochet.CurrentCharges < 3 ||
                        act == ChainSaw && ChainSaw.IsCoolingDown ||
                        act == Wildfire && Wildfire.IsCoolingDown ||
                        act == BarrelStabilizer && BarrelStabilizer.IsCoolingDown ||
                        act == GaussRound && GaussRound.CurrentCharges < 3) && TimeSinceLastAction.TotalSeconds >= 3)
                {
                    CurrentState = OpenerState.FailedOpener;
                    return false;
                }
            }
            else if (Configs.GetCombo("RotationSelection") == 1)
            {
                if (IsLastAction(ActionID.HeatedSplitShot) && OpenerStep == 1)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 1)
                {
                    SplitShot.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (IsLastAction(ActionID.GaussRound) && OpenerStep == 2)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 2)
                {
                    GaussRound.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (IsLastAction(ActionID.Ricochet) && OpenerStep == 3)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 3)
                {
                    Ricochet.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (IsLastAction(ActionID.Drill) && OpenerStep == 4)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 4)
                {
                    Drill.CanUse(out act);
                }

                if (IsLastAction(ActionID.BarrelStabilizer) && OpenerStep == 5)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 5)
                {
                    BarrelStabilizer.CanUse(out act);
                }

                if (IsLastAction(ActionID.HeatedSlugShot) && OpenerStep == 6)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 6)
                {
                    SlugShot.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (IsLastAction(ActionID.Ricochet) && OpenerStep == 7)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 7)
                {
                    Ricochet.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (IsLastAction((ActionID)ActionIDs.HeatedCleanShot) && OpenerStep == 8)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 8)
                {
                    CleanShot.CanUse(out act);
                }

                if (IsLastAction(ActionID.Reassemble) && OpenerStep == 9)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 9)
                {
                    Reassemble.CanUse(out act);
                }

                if (IsLastAction(ActionID.GaussRound) && OpenerStep == 10)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 10)
                {
                    GaussRound.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (IsLastAction(ActionID.AirAnchor) && OpenerStep == 11)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 11)
                {
                    AirAnchor.CanUse(out act);
                }

                if (IsLastAction(ActionID.Reassemble) && OpenerStep == 12)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 12)
                {
                    Reassemble.CanUse(out act);
                }

                if (IsLastAction(ActionID.Wildfire) && OpenerStep == 13)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 13)
                {
                    Wildfire.CanUse(out act);
                }

                if (IsLastAction(ActionID.ChainSaw) && OpenerStep == 14)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 14)
                {
                    ChainSaw.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (IsLastAction((ActionID)ActionIDs.AutomatonQueen) && OpenerStep == 15)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 15)
                {
                    RookAutoturret.CanUse(out act);
                }

                if (IsLastAction(ActionID.Hypercharge) && OpenerStep == 16)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 16)
                {
                    Hypercharge.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (IsLastAction(ActionID.HeatBlast) && HeatStacks == 4 && OpenerStep == 17)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 17)
                {
                    HeatBlast.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (IsLastAction(ActionID.Ricochet) && OpenerStep == 18)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 18)
                {
                    Ricochet.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (IsLastAction(ActionID.HeatBlast) && HeatStacks == 3 && OpenerStep == 19)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 19)
                {
                    HeatBlast.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (IsLastAction(ActionID.GaussRound) && OpenerStep == 20)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 20)
                {
                    GaussRound.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (IsLastAction(ActionID.HeatBlast) && HeatStacks == 2 && OpenerStep == 21)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 21)
                {
                    HeatBlast.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (IsLastAction(ActionID.Ricochet) && OpenerStep == 22)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 22)
                {
                    Ricochet.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (IsLastAction(ActionID.HeatBlast) && HeatStacks == 1 && OpenerStep == 23)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 23)
                {
                    HeatBlast.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (IsLastAction(ActionID.GaussRound) && OpenerStep == 24)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 24)
                {
                    GaussRound.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (IsLastAction(ActionID.HeatBlast) && HeatStacks == 0 && OpenerStep == 25)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 25)
                {
                    HeatBlast.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (IsLastAction(ActionID.Ricochet) && OpenerStep == 26)
                {
                    OpenerStep++;
                }
                else if (OpenerStep == 26)
                {
                    Ricochet.CanUse(out act, CanUseOption.MustUseEmpty);
                }

                if (IsLastAction(ActionID.Drill) && OpenerStep == 27)
                {
                    CurrentState = OpenerState.OpenerFinished;
                }
                else if (OpenerStep == 27)
                {
                    Drill.CanUse(out act);
                }
            }

            if (TimeSinceLastAction.TotalSeconds >= 5 && currentState == OpenerState.InOpener)
            {
                CurrentState = OpenerState.FailedOpener;
            }

            if ((act == Ricochet && Ricochet.CurrentCharges < 3 ||
                    act == ChainSaw && ChainSaw.IsCoolingDown ||
                    act == Wildfire && Wildfire.IsCoolingDown ||
                    act == BarrelStabilizer && BarrelStabilizer.IsCoolingDown ||
                    act == GaussRound && GaussRound.CurrentCharges < 3) && TimeSinceLastAction.TotalSeconds >= 3)
            {
                CurrentState = OpenerState.FailedOpener;
                return false;
            }
            return true;
        }

        return false;
    }

    private void ResetOpener()
    {
        PrePullStep = 0;
        OpenerStep = 0;
    }

    public bool DoFullOpener(out IAction act)
    {
        act = null;

        if (!LevelChecked)
        {
            CurrentState = OpenerState.FailedOpener;
        }

        if (CurrentState == OpenerState.PrePull)
        {
            if (DoPrePullSteps(out act))
            {
                return true;
            }
        }

        if (CurrentState == OpenerState.InOpener)
        {
            if (DoOpener(out act))
            {
                return true;
            }
        }

        if (!Player.InCombat())
        {
            ResetOpener();
            CurrentState = OpenerState.PrePull;
        }

        return false;
    }

    public static void DisplayCurrentOpenerState()
    {
        string stateAsString = OpenerHelpers.CurrentOpenerState.ToString();
        Serilog.Log.Information($"Current Opener State: {stateAsString}");
    }

    #endregion Opener

    #region Special Rotation

    internal bool UCoBRotation(IAction nextGCD, out IAction act)
    {
        act = null;
        if (BattleCharaEx.SaveAction || !HostileTarget.IsTargetable)
        {
            return false;
        }

        if (Wildfire.CanUse(out act, (CanUseOption)16))
        {
            if (Heat >= 50 ||
                IsLastAbility(ActionID.Hypercharge) && MCHLogic.HeatStacks > 4 ||
                Heat >= 45 && !Drill.WillHaveOneCharge(5) && !HotShot.WillHaveOneCharge(7.5f))
            {
                return true;
            }
        }

        if (RookAutoturret.CanUse(out act) && HostileTarget.GetHealthRatio() > 0.1 && HostileTarget.IsTargetable)
        {
            if (Battery == 100 || RatioOfMembersIn2minsBurst > 0.5 || BurstHelpers.InBurst)
            {
                return true;
            }
        }

        if (HostileTarget.GetHealthRatio() > 0.1 && HostileTarget.IsTargetable)
        {
            if (Hypercharge.CanUse(out act) && (IsLastAbility(ActionID.Wildfire) || !WillhaveTool && (
                BurstHelpers.InBurst && IsLastGCD(ActionID.Drill, ActionID.SplitShot, ActionID.SlugShot, ActionID.CleanShot, ActionID.HeatedSplitShot, ActionID.HeatedSlugShot) ||
                Heat >= 100 && Wildfire.WillHaveOneCharge(10f) ||
                Heat >= 100 && Wildfire.WillHaveOneCharge(40f) ||
                Heat >= 50 && !Wildfire.WillHaveOneCharge(40f)
            )))
            {
                return true;
            }
        }

        if (BarrelStabilizer.CanUse(out act, CanUseOption.MustUseEmpty))
        {
            return true;
        }

        if (Reassemble.CanUse(out act, CanUseOption.MustUseEmpty) && nextGCD == Drill)
        {
            return true;
        }

        if (GaussRound.CanUse(out act, CanUseOption.MustUseEmpty) || Ricochet.CanUse(out act, CanUseOption.MustUseEmpty))
        {
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
                return Ricochet.HasOneCharge && Ricochet.CanUse(out act, CanUseOption.MustUseEmpty);
            }

            return GaussRound.CanUse(out act, CanUseOption.MustUseEmpty);
        }

        return false;
    }

    #endregion Special Rotation
}