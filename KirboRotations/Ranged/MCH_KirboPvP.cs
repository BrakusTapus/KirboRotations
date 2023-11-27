//using ExCSS;
//using Lumina.Excel.GeneratedSheets;
using static ImGuiNET.ImGui;
using static KirboRotations.Utility.KirboImGuiHelpers;
using static KirboRotations.Utility.Methods;

namespace KirboRotations.Ranged;

[BetaRotation]
[RotationDesc(ActionID.Wildfire)]
public class MCH_KirboPvP : MCH_Base
{
    #region Rotation Info
    public override CombatType Type => CombatType.PvP;
    public override string GameVersion => "6.51";
    public override string RotationName => "Kirbo's Machinist (PvP)";
    public override string Description => "Kirbo's Machinist for PvP";
    public static string ApiVersionRS => "3.5.7";
    #endregion

    #region IBaseActions
    // Enemies with our Wildfire Take highest priority, if none falls back to lowest HP in range
    private static new IBaseAction PvP_BlastCharge { get; } = new BaseAction(ActionID.PvP_BlastCharge)
    {
        ChoiceTarget = (Targets, mustUse) =>
        {
            // First, prioritize targets with the PvP_WildfireDebuff
            var targetWithWildfire = Targets.FirstOrDefault(b => b.HasStatus(true, StatusID.PvP_WildfireDebuff));
            if (targetWithWildfire != null)
            {
                return targetWithWildfire;
            }

            // If no target with PvP_WildfireDebuff, use existing logic
            Targets = Targets.Where(b => b.YalmDistanceX < 25 &&
            !b.HasStatus(false, (StatusID)1240, (StatusID)1308, (StatusID)2861, (StatusID)3255, (StatusID)3054, (StatusID)3054, (StatusID)3039, (StatusID)1312)).ToArray();
            if (Targets.Any())
            {
                return Targets.OrderBy(ObjectHelper.GetHealthRatio).First();
            }
            return null;
        },
        ActionCheck = (BattleChara b, bool m) => !CustomRotation.Player.HasStatus(true, StatusID.PvP_Guard),
    };
    private static new IBaseAction PvP_Drill { get; } = new BaseAction(ActionID.PvP_Drill)
    {
        ActionCheck = (BattleChara b, bool m) => !Player.HasStatus(true, StatusID.PvP_Overheated) && !CustomRotation.Player.HasStatus(true, StatusID.PvP_Guard),
        StatusNeed = new StatusID[1] { StatusID.PvP_DrillPrimed },
        StatusProvide = new StatusID[1] { StatusID.PvP_BioblasterPrimed },
    };
    // Will try and switch to a target when in 12yalm range
    private static new IBaseAction PvP_Bioblaster { get; } = new BaseAction(ActionID.PvP_Bioblaster)
    {
        ChoiceTarget = (Targets, mustUseEmpty) =>
        {
            Targets = Targets.Where(b => b.YalmDistanceX <= 12);
            if (Targets.Any())
            {
                return Targets.OrderBy(ObjectHelper.GetHealthRatio).First();
            }
            return null;
        },
        ActionCheck = (BattleChara b, bool m) => !Player.HasStatus(true, StatusID.PvP_Overheated) && !CustomRotation.Player.HasStatus(true, StatusID.PvP_Guard),
        StatusNeed = new StatusID[1] { StatusID.PvP_BioblasterPrimed },
        StatusProvide = new StatusID[1] { StatusID.PvP_AirAnchorPrimed },
    };
    private static new IBaseAction PvP_AirAnchor { get; } = new BaseAction(ActionID.PvP_AirAnchor)
    {
        ActionCheck = (BattleChara b, bool m) => !Player.HasStatus(true, StatusID.PvP_Overheated) && !CustomRotation.Player.HasStatus(true, StatusID.PvP_Guard),
        StatusNeed = new StatusID[1] { StatusID.PvP_AirAnchorPrimed },
        StatusProvide = new StatusID[1] { StatusID.PvP_ChainSawPrimed },
    };
    private static new IBaseAction PvP_ChainSaw { get; } = new BaseAction(ActionID.PvP_ChainSaw)
    {
        ActionCheck = (BattleChara b, bool m) => !Player.HasStatus(true, StatusID.PvP_Overheated) && !CustomRotation.Player.HasStatus(true, StatusID.PvP_Guard),
        StatusNeed = new StatusID[1] { StatusID.PvP_ChainSawPrimed },
        StatusProvide = new StatusID[1] { StatusID.PvP_DrillPrimed },
        ChoiceTarget = (Targets, mustUse) =>
        {
            // Filter targets based on health ratio
            var suitableTargets = Targets.Where(b => ObjectHelper.GetHealthRatio(b) <= 0.55).ToArray();

            if (suitableTargets.Any())
            {
                // Optionally, you can add more logic here to choose the best target
                // For now, it's selecting the first suitable target found
                return suitableTargets.First();
            }

            return null;
        }
    };
    private static new IBaseAction PvP_Scattergun { get; } = new BaseAction(ActionID.PvP_Scattergun)
    {
        ActionCheck = (BattleChara b, bool m) => !Player.HasStatus(true, StatusID.PvP_Overheated) && !CustomRotation.Player.HasStatus(true, StatusID.PvP_Guard),
    };
    // Picks a target who does not have either invulns nor any Mits picks lowest HP target
    private static new IBaseAction PvP_MarksmansSpite { get; } = new BaseAction(ActionID.PvP_MarksmansSpite, ActionOption.Attack | ActionOption.RealGCD)
    {
        ChoiceTarget = (Targets, mustUse) =>
        {
            // Define HP thresholds for each role
            const float rangedMagicalHpThreshold = 33000; // CurrentHp
            const float rangedPhysicalHpThreshold = 30000; // CurrentHp
            const float healerHpThreshold = 30000; // CurrentHp
            const float tankHpThreshold = 15000; // CurrentHp

            // Filter and prioritize targets based on role and HP
            var mageTarget = Targets.GetJobCategory(JobRole.RangedMagical)
            .Where(b => b.CurrentHp < rangedMagicalHpThreshold)
            .OrderBy(ObjectHelper.GetHealthRatio)
            .FirstOrDefault();
            if (mageTarget != null)
                return mageTarget;

            var rangeTarget = Targets.GetJobCategory(JobRole.RangedPhysical)
            .Where(b => b.CurrentHp < rangedPhysicalHpThreshold)
            .OrderBy(ObjectHelper.GetHealthRatio)
            .FirstOrDefault();
            if (rangeTarget != null)
                return rangeTarget;

            var healerTarget = Targets.GetJobCategory(JobRole.Healer)
            .Where(b => b.CurrentHp < healerHpThreshold)
            .OrderBy(ObjectHelper.GetHealthRatio)
            .FirstOrDefault();
            if (healerTarget != null)
                return healerTarget;

            var tankTarget = Targets.GetJobCategory(JobRole.Tank)
            .Where(b => b.CurrentHp < tankHpThreshold)
            .OrderBy(ObjectHelper.GetHealthRatio)
            .FirstOrDefault();
            return tankTarget ?? null;
        },
        ActionCheck = (BattleChara b, bool m) => LimitBreakLevel >= 1
    };

