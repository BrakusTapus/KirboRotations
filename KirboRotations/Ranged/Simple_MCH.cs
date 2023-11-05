namespace KirboRotations.Ranged;

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
	public override bool ShowStatus => true;
	public override void DisplayStatus()
	{
		ImGui.Text("GCD remain:        " + WeaponRemain);
		ImGui.Text("Drill CD remain: " + Drill.CooldownRemaining);
		ImGui.Text("Target.GetHealthRatio" + Target.GetHealthRatio().ToString());
		ImGui.Text("Target.GetHealthRatio" + HostileTarget.StatusTime(true, StatusID.Wildfire).ToString());
		ImGui.Text("Target.Name" + Target.Name.ToString());
	}

	protected override bool GeneralGCD(out IAction act)
	{
		if(DataCenter)
		{
			if(PvP_HeatBlast.CanUse(out act, CanUseOption.MustUseEmpty))
			{
				return true;
			}

			if(PvP_MarksmansSpite.CanUse(out act, CanUseOption.MustUseEmpty)
				&& !HostileTarget.HasStatus(false, StatusID.PvP_Guard)
				&& !IsOverheated)
			{
				return true;
			}

			if(PvP_Drill.CanUse(out act, CanUseOption.MustUseEmpty))
			{
				return true;
			}

			if(PvP_Bioblaster.CanUse(out act, CanUseOption.MustUseEmpty, 1) && Target.DistanceToPlayer() < 12)
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

			if(PvP_Scattergun.CanUse(out act, CanUseOption.MustUseEmpty))
			{
				return true;
			}

			if(PvP_BlastCharge.CanUse(out act, CanUseOption.IgnoreCastCheck | CanUseOption.MustUse))
			{
				return true;
			}

			return base.GeneralGCD(out act);
		}
		else
		{
			float tolerance = 0.5f;
			if(AutoCrossbow.CanUse(out act) && Target.DistanceToPlayer() <= 12)
			{
				return true;
			}

			if(HeatBlast.CanUse(out act, CanUseOption.MustUse))
			{
				return true;
			}

			if(Target.GetHealthRatio() > 0.6 && BioBlaster.CanUse(out act, CanUseOption.MustUse, 2) && Target.DistanceToPlayer() <= 12)
			{
				return true;
			}

			if(Drill.CanUse(out act))
			{
				return true;
			}

			if(AirAnchor.CanUse(out act, CanUseOption.MustUse | CanUseOption.EmptyOrSkipCombo))
			{
				return true;
			}

			if(!AirAnchor.EnoughLevel && HotShot.CanUse(out act, CanUseOption.MustUse | CanUseOption.EmptyOrSkipCombo))
			{
				return true;
			}

			if(ChainSaw.CanUse(out act, CanUseOption.MustUse | CanUseOption.EmptyOrSkipCombo))
			{
				return true;
			}

			if(SpreadShot.CanUse(out act, CanUseOption.MustUse, 2))
			{
				return true;
			}

			if(CleanShot.CanUse(out act)) // default skill
			{
				if(Drill.CooldownRemaining < tolerance)
				{
					return false;
				}
				if(AirAnchor.CooldownRemaining < tolerance)
				{
					return false;
				}
				if(ChainSaw.CooldownRemaining < tolerance)
				{
					return false;
				}
				return true;
			}

			if(SlugShot.CanUse(out act)) // default skill
			{
				if(Drill.CooldownRemaining < tolerance)
				{
					return false;
				}
				if(AirAnchor.CooldownRemaining < tolerance)
				{
					return false;
				}
				if(ChainSaw.CooldownRemaining < tolerance)
				{
					return false;
				}
				return true;
			}

			if(SplitShot.CanUse(out act)) // default skill
			{
				if(Drill.CooldownRemaining < tolerance)
				{
					return false;
				}
				if(AirAnchor.CooldownRemaining < tolerance)
				{
					return false;
				}
				if(ChainSaw.CooldownRemaining < tolerance)
				{
					return false;
				}
				return true;
			}
			return base.GeneralGCD(out act);
		}
		return base.GeneralGCD(out act);
	}

	//0GCD actions here.
	protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
	{

		if(InPvP)
		{
			act = null;
			if(PvP_Wildfire.CanUse(out act, CanUseOption.OnLastAbility) && nextGCD == PvP_HeatBlast)
			{
				return true;
			}
			if(nextGCD == PvP_Drill && PvP_Analysis.CanUse(out act, CanUseOption.MustUseEmpty))
			{
				return true;
			}
			if(nextGCD == PvP_ChainSaw && Target.GetHealthRatio() < 0.5 && PvP_Analysis.CanUse(out act, CanUseOption.MustUseEmpty))
			{
				return true;
			}
			if(nextGCD == PvP_Bioblaster && PvP_Analysis.CurrentCharges > 1 && PvP_Analysis.CanUse(out act, CanUseOption.MustUse))
			{
				return true;
			}
			if(nextGCD == PvP_AirAnchor && PvP_Analysis.CurrentCharges > 1 && PvP_Analysis.CanUse(out act, CanUseOption.MustUse))
			{
				return true;
			}

			if(PvP_BishopAutoTurret.CanUse(out act, CanUseOption.MustUse))
			{
				if(NumberOfHostilesInRange == 0)
				{
					return false;
				}
				else if(TarOnMeTargetCount >= 1 || NumberOfHostilesInRange >= 1)
				{
					return true;
				}
				return false;
			}
			return base.EmergencyAbility(nextGCD, out act);
		}
		else
		{

			if(ShouldUseBurstMedicine(out act))
			{
				return true;
			}

			if(ShouldUseWildfire(out act))
			{
				return true;
			}

			if(ShouldUseBarrelStabilizer(out act))
			{
				return true;
			}

			if(ShouldUseRookAutoturret(nextGCD, out act) && NextAbilityToNextGCD > 0.6)
			{
				return true;
			}

			if(UseReassembleForAbilities(nextGCD, out act) && !IsOverheated && NextAbilityToNextGCD > 0.6)
			{
				// The 'act' variable is set to Reassemble by 'TryUseReassembleForAbilities' if it's usable.
				return true;
			}

			if(ShouldUseHypercharge(out act))
			{
				return true;
			}

			if(Detonator.CanUse(out act))
			{
				return true;
			}

			if(QueenOverdrive.CanUse(out act))
			{
				return true;
			}

			if(GaussRound.CanUse(out act, CanUseOption.MustUseEmpty) && NextAbilityToNextGCD > 0.6)
			{
				return true;
			}
			if(Ricochet.CanUse(out act, CanUseOption.MustUseEmpty | CanUseOption.OnLastAbility) && NextAbilityToNextGCD > 0.6)
			{
				return true;
			}
			return base.EmergencyAbility(nextGCD, out act);
		}
	}

	private bool ShouldUseBurstMedicine(out IAction act)
	{
		act = null; // Default to null if Burst Medicine cannot be used.

		if(Player.HasStatus(true, StatusID.Weakness))
		{
			return false;
		}

		// Check if the conditions for using Burst Medicine are met.
		if(Wildfire.CooldownRemaining <= 20 && CombatTime > 60 && NextAbilityToNextGCD > 1.2 && !Player.HasStatus(true, StatusID.Medicated)
			&& !TinctureOfDexterity6.IsCoolingDown && !TinctureOfDexterity7.IsCoolingDown && !TinctureOfDexterity8.IsCoolingDown && Drill.CooldownRemaining < 3)
		{
			// Attempt to use Burst Medicine.
			return UseBurstMedicine(out act, false);
		}

		// If the conditions are not met, return false.
		return false;
	}
	private bool UseReassembleForAbilities(IAction nextGCD, out IAction act)
	{
		act = null; // Default to null if Reassemble cannot be used.

		// Define the upper and lower bounds for the cooldown check.
		const float lowerThreshold = 1.0f; // 1 second lower bound
		const float upperThreshold = 2.5f; // 2.5 seconds upper bound

		// Check the cooldowns of Drill, AirAnchor, and ChainSaw.
		bool isDrillCooldownWithinRange = Drill.CooldownRemaining >= lowerThreshold && Drill.CooldownRemaining <= upperThreshold;
		bool isAirAnchorCooldownWithinRange = AirAnchor.CooldownRemaining >= lowerThreshold && AirAnchor.CooldownRemaining <= upperThreshold;
		bool isChainSawCooldownWithinRange = ChainSaw.CooldownRemaining >= lowerThreshold && ChainSaw.CooldownRemaining <= upperThreshold;

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
		if((isDrillCooldownWithinRange || isAirAnchorCooldownWithinRange || isChainSawCooldownWithinRange) || isNextGCDEligible)
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
		bool hasWildfire = Target.HasStatus(true, StatusID.Wildfire);

		// Check if the Wildfire cooldown is greater than 30 seconds.
		bool isWildfireCooldownLong = Wildfire.CooldownRemaining > 30;

		// Check if the Wildfire cooldown is less than 30 seconds.
		bool isWildfireCooldownShort = Wildfire.CooldownRemaining < 30;

		// Check if the Heat gauge is at least 50.
		bool isHeatAtLeast50 = Heat >= 50;

		// Check if the Heat gauge is between 90 and 100.
		bool isHeatFullAlmostFull = Heat <= 100 && Heat >= 90;

		// Check the cooldowns of your main abilities to see if they will be ready soon.
		bool isAnyMainAbilityReadySoon = Drill.CooldownRemaining < 8 ||
										 AirAnchor.CooldownRemaining < 8 ||
										 ChainSaw.CooldownRemaining < 8;

		// Check if the last ability used was Wildfire.
		bool isLastAbilityWildfire = IsLastAbility(ActionID.Wildfire);

		// Determine if Hypercharge should be used based on the presence of Wildfire status,
		// the Wildfire's cooldown, the current heat, not being overheated, and all main abilities not being ready soon,
		// with an exception if the last ability used was Wildfire.
		bool shouldUseHypercharge = (hasWildfire) ||
									(!isAnyMainAbilityReadySoon || isLastAbilityWildfire) &&
									((isWildfireCooldownLong && isHeatAtLeast50) ||
									 (isWildfireCooldownShort && isHeatFullAlmostFull));

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
		if(!Target.IsBoss() && !Target.IsDummy())
		{
			return false;
		}


		// Check the cooldowns of your main abilities.
		bool isDrillReadySoon = Drill.CooldownRemaining < 8;
		bool isAirAnchorReadySoon = AirAnchor.CooldownRemaining < 8;
		bool isChainSawReadySoon = ChainSaw.CooldownRemaining < 8;

		// Check if the combat time is less than 10 seconds and the last action was AirAnchor.
		bool isEarlyCombatAndLastActionAirAnchor = CombatTime < 15 && IsLastGCD(ActionID.AirAnchor);

		// Determine if Wildfire should be used based on the conditions provided.
		bool shouldUseWildfire = !isDrillReadySoon && !isAirAnchorReadySoon && !isChainSawReadySoon ||
								 isEarlyCombatAndLastActionAirAnchor;

		// If the conditions are met, attempt to use Wildfire.
		if(shouldUseWildfire)
		{
			return Wildfire.CanUse(out act, CanUseOption.OnLastAbility | CanUseOption.IgnoreClippingCheck);
		}

		// If the conditions are not met, return false.
		return false;
	}
	private bool ShouldUseBarrelStabilizer(out IAction act)
	{
		act = null; // Default to null if Barrel Stabilizer cannot be used.

		// Check if the target is not a boss or a dummy.
		if(!Target.IsBoss() || !Target.IsDummy())
		{
			return BarrelStabilizer.CanUse(out act, CanUseOption.MustUse);
		}

		// Check if the combat time is less than 10 seconds and the last action was Drill.
		bool isEarlyCombatAndLastActionDrill = CombatTime < 10 && IsLastAction(ActionID.Drill);

		// Check the relative cooldowns of Wildfire and Barrel Stabilizer.
		bool isWildfireCooldownLonger = Wildfire.CooldownRemaining > BarrelStabilizer.CooldownRemaining;
		bool isBarrelStabilizerCooldownLonger = BarrelStabilizer.CooldownRemaining > Wildfire.CooldownRemaining;

		// Determine if Barrel Stabilizer should be used based on the conditions provided.
		bool shouldUseBarrelStabilizer = isEarlyCombatAndLastActionDrill ||
										 isWildfireCooldownLonger ||
										 isBarrelStabilizerCooldownLonger;

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
		if(Target.IsBoss() || Target.IsDummy())
		{
			// If combat time is less than 80 seconds and last summon battery power was at least 50.
			if(CombatTime < 80 && Battery >= 50 && !RookAutoturret.IsCoolingDown)
			{
				return RookAutoturret.CanUse(out act, CanUseOption.MustUse);
			}
			// If combat time is more than 80 seconds and additional conditions are met, use Rook Autoturret.
			else if(CombatTime >= 80)
			{
				bool hasWildfireStatus = Target.HasStatus(true, StatusID.Wildfire);
				bool isWildfireCooldownLong = Wildfire.CooldownRemaining > 30;
				bool isBatteryHighEnough = Battery >= 80;
				bool isAirAnchorOrChainSawSoon = AirAnchor.CooldownRemaining < 3 || ChainSaw.CooldownRemaining < 3;
				bool isNextGCDCleanShot = nextGCD == CleanShot;

				if((isWildfireCooldownLong && isBatteryHighEnough) ||
					(hasWildfireStatus) ||
					(!hasWildfireStatus && Wildfire.CooldownRemaining < 30 && (isBatteryHighEnough && isAirAnchorOrChainSawSoon)) ||
					(isBatteryHighEnough && (isAirAnchorOrChainSawSoon || isNextGCDCleanShot)))
				{
					return RookAutoturret.CanUse(out act, CanUseOption.MustUse);
				}
			}
		}
		// Logic when the target is not a boss.
		else
		{
			// If the target's time to kill is 17 seconds or more and battery is full.
			bool isAirAnchorOrChainSawSoon = AirAnchor.CooldownRemaining < 3 || ChainSaw.CooldownRemaining < 3;
			if(Target.GetTimeToKill(false) >= 17 && Battery == 100)
			{
				// If the next GCD is Clean Shot or if Air Anchor or Chain Saw are about to be ready.
				if(nextGCD == CleanShot || isAirAnchorOrChainSawSoon)
				{
					return RookAutoturret.CanUse(out act, CanUseOption.MustUse);
				}
			}
			// If the target's time to kill is 17 seconds or more, use Rook Autoturret.
			else if(Target.GetTimeToKill(false) >= 17)
			{
				return RookAutoturret.CanUse(out act, CanUseOption.MustUse);
			}
		}

		// If none of the conditions are met, return false.
		return false;
	}

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

}
