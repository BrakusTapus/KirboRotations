using KirboRotations.Ranged;
using KirboRotations.Utility.Core;
using KirboRotations.Utility.GameAssists;
using RotationSolver.Basic.Helpers;

namespace KirboRotations.Utility.Rotations.ActionHandling
{
    public class MCH_ActionLogic
    {
        private static byte HeatStacks
        {
            get
            {
                byte stacks = PlayerData.Object.StatusStack(true, StatusID.Overheated);
                return stacks == byte.MaxValue ? (byte)5 : stacks;
            }
        }

        // Reassemble Conditions
        private bool ShouldUseReassemble(IAction nextGCD, out IAction act)
        {
            act = null; // Default to null if Reassemble cannot be used.

            // Common checks before considering rotation variants
            bool hasReassemble = PlayerData.Object.HasStatus(true, StatusID.Reassemble);
            bool isPlayerLevelTooLowForDrill = !MCH_Base.Drill.EnoughLevel;
            bool isNextGCDEligibleForDefault =
            nextGCD == MCH_Base.Drill ||
            nextGCD == MCH_Base.AirAnchor ||
            nextGCD == MCH_Base.ChainSaw ||
            (isPlayerLevelTooLowForDrill && nextGCD == MCH_Base.CleanShot);

            // If the player already has Reassemble, return false.
            if (hasReassemble)
            {
                return false;
            }
            // If none of the conditions are met for any rotation variant, return false.
            return false;
        }

        // Hypercharge Conditions
        private bool ShouldUseHypercharge(out IAction act)
        {
            act = null; // Default to null if Hypercharge cannot be used.

            // Check if currently overheated, which would make using Hypercharge unnecessary or impossible.
            if (MCH_Base.IsOverheated)
            {
                return false;
            }

            // Check if the target has the Wildfire status.
            bool hasWildfire = UserRotations.HostileTarget.HasStatus(true, StatusID.Wildfire) || UserRotations.Player.HasStatus(true, StatusID.Wildfire);

            // Check if the Wildfire cooldown is greater than 30 seconds.
            bool isWildfireCooldownLong = !MCH_Base.Wildfire.WillHaveOneCharge(60);

            // Check if the Wildfire cooldown is less than 30 seconds.
            bool isWildfireCooldownShort = MCH_Base.Wildfire.WillHaveOneCharge(60);

            // Check if the Heat gauge is at least 50.
            bool isHeatAtLeast50 = MCH_Base.Heat >= 50;

            // Check if the Heat gauge is 95 or more.
            bool isHeatFullAlmostFull = MCH_Base.Heat >= 100;

            // Check the cooldowns of your main abilities to see if they will be ready soon.
            bool isAnyMainAbilityReadySoon = MCH_Base.Drill.WillHaveOneCharge(7.5f) ||
                                         MCH_Base.AirAnchor.WillHaveOneCharge(7.5f) ||
                                         MCH_Base.ChainSaw.WillHaveOneCharge(7.5f);

            // Check if the last ability used was Wildfire.
            bool isLastAbilityWildfire = UserRotations.IsLastAbility(ActionID.Wildfire);

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
                return MCH_Base.Hypercharge.CanUse(out act, CanUseOption.MustUse);
            }
            // If the conditions are not met, return false.
            return false;
        }

        // Wildfire set 1 Conditions
        private bool ShouldUseWildfire(out IAction act)
        {
            act = null; // Default to null if Wildfire cannot be used.

            // Check if the target is a boss. If not, return false immediately.
            if (!UserRotations.HostileTarget.IsBossFromTTK() && !UserRotations.HostileTarget.IsDummy())
            {
                return false;
            }

            // Check the cooldowns of your main abilities.
            bool isDrillReadySoon = MCH_Base.Drill.WillHaveOneCharge(7.5f);
            bool isAirAnchorReadySoon = MCH_Base.AirAnchor.WillHaveOneCharge(7.5f);
            bool isChainSawReadySoon = MCH_Base.ChainSaw.WillHaveOneCharge(7.5f);

            // Check if the combat time is less than 15 seconds and the last action was AirAnchor.
            bool isEarlyCombatAndLastActionAirAnchor = UserRotations.CombatTime < 15 && UserRotations.IsLastGCD(ActionID.AirAnchor);

            // Determine if Wildfire should be used based on the conditions provided.
            bool shouldUseWildfire = !isDrillReadySoon && !isAirAnchorReadySoon && !isChainSawReadySoon ||
                                 isEarlyCombatAndLastActionAirAnchor;

            // If the conditions are met, attempt to use Wildfire.
            if (shouldUseWildfire)
            {
                return MCH_Base.Wildfire.CanUse(out act, CanUseOption.OnLastAbility);
            }
            // If the conditions are not met, return false.
            return false;
        }