    // Will pick a Target with around 85% hp or less left
    private static new IBaseAction PvP_Wildfire { get; } = new BaseAction(ActionID.PvP_Wildfire, ActionOption.Attack)
    {
        ChoiceTarget = (Targets, mustUse) =>
        {
            // target the closest enemy with HP below a certain threshold (e.g., 85%)
            var fallbackTarget = Targets
            .Where(b => b.YalmDistanceX < 20 && b.GetHealthRatio() < 0.85)
            .OrderBy(b => b.YalmDistanceX)
            .FirstOrDefault();

            return fallbackTarget;
        },
        ActionCheck = (BattleChara b, bool m) => Player.HasStatus(true, StatusID.PvP_Overheated) && !CustomRotation.Player.HasStatus(true, StatusID.PvP_Guard),
    };

    private static new IBaseAction PvP_Analysis { get; } = new BaseAction(ActionID.PvP_Analysis, ActionOption.Friendly)
    {
        StatusProvide = new StatusID[1] { StatusID.PvP_Analysis },
        ActionCheck = (BattleChara b, bool m) => !CustomRotation.Player.HasStatus(true, StatusID.PvP_Analysis) && CustomRotation.HasHostilesInRange && PvP_Analysis.CurrentCharges > 0 && !CustomRotation.Player.HasStatus(true, StatusID.PvP_Guard),

    };
    // Will try and determine the best AoE target based on how many targets are present
    private static new IBaseAction PvP_BishopAutoTurret { get; } = new BaseAction(ActionID.PvP_BishopAutoTurret, ActionOption.Attack)
    {
        ActionCheck = (BattleChara b, bool m) => CustomRotation.HasHostilesInRange && !CustomRotation.Player.HasStatus(true, StatusID.PvP_Guard),
        ChoiceTarget = (Targets, mustUse) =>
        {
            // Combine party members and hostile targets into a single collection
            var combinedTargets = PartyMembers.Concat(HostileTargets);

            // Filter targets to those within a reasonable range (e.g., 15 yalms)
            combinedTargets = combinedTargets.Where(b => b.YalmDistanceX < 15).ToArray();

            // Select the target that has the highest number of nearby players
            BattleChara bestTarget = null;
            int maxNearbyPlayers = 0;
            foreach (var target in combinedTargets)
            {
                int nearbyPlayersCount = combinedTargets.Count(b => b.YalmDistanceX - target.YalmDistanceX < 5); // 5 yalms as an example radius
                if (nearbyPlayersCount > maxNearbyPlayers)
                {
                    maxNearbyPlayers = nearbyPlayersCount;
                    bestTarget = target;
                }
            }

            // Return the target that maximizes the AoE effect, or null if no suitable target
            return bestTarget;
        }
    };
    #endregion

