using Dalamud.Logging;
using Dalamud.Plugin.Services;
using KirboRotations.Utility;
using Newtonsoft.Json.Linq;
using RotationSolver.Basic.Configuration;
using Serilog;
using Serilog.Events;

namespace BaseRotations.Ranged;
// Link to the Ratation source code
//[SourceCode(Path = "%Branch/FilePath to your sourse code% eg. main/BaseRotations/Melee/NIN_Default.cs%")]
// The detailed or extended description links of this Ratation, such as loop diagrams, recipe urls, teaching videos, etc.,
// can be written more than one
//[LinkDescription("%Link to the pics or just a link%", "%Description about your rotation.%")]
//[YoutubeLink(ID = "%If you got a youtube video link, please add here, just video id!%")]
[RotationDesc(ActionID.Wildfire)]
public sealed class Simple_MCH : MCH_Base
{
	public override string GameVersion => "6.51";

	public override string RotationName => "Simple_MCH";

	public override CombatType Type => CombatType.Both;

	protected override IRotationConfigSet CreateConfiguration() => base.CreateConfiguration()
		.SetFloat(ConfigUnitType.Percent, CombatType.Both, "MarksManThreshold", 0.31f, "Marksman Threshold", 0f, 1f, 0.1f);

	protected override bool GeneralGCD(out IAction act)
	{
		if(Methods.InPvP()) // PvP
		{
			act = null;
			if(Player.HasStatus(true, StatusID.PvP_Guard))
			{
				return false;
			}

			if(HostileTarget.HasStatus(false, StatusID.PvP_Guard, StatusID.PvP_HallowedGround, StatusID.PvP_UndeadRedemption, StatusID.PvP_Chiten))
			{
				return false;
			}

			if(PvP_HeatBlast.CanUse(out act, CanUseOption.MustUseEmpty))
			{
				return true;
			}

			if(PvP_MarksmansSpite.CanUse(out act, CanUseOption.MustUseEmpty)
				&& HostileTarget.GetHealthRatio() * 100 <= Configs.GetFloat("MarksManThreshold")
				&& !IsOverheated)
			{
				return true;
			}

			if(PvP_Drill.CanUse(out act, CanUseOption.MustUseEmpty))
			{
				return true;
			}

			if(PvP_Bioblaster.CanUse(out act, CanUseOption.MustUseEmpty, 1) && HostileTarget.DistanceToPlayer() < 12)
			{
				return true;
			}

			if(PvP_AirAnchor.CanUse(out act, CanUseOption.MustUseEmpty))
			{
				return true;
			}

			if(PvP_ChainSaw.CanUse(out act, CanUseOption.MustUseEmpty))
			{
				return true;
			}

			if(PvP_Scattergun.CanUse(out act, CanUseOption.MustUseEmpty, 1) && HostileTarget.DistanceToPlayer() < 12)
			{
				return true;
			}

			if(PvP_BlastCharge.CanUse(out act, CanUseOption.IgnoreCastCheck | CanUseOption.MustUse))
			{
				return true;
			}

			return base.GeneralGCD(out act);
		}
		else // PvE
		{
			float tolerance = 0.5f;
			bool NotOverheated = !IsOverheated;
			if(AutoCrossbow.CanUse(out act) && HostileTarget.DistanceToPlayer() <= 12)
			{
				return true;
			}

			if(HeatBlast.CanUse(out act, CanUseOption.MustUse))
			{
				return true;
			}

			if(HostileTarget.GetHealthRatio() > 0.6 && BioBlaster.CanUse(out act, CanUseOption.MustUse, 2) && HostileTarget.DistanceToPlayer() <= 12)
			{
				return true;
			}

			if(Drill.CanUse(out act) && NotOverheated)
			{
				return true;
			}

			if(AirAnchor.CanUse(out act, CanUseOption.MustUse | CanUseOption.EmptyOrSkipCombo) && NotOverheated)
			{
				return true;
			}

			if(!AirAnchor.EnoughLevel && HotShot.CanUse(out act, CanUseOption.MustUse | CanUseOption.EmptyOrSkipCombo) && NotOverheated)
			{
				return true;
			}

			if(ChainSaw.CanUse(out act, CanUseOption.MustUse | CanUseOption.EmptyOrSkipCombo) && NotOverheated)
			{
				return true;
			}

			if(SpreadShot.CanUse(out act, CanUseOption.MustUse, 2))
			{
				return true;
			}

			if(CleanShot.CanUse(out act)) // 3
			{
				if(Drill.WillHaveOneCharge(tolerance))
				{
					return false;
				}
				if(AirAnchor.WillHaveOneCharge(tolerance))
				{
					return false;
				}
				if(ChainSaw.WillHaveOneCharge(tolerance))
				{
					return false;
				}
				return true;
			}

			if(SlugShot.CanUse(out act)) // 2
			{
				if(Drill.WillHaveOneCharge(tolerance))
				{
					return false;
				}
				if(AirAnchor.WillHaveOneCharge(tolerance))
				{
					return false;
				}
				if(ChainSaw.WillHaveOneCharge(tolerance))
				{
					return false;
				}
				return true;
			}

			if(SplitShot.CanUse(out act)) // 1
			{
				if(Drill.WillHaveOneCharge(tolerance))
				{
					return false;
				}
				if(AirAnchor.WillHaveOneCharge(tolerance))
				{
					return false;
				}
				if(ChainSaw.WillHaveOneCharge(tolerance))
				{
					return false;
				}
				return true;
			}
			return base.GeneralGCD(out act);
		}
	}

