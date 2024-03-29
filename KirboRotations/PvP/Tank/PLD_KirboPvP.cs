﻿using Dalamud.Game.ClientState.Objects.Types;
using KirboRotations.Extensions;
using Lumina.Excel.GeneratedSheets2;
using static KirboRotations.Extensions.BattleCharaEx;

namespace KirboRotations.PvP.Tank;

[BetaRotation]
[RotationDesc(ActionID.PvP_Phalanx)]
internal class PLD_KirboPvP : PLD_Base
{
    #region Rotation Info

    public override string GameVersion => "6.51";

    public override string RotationName => $"{USERNAME}'s {ClassJob.Abbreviation} [{Type}]";

    public override CombatType Type => CombatType.PvP;

    #endregion Rotation Info

    #region PvP IBaseActions

    /// <summary> Delivers an attack with a potency of 8,000. Additional Effect: Restores own HP Cure Potency: 4,000 </summary>
    private static IBaseAction PvP_Atonement { get; } = new BaseAction(ActionID.PvP_Atonement)
    {
        // Sword Oath ID = 1991
        StatusNeed = new StatusID[1] { (StatusID)1991 },
    };

    /// <summary> Deals unaspected damage with a potency of 6,000 </summary>
    private static IBaseAction PvP_BladeOfFaith { get; } = new BaseAction(ActionID.PvP_BladeOfFaith)
    {
        // Blade of Faith Ready ID = 3250
        StatusNeed = new StatusID[1] { (StatusID)3250 },

        // Sacred Claim ID = 3025
        TargetStatus = new StatusID[1] { (StatusID)3025 },
    };

    /// <summary> Deals unaspected damage with a potency of 7,000 </summary>
    private static IBaseAction PvP_BladeOfTruth { get; } = new BaseAction(ActionID.PvP_BladeOfTruth)
    {
        // Sacred Claim ID = 3025
        TargetStatus = new StatusID[1] { (StatusID)3025 },
    };

    /// <summary> Deals unaspected damage with a potency of 8,000 </summary>
    private static IBaseAction PvP_BladeOfValor { get; } = new BaseAction(ActionID.PvP_BladeOfValor)
    {
        // Sacred Claim ID = 3025
        TargetStatus = new StatusID[1] { (StatusID)3025 },
    };

    /// <summary>
    ///     Deals unaspected damage with a potency of 8,000 to target and all enemies nearby it. Additional Effect: Afflicts target with Sacred Claim
    ///     Sacred Claim Effect: Restores HP when successfully landing an attack on targets under this effect
    /// </summary>
    private static IBaseAction PvP_Confiteor { get; } = new BaseAction(ActionID.PvP_Confiteor)
    {
        // Sacred Claim ID = 3025
        TargetStatus = new StatusID[1] { (StatusID)3025 },
    };

    /// <summary> Delivers an attack with a potency of 3,000. </summary>
    private static IBaseAction PvP_Fastblade { get; } = new BaseAction(ActionID.PvP_Fastblade)
    {
    };

    /// <summary> Rush to a target party member's side. Additional Effect: Take all damage intended for the targeted party member </summary>
    private static IBaseAction PvP_Guardian { get; } = new BaseAction(ActionID.PvP_Guardian, ActionOption.Friendly)
    {
        ChoiceTarget = (Targets, mustUse) =>
        {
            // Filter party members with health below 50%
            var injuredPartyMembers = PartyMembers.Where(pm => pm.GetHealthRatio() < 0.5).ToList();

            // Select the party member with the lowest health ratio
            BattleChara bestTarget = injuredPartyMembers.OrderBy(pm => pm.GetHealthRatio()).FirstOrDefault();

            // Return the party member with the lowest health ratio below 50%, or null if no suitable target
            return bestTarget;
        },
        ActionCheck = (b, m) => PvP_Guardian.Target.DistanceToPlayer() <= 10 && Player.IsInCombat() && Player.GetHealthRatio() > 0.95,
    };