        // Wildfire set 2 Conditions
        private bool ShouldUseWildfire(IAction nextGCD, out IAction act)
        {
            if (MCH_Base.Wildfire.CanUse(out act, CanUseOption.OnLastAbility))
            {
                if (MCH_Base.ChainSaw.EnoughLevel && nextGCD == MCH_Base.ChainSaw && MCH_Base.Heat >= 50)
                {
                    return true;
                }

                if (MCH_Base.Drill.IsCoolingDown && MCH_Base.AirAnchor.IsCoolingDown && MCH_Base.ChainSaw.IsCoolingDown && MCH_Base.Heat >= 45)
                {
                    return true;
                }

                if (!UserRotations.CombatElapsedLessGCD(2) && MCH_Base.Heat >= 50)
                {
                    return true;
                }

                if (MCH_Base.IsOverheated && HeatStacks > 4)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        // BarrelStabilizer Conditions
        private bool ShouldUseBarrelStabilizer(out IAction act)
        {
            act = null; // Default to null if Barrel Stabilizer cannot be used.

            // Check if the target is not a boss or a dummy.
            if (!UserRotations.HostileTarget.IsBossFromTTK() || !UserRotations.HostileTarget.IsDummy())
            {
                return MCH_Base.BarrelStabilizer.CanUse(out act, CanUseOption.MustUse);
            }

            // Check if the combat time is less than 10 seconds and the last action was Drill.
            bool isEarlyCombatAndLastActionDrill = UserRotations.CombatTime < 10 && UserRotations.IsLastAction(ActionID.Drill);

            // Check the relative cooldowns of Wildfire and Barrel Stabilizer.
            bool isWildfireCooldownShorter = MCH_Base.Wildfire.WillHaveOneCharge(30) && MCH_Base.Heat >= 50;
            bool isWildfireCooldownLonger = !MCH_Base.Wildfire.WillHaveOneCharge(30);

            // Determine if Barrel Stabilizer should be used based on the conditions provided.
            bool shouldUseBarrelStabilizer = isEarlyCombatAndLastActionDrill ||
                                         isWildfireCooldownShorter ||
                                         isWildfireCooldownLonger;

            // If the conditions are met, attempt to use Barrel Stabilizer.
            if (shouldUseBarrelStabilizer)
            {
                return MCH_Base.BarrelStabilizer.CanUse(out act, CanUseOption.MustUse);
            }
            // If the conditions are not met, return false.
            return false;
        }

        // RookAutoturret Condition
        private bool ShouldUseRookAutoturret(IAction nextGCD, out IAction act)
        {
            act = null; // Default to null if Rook Autoturret cannot be used.

            // Logic when the target is a boss.
            if (UserRotations.HostileTarget.IsBossFromTTK() || UserRotations.HostileTarget.IsDummy())
            {
                // If combat time is less than 80 seconds and last summon battery power was at least 50.
                if (UserRotations.CombatTime < 80 && MCH_Base.Battery >= 50 && !MCH_Base.RookAutoturret.IsCoolingDown)
                {
                    return MCH_Base.RookAutoturret.CanUse(out act, CanUseOption.MustUse);
                }
                // If combat time is more than 80 seconds and additional conditions are met, use Rook Autoturret.
                else if (UserRotations.CombatTime >= 80)
                {
                    bool hasWildfireStatus = UserRotations.HostileTarget.HasStatus(true, StatusID.Wildfire);
                    bool isWildfireCooldownLong = !MCH_Base.Wildfire.WillHaveOneCharge(30);
                    bool isBatteryHighEnough = MCH_Base.Battery >= 80;
                    bool isAirAnchorOrChainSawSoon = MCH_Base.AirAnchor.WillHaveOneCharge(2.5f) || MCH_Base.ChainSaw.WillHaveOneCharge(2.5f);
                    bool isNextGCDCleanShot = nextGCD == MCH_Base.CleanShot;

                    if ((isWildfireCooldownLong && isBatteryHighEnough) ||
                        (hasWildfireStatus) ||
                        (!hasWildfireStatus && MCH_Base.Wildfire.WillHaveOneCharge(30) && (isBatteryHighEnough && isAirAnchorOrChainSawSoon)) ||
                        (isBatteryHighEnough && (isAirAnchorOrChainSawSoon || isNextGCDCleanShot)))
                    {
                        return MCH_Base.RookAutoturret.CanUse(out act, CanUseOption.MustUse);
                    }
                }
            }
            else // Logic when the target is not a boss.
            {
                // If the target's time to kill is 17 seconds or more and battery is full.
                bool isAirAnchorOrChainSawSoon = MCH_Base.AirAnchor.WillHaveOneCharge(2.5f) || MCH_Base.ChainSaw.WillHaveOneCharge(2.5f);
                if (UserRotations.HostileTarget.GetTimeToKill(false) >= 17 && MCH_Base.Battery == 100)
                {
                    // If the next GCD is Clean Shot or if Air Anchor or Chain Saw are about to be ready.
                    if (nextGCD == MCH_Base.CleanShot || isAirAnchorOrChainSawSoon)
                    {
                        return MCH_Base.RookAutoturret.CanUse(out act, CanUseOption.MustUse);
                    }
                }
                // If the target's time to kill is 17 seconds or more, use Rook Autoturret.
                else if (UserRotations.HostileTarget.GetTimeToKill(false) >= 17)
                {
                    return MCH_Base.RookAutoturret.CanUse(out act, CanUseOption.MustUse);
                }
            }
            // If none of the conditions are met, return false.
            return false;
        }
    }
}