using KirboRotations.Utility.ImGuiEx;
using static KirboRotations.Utility.Methods;
using static KirboRotations.Utility.StatusID_Buffs;

namespace KirboRotations.Ranged;

[RotationDesc(ActionID.BattleVoice)]
[SourceCode(Path = "main/KirboRotations/Ranged/BRD_Default.cs")]
public sealed class BRD_Kirbo : BRD_Base
{
    #region Rotation Info
    public override CombatType Type => CombatType.PvE;
    public override string GameVersion => "6.51";
    public override string RotationName => "Kirbo's Bard (PvE)";
    public override string Description => "Please make sure that the three song times add up to 120 seconds!";
    #endregion

    #region New PvE IBaseActions
    // Not yet implemented
    #endregion

    #region Debug window
    public override bool ShowStatus => true;
    public override void DisplayStatus()
    {
        try
        {
            try
            {
                ImGuiEx.TripleSpacing();
                ImGuiEx.CollapsingHeaderWithContent("General Info", () =>
                {
                    ImGui.Text($"Rotation: {RotationName} - v{RotationVersion}");
                    ImGuiEx.ImGuiColoredText("Rotation  Job: ", ClassJob.Abbreviation, EColor.GreenBright);
                    ImGuiEx.SeperatorWithSpacing();
                    ImGui.Text($"Player Name: {Player.Name}");
                    ImGui.Text($"Player HP: {Player.GetHealthRatio() * 100:F2}%%");
                    ImGuiEx.ImGuiColoredText("Player MP: ", (int)Player.CurrentMp, EColor.Blue);
                    ImGuiEx.SeperatorWithSpacing();
                    ImGui.Text("In Combat: " + InCombat);
                    // ... other general info ...
                });
                ImGuiEx.Tooltip("Displays General information like:\n-Rotation Name\n-Player's Health\n-InCombat Status");
            }
            catch { Serilog.Log.Error($"{ErrorDebug} - General Info"); }

            ImGuiEx.TripleSpacing();

            try
            {
                ImGuiEx.CollapsingHeaderWithContent("Rotation Status", () =>
                {
                    ImGui.Text("Openerstep: " + Methods.OpenerStep);
                    ImGui.Text("OpenerActionsAvailable: " + Methods.OpenerActionsAvailable);
                    ImGui.Text("OpenerInProgress: " + Methods.OpenerInProgress);
                    ImGui.Text("OpenerHasFailed: " + Methods.OpenerHasFailed);
                    ImGui.Text("OpenerHasFinished: " + Methods.OpenerHasFinished);
                    ImGui.Text("_openerFlag: " + Methods._openerFlag);
                    
                    // ... other rotation status ...
                });
                ImGuiEx.Tooltip("Displays Rotation information like:\n-Selected Rotation\n-Opener Status");
            }
            catch { Serilog.Log.Error($"{ErrorDebug} - Rotation Status"); }

            ImGuiEx.TripleSpacing();

            try
            {
                ImGuiEx.CollapsingHeaderWithContent("Burst Status", () =>
                {
                    ImGui.Text("BurstStep: " + Methods.BurstStep);
                    ImGui.Text("BurstActionsAvailable: " + Methods.BurstActionsAvailable);
                    ImGui.Text("BurstInProgress: " + Methods.BurstInProgress);
                    ImGui.Text("BurstHasFailed: " + Methods.BurstHasFailed);
                    ImGui.Text("BurstHasFinished: " + Methods.BurstHasFinished);
                    // ... other Burst status ...
                });
                ImGuiEx.Tooltip("Displays Burst information like:\n-Burst Available\n-Burst HasFailed");
            }
            catch { Serilog.Log.Error($"{ErrorDebug} - Burst Status"); }

            ImGuiEx.TripleSpacing();

            try
            {
                ImGuiEx.CollapsingHeaderWithContent("Action Details", () =>
                {
                    if (ImGui.BeginTable("actionTable", 2,ImGuiTableFlags.BordersOuter))
                    {
                        ImGui.TableSetupColumn("Description"); ImGui.TableSetupColumn("Value"); ImGui.TableHeadersRow();
                        ImGui.TableNextRow();
                        ImGui.TableNextColumn(); ImGui.Text("GCD Remain:"); ImGui.TableNextColumn(); ImGui.Text(WeaponRemain.ToString());
                        ImGui.TableNextRow();

                        ImGui.TableNextColumn(); ImGui.Text($"TimeSinceLastAction:"); ImGui.TableNextColumn(); ImGui.Text(TimeSinceLastAction.TotalSeconds.ToString());

                        // Add more rows as needed...

                        ImGui.EndTable();
                    }
                });
                ImGuiEx.Tooltip("Displays action information like:\n-LastAction Used\n-LastGCD Used\n-LastAbility Used");
            }
            catch { Serilog.Log.Error($"{ErrorDebug} - Action Details"); }

            ImGuiEx.TripleSpacing();

            try
            {
                float remainingSpace = ImGuiEx.CalculateRemainingVerticalSpace();
                ImGui.Text($"Remaining Vertical Space: {remainingSpace} pixels");

                // Calculate the remaining vertical space in the window
                // Subtracting button height with spacing
                float remainingSpace2 = ImGui.GetContentRegionAvail().Y - ImGui.GetFrameHeightWithSpacing();
                if (remainingSpace2 > 0)
                { ImGui.SetCursorPosY(ImGui.GetCursorPosY() + remainingSpace2); }
                ImGuiEx.DisplayResetButton("Reset Properties");
            }
            catch { Serilog.Log.Error($"{ErrorDebug} - Extra + Reset Button"); }

        }
        catch { Serilog.Log.Warning($"{ErrorDebug} - DisplayStatus"); }
    }
    #endregion