    /// <summary>
    ///     Grants Holy Sheltron and Knight's Resolve. Holy Sheltron Effect: Creates a barrier around self that absorbs damage equivalent to a heal of
    ///     12,000 potency Knight's Resolve Effect: Reduces damage taken by 15%
    /// </summary>
    private static IBaseAction PvP_HolySheltron { get; } = new BaseAction(ActionID.PvP_HolySheltron, ActionOption.Buff)
    {
        // Holy Sheltron ID = 3026 Knight's Resolve ID = 3188 (Also inflicts Heavy on target?)
        StatusProvide = new StatusID[2] { (StatusID)3026, (StatusID)3188 },
    };

    /// <summary>
    ///     Rushes target and delivers an attack with a potency of 2,000. Additional Effect: Grants a stack of Sword Oath, up to a maximum of 3
    /// </summary>
    private static IBaseAction PvP_Intervene { get; } = new BaseAction(ActionID.PvP_Intervene)
    {
        // Sword Oath ID = 1991
        StatusProvide = new StatusID[1] { (StatusID)1991 },
    };

    /// <summary>
    ///     Grants the effect of Hallowed Ground to self and Phalanx to nearby party members. Hallowed Ground Effect: Renders you impervious to most
    ///     attacks Phalanx Effect: Reduces damage taken by 33% Additional Effect: Grants Blade of Faith Ready
    /// </summary>
    private static IBaseAction PvP_Phalanx { get; } = new BaseAction(ActionID.PvP_Phalanx, ActionOption.Friendly)
    {
        // HallowedGround ID = 1302 Blade Of Faith Ready ID = 3205
        StatusProvide = new StatusID[2] { (StatusID)1302, (StatusID)3250 },
        ActionCheck = (t, m) => LimitBreakLevel >= 1 && Player.IsInCombat()
    };

    /// <summary> Delivers an attack with a potency of 4,000. </summary>
    private static IBaseAction PvP_Riotblade { get; } = new BaseAction(ActionID.PvP_Riotblade)
    {
    };

    /// <summary> Delivers an attack with a potency of 5,000. Additional Effect: Grants a stack of Sword Oath, up to a maximum of 3 </summary>
    private static IBaseAction PvP_Royalauthority { get; } = new BaseAction(ActionID.PvP_Royalauthority)
    {
    };

    /// <summary>
    ///     Delivers an attack with a potency of 4,000. Additional Effect: Stun Additional Effect: Grants a stack of Sword Oath, up to a maximum of 3
    /// </summary>
    private static IBaseAction PvP_Shieldbash { get; } = new BaseAction(ActionID.PvP_Shieldbash)
    {
        TargetStatus = new StatusID[1] { StatusID.PvP_Stun },

        // Sword Oath ID = 1991
        StatusProvide = new StatusID[1] { (StatusID)1991 },
        ActionCheck = (b, m) => !Target.HasStatus(false, StatusID.PvP_Stun),
    };

    #endregion PvP IBaseActions

    #region Action Related Properties

    private static byte PvP_SwordOathStacks
    {
        get
        {
            // Sword Oath ID = 1991
            byte pvp_swordoathstacks = Player.StatusStack(true, (StatusID)1991);
            return pvp_swordoathstacks == byte.MaxValue ? (byte)3 : pvp_swordoathstacks;
        }
    }

    private bool HasInvulnv => Player.HasStatus(true, StatusID.PvP_HallowedGround);

    #endregion Action Related Properties

    #region Rotation Config

    protected override IRotationConfigSet CreateConfiguration() => base.CreateConfiguration()
        .SetInt(CombatType.PvP, "Recuperate", 45000, "HP Threshold for Recuperate", 1, 60000)
        .SetBool(CombatType.PvP, "GuardCancel", true, "Turn on if you want to FORCE RS to use nothing while in guard in PvP")
        .SetBool(CombatType.PvP, "PreventActionWaste", true, "Turn on to prevent using actions on targets with invulns\n(For example: DRK with Undead Redemption)")
        .SetBool(CombatType.PvP, "SafetyCheck", true, "Turn on to prevent using actions on targets that have a dangerous status\n(For example a SAM with Chiten)")
        .SetInt(CombatType.PvP, "PhalanxEnemyThresHold", 1, "Amount of Enemies in range needed to use Phalanx\n(Ex: If Threshold is 1, Phalanx wil be used if there's 1 or more attackable targets in melee range)", 1, 50)
        .SetBool(CombatType.PvP, "LowHPNoAttacks", true, "Prevents the use of actions if player is moving with low HP\n(HP Threshold set in next option)")
        .SetInt(CombatType.PvP, "LowHPThreshold", 20000, "HP Threshold for the 'LowHPNoAttacks' option", 1, 60000)
        .SetBool(CombatType.PvP, "UseDash", true, "Let rotation use Intervene");

