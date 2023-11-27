﻿namespace KirboRotations.Ranged;

[BetaRotation]
[RotationDesc(ActionID.Wildfire)]
[LinkDescription("https://i.imgur.com/vekKW2k.jpg", "Delayed Tools")]
public class MCH_KirboPvPCopy : MCH_Base
{
    #region Rotation Info
    public override CombatType Type => CombatType.PvP;
    public override string GameVersion => "6.51";
    public override string RotationName => "Kirbo's Machinist (PvP)";
    public override string Description => "Kirbo's Machinist for PvP";
    #endregion

    #region New PvP IBaseActions
    private static new IBaseAction PvP_BlastCharge { get; } = new BaseAction(ActionID.PvP_BlastCharge)
    {
        ChoiceTarget = (Targets, mustUse) =>
        {
            Targets = Targets.Where(b => b.YalmDistanceX < 25 &&
            !b.HasStatus(false, (StatusID)1240, (StatusID)1308, (StatusID)2861, (StatusID)3255, (StatusID)3054, (StatusID)3054, (StatusID)3039, (StatusID)1312)).ToArray();
            if (Targets.Any())
            {
                return Targets.OrderBy(ObjectHelper.GetHealthRatio).First();
            }
            return null;
        },
    };
    private static new IBaseAction PvP_Drill { get; } = new BaseAction(ActionID.PvP_Drill)
    {
        ActionCheck = (BattleChara b, bool m) => !Player.HasStatus(true, StatusID.PvP_Overheated),
        StatusNeed = new StatusID[1] { StatusID.PvP_DrillPrimed },
        StatusProvide = new StatusID[1] { StatusID.PvP_BioblasterPrimed },
    };
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
        ActionCheck = (BattleChara b, bool m) => !Player.HasStatus(true, StatusID.PvP_Overheated),
        StatusNeed = new StatusID[1] { StatusID.PvP_BioblasterPrimed },
        StatusProvide = new StatusID[1] { StatusID.PvP_AirAnchorPrimed },
    };
    private static new IBaseAction PvP_AirAnchor { get; } = new BaseAction(ActionID.PvP_AirAnchor)
    {
        ActionCheck = (BattleChara b, bool m) => !Player.HasStatus(true, StatusID.PvP_Overheated),
        StatusNeed = new StatusID[1] { StatusID.PvP_AirAnchorPrimed },
        StatusProvide = new StatusID[1] { StatusID.PvP_ChainSawPrimed },
    };
    private static new IBaseAction PvP_ChainSaw { get; } = new BaseAction(ActionID.PvP_ChainSaw)
    {
        ActionCheck = (BattleChara b, bool m) => !Player.HasStatus(true, StatusID.PvP_Overheated),
        StatusNeed = new StatusID[1] { StatusID.PvP_ChainSawPrimed },
        StatusProvide = new StatusID[1] { StatusID.PvP_DrillPrimed },
    };
    private static new IBaseAction PvP_Scattergun { get; } = new BaseAction(ActionID.PvP_Scattergun)
    {
        ActionCheck = (BattleChara b, bool m) => !Player.HasStatus(true, StatusID.PvP_Overheated),
    };
    private static new IBaseAction PvP_MarksmansSpite { get; } = new BaseAction(ActionID.PvP_MarksmansSpite, ActionOption.Attack | ActionOption.RealGCD)
    {
        // Thank you Rabbs!
        ChoiceTarget = (Targets, mustUse) =>
        {
            Targets = Targets.Where(b => b.YalmDistanceX < 50 &&
            (b.CurrentHp /*+ b.CurrentMp * 6*/) <= 34000 &&
            !b.HasStatus(false, StatusID.PvP_Chiten, StatusID.PvP_BlackestNight, (StatusID)2861, (StatusID)3255, (StatusID)3220, (StatusID)3156, StatusID.PvP_Guard, StatusID.PvP_UndeadRedemption, StatusID.PvP_HallowedGround, StatusID.PvP_sheltron, StatusID.PvP_EarthResonance, StatusID.PvP_Burst, (StatusID)2413)).ToArray();

            if (Targets.Any())
            {
                return Targets.OrderBy(ObjectHelper.GetHealthRatio).Last();
            }
            return null;
        },
        ActionCheck = (BattleChara b, bool m) => LimitBreakLevel >= 1
    };
    private static new IBaseAction PvP_Analysis { get; } = new BaseAction(ActionID.PvP_Analysis, ActionOption.Friendly)
    {
        StatusProvide = new StatusID[1] { StatusID.PvP_Analysis },
        ActionCheck = (BattleChara b, bool m) => !CustomRotation.Player.HasStatus(true, StatusID.PvP_Analysis) && CustomRotation.HasHostilesInRange && PvP_Analysis.CurrentCharges > 0,
    };
    #endregion

    #region Debug window stuff
    public override bool ShowStatus => true;
    public override void DisplayStatus()
    {
        try
        {
            ImGui.Separator();
            ImGui.Text("GCD remain: " + Drill);
            ImGui.Text("GCD remain: " + WeaponRemain);
            ImGui.Text("HeatStacks: " + PvP_HeatStacks);
            ImGui.Separator();
            ImGui.Spacing();
            //ImGui.Text($"Player.HealthRatio: {Player.GetHealthRatio() * 100:F2}%%");
            if (Target != Player)
            {
                ImGui.Text("Target: " + Target.Name);
            }

            ImGui.Text($"Player.HealthRatio: {Player.CurrentHp}");
            ImGui.Separator();
            ImGui.Spacing();

            ImGui.Text("IsPvPOverheated: " + IsPvPOverheated);
            ImGui.Text("PvP_HeatStacks: " + PvP_HeatStacks);
            ImGui.Text("PvP_Analysis CurrentCharges: " + PvP_Analysis.CurrentCharges);

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
    private bool InBurst { get; set; }
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
        //.SetBool(CombatType.PvP, "LBInPvP", true, "Use the LB in PvP when Target is killable by it")
        //.SetInt(CombatType.PvP, "MarksmanRifleThreshold", 32000, "Marksman Rifle HP Threshold\n(Doule click or hold CTRL and click to set value)", 0, 75000)
        .SetInt(CombatType.PvP, "Recuperate", 37500, "HP Threshold for Recuperate", 0, 52500)
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
        int marksmanRifleThreshold = Configs.GetInt("MarksmanRifleThreshold");
        bool guardCancel = Configs.GetBool("GuardCancel");
        bool lowHPNoBlastCharge = Configs.GetBool("LowHPNoBlastCharge");
        int lowHPThreshold = Configs.GetInt("LowHPThreshold");
        bool preventActionWaste = Configs.GetBool("PreventActionWaste");
        bool safetyCheck = Configs.GetBool("SafetyCheck");
        bool drillOnGuard = Configs.GetBool("DrillOnGuard");

        if (Methods.InPvP())
        {
            if (guardCancel && playerHasGuard)
            {
                return false;
            }

            if (Player.HasStatus(false, StatusID.PvP_Bind, StatusID.PvP_Stun) && PvP_Purify.CanUse(out act, CanUseOption.MustUseEmpty | CanUseOption.IgnoreTarget | CanUseOption.IgnoreClippingCheck))
            {
                return true;
            }

            if (drillOnGuard && targetHasGuard && PvP_Drill.CanUse(out act, CanUseOption.MustUseEmpty) && Player.HasStatus(true, StatusID.PvP_Analysis))
            {
                return true;
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
                if (Player.HasStatus(true, StatusID.PvP_DrillPrimed) && Player.HasStatus(true, StatusID.PvP_Analysis) && PvP_Drill.CanUse(out act, CanUseOption.MustUseEmpty))
                {
                    return true;
                }
            }*/

            if (safetyCheck && hasChiten)
            {
                return false;
            }

            if (preventActionWaste && (targetHasGuard || hasHallowedGround || hasUndeadRedemption))
            {
                return false;
            }

            //if (!IsPvPOverheated)
            //{
            //if (Target.CurrentHp <= marksmanRifleThreshold && TargetIsNotPlayer && !hasGuard && !hasHallowedGround && !hasUndeadRedemption
            //    && PvP_MarksmansSpite.CanUse(out act, CanUseOption.MustUse))
            //{
            //    return true;
            //}
            if (!hasHallowedGround && !hasUndeadRedemption && PvP_MarksmansSpite.CanUse(out act, CanUseOption.MustUse))
            {
                return true;
            }

            //if (PvP_Analysis.CurrentCharges > 0 && Player.HasStatus(true, StatusID.PvP_DrillPrimed) && PvP_HeatStacks <= 4 && !Wildfire.WillHaveOneCharge(10))
            //{
            //    return true;
            //}

            if (PvP_Drill.CanUse(out act, CanUseOption.MustUseEmpty) && Target != Player && Target.GetHealthRatio() <= 0.99)
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

            if (PvP_Bioblaster.CanUse(out act, CanUseOption.MustUseEmpty, 1)) return true;

            if (PvP_AirAnchor.CanUse(out act, CanUseOption.MustUseEmpty)) return true;

            if (PvP_ChainSaw.CanUse(out act, CanUseOption.MustUseEmpty, 1)) return true;

            if (PvP_Scattergun.CanUse(out act, CanUseOption.MustUseEmpty, 1) && HostileTarget.DistanceToPlayer() <= 12)
            {
                return true;
            }
            //}

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
        }
        return base.GeneralGCD(out act);
    }
    #endregion

    #region oGCD Logic
    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
    {
        act = null;

        #region PvP
        // Status checks
        bool TargetIsNotPlayer = Target != Player;
        bool playerHasGuard = Player.HasStatus(true, StatusID.PvP_Guard) && TargetIsNotPlayer;
        bool targetHasGuard = Target.HasStatus(false, StatusID.PvP_Guard) && TargetIsNotPlayer;
        bool hasChiten = Target.HasStatus(false, StatusID.PvP_Chiten) && TargetIsNotPlayer;
        bool hasHallowedGround = Target.HasStatus(false, StatusID.PvP_HallowedGround) && TargetIsNotPlayer;
        bool hasUndeadRedemption = Target.HasStatus(false, StatusID.PvP_UndeadRedemption) && TargetIsNotPlayer;

        // Config checks
        bool analysisOnDrill = Configs.GetBool("AnalysisOnDrill");
        bool analysisOnAirAnchor = Configs.GetBool("AnalysisOnAirAnchor");
        bool analysisOnBioBlaster = Configs.GetBool("AnalysisOnBioBlaster");
        bool analysisOnChainsaw = Configs.GetBool("AnalysisOnChainsaw");
        bool guardCancel = Configs.GetBool("GuardCancel");
        bool preventActionWaste = Configs.GetBool("PreventActionWaste");
        bool safetyCheck = Configs.GetBool("SafetyCheck");
        bool drillOnGuard = Configs.GetBool("DrillOnGuard");
        int RecuperateThreshold = Configs.GetInt("Recuperate");

        if (Methods.InPvP())
        {
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

            if (drillOnGuard && targetHasGuard && PvP_Analysis.CanUse(out act, CanUseOption.MustUseEmpty))
            {
                return true;
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

            if (safetyCheck && hasChiten)
            {
                return false;
            }

            if (preventActionWaste && Target != Player && Target != null && (targetHasGuard || hasHallowedGround || hasUndeadRedemption))
            {
                return false;
            }

            if (IsPvPOverheated && !Player.WillStatusEnd(3.5f, true, StatusID.PvP_Overheated) && HostileTarget.DistanceToPlayer() < 20 && PvP_Wildfire.CanUse(out act, CanUseOption.MustUse))
            {
                return true;
            }

            if (PvP_BishopAutoTurret.CanUse(out act, CanUseOption.MustUse) && Target != Player)
            {
                return true;
            }

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
                else if (analysisOnChainsaw && nextGCD == PvP_ChainSaw && Target != Player && Target.GetHealthRatio() <= 0.5)
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
                /*else if (analysisOnDrill && PvP_Analysis.CurrentCharges > 0 && Player.HasStatus(true, StatusID.PvP_DrillPrimed) && NumberOfAllHostilesInRange > 0)
                {
                    return true;
                }
                else if (analysisOnChainsaw && PvP_Analysis.CurrentCharges > 0 && Player.HasStatus(true, StatusID.PvP_ChainSawPrimed) && NumberOfAllHostilesInRange > 0)
                {
                    return true;
                }
                else if (analysisOnBioBlaster && PvP_Analysis.CurrentCharges > 1 && Player.HasStatus(true, StatusID.PvP_BioblasterPrimed) && NumberOfAllHostilesInRange > 0)
                {
                    return true;
                }
                else if (analysisOnAirAnchor && PvP_Analysis.CurrentCharges > 1 && Player.HasStatus(true, StatusID.PvP_AirAnchorPrimed) && NumberOfAllHostilesInRange > 0)
                {
                    return true;
                }*/
            }
        }
        #endregion

        return base.EmergencyAbility(nextGCD, out act);
    }
    #endregion

    #region PvP Helper Methods
    // Analysis Condition
    private bool ShouldUseAnalysis(out IAction act)
    {
        act = null;

        bool hasEnemiesInRange = HasHostilesInRange && Target != Player && Target.CanSee();
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
    }
    #endregion

    /*#region Extra Helper Methods
    // Updates Status of other extra helper methods on every frame
    protected override void UpdateInfo()
    {
        HandleOpenerAvailability();
        ToolKitCheck();
        StateOfOpener();
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

    // Controls various Opener properties depending on various conditions
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
        Serilog.Log.Debug($"Openerstep = {Openerstep}");
        Serilog.Log.Debug($"OpenerHasFinished = {OpenerHasFinished}");
        Serilog.Log.Debug($"OpenerHasFailed = {OpenerHasFailed}");
        Serilog.Log.Debug($"OpenerActionsAvailable = {OpenerActionsAvailable}");
        Serilog.Log.Debug($"OpenerInProgress = {OpenerInProgress}");
    }

    // Used to check OpenerAvailability
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

        // Future Opener conditions for ULTS
    }
    #endregion*/

}