    #region Rotation Config
    protected override IRotationConfigSet CreateConfiguration() => base.CreateConfiguration()
            .SetBool(CombatType.PvE, "BindWAND", false, @"Use Raging Strikes on ""Wanderer's Minuet""")
            .SetCombo(CombatType.PvE, "FirstSong", 0, "First Song", "Wanderer's Minuet", "Mage's Ballad", "Army's Paeon")
            .SetFloat(RotationSolver.Basic.Configuration.ConfigUnitType.Seconds, CombatType.PvE, "WANDTime", 43, "Wanderer's Minuet Uptime", min: 0, max: 45, speed: 1)
            .SetFloat(RotationSolver.Basic.Configuration.ConfigUnitType.Seconds, CombatType.PvE, "MAGETime", 34, "Mage's Ballad Uptime", min: 0, max: 45, speed: 1)
            .SetFloat(RotationSolver.Basic.Configuration.ConfigUnitType.Seconds, CombatType.PvE, "ARMYTime", 43, "Army's Paeon Uptime", min: 0, max: 45, speed: 1)
            .SetBool(CombatType.PvE, "2and8minTincture", false, "Will use Tincture for the 2min and 8min burst\n(only use if fight actually takes more then 8min and 30seconds)");

    private bool BindWAND => Configs.GetBool("BindWAND") && WanderersMinuet.EnoughLevel;
    private int FirstSong => Configs.GetCombo("FirstSong");
    private float WANDRemainTime => 45 - Configs.GetFloat("WANDTime");
    private float MAGERemainTime => 45 - Configs.GetFloat("MAGETime");
    private float ARMYRemainTime => 45 - Configs.GetFloat("ARMYTime");
    #endregion

    #region Countdown Logic
    protected override IAction CountDownAction(float remainTime)
    {
        if (remainTime <= WindBite.AnimationLockTime && WindBite.CanUse(out _))
        {
            Methods.OpenerInProgress = true;
            return WindBite;
        }

        // Use Tincture if Tincture use is enabled and the countdown time is less or equal to GCD+Tincture animationlock (1.8s)
        IAction act0;
        if (remainTime <= WindBite.AnimationLockTime + TinctureOfDexterity8.AnimationLockTime && UseBurstMedicine(out act0, false))
        {
            return act0;
        }
        return base.CountDownAction(remainTime);
    }
    #endregion

