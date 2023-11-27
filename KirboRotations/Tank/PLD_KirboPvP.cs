using static ImGuiNET.ImGui;
using static KirboRotations.Utility.KirboImGuiHelpers;
using static KirboRotations.Utility.Methods;

namespace KirboRotations.Ranged;

[BetaRotation]
[RotationDesc(ActionID.PvP_Phalanx)]
public class PLD_KirboPvP : PLD_Base
{
    #region Rotation Info
    public override CombatType Type => CombatType.PvP;
    public override string GameVersion => "6.51";
    public override string RotationName => "Kirbo's Paladin (PvP)";
    public override string Description => "Kirbo's Paladin for PvP";
    #endregion

    #region New PvP IBaseActions
    /// <summary>
    /// 1-2-3 combo for PLD
    /// </summary>
    private static IBaseAction PvP_RoyalAuthorityCombo { get; } = new BaseAction(ActionID.PvP_RoyalAuthorityCombo)
    {

    };

    /// <summary>
    /// Delivers an attack with a potency of 3,000.
    /// </summary>
    private static IBaseAction PvP_Fastblade { get; } = new BaseAction(ActionID.PvP_Fastblade)
    {

    };

    /// <summary>
    /// Delivers an attack with a potency of 4,000.
    /// </summary>
    private static IBaseAction PvP_Riotblade { get; } = new BaseAction(ActionID.PvP_Riotblade)
    {

    };

    /// <summary>
    /// Delivers an attack with a potency of 5,000.
    /// Additional Effect: Grants a stack of Sword Oath, up to a maximum of 3
    /// </summary>
    private static IBaseAction PvP_Royalauthority { get; } = new BaseAction(ActionID.PvP_Royalauthority)
    {

    };

    /// <summary>
    /// Deals unaspected damage with a potency of 8,000 to target and all enemies nearby it.
    /// Additional Effect: Afflicts target with Sacred Claim
    /// Sacred Claim Effect: Restores HP when successfully landing an attack on targets under this effect
    /// </summary>
    private static IBaseAction PvP_Confiteor { get; } = new BaseAction(ActionID.PvP_Confiteor)
    {
        // Sacred Claim ID = 3025
        TargetStatus = new StatusID[1] { (StatusID)3025 },
    };

    /// <summary>
    /// Delivers an attack with a potency of 4,000.
    /// Additional Effect: Stun
    /// Additional Effect: Grants a stack of Sword Oath, up to a maximum of 3
    /// </summary>
    private static IBaseAction PvP_Shieldbash { get; } = new BaseAction(ActionID.PvP_Shieldbash)
    {
        TargetStatus = new StatusID[1] { StatusID.PvP_Stun },
        // Sword Oath ID = 1991
        StatusProvide = new StatusID[1] { (StatusID)1991 },
    };

    /// <summary>
    /// Rushes target and delivers an attack with a potency of 2,000.
    /// Additional Effect: Grants a stack of Sword Oath, up to a maximum of 3
    /// </summary>
    private static IBaseAction PvP_Intervene { get; } = new BaseAction(ActionID.PvP_Intervene)
    {
        // Sword Oath ID = 1991
        StatusProvide = new StatusID[1] { (StatusID)1991 },
    };

    /// <summary>
    /// Rush to a target party member's side.
    /// Additional Effect: Take all damage intended for the targeted party member
    /// </summary>
    private static IBaseAction PvP_Guardian { get; } = new BaseAction(ActionID.PvP_Guardian, ActionOption.Friendly)
    {
        ActionCheck = (BattleChara b, bool m) => PvP_Guardian.Target.DistanceToPlayer() <= 10,
    };

    /// <summary>
    /// Grants Holy Sheltron and Knight's Resolve.
    /// Holy Sheltron Effect: Creates a barrier around self that absorbs damage equivalent to a heal of 12,000 potency
    /// Knight's Resolve Effect: Reduces damage taken by 15%
    /// </summary>
    private static IBaseAction PvP_HolySheltron { get; } = new BaseAction(ActionID.PvP_HolySheltron, ActionOption.Buff)
    {
        // Holy Sheltron ID = 3026
        // Knight's Resolve ID = 3188 (Also inflicts Heavy on target?)
        StatusProvide = new StatusID[2] { (StatusID)3026, (StatusID)3188 },
    };

    /// <summary>
    /// Delivers an attack with a potency of 8,000.
    /// Additional Effect: Restores own HP
    /// Cure Potency: 4,000
    /// </summary>
    private static IBaseAction PvP_Atonement { get; } = new BaseAction(ActionID.PvP_Atonement)
    {
        // Sword Oath ID = 1991
        StatusNeed = new StatusID[1] { (StatusID)1991 },
    };

