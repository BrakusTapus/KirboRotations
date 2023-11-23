namespace KirboRotations.Ranged;

[BetaRotation]
[RotationDesc(ActionID.Wildfire)]
[LinkDescription("https://i.imgur.com/vekKW2k.jpg", "Delayed Tools")]
public class MCH_KirboPvP : MCH_Base
{
    #region Rotation Info
    public override CombatType Type => CombatType.PvP;
    public override string GameVersion => "6.51";
    public override string RotationName => "Kirbo's Machinist (PvP)";
    public override string Description => "Kirbo's Machinist for PvP";
#pragma warning disable CS0618 // Type or member is obsolete
    #endregion

    #region New PvP IBaseActions
    private static new IBaseAction PvP_MarksmansSpite { get; } = new BaseAction(ActionID.PvP_MarksmansSpite)
    {
        // Thank you Rabbs!
        ChoiceTarget = (Targets, mustUse) =>
        {
            Targets = Targets.Where(b => b.YalmDistanceX < 50 &&
            (b.CurrentHp /*+ b.CurrentMp * 6*/) < 40000 &&
            !b.HasStatus(false, (StatusID)1240, (StatusID)1308, (StatusID)2861, (StatusID)3255, (StatusID)3054, (StatusID)3054, (StatusID)3039, (StatusID)1312)).ToArray();

            if (Targets.Any())
            {
                return Targets.OrderBy(ObjectHelper.GetHealthRatio).Last();
            }
            return null;
        },
        ActionCheck = (BattleChara b, bool m) => LimitBreakLevel >= 1
    };
    private static new IBaseAction PvP_Drill { get; } = new BaseAction(ActionID.PvP_Drill)
    {
        StatusNeed = new StatusID[1] { StatusID.PvP_DrillPrimed },
        StatusProvide = new StatusID[1] { StatusID.PvP_BioblasterPrimed },
    };
    private static new IBaseAction PvP_Bioblaster { get; } = new BaseAction(ActionID.PvP_Bioblaster)
    {
        StatusNeed = new StatusID[1] { StatusID.PvP_BioblasterPrimed },
        StatusProvide = new StatusID[1] { StatusID.PvP_AirAnchorPrimed },
    };
    private static new IBaseAction PvP_AirAnchor { get; } = new BaseAction(ActionID.PvP_AirAnchor)
    {
        StatusNeed = new StatusID[1] { StatusID.PvP_AirAnchorPrimed },
        StatusProvide = new StatusID[1] { StatusID.PvP_ChainSawPrimed },
    };
    private static new IBaseAction PvP_ChainSaw { get; } = new BaseAction(ActionID.PvP_ChainSaw)
    {
        StatusNeed = new StatusID[1] { StatusID.PvP_ChainSawPrimed },
        StatusProvide = new StatusID[1] { StatusID.PvP_DrillPrimed },
    };
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
    private static new IBaseAction PvP_Analysis { get; } = new BaseAction(ActionID.PvP_Analysis, ActionOption.Friendly)
    {
        StatusProvide = new StatusID[1] { StatusID.PvP_Analysis },
        ActionCheck = (BattleChara b, bool m) => !CustomRotation.Player.HasStatus(true, StatusID.PvP_Analysis) && CustomRotation.HasHostilesInRange,
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
            ImGui.Text("Target: " + CurrentTarget.Name);
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
        bool TargetIsNotPlayer = Target != Player;
        bool hasGuard = Target.HasStatus(false, StatusID.PvP_Guard) && TargetIsNotPlayer;
        bool tarHasGuard = Target.HasStatus(false, StatusID.PvP_Guard) && TargetIsNotPlayer;
        bool hasChiten = Target.HasStatus(false, StatusID.PvP_Chiten) && TargetIsNotPlayer;
        bool hasHallowedGround = Target.HasStatus(false, StatusID.PvP_HallowedGround) && TargetIsNotPlayer;
        bool hasUndeadRedemption = Target.HasStatus(false, StatusID.PvP_UndeadRedemption) && TargetIsNotPlayer;

        // Config checks
        int marksmanRifleThreshold = Configs.GetInt("MarksmanRifleThreshold");
        int lowHPThreshold = Configs.GetInt("LowHPThreshold");
        bool guardCancel = Configs.GetBool("GuardCancel");
        bool preventActionWaste = Configs.GetBool("PreventActionWaste");
        bool safetyCheck = Configs.GetBool("SafetyCheck");
        bool drillOnGuard = Configs.GetBool("DrillOnGuard");

        if (Methods.InPvP())
        {
            if (guardCancel && Player.HasStatus(true, StatusID.PvP_Guard))
            {
                return false;
            }

            if (drillOnGuard && tarHasGuard)
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
            }

            if (safetyCheck && hasChiten)
            {
                return false;
            }

            if (preventActionWaste && (hasGuard || hasHallowedGround || hasUndeadRedemption))
            {
                return false;
            }

            if (!IsPvPOverheated)
            {
                //if (Target.CurrentHp <= marksmanRifleThreshold && TargetIsNotPlayer && !hasGuard && !hasHallowedGround && !hasUndeadRedemption
                //    && PvP_MarksmansSpite.CanUse(out act, CanUseOption.MustUse))
                //{
                //    return true;
                //}
                if (!hasHallowedGround && !hasUndeadRedemption && PvP_MarksmansSpite.CanUse(out act, CanUseOption.MustUse))
                {
                    return true;
                }
                if (Player.HasStatus(true, StatusID.PvP_DrillPrimed))
                {
                    if (PvP_Drill.CanUse(out act, CanUseOption.MustUseEmpty)) return true;
                }
                else if (Player.HasStatus(true, StatusID.PvP_BioblasterPrimed) && HostileTarget && HostileTarget.DistanceToPlayer() <= 12)
                {
                    if (PvP_Bioblaster.CanUse(out act, CanUseOption.MustUseEmpty)) return true;
                }
                else if (Player.HasStatus(true, StatusID.PvP_AirAnchorPrimed))
                {
                    if (PvP_AirAnchor.CanUse(out act, CanUseOption.MustUseEmpty)) return true;
                }
                else if (Player.HasStatus(true, StatusID.PvP_ChainSawPrimed))
                {
                    if (PvP_ChainSaw.CanUse(out act, CanUseOption.MustUseEmpty, 1)) return true;
                }
                if (PvP_Scattergun.CanUse(out act, CanUseOption.MustUseEmpty, 1) && HostileTarget.DistanceToPlayer() <= 12)
                {
                    return true;
                }
            }

            if (PvP_BlastCharge.CanUse(out act, CanUseOption.IgnoreCastCheck))
            {
                if (Player.CurrentHp <= lowHPThreshold && IsMoving && !IsPvPOverheated) // Maybe add InCombat as well
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
        bool TargetIsNotPlayer = CurrentTarget != Player;
        bool hasGuard = HostileTarget.HasStatus(false, StatusID.PvP_Guard) && TargetIsNotPlayer;
        bool tarHasGuard = Target.HasStatus(false, StatusID.PvP_Guard) && TargetIsNotPlayer;
        bool hasChiten = HostileTarget.HasStatus(false, StatusID.PvP_Chiten) && TargetIsNotPlayer;
        bool hasHallowedGround = HostileTarget.HasStatus(false, StatusID.PvP_HallowedGround) && TargetIsNotPlayer;
        bool hasUndeadRedemption = HostileTarget.HasStatus(false, StatusID.PvP_UndeadRedemption) && TargetIsNotPlayer;

        // Config checks
        bool guardCancel = Configs.GetBool("GuardCancel");
        bool preventActionWaste = Configs.GetBool("PreventActionWaste");
        bool safetyCheck = Configs.GetBool("SafetyCheck");
        bool drillOnGuard = Configs.GetBool("DrillOnGuard");

        if (Methods.InPvP())
        {
            if (guardCancel && Player.HasStatus(true, StatusID.PvP_Guard))
            {
                return false;
            }

            if (drillOnGuard && tarHasGuard)
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
            }

            if (safetyCheck && hasChiten)
            {
                return false;
            }

            if (preventActionWaste && (hasGuard || hasHallowedGround || hasUndeadRedemption))
            {
                return false;
            }

            if (!hasHallowedGround && !hasUndeadRedemption && PvP_MarksmansSpite.CanUse(out act, CanUseOption.MustUse))
            {
                return true;
            }

            if (IsPvPOverheated && !Player.WillStatusEnd(3.5f, true, StatusID.PvP_Overheated) && HostileTarget.DistanceToPlayer() < 20 && PvP_Wildfire.CanUse(out act, CanUseOption.MustUse))
            {
                return true;
            }

            if (PvP_BishopAutoTurret.CanUse(out act, CanUseOption.MustUse))
            {
                return true;
            }

            if (PvP_Analysis.CanUse(out act, CanUseOption.MustUseEmpty) && !Player.HasStatus(true, StatusID.PvP_Analysis) && NumberOfAllHostilesInRange > 0)
            {
                if (PvP_Analysis.CurrentCharges > 0 && (nextGCD == PvP_Drill || nextGCD == PvP_ChainSaw))
                {
                    return true;
                }
                else if (PvP_Analysis.CurrentCharges > 0 && Player.HasStatus(true, StatusID.PvP_DrillPrimed) && NumberOfAllHostilesInRange > 0)
                {
                    return true;
                }
                else if (PvP_Analysis.CurrentCharges > 0 && Player.HasStatus(true, StatusID.PvP_ChainSawPrimed) && NumberOfAllHostilesInRange > 0)
                {
                    return true;
                }
                else if (PvP_Analysis.CurrentCharges > 1 && Player.HasStatus(true, StatusID.PvP_BioblasterPrimed) && NumberOfAllHostilesInRange > 0)
                {
                    return true;
                }
                else if (PvP_Analysis.CurrentCharges > 1 && Player.HasStatus(true, StatusID.PvP_AirAnchorPrimed) && NumberOfAllHostilesInRange > 0)
                {
                    return true;
                }
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

        bool hasEnemiesInRange = HasHostilesInRange && CurrentTarget.CanSee();
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