    #endregion Rotation Config

    #region GCD Logic

    protected override bool GeneralGCD(out IAction act)
    {
        act = null;

        // Status checks
        bool targetIsNotPlayer = Target != Player;
        bool playerHasGuard = Player.HasStatus(true, StatusID.PvP_Guard);
        bool targetHasGuard = Target.HasStatus(false, StatusID.PvP_Guard) && targetIsNotPlayer;
        bool hasChiten = Target.HasStatus(false, StatusID.PvP_Chiten) && targetIsNotPlayer;
        bool hasHallowedGround = Target.HasStatus(false, StatusID.PvP_HallowedGround) && targetIsNotPlayer;
        bool hasUndeadRedemption = Target.HasStatus(false, StatusID.PvP_UndeadRedemption) && targetIsNotPlayer;

        // Config checks
        bool guardCancel = Configs.GetBool("GuardCancel");
        bool preventActionWaste = Configs.GetBool("PreventActionWaste");
        bool safetyCheck = Configs.GetBool("SafetyCheck");
        bool lowHPNoAttacks = Configs.GetBool("LowHPNoAttacks");
        int lowHPThreshold = Configs.GetInt("LowHPThreshold");

        if (BattleCharaEx.InPvP())
        {
            if (guardCancel && playerHasGuard)
            {
                return false;
            }

            if (safetyCheck && targetIsNotPlayer && hasChiten)
            {
                return false;
            }

            if (preventActionWaste && targetIsNotPlayer && (targetHasGuard || hasHallowedGround || hasUndeadRedemption))
            {
                return false;
            }

            if (PvP_Phalanx.CanUse(out act))
            {
                double healthPercentage = Player.GetHealthRatio() * 100;
                if (healthPercentage < 100 && NumberOfAllHostilesInRange >= Configs.GetInt("PhalanxEnemyThresHold"))
                {
                    return true;
                }
            }

            if (PvP_BladeOfValor.CanUse(out act, CanUseOption.MustUse) && HostileTarget.DistanceToPlayer() <= 25)
            {
                return true;
            }

            if (PvP_BladeOfTruth.CanUse(out act, CanUseOption.MustUse) && HostileTarget.DistanceToPlayer() <= 25)
            {
                return true;
            }

            if (PvP_BladeOfFaith.CanUse(out act, CanUseOption.MustUse) && HostileTarget.DistanceToPlayer() <= 25)
            {
                return true;
            }

            if (PvP_Atonement.CanUse(out act, CanUseOption.MustUse) && HostileTarget.DistanceToPlayer() <= 5)
            {
                return true;
            }

            if (PvP_Confiteor.CanUse(out act, CanUseOption.MustUse) && HostileTarget.DistanceToPlayer() <= 25)
            {
                return true;
            }

            if (CanUsePvPComboSkill(out act))
            {
                return CanUsePvPComboSkill(out act);
            }
        }
        return base.GeneralGCD(out act);
    }

    // Combo Action
    private static bool CanUsePvPComboSkill(out IAction act)
    {
        act = null;

        // Check for PvP combo skills in descending order of priority
        if (PvP_Royalauthority.CanUse(out act, CanUseOption.MustUse))
        {
            return true;
        }
        if (PvP_Riotblade.CanUse(out act, CanUseOption.MustUse))
        {
            return true;
        }
        if (PvP_Fastblade.CanUse(out act, CanUseOption.MustUse))
        {
            return true;
        }

        // If none of the combo skills can be used, return false
        return false;
    }

    #endregion GCD Logic