    /// <summary>
    /// Deals unaspected damage with a potency of 6,000
    /// </summary>
    private static IBaseAction PvP_BladeOfFaith { get; } = new BaseAction(ActionID.PvP_BladeOfFaith)
    {
        // Blade of Faith Ready ID = 3250
        StatusNeed = new StatusID[1] { (StatusID)3250 },
        // Sacred Claim ID = 3025
        TargetStatus = new StatusID[1] { (StatusID)3025 },
    };

    /// <summary>
    /// Deals unaspected damage with a potency of 7,000
    /// </summary>
    private static IBaseAction PvP_BladeOfTruth { get; } = new BaseAction(ActionID.PvP_BladeOfTruth)
    {
        // Sacred Claim ID = 3025
        TargetStatus = new StatusID[1] { (StatusID)3025 },
    };

    /// <summary>
    /// Deals unaspected damage with a potency of 8,000
    /// </summary>
    private static IBaseAction PvP_BladeOfValor { get; } = new BaseAction(ActionID.PvP_BladeOfValor)
    {
        // Sacred Claim ID = 3025
        TargetStatus = new StatusID[1] { (StatusID)3025 },
    };

    /// <summary>
    /// Grants the effect of Hallowed Ground to self and Phalanx to nearby party members.
    /// Hallowed Ground Effect: Renders you impervious to most attacks
    /// Phalanx Effect: Reduces damage taken by 33%
    /// Additional Effect: Grants Blade of Faith Ready
    /// </summary>
    private static IBaseAction PvP_Phalanx { get; } = new BaseAction(ActionID.PvP_Phalanx, ActionOption.Friendly)
    {
        // HallowedGround ID = 1302
        // Blade Of Faith Ready ID = 3205
        StatusProvide = new StatusID[2] { (StatusID)1302, (StatusID)3250 },
        ActionCheck = (BattleChara t, bool m) => CustomRotation.LimitBreakLevel >= 1 && InCombat
    };
    #endregion

    #region Debug window stuff
    public override bool ShowStatus => true;
    public override void DisplayStatus()
    {
        try
        {
            Text("GCD Speed: " + WeaponTotal);
            Text("GCD remain: " + WeaponRemain);
            Separator();
            Spacing();

            if (Player != null)
            {
                ImGuiColoredText("Job: ", ClassJob.Abbreviation, new Vector4(0.68f, 0.85f, 1.0f, 1.0f)); // Light blue for the abbreviation
                Text($"Player.HealthRatio: {Player.GetHealthRatio() * 100:F2}%%");
                Text($"Player.CurrentHp: {Player.CurrentHp}");
                Separator();
                Spacing();
            }
            if (InPvP())
            {
                Text("HasInvulnv: " + HasInvulnv);
                Text("PvP_SwordOathStacks: " + PvP_SwordOathStacks);
                Text("PvP_Intervene CurrentCharges: " + PvP_Intervene.CurrentCharges);
                Separator();
                Spacing();
            }
            // Calculate the remaining vertical space in the window
            float remainingSpace = GetContentRegionAvail().Y - GetFrameHeightWithSpacing(); // Subtracting button height with spacing
            if (remainingSpace > 0)
            {
                SetCursorPosY(GetCursorPosY() + remainingSpace);
            }

            // Add a button for resetting rotation properties
            if (Button("Reset Rotation"))
            {
                //ResetRotationProperties();
            }
        }
        catch
        {
            Serilog.Log.Warning("Something wrong with DisplayStatus");
        }
    }
    #endregion

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
    #endregion

    #region Rotation Config
    protected override IRotationConfigSet CreateConfiguration() => base.CreateConfiguration()
        .SetInt(CombatType.PvP, "Recuperate", 45000, "HP Threshold for Recuperate", 1, 60000)
        .SetBool(CombatType.PvP, "GuardCancel", true, "Turn on if you want to FORCE RS to use nothing while in guard in PvP")
        .SetBool(CombatType.PvP, "PreventActionWaste", true, "Turn on to prevent using actions on targets with invulns\n(For example: DRK with Undead Redemption)")
        .SetBool(CombatType.PvP, "SafetyCheck", true, "Turn on to prevent using actions on targets that have a dangerous status\n(For example a SAM with Chiten)")
        .SetBool(CombatType.PvP, "LowHPNoAttacks", true, "Prevents the use of actions if player is moving with low HP\n(HP Threshold set in next option)")
        .SetInt(CombatType.PvP, "LowHPThreshold", 20000, "HP Threshold for the 'LowHPNoAttacks' option", 1, 60000)
        .SetBool(CombatType.PvP, "UseIntervene", false, "Let rotation use Intervene");
    #endregion

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