    #region Opener Logic
    private bool Opener(out IAction act)
    {
        act = default(IAction);
        while (Methods.OpenerInProgress)
        {
            if (!Methods._openerFlag && (Player.IsDead) || (TimeSinceLastAction.TotalSeconds > 3.0))
            {
                Methods.OpenerHasFailed = true;
                Methods._openerFlag = true;
            }
            switch (Methods.OpenerStep)
            {
                case 0:
                    return Methods.OpenerController(IsLastGCD(true, WindBite), WindBite.CanUse(out act, CanUseOption.MustUse));
                case 1:
                    return Methods.OpenerController(IsLastAbility(false, WanderersMinuet), WanderersMinuet.CanUse(out act, CanUseOption.MustUseEmpty));
                case 2:
                    return Methods.OpenerController(IsLastAbility(false, RagingStrikes), RagingStrikes.CanUse(out act, CanUseOption.OnLastAbility));
                case 3:
                    return Methods.OpenerController(IsLastGCD(true, VenomousBite), VenomousBite.CanUse(out act, CanUseOption.MustUse));
                case 4:
                    return Methods.OpenerController(IsLastAbility(false, EmpyrealArrow), EmpyrealArrow.CanUse(out act, CanUseOption.MustUseEmpty));
                case 5:
                    return Methods.OpenerController(IsLastAbility(true, Bloodletter), Bloodletter.CanUse(out act, CanUseOption.MustUseEmpty | CanUseOption.OnLastAbility));
                case 6:
                    return Methods.OpenerController(IsLastGCD(true, HeavyShoot), HeavyShoot.CanUse(out act, CanUseOption.MustUse));
                case 7:
                    return Methods.OpenerController(IsLastAbility(false, RadiantFinale), RadiantFinale.CanUse(out act, CanUseOption.MustUse));
                case 8:
                    return Methods.OpenerController(IsLastAbility(false, BattleVoice), BattleVoice.CanUse(out act, CanUseOption.OnLastAbility));
                case 9:
                    return Methods.OpenerController(IsLastGCD(true, HeavyShoot), HeavyShoot.CanUse(out act, CanUseOption.MustUse));
                case 10:
                    return Methods.OpenerController(IsLastAbility(false, Barrage), Barrage.CanUse(out act, CanUseOption.MustUseEmpty));
                case 11:
                    return Methods.OpenerController(IsLastGCD(true, StraitShoot), StraitShoot.CanUse(out act, CanUseOption.MustUse));
                case 12:
                    return Methods.OpenerController(IsLastAbility(false, Sidewinder), Sidewinder.CanUse(out act, (CanUseOption)17));
                case 13:
                    return Methods.OpenerController(IsLastGCD(true, HeavyShoot), HeavyShoot.CanUse(out act, CanUseOption.MustUse));
                case 14:
                    Methods.OpenerHasFinished = true;
                    Methods.OpenerInProgress = false;
                    Serilog.Log.Information($"{OpenerComplete} - BRD Opener");
                    // Finished Opener
                    break;
            }
        }
        act = null;
        return false;
    }
    #endregion