	public static byte HeatStacks
	{
		get
		{
			byte stacks = Player.StatusStack(true, StatusID.Overheated);
			return stacks == byte.MaxValue ? (byte)5 : stacks;
		}
	}
	protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
	{
		if(Methods.InPvP()) // PvP
		{
			act = null;

			if(Player.HasStatus(true, StatusID.PvP_Guard))
			{
				return false;
			}

			if(HostileTarget.HasStatus(false, StatusID.PvP_Guard, StatusID.PvP_HallowedGround, StatusID.PvP_UndeadRedemption, StatusID.PvP_Chiten))
			{
				return false;
			}

			if(PvP_MarksmansSpite.CanUse(out act, CanUseOption.MustUseEmpty)
				&& HostileTarget.GetHealthRatio() * 100 <= Configs.GetFloat("MarksManThreshold")
				&& !IsOverheated)
			{
				return true;
			}

			if(PvP_Wildfire.CanUse(out act, CanUseOption.OnLastAbility) && nextGCD == PvP_HeatBlast)
			{
				return true;
			}

			if((nextGCD == PvP_Drill || (Player.HasStatus(true, StatusID.PvP_DrillPrimed) && NumberOfAllHostilesInRange > 0)) && PvP_Analysis.CanUse(out act, CanUseOption.MustUseEmpty))
			{
				return true;
			}

			if(Player.HasStatus(true, StatusID.PvP_ChainSawPrimed) && HostileTarget.GetHealthRatio() < 0.5 && PvP_Analysis.CanUse(out act, CanUseOption.MustUseEmpty))
			{
				return true;
			}

			if(Player.HasStatus(true, StatusID.PvP_BioblasterPrimed) && PvP_Analysis.CurrentCharges > 1 && PvP_Analysis.CanUse(out act, CanUseOption.MustUse))
			{
				return true;
			}

			if(Player.HasStatus(true, StatusID.PvP_AirAnchorPrimed) && PvP_Analysis.CurrentCharges > 1 && PvP_Analysis.CanUse(out act, CanUseOption.MustUse))
			{
				return true;
			}

			if(PvP_BishopAutoTurret.CanUse(out act, CanUseOption.MustUse))
			{
				if(NumberOfAllHostilesInRange == 0)
				{
					return false;
				}
				else if(NumberOfAllHostilesInRange >= 1)
				{
					return true;
				}
				return false;
			}
			return base.EmergencyAbility(nextGCD, out act);
		}
		else // PvE
		{
			if(ShouldUseBurstMedicine(out act))
			{
				return true;
			}

			if(ShouldUseBarrelStabilizer(out act))
			{
				return true;
			}

			if(Wildfire.CanUse(out act, CanUseOption.OnLastAbility))
			{
				if(ChainSaw.EnoughLevel && nextGCD == ChainSaw && Heat >= 50)
				{
					return true;
				}

				if(Drill.IsCoolingDown && AirAnchor.IsCoolingDown && ChainSaw.IsCoolingDown && Heat >= 45)
				{
					return true;
				}

				if(!CombatElapsedLessGCD(2) && Heat >= 50)
				{
					return true;
				}

				if(IsOverheated && HeatStacks > 4)
				{
					return true;
				}

				return false;
			}

			/*if(ShouldUseWildfire(nextGCD, out act))
			{
				return true;
			}*/

			if(ShouldUseRookAutoturret(nextGCD, out act) && NextAbilityToNextGCD > RookAutoturret.AnimationLockTime + Ping)
			{
				return true;
			}

			if(ShouldUseReassemble(nextGCD, out act) && !IsOverheated && NextAbilityToNextGCD > Reassemble.AnimationLockTime + Ping)
			{
				return true;
			}

			if(ShouldUseHypercharge(out act))
			{
				return true;
			}

			/*if(Detonator.CanUse(out act))
			{
				return true;
			}

			if(QueenOverdrive.CanUse(out act))
			{
				return true;
			}*/

			if(ShouldUseGaussroundOrRicochet(out act) && NextAbilityToNextGCD > GaussRound.AnimationLockTime + Ping)
			{
				return true;
			}

			/*if(GaussRound.CanUse(out act, CanUseOption.MustUseEmpty) && NextAbilityToNextGCD > 0.6)
			{
				return true;
			}
			if(Ricochet.HasOneCharge && Ricochet.CanUse(out act, CanUseOption.MustUseEmpty | CanUseOption.OnLastAbility) && NextAbilityToNextGCD > 0.6)
			{
				return true;
			}*/

			return base.EmergencyAbility(nextGCD, out act);
		}
	}