    #region oGCD Logic

    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
    {
        act = null;

        // Status checks
        bool targetIsNotPlayer = Target != Player;
        bool playerHasGuard = Player.HasStatus(true, StatusID.PvP_Guard);
        bool targetHasGuard = Target.HasStatus(false, StatusID.PvP_Guard) && targetIsNotPlayer;
        bool hasChiten = Target.HasStatus(false, StatusID.PvP_Chiten) && targetIsNotPlayer;
        bool hasHallowedGround = Target.HasStatus(false, StatusID.PvP_HallowedGround) && targetIsNotPlayer;
        bool hasUndeadRedemption = Target.HasStatus(false, StatusID.PvP_UndeadRedemption) && targetIsNotPlayer;

        // Config checks
        int RecuperateThreshold = Configs.GetInt("Recuperate");
        bool guardCancel = Configs.GetBool("GuardCancel");
        bool preventActionWaste = Configs.GetBool("PreventActionWaste");
        bool safetyCheck = Configs.GetBool("SafetyCheck");
        bool lowHPNoAttacks = Configs.GetBool("LowHPNoAttacks");
        int lowHPThreshold = Configs.GetInt("LowHPThreshold");
        bool UseDash = Configs.GetBool("UseDash");
        double healthPercentage = Player.GetHealthRatio() * 100;

        // Combine the four conditions into one block
        if ((guardCancel && playerHasGuard) ||
            (Player.CurrentHp <= RecuperateThreshold && Player.CurrentMp >= 2500 && PvP_Recuperate.CanUse(out act, CanUseOption.MustUseEmpty | CanUseOption.IgnoreClippingCheck)) ||
            (safetyCheck && targetIsNotPlayer && hasChiten) ||
            (preventActionWaste && targetIsNotPlayer && (targetHasGuard || hasHallowedGround || hasUndeadRedemption)) ||
            (lowHPNoAttacks && Player.CurrentHp <= lowHPThreshold))
        {
            return false;
        }

        // provides dmg reduction and shield should be use pre-emptively
        if (PvP_HolySheltron.CanUse(out act, CanUseOption.MustUse))
        {
            int numberOfPartyMembers = PartyMembers.Count();
            if (NumberOfHostilesInMaxRange >= 5 && numberOfPartyMembers > 5 && !Player.IsInCombat())
            {
                return true;
            }
            if (HasHostilesInRange && NumberOfHostilesInRange > 1)
            {
                return true;
            }
            if (healthPercentage < 100 && Player.IsInCombat())
            {
                return true;
            }
        }

        if (PvP_Shieldbash.CanUse(out act, CanUseOption.MustUse) && targetIsNotPlayer)
        {
            if (IsLastAbility(ActionID.PvP_Intervene) && Target.GetHealthRatio() >= 0.70)
            {
                return true;
            }
            else if (Target.GetHealthRatio() <= 0.25)
            {
                return true;
            }
            else if (Target.IsCasting)
            {
                return false;
            }
        }

        // Maybe After Use follow Intervene up with a Shield Bash
        if (PvP_Intervene.CanUse(out act) && targetIsNotPlayer && UseDash)
        {
            if (!Player.IsInCombat())
            {
                if (Target.DistanceToPlayer() <= 20 && Target.DistanceToPlayer() >= 5 && healthPercentage >= 100)
                {
                    return true;
                }
            }
            else
            {
                if (PvP_Intervene.CurrentCharges == 2 && Target.DistanceToPlayer() <= 5)
                {
                    return true;
                }
                if (Target.DistanceToPlayer() <= 10 && Target.GetHealthRatio() <= 0.90 && PvP_Intervene.CurrentCharges > 1 && !PvP_Shieldbash.IsCoolingDown)
                {
                    return true;
                }
            }
        }

        // Auto use idea for Guardian. Use on partymember with low hp + guard OR + low hp AND check if Partymember is not using Standard Elixer
        if (PvP_Guardian.CanUse(out act, CanUseOption.MustUse) && Target != Player && Target.DistanceToPlayer() <= 20)
        {
            return true;
        }

        return base.EmergencyAbility(nextGCD, out act);
    }

    #endregion oGCD Logic

    #region Extra Helper Methods

    // WIP

    #endregion Extra Helper Methods
}