    #region GCD Logic
    protected override bool GeneralGCD(out IAction act)
    {
        act = null;

        if (Player.HasStatus(true, (StatusID)Transcendent))
        {
            // Logic when the player is recently revived
            return false;
        }

        if (Methods.OpenerInProgress)
        {
            return Opener(out act);
        }
        if (!Methods.OpenerInProgress)
        {
            if (IronJaws.CanUse(out act))
            {
                return true;
            }

            if (IronJaws.CanUse(out act, CanUseOption.MustUse) && IronJaws.Target.WillStatusEnd(30, true, IronJaws.TargetStatus))
            {
                if (Player.HasStatus(true, StatusID.RagingStrikes) && Player.WillStatusEndGCD(1, 0, true, StatusID.RagingStrikes))
                {


                    return true;
                }
            }

            if (CanUseApexArrow(out act))
            {
                return true;
            }

            if (BlastArrow.CanUse(out act, CanUseOption.MustUse))
            {
                if (!Player.HasStatus(true, StatusID.RagingStrikes))
                {
                    return true;
                }
                if (Player.HasStatus(true, StatusID.RagingStrikes) && Barrage.IsCoolingDown)
                {
                    return true;
                }
            }

            if (ShadowBite.CanUse(out act))
            {
                return true;
            }

            if (QuickNock.CanUse(out act))
            {
                return true;
            }

            if (WindBite.CanUse(out act))
            {
                return true;
            }

            if (VenomousBite.CanUse(out act))
            {
                return true;
            }

            if (StraitShoot.CanUse(out act))
            {
                return true;
            }

            if (HeavyShoot.CanUse(out act))
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
        if (ShouldUseBurstMedicine(out act))
        {
            return true;
        }
        if (Methods.OpenerInProgress)
        {
            return Opener(out act);
        }
        if (!Methods.OpenerInProgress)
        {
            if (nextGCD.IsTheSameTo(true, StraitShoot, VenomousBite, WindBite, IronJaws))
            {
                return base.EmergencyAbility(nextGCD, out act);
            }
            else if ((!RagingStrikes.EnoughLevel || Player.HasStatus(true, StatusID.RagingStrikes)) && (!BattleVoice.EnoughLevel || Player.HasStatus(true, StatusID.BattleVoice)))
            {
                if ((EmpyrealArrow.IsCoolingDown && !EmpyrealArrow.WillHaveOneChargeGCD(1) || !EmpyrealArrow.EnoughLevel) && Repertoire != 3)
                {
                    if (!Player.HasStatus(true, StatusID.StraightShotReady) && Barrage.CanUse(out act))
                    {
                        return true;
                    }
                }
            }
        }
        return base.EmergencyAbility(nextGCD, out act);
    }

    protected override bool AttackAbility(out IAction act)
    {
        act = null;
        if (Methods.OpenerInProgress)
        {
            return Opener(out act);
        }
        if (!Methods.OpenerInProgress)
        {
            if (Song == Song.NONE)
            {
                if (FirstSong == 0 && WanderersMinuet.CanUse(out act))
                {
                    return true;
                }
                if (FirstSong == 1 && MagesBallad.CanUse(out act))
                {
                    return true;
                }
                if (FirstSong == 2 && ArmysPaeon.CanUse(out act))
                {
                    return true;
                }
                if (WanderersMinuet.CanUse(out act))
                {
                    return true;
                }
                if (MagesBallad.CanUse(out act))
                {
                    return true;
                }
                if (ArmysPaeon.CanUse(out act))
                {
                    return true;
                }
            }

            if (IsBurst && Song != Song.NONE && MagesBallad.EnoughLevel)
            {
                if (RagingStrikes.CanUse(out act))
                {
                    if (BindWAND && Song == Song.WANDERER && WanderersMinuet.EnoughLevel)
                    {
                        return true;
                    }
                    if (!BindWAND)
                    {
                        return true;
                    }
                }

                if (RadiantFinale.CanUse(out act, CanUseOption.MustUse))
                {
                    if (Player.HasStatus(true, StatusID.RagingStrikes) && RagingStrikes.ElapsedOneChargeAfterGCD(1))
                    {
                        return true;
                    }
                }

                if (BattleVoice.CanUse(out act, CanUseOption.MustUse))
                {
                    if (IsLastAction(true, RadiantFinale))
                    {
                        return true;
                    }

                    if (Player.HasStatus(true, StatusID.RagingStrikes) && RagingStrikes.ElapsedOneChargeAfterGCD(1))
                    {
                        return true;
                    }
                }
            }

            if (RadiantFinale.EnoughLevel && RadiantFinale.IsCoolingDown && BattleVoice.EnoughLevel && !BattleVoice.IsCoolingDown)
            {
                return false;
            }

            if (WanderersMinuet.CanUse(out act, CanUseOption.OnLastAbility))
            {
                if (SongEndAfter(ARMYRemainTime) && (Song != Song.NONE || Player.HasStatus(true, StatusID.ArmyEthos)))
                {
                    return true;
                }
            }

            if (Song != Song.NONE && EmpyrealArrow.CanUse(out act))
            {


                return true;
            }

            if (PitchPerfect.CanUse(out act))
            {
                if (SongEndAfter(3) && Repertoire > 0)
                {
                    return true;
                }

                if (Repertoire == 3)
                {


                    return true;
                }

                if (Repertoire == 2 && EmpyrealArrow.WillHaveOneChargeGCD(1) && NextAbilityToNextGCD < PitchPerfect.AnimationLockTime + Ping)
                {


                    return true;
                }

                if (Repertoire == 2 && EmpyrealArrow.WillHaveOneChargeGCD() && NextAbilityToNextGCD > PitchPerfect.AnimationLockTime + Ping)
                {


                    return true;
                }
            }

            if (MagesBallad.CanUse(out act))
            {
                if (Song == Song.WANDERER && SongEndAfter(WANDRemainTime) && Repertoire == 0)
                {


                    return true;
                }
                if (Song == Song.ARMY && SongEndAfterGCD(2) && WanderersMinuet.IsCoolingDown)
                {


                    return true;
                }
            }

            if (ArmysPaeon.CanUse(out act))
            {
                if (WanderersMinuet.EnoughLevel && SongEndAfter(MAGERemainTime) && Song == Song.MAGE)
                {


                    return true;
                }
                if (WanderersMinuet.EnoughLevel && SongEndAfter(2) && MagesBallad.IsCoolingDown && Song == Song.WANDERER)
                {


                    return true;
                }
                if (!WanderersMinuet.EnoughLevel && SongEndAfter(2))
                {


                    return true;
                }
            }

            if (Sidewinder.CanUse(out act))
            {
                if (Player.HasStatus(true, StatusID.BattleVoice) && (Player.HasStatus(true, StatusID.RadiantFinale) || !RadiantFinale.EnoughLevel))
                {


                    return true;
                }

                if (!BattleVoice.WillHaveOneCharge(10) && !RadiantFinale.WillHaveOneCharge(10))
                {


                    return true;
                }

                if (RagingStrikes.IsCoolingDown && !Player.HasStatus(true, StatusID.RagingStrikes))
                {


                    return true;
                }
            }

            if (EmpyrealArrow.IsCoolingDown || !EmpyrealArrow.WillHaveOneChargeGCD() || Repertoire != 3 || !EmpyrealArrow.EnoughLevel)
            {
                if (RainOfDeath.CanUse(out act, CanUseOption.EmptyOrSkipCombo))
                {


                    return true;
                }

                if (Bloodletter.CanUse(out act, CanUseOption.EmptyOrSkipCombo))
                {


                    return true;
                }
            }
        }

        return base.AttackAbility(out act);
    }
    #endregion

    #region Helper Methods
    private static bool CanUseApexArrow(out IAction act)
    {
        if (!ApexArrow.CanUse(out act, CanUseOption.MustUse)) return false;

        if (QuickNock.CanUse(out _) && SoulVoice == 100)
        {


            return true;
        }

        if (SoulVoice == 100 && BattleVoice.WillHaveOneCharge(25))
        {
            return false;
        }

        if (SoulVoice >= 80 && Player.HasStatus(true, StatusID.RagingStrikes) && Player.WillStatusEnd(10, false, StatusID.RagingStrikes))
        {


            return true;
        }

        if (SoulVoice == 100 && Player.HasStatus(true, StatusID.RagingStrikes) && Player.HasStatus(true, StatusID.BattleVoice))
        {


            return true;
        }

        if (Song == Song.MAGE && SoulVoice >= 80 && SongEndAfter(22) && SongEndAfter(18))
        {


            return true;
        }

        if (!Player.HasStatus(true, StatusID.RagingStrikes) && SoulVoice == 100)
        {


            return true;
        }

        return false;
    }

    private bool ShouldUseBurstMedicine(out IAction act)
    {
        act = null; // Default to null if Tincture cannot be used.

        // Don't use Tincture if player has the 'Weakness' status
        if (Player.HasStatus(true, StatusID.Weakness) || Player.HasStatus(true, (StatusID)Transcendent))
        {
            return false;
        }

        bool useTinctureFor2And8MinBurst = Configs.GetBool("2and8minTincture");

        if ((((CombatTime > 105 && CombatTime < 120) || (CombatTime > 465 && CombatTime < 480))
             && NextAbilityToNextGCD > 1.2
             && !Player.HasStatus(true, StatusID.Weakness)
             && !TinctureOfDexterity6.IsCoolingDown
             && !TinctureOfDexterity7.IsCoolingDown
             && !TinctureOfDexterity8.IsCoolingDown)
             && useTinctureFor2And8MinBurst)
        {
            // Attempt to use Burst Medicine.
            return UseBurstMedicine(out act, false);
        }
        // If the conditions are not met, return false.
        return false;
    }
    #endregion

    #region Extra Helper Methods
    // Updates Status of other extra helper methods on every frame
    protected override void UpdateInfo()
    {
        HandleOpenerAvailability();
        Methods.StateOfOpener();
        BurstActionCheck();
    }

    public void BurstActionCheck()
    {
        Methods.InBurst = Player.HasStatus(true, StatusID.BattleVoice);
    }

    // Used to check OpenerAvailability
    public void HandleOpenerAvailability()
    {
        bool Lvl90 = Player.Level >= 90;
        bool HasWM = !WanderersMinuet.IsCoolingDown;
        bool HasRS = !RagingStrikes.IsCoolingDown;
        bool HasEA = !EmpyrealArrow.IsCoolingDown;
        bool HasRF = !RadiantFinale.IsCoolingDown;
        var BLcharges = Bloodletter.CurrentCharges;
        bool HasBV = !BattleVoice.IsCoolingDown;
        bool HasBar = !Barrage.IsCoolingDown;
        bool HasSideWinder = !Sidewinder.IsCoolingDown;
        bool Openerstep0 = Methods.OpenerStep == 0;
        Methods.OpenerActionsAvailable = HasWM && HasRS && HasEA && HasRF && HasBV && BLcharges == 3 && HasBar && Lvl90 && HasSideWinder && Openerstep0;
    }
    #endregion
}