	#region PvE Methods
	private bool ShouldUseBurstMedicine(out IAction act)
	{
		act = null; // Default to null if Burst Medicine cannot be used.

		if(Player.HasStatus(true, StatusID.Weakness))
		{
			return false;
		}

		// Check if the conditions for using Burst Medicine are met.
		if(Wildfire.WillHaveOneCharge(20) && CombatTime > 60 && NextAbilityToNextGCD > 1.2 && !Player.HasStatus(true, StatusID.Weakness)
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

		// Check if Player already has Reassemble
		bool hasReassemble = Player.HasStatus(true, StatusID.Reassemble);

		// Check if the player's level is too low for Drill.
		bool isPlayerLevelTooLowForDrill = !Drill.EnoughLevel;

		// Check if the next GCD is Drill, AirAnchor, or ChainSaw, or if the player's level is too low for Drill and the next GCD is CleanShot.
		bool isNextGCDEligible =
			nextGCD == Drill ||
			nextGCD == AirAnchor ||
			nextGCD == ChainSaw ||
			(isPlayerLevelTooLowForDrill && nextGCD == CleanShot);

		// If the cooldown of any of the abilities is within the specified range or the next GCD is eligible,
		// attempt to use Reassemble.
		if(isNextGCDEligible && !hasReassemble)
		{
			// Try to use Reassemble. If it can be used, set 'act' and return true.
			return Reassemble.CanUse(out act, CanUseOption.MustUseEmpty);
		}

		// If none of the conditions are met, return false.
		return false;
	}
	private bool ShouldUseHypercharge(out IAction act)
	{
		act = null; // Default to null if Hypercharge cannot be used.

		// Check if currently overheated, which would make using Hypercharge unnecessary or impossible.
		if(IsOverheated)
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
		if(shouldUseHypercharge)
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
		if(!HostileTarget.IsBossFromTTK() && !HostileTarget.IsDummy())
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
		if(shouldUseWildfire)
		{
			return Wildfire.CanUse(out act, CanUseOption.OnLastAbility);
		}

		// If the conditions are not met, return false.
		return false;
	}

	/*private bool ShouldUseWildfire(IAction nextGCD, out IAction act)
	{
		act = null; // Default to null if Wildfire cannot be used.
		bool notEnoughHeatForHypercharge = Heat < 45;
		bool sufficientHeatForWildfire = Heat >= 45; // Ensure at least 50 heat for later use of Wildfire.

		if(notEnoughHeatForHypercharge || !InCombat || (IsOverheated && HeatStacks <= 4))
		{
			return false;
		}

		// Check if the target is a boss. If not, return false immediately.
		if(!HostileTarget.IsBossFromTTK() && !HostileTarget.IsDummy())
		{
			return false;
		}

		// Check if the combat time is less than 15 seconds.
		bool isEarlyCombat = CombatElapsedLess(15);

		// Determine if Wildfire should be used early in combat.
		bool shouldUseWildfireEarly = isEarlyCombat && Drill.IsCoolingDown && AirAnchor.IsCoolingDown;

		// After the initial sequence, use Wildfire when two of the main abilities are cooling down and the third is the next GCD, or when all three are cooling down.
		bool allAbilitiesCoolingDown = Drill.IsCoolingDown && AirAnchor.IsCoolingDown && ChainSaw.IsCoolingDown;
		bool shouldUseWildfireLater = !isEarlyCombat 
										&& sufficientHeatForWildfire 
										&&
									  (allAbilitiesCoolingDown ||
									   (Drill.IsCoolingDown && AirAnchor.IsCoolingDown && nextGCD == ChainSaw) ||
									   (Drill.IsCoolingDown && ChainSaw.IsCoolingDown && nextGCD == AirAnchor) ||
									   (AirAnchor.IsCoolingDown && ChainSaw.IsCoolingDown && nextGCD == Drill));

		// Combine the early combat and later conditions.
		bool shouldUseWildfire = shouldUseWildfireEarly || shouldUseWildfireLater;

		// If the conditions are met, attempt to use Wildfire.
		if(shouldUseWildfire)
		{
			return Wildfire.CanUse(out act, CanUseOption.OnLastAbility);
		}

		// If the conditions are not met, return false.
		return false;
	}*/
	private bool ShouldUseBarrelStabilizer(out IAction act)
	{
		act = null; // Default to null if Barrel Stabilizer cannot be used.

		// Check if the target is not a boss or a dummy.
		if(!HostileTarget.IsBossFromTTK() || !HostileTarget.IsDummy())
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
		if(shouldUseBarrelStabilizer)
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
		if(HostileTarget.IsBossFromTTK() || HostileTarget.IsDummy())
		{
			// If combat time is less than 80 seconds and last summon battery power was at least 50.
			if(CombatTime < 80 && Battery >= 50 && !RookAutoturret.IsCoolingDown)
			{
				return RookAutoturret.CanUse(out act, CanUseOption.MustUse);
			}
			// If combat time is more than 80 seconds and additional conditions are met, use Rook Autoturret.
			else if(CombatTime >= 80)
			{
				bool hasWildfireStatus = HostileTarget.HasStatus(true, StatusID.Wildfire);
				bool isWildfireCooldownLong = !Wildfire.WillHaveOneCharge(30);
				bool isBatteryHighEnough = Battery >= 80;
				bool isAirAnchorOrChainSawSoon = AirAnchor.WillHaveOneCharge(2.5f) || ChainSaw.WillHaveOneCharge(2.5f);
				bool isNextGCDCleanShot = nextGCD == CleanShot;

				if((isWildfireCooldownLong && isBatteryHighEnough) ||
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
			if(HostileTarget.GetTimeToKill(false) >= 17 && Battery == 100)
			{
				// If the next GCD is Clean Shot or if Air Anchor or Chain Saw are about to be ready.
				if(nextGCD == CleanShot || isAirAnchorOrChainSawSoon)
				{
					return RookAutoturret.CanUse(out act, CanUseOption.MustUse);
				}
			}
			// If the target's time to kill is 17 seconds or more, use Rook Autoturret.
			else if(HostileTarget.GetTimeToKill(false) >= 17)
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
		if(!GaussRound.HasOneCharge && !Ricochet.HasOneCharge)
		{
			return false;
		}

		if(!GaussRound.HasOneCharge && !Ricochet.EnoughLevel)
		{
			return false;
		}

		// Second, check if Ricochet is not at a sufficient level to be used.
		// If not, default to GaussRound (if it can be used).
		if(!Ricochet.EnoughLevel)
		{
			return GaussRound.CanUse(out act, CanUseOption.MustUseEmpty);
		}

		// Third, check if GaussRound and Ricochet have the same number of charges.
		// If they do, prefer using GaussRound.
		if(GaussRound.CurrentCharges >= Ricochet.CurrentCharges)
		{
			return GaussRound.CanUse(out act, CanUseOption.MustUseEmpty);
		}

		// Fourth, check if Ricochet has more or an equal number of charges compared to GaussRound.
		// If so, prefer using Ricochet.
		if(Ricochet.CurrentCharges >= GaussRound.CurrentCharges)
		{
			return Ricochet.HasOneCharge && Ricochet.CanUse(out act, CanUseOption.MustUseEmpty);
		}

		// If none of the above conditions are met, default to using GaussRound.
		// This is a fallback in case other conditions fail to determine a clear action.
		return GaussRound.CanUse(out act, CanUseOption.MustUseEmpty);
	}
	#endregion

	protected override IAction CountDownAction(float remainTime)
	{
		if(remainTime < Drill.AnimationLockTime && Player.HasStatus(true, StatusID.Reassemble) && Drill.CanUse(out _))
		{
			return Drill;
		}
		///if(remainTime <= 1.6 && UseBurstMedicine(out IAction act0, false))
		///{
		///return act0;
		///}
		if(remainTime < 5)
		{
			if(Player.Level >= 90 && Reassemble.CurrentCharges > 1 && !Player.HasStatus(true, StatusID.Reassemble))
			{
				return Reassemble;
			}
			else
			{
				if(Player.Level < 84 && Reassemble.CurrentCharges > 0 && !Player.HasStatus(true, StatusID.Reassemble))
					return Reassemble;
			}
		}

		return base.CountDownAction(remainTime);
	}

	protected override void UpdateInfo()
	{
		//Methods.InPvP();
	}

	public override bool ShowStatus => true;
	public override void DisplayStatus()
	{
		ImGui.Text("GCD remain: " + WeaponRemain);
		ImGui.Text("InPvP: " + Methods.InPvP());
		ImGui.Text("HeatStacks: " + HeatStacks);
		ImGui.Text($"HostileTarget GetHealthRatio:  {HostileTarget.GetHealthRatio() * 100:F2}%%");
		/*foreach(var hostileTarget in AllHostileTargets)
		{
			// Display the name of the hostile target
			ImGui.Text($"Name: {hostileTarget.Name}");

			var classJob = hostileTarget.ClassJob.GameData;
			if(classJob != null)
			{
				ImGui.Text($"Job Abbreviation: {classJob.Abbreviation}");
			}
			else
			{
				ImGui.Text("Job Abbreviation: Unknown");
			}

			// Indent the following items for visual hierarchy
			ImGui.Indent();

			// Display the distance to the player
			ImGui.Text($"Distance: {hostileTarget.DistanceToPlayer():F0}y");

			// Calculate the health percentage of the hostile target
			float healthPercentage = hostileTarget.CurrentHp / hostileTarget.MaxHp * 100;

			// Display the health of the hostile target
			ImGui.Text($"Health: {healthPercentage:F2}%%");

			// Unindent to reset the indentation for the next target
			ImGui.Unindent();
		}*/

	}

}