        if (Methods.InPvP())
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

            if (PvP_Phalanx.CanUse(out act, CanUseOption.MustUse))
            {
                return true;
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

            // 3
            if (PvP_Royalauthority.CanUse(out act, CanUseOption.MustUse))
            {
                return true;
            }
            // 2
            if (PvP_Riotblade.CanUse(out act, CanUseOption.MustUse))
            {
                return true;
            }
            // 1
            if (PvP_Fastblade.CanUse(out act, CanUseOption.MustUse))
            {
                return true;
            }
        }
        return base.GeneralGCD(out act);
    }
    #endregion

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
        bool useIntervene = Configs.GetBool("UseIntervene");

        if (guardCancel && playerHasGuard)
        {
            return false;
        }

        if (Player.CurrentHp <= RecuperateThreshold && Player.CurrentMp >= 2500 && PvP_Recuperate.CanUse(out act, CanUseOption.MustUseEmpty | CanUseOption.IgnoreClippingCheck))
        {
            if (guardCancel && playerHasGuard)
            {
                return false;
            }
            return true;
        }

        if (safetyCheck && targetIsNotPlayer && hasChiten)
        {
            return false;
        }

        if (preventActionWaste && targetIsNotPlayer && (targetHasGuard || hasHallowedGround || hasUndeadRedemption))
        {
            return false;
        }

        if (PvP_HolySheltron.CanUse(out act, CanUseOption.MustUse) && NumberOfHostilesInRange > 0)
        {
            return true;
        }

        if (PvP_Shieldbash.CanUse(out act, CanUseOption.MustUse) && targetIsNotPlayer && Target.DistanceToPlayer() <= 5)
        {
            return true;
        }

        // Maybe After Use follow Intervene up with a Shield Bash
        if (PvP_Intervene.CanUse(out act, CanUseOption.MustUseEmpty) && targetIsNotPlayer && Target.DistanceToPlayer() <= 20)
        {
            if (!useIntervene)
            {
                return false;
            }
            if (lowHPNoAttacks && Player.CurrentHp <= lowHPThreshold)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        // Auto use idea for Guardian. Use on partymember with low hp + guard OR + low hp AND check if Partymember is not using Standard Elixer
        //if (PvP_Guardian.CanUse(out act, CanUseOption.MustUse) && Target != Player && Target.DistanceToPlayer() <= 20)
        //{
        //    return true;
        //}

        return base.EmergencyAbility(nextGCD, out act);
    }
    #endregion

    #region Extra Helper Methods
    // Updates Status of other extra helper methods on every frame
    /*protected override void UpdateInfo()
    {
        HandleOpenerAvailability();
        ToolKitCheck();
        StateOfOpener();
    }*/

    // Checks if any major tool skill will almost come off CD (only at lvl 90), and sets "InBurst" to true if Player has Wildfire active
    /*private void ToolKitCheck()
    {
        bool WillHaveDrill = Drill.WillHaveOneCharge(5f);
        bool WillHaveAirAnchor = AirAnchor.WillHaveOneCharge(5f);
        bool WillHaveChainSaw = ChainSaw.WillHaveOneCharge(5f);
        if (Player.Level >= 90)
        {
            WillhaveTool = WillHaveDrill || WillHaveAirAnchor || WillHaveChainSaw;
        }

        InBurst = Player.HasStatus(true, StatusID.Wildfire);
    }*/

    // Controls various Opener properties depending on various conditions
    /*public void StateOfOpener()
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
    }*/

    // Used by Reset button to in Displaystatus
    /*private void ResetRotationProperties()
    {
        Openerstep = 0;
        OpenerHasFinished = false;
        OpenerHasFailed = false;
        OpenerActionsAvailable = false;
        OpenerInProgress = false;
        Serilog.Log.Debug($"Openerstep = {Openerstep}");
        Serilog.Log.Debug($"OpenerHasFinished = {OpenerHasFinished}");
        Serilog.Log.Debug($"OpenerHasFailed = {OpenerHasFailed}");
        Serilog.Log.Debug($"OpenerActionsAvailable = {OpenerActionsAvailable}");
        Serilog.Log.Debug($"OpenerInProgress = {OpenerInProgress}");
    }*/

    // Used to check OpenerAvailability
    /*public void HandleOpenerAvailability()
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

        // Future Opener conditions for ULTS
    }*/
    #endregion

}