    #region Debug window
    public override bool ShowStatus => true;
    public override void DisplayStatus()
    {
        try
        {
            Text("RS API Version: " + ApiVersionRS);
            Separator();
            Spacing();

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
                Text("IsPvPOverheated: " + IsPvPOverheated);
                Text("PvP_HeatStacks: " + PvP_HeatStacks);
                Text("PvP_Analysis CurrentCharges: " + PvP_Analysis.CurrentCharges);
                Separator();
                Spacing();
            }
            // Calculate the remaining vertical space in the window
            float remainingSpace = GetContentRegionAvail().Y - GetFrameHeightWithSpacing(); // Subtracting button height with spacing
            if (remainingSpace > 0)
            {
                SetCursorPosY(GetCursorPosY() + remainingSpace);
            }

            if (Button("Reset Rotation"))
            {
                /*ResetRotationProperties();
                Serilog.Log.Debug($"BurstActionsAvailable = {BurstActionsAvailable}");
                Serilog.Log.Debug($"BurstInProgress = {BurstInProgress}");
                Serilog.Log.Debug($"BurstIsFinished = {BurstIsFinished}");*/
            }
        }
        catch
        {
            Serilog.Log.Warning("Something wrong with DisplayStatus");
        }
    }
    #endregion

    #region Action Properties
    private bool BurstActionsAvailable { get; set; }
    private bool InBurst { get; set; }
    private bool BurstInProgress { get; set; }
    private bool BurstIsFinished { get; set; }
    private static byte PvP_HeatStacks
    {
        get
        {
            byte pvp_heatstacks = Player.StatusStack(true, StatusID.PvP_HeatStack);
            return pvp_heatstacks == byte.MaxValue ? (byte)5 : pvp_heatstacks;
        }
    }
    private bool IsPvPOverheated => Player.HasStatus(true, StatusID.PvP_Overheated);
    #endregion

    #region Rotation Config
    protected override IRotationConfigSet CreateConfiguration() => base.CreateConfiguration()
        .SetInt(CombatType.PvP, "Recuperate", 37500, "HP Threshold for Recuperate", 0, 52500)
        .SetInt(CombatType.PvP, "Guard", 27500, "HP Threshold for Guard", 0, 52500)
        .SetBool(CombatType.PvP, "AnalysisOnDrill", true, "Use Analysis on Drill")
        .SetBool(CombatType.PvP, "AnalysisOnAirAnchor", false, "Use Analysis on Air Anchor")
        .SetBool(CombatType.PvP, "AnalysisOnBioBlaster", false, "Use Analysis on BioBlaster")
        .SetBool(CombatType.PvP, "AnalysisOnChainsaw", true, "Use Analysis on ChainSaw")
        .SetBool(CombatType.PvP, "GuardCancel", true, "Turn on if you want to FORCE RS to use nothing while in guard in PvP")
        .SetBool(CombatType.PvP, "PreventActionWaste", true, "Turn on to prevent using actions on targets with invulns\n(For example: DRK with Undead Redemption)")
        .SetBool(CombatType.PvP, "SafetyCheck", true, "Turn on to prevent using actions on targets that have a dangerous status\n(For example a SAM with Chiten)")
        .SetBool(CombatType.PvP, "DrillOnGuard", true, "Try to use a Analysis buffed Drill on a Target with Guard\n(Thank you Const Mar for the suggestion!)")
        .SetBool(CombatType.PvP, "LowHPNoBlastCharge", true, "Prevents the use of Blast Charge if player is moving with low HP\n(HP Threshold set in next option)")
        .SetInt(CombatType.PvP, "LowHPThreshold", 20000, "HP Threshold for the 'LowHPNoBlastCharge' option", 0, 52500);
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
        int guardthreshold = Configs.GetInt("Guard");
        bool guardCancel = Configs.GetBool("GuardCancel");
        bool lowHPNoBlastCharge = Configs.GetBool("LowHPNoBlastCharge");
        int lowHPThreshold = Configs.GetInt("LowHPThreshold");
        bool preventActionWaste = Configs.GetBool("PreventActionWaste");
        bool safetyCheck = Configs.GetBool("SafetyCheck");
        bool drillOnGuard = Configs.GetBool("DrillOnGuard");

        // Should prevent any actions if the option 'guardCancel' is enabled and Player has the Guard buff up
        if (guardCancel && Player.HasStatus(true, StatusID.PvP_Guard))
        {
            return false;
        }

        // A Analysis buffed Drill should be used if target has Guard
        if (drillOnGuard && targetHasGuard && PvP_Drill.CanUse(out act, CanUseOption.MustUseEmpty))
        {    
            if (PvP_Analysis.CurrentCharges > 0 && !Player.HasStatus(true, StatusID.PvP_Analysis))
            {
                return PvP_Analysis.CanUse(out act, CanUseOption.MustUseEmpty);
            }
            if (Player.HasStatus(true, StatusID.PvP_Analysis))
            {
                return PvP_Drill.CanUse(out act, CanUseOption.MustUseEmpty);
            }
        }

        // At the moment just prevents attacks if a SAM has Chiten
        // Note: Try to add cast checks for various enemy LB's
        if (safetyCheck && hasChiten)
        {
            return false;
        }

        // Prevent Action Waste aims to not use vital skills on targets with invulns
        // Note: currently just blocks attacks, consider just blocking valueable skills and allowing Basic attacks
        if (preventActionWaste && (targetHasGuard || hasHallowedGround || hasUndeadRedemption))
        {
            return false;
        }

        // Marks Man should already be taking into invulns into account
        if (PvP_MarksmansSpite.CanUse(out act, CanUseOption.MustUse))
        {
            return true;
        }

        // When Drill can be used we first check if we can buff it with analysis
        // Note: Drill should always be buffed tbh
        if (PvP_Drill.CanUse(out act, CanUseOption.MustUseEmpty) && Target != Player && Target.GetHealthRatio() < 0.75)
        {
            if (PvP_Analysis.CurrentCharges > 0 && !Player.HasStatus(true, StatusID.PvP_Analysis))
            {
                return PvP_Analysis.CanUse(out act, CanUseOption.MustUseEmpty);
            }
            if (Player.HasStatus(true, StatusID.PvP_Analysis))
            {
                return true;
            }
            if (PvP_Analysis.CurrentCharges == 0)
            {
                return true;
            }
        }

        // Uses BioBlaster automatically when a Target is in range
        if (PvP_Bioblaster.CanUse(out act, CanUseOption.MustUseEmpty, 1))
        {
            return true;
        }
        
        // Air Anchor is used if Player is not overheated and available
        if (PvP_AirAnchor.CanUse(out act, CanUseOption.MustUseEmpty))
        {
            return true;
        }

        // Chain Saw is used if Player is not overheated and available
        // Note: Analysis will be used to buff Chain Saw if Target has around half of their HP
        if (PvP_ChainSaw.CanUse(out act, CanUseOption.MustUseEmpty, 1))
        {
            return true;
        }

        // Scattergun is used if Player is not overheated and available
        if (PvP_Scattergun.CanUse(out act, CanUseOption.MustUseEmpty, 1) && HostileTarget.DistanceToPlayer() <= 12)
        {
            return true;
        }

        // Blast Charge is used if available
        // Note: Stop Using Blast Charge if Player's HP is low + moving + not overheated (since our movement slows down a lot we do this to be able retreat)
        if (PvP_BlastCharge.CanUse(out act, CanUseOption.IgnoreCastCheck))
        {
            if (guardCancel && playerHasGuard)
            {
                return false;
            }
            if (Player.CurrentHp <= lowHPThreshold && lowHPNoBlastCharge && IsMoving && !IsPvPOverheated) // Maybe add InCombat as well
            {
                return false;
            }
            return true;
        }
        return base.GeneralGCD(out act);
    }
    #endregion

    #region oGCD Logic
    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
    {
        act = null;

        //var highPriority = availableCharas.Where(ObjectHelper.IsTopPriorityHostile);
        //if (highPriority.Any())
        //{
        //    availableCharas = highPriority;
        //}

        // Status checks
        bool targetIsNotPlayer = Target != Player;
        bool playerHasGuard = Player.HasStatus(true, StatusID.PvP_Guard);
        bool targetHasGuard = Target.HasStatus(false, StatusID.PvP_Guard) && targetIsNotPlayer;
        bool hasChiten = Target.HasStatus(false, StatusID.PvP_Chiten) && targetIsNotPlayer;
        bool hasHallowedGround = Target.HasStatus(false, StatusID.PvP_HallowedGround) && targetIsNotPlayer;
        bool hasUndeadRedemption = Target.HasStatus(false, StatusID.PvP_UndeadRedemption) && targetIsNotPlayer;

        // Config checks
        bool analysisOnDrill = Configs.GetBool("AnalysisOnDrill");
        bool analysisOnAirAnchor = Configs.GetBool("AnalysisOnAirAnchor");
        bool analysisOnBioBlaster = Configs.GetBool("AnalysisOnBioBlaster");
        bool analysisOnChainsaw = Configs.GetBool("AnalysisOnChainsaw");
        bool guardCancel = Configs.GetBool("GuardCancel");
        bool preventActionWaste = Configs.GetBool("PreventActionWaste");
        bool safetyCheck = Configs.GetBool("SafetyCheck");
        bool drillOnGuard = Configs.GetBool("DrillOnGuard");
        int recuperateThreshold = Configs.GetInt("Recuperate");
        int guardThreshold = Configs.GetInt("Guard");

        // Should prevent any actions if the option 'guardCancel' is enabled and Player has the Guard buff up
        if (guardCancel && playerHasGuard)
        {
            return false;
        }

        // Uses Recuperate to heal if HP falls below threshold  and have minimum required amount of MP to cast Recuperate
        if (Player.CurrentHp <= recuperateThreshold && Player.CurrentMp >= 2500 && PvP_Recuperate.CanUse(out act, CanUseOption.MustUseEmpty | CanUseOption.IgnoreClippingCheck))
        {
            if (!playerHasGuard)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        // Will use Analysis to buff Drill if Analysis has charges and Player has Drill Primed status
        if (!playerHasGuard && drillOnGuard && targetHasGuard && Player.HasStatus(true, StatusID.PvP_DrillPrimed) && (PvP_Drill.CanUse(out act, CanUseOption.MustUseEmpty) || PvP_Drill.WillHaveOneCharge(5)))
        {
            return PvP_Analysis.CanUse(out act, CanUseOption.MustUseEmpty);
        }

        /*if (drillOnGuard && targetHasGuard)
        {
            if (!Player.HasStatus(true, StatusID.PvP_DrillPrimed))
            {
                return false;
            }
            if (Player.HasStatus(true, StatusID.PvP_DrillPrimed) && PvP_Analysis.CurrentCharges == 0 && !Player.HasStatus(true, StatusID.PvP_Analysis))
            {
                return false;
            }
            if (Player.HasStatus(true, StatusID.PvP_DrillPrimed) && PvP_Analysis.CurrentCharges >= 1 && PvP_Analysis.CanUse(out act, CanUseOption.MustUseEmpty))
            {
                return true;
            }
        }*/

        // At the moment just prevents attacks if a SAM has Chiten
        // Note: Try to add cast checks for various enemy LB's
        if (safetyCheck && hasChiten)
        {
            return false;
        }

        // Prevent Action Waste aims to not use vital skills on targets with invulns
        // Note: currently just blocks attacks, consider just blocking valueable skills and allowing Basic attacks
        if (preventActionWaste && (targetHasGuard || hasHallowedGround || hasUndeadRedemption))
        {
            return false;
        }

        // Wildfire Should be used only right after getting the 5th Heat Stacks
        if (IsPvPOverheated && !Player.WillStatusEnd(3.5f, true, StatusID.PvP_Overheated) && PvP_Wildfire.CanUse(out act, CanUseOption.MustUse))
        {
            return true;
        }

        // Bishop Turret should be used off cooldown 
        // Note: Could prolly be improved using 'ChoiceTarget' in the IBaseAction
        if (PvP_BishopAutoTurret.CanUse(out act, CanUseOption.MustUse)) // Without MustUse, returns CastType 7 invalid
        {
            return true;
        }

        // Analysis should be used on any of the tools depending on which options are enabled
        if (PvP_Analysis.CanUse(out act, CanUseOption.MustUseEmpty) && NumberOfAllHostilesInRange > 0 && !IsPvPOverheated)
        {
            if (PvP_Analysis.CurrentCharges > 0 && Player.HasStatus(true, StatusID.PvP_DrillPrimed) && PvP_HeatStacks <= 4 && !Wildfire.WillHaveOneCharge(10))
            {
                return true;
            }
            if (analysisOnDrill && nextGCD == PvP_Drill)
            {
                return true;
            }
            else if (analysisOnChainsaw && nextGCD == PvP_ChainSaw && Target != Player && Target.GetHealthRatio() <= 0.55)
            {
                return true;
            }
            else if (analysisOnBioBlaster && nextGCD == PvP_Bioblaster)
            {
                return true;
            }
            else if (analysisOnAirAnchor && nextGCD == PvP_AirAnchor)
            {
                return true;
            }
        }
        return base.EmergencyAbility(nextGCD, out act);
    }
    #endregion

    #region PvP Helper Methods
    // Analysis Condition
    /*private bool ShouldUseAnalysis(out IAction act)
    {
        act = null;

        bool hasEnemiesInRange = HasHostilesInRange && Target.CanSee();
        bool drillPrimeAndHasAnalysis = Player.HasStatus(true, StatusID.PvP_DrillPrimed) && PvP_Analysis.CurrentCharges > 0;

        if (Player.HasStatus(true, StatusID.PvP_Analysis))
        {
            return false;
        }
        if (hasEnemiesInRange && drillPrimeAndHasAnalysis)
        {
            return PvP_Analysis.CanUse(out act, CanUseOption.MustUseEmpty);
        }
        return false;
    }*/
    #endregion

    #region Extra Helper Methods
    // Updates Status of other extra helper methods on every frame
    /*protected override void UpdateInfo()
    {
        HandleBurstAvailability();
        ToolKitCheck();
        StateOfRotation();
    }*/

    // Checks if any major tool skill will almost come off CD (only at lvl 90), and sets "InBurst" to true if Player has Wildfire active
    /*private void ToolKitCheck()
    {
        //bool WillHaveDrill = Drill.WillHaveOneCharge(5f);
        //bool WillHaveAirAnchor = AirAnchor.WillHaveOneCharge(5f);
        //bool WillHaveChainSaw = ChainSaw.WillHaveOneCharge(5f);

        //WillhaveTool = WillHaveDrill || WillHaveAirAnchor || WillHaveChainSaw;

        if (IsPvPOverheated)
        {
            InBurst = true;
        }
        else
        {
            InBurst = false;
        }

    }*/

    // Controls various Opener properties depending on various conditions
    /*public void StateOfRotation()
    {
        while (Player.IsDead)
        {
            BurstActionsAvailable = false;
            BurstInProgress = false;
        }
        if (!InCombat)
        {
            BurstIsFinished = false;
        }
        if (BurstIsFinished)
        {
            BurstInProgress = false;
        }
    }*/

    // Used by Reset button to in Displaystatus
    /*private void ResetRotationProperties()
    {
        BurstIsFinished = false;
        BurstActionsAvailable = false;
        BurstInProgress = false;
    }*/

    // Used to check OpenerAvailability
    /*public void HandleBurstAvailability()
    {
        bool InPvPArea = Methods.InPvP();
        bool HasWildfire = !PvP_Wildfire.IsCoolingDown;
        bool HasDrill = !PvP_Drill.IsCoolingDown && PvP_Drill.CurrentCharges >= 1;
        bool HasAnalysisReady = PvP_Analysis.CurrentCharges >= 1 || !PvP_Analysis.IsCoolingDown;
        //bool Openerstep0 = Openerstep == 0;
        BurstActionsAvailable = InPvPArea && HasAnalysisReady && HasWildfire && HasDrill && HasWildfire && InBurst;

        // Future Opener conditions for ULTS
    }*/
    #endregion

}