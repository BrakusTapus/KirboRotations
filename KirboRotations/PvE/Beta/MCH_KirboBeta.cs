using ImGuiNET;
using KirboRotations.Configurations;
using KirboRotations.Extensions;
using KirboRotations.Helpers;
using KirboRotations.Helpers.JobHelpers;
using KirboRotations.UI;
using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Attributes;
using RotationSolver.Basic.Configuration.RotationConfig;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Helpers;
using RotationSolver.Basic.Rotations.Basic;

namespace KirboRotations.PvE.Beta;

[RotationDesc(ActionID.Wildfire)]
[LinkDescription("https://i.imgur.com/23r8kFK.png", "Early AA")]
[LinkDescription("https://i.imgur.com/vekKW2k.jpg", "Delayed Tools")]
[BetaRotation]
internal class MCH_KirboBeta : MCH_Base
{
    #region Rotation Info
    public override CombatType Type => CombatType.PvE;
    public override string GameVersion => "0";
    public override string RotationName => $"{RotationConfigs.USERNAME}'s Test_MCH]";
    #endregion Rotation Info

    internal static MCHLogic MCHLogic = new();

    #region New PvE IBaseActions

    private static IBaseAction HeatedCleanShot { get; } = new BaseAction((ActionID)7413)
    {
    };

    private new static IBaseAction Dismantle { get; } = new BaseAction(ActionID.Dismantle, ActionOption.None | ActionOption.Defense);

    private new static IBaseAction Drill { get; } = new BaseAction(ActionID.Drill)
    {
        ActionCheck = (b, m) => !IsOverheated,
    };

    private new static IBaseAction AirAnchor { get; } = new BaseAction(ActionID.AirAnchor)
    {
        ChoiceTarget = (Targets, mustUse) =>
        {
            // If no target with PvP_WildfireDebuff, use existing logic
            Targets = Targets.Where(b => b.YalmDistanceX < 25);
            if (Targets.Any())
            {
                return Targets.OrderBy(ObjectHelper.GetHealthRatio).First();
            }
            return null;
        },
        ActionCheck = (b, m) => !IsOverheated,
    };

    private new static IBaseAction ChainSaw { get; } = new BaseAction(ActionID.ChainSaw)
    {
        ActionCheck = (b, m) => !IsOverheated,
    };

    private new static IBaseAction Wildfire { get; } = new BaseAction(ActionID.Wildfire)
    {
        ActionCheck = (b, m) => (Player.HasStatus(true, StatusID.Overheated) && HeatStacks > 4 || Heat >= 45) && InCombat
    };

    private new static IBaseAction Reassemble { get; } = new BaseAction(ActionID.Reassemble)
    {
        StatusProvide = new StatusID[1] { StatusID.Reassemble },
        ActionCheck = (b, m) => !Player.HasStatus(true, StatusID.Reassemble),
    };

    private new static IBaseAction Hypercharge { get; } = new BaseAction(ActionID.Hypercharge, ActionOption.UseResources)
    {
        StatusProvide = new StatusID[1] { StatusID.Overheated },
        ActionCheck = (b, m) => !IsOverheated && Heat >= 50 && IsLongerThan(10f)
    };

    private new static IBaseAction BarrelStabilizer { get; } = new BaseAction(ActionID.BarrelStabilizer)
    {
        ActionCheck = (b, m) => Heat <= 45 && InCombat && Target.IsTargetable && Target != Player
    };

    #endregion New PvE IBaseActions

    #region Debug window

    /// <summary>
    /// Displays the debug status in the rotation status panel
    /// </summary>
    public override bool ShowStatus => true;

    public override void DisplayStatus()
    {
        RotationConfigs CompatibilityAndFeatures = new();
        //MCHOpenerLogic CurrentState = new();
        CompatibilityAndFeatures.AddUltimateCompatibility(UltimateCompatibility.UCoB);

        CompatibilityAndFeatures.AddContentCompatibility(ContentCompatibility.DutyRoulette);
        CompatibilityAndFeatures.AddContentCompatibility(ContentCompatibility.SavageRaids);
        CompatibilityAndFeatures.AddContentCompatibility(ContentCompatibility.ExtremeTrials);
        CompatibilityAndFeatures.AddContentCompatibility(ContentCompatibility.Criterion);
        CompatibilityAndFeatures.AddContentCompatibility(ContentCompatibility.Hunts);

        CompatibilityAndFeatures.AddFeatures(Features.UseTincture);
        CompatibilityAndFeatures.AddFeatures(Features.SavageOptimized);
        CompatibilityAndFeatures.AddFeatures(Features.HasUserConfig);

        CompatibilityAndFeatures.SetRotationOpeners("Early AA", "Delayed Tools");
        CompatibilityAndFeatures.CurrentRotationSelection = Configs.GetCombo("RotationSelection");

        DebugWindow.DisplayRotationTabs(RotationName, CompatibilityAndFeatures);
    }

    #endregion Debug window

    #region Action Related Properties

    // Check if the major tools will come off cooldown soon
    private bool WillhaveTool { get; set; }

    // Holds the remaining amount of Heat stacks
    private static byte HeatStacks
    {
        get
        {
            byte stacks = Player.StatusStack(true, StatusID.Overheated);
            return stacks == byte.MaxValue ? (byte)5 : stacks;
        }
    }

    #endregion Action Related Properties

    #region Rotation Config

    protected override IRotationConfigSet CreateConfiguration() => base.CreateConfiguration()
        .SetCombo(CombatType.PvE, "RotationSelection", 0, "Select which Rotation will be used. (Openers will only be followed at level 90)", "Early AA", "Delayed Tools");

    #endregion Rotation Config

    #region Countdown Logic

    protected override IAction CountDownAction(float remainTime)
    {

        // Check if Pre-Pull actions are available and execute them
        if (remainTime <= 5f && MCHLogic.CanOpener)
        {
            if (MCHLogic.DoPrePullSteps(out IAction prePullAction))
            {
                return prePullAction;
            }
        }
        if (remainTime <= 1.5f && MCHLogic.CanOpener)
        {
            return AirAnchor;
        }


        return base.CountDownAction(remainTime);
    }

    #endregion Countdown Logic

    #region GCD Logic
    protected override bool GeneralGCD(out IAction act)
    {
        act = null;

        // Opener for MCH
        if (MCHLogic.DoFullOpener(out act))
            return true;

        return base.GeneralGCD(out act);
    }
    #endregion GCD Logic

    #region oGCD Logic
    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
    {
        act = null;

        // Opener for MCH
        if (MCHLogic.DoFullOpener(out act))
            return true;

        return base.EmergencyAbility(nextGCD, out act);
    }
    #endregion oGCD Logic

    #region Job Helper Methods

    // This should be relevant to the Action shown in the [RotationDesc(ActionID.Action]
    private void BurstActionCheck()
    {
        BurstHelpers.InBurst = Player.HasStatus(true, StatusID.Wildfire);
    }

    // GaussRound & Ricochet Condition
    private bool ShouldUseGaussroundOrRicochet(out IAction act)
    {
        act = null; // Initialize the action as null.

        // First, check if both GaussRound and Ricochet do not have at least one charge.
        // If neither has a charge, we cannot use either, so return false.
        if (!GaussRound.HasOneCharge && !Ricochet.HasOneCharge)
        {
            return false;
        }

        if (!GaussRound.HasOneCharge && !Ricochet.EnoughLevel)
        {
            return false;
        }

        // Second, check if Ricochet is not at a sufficient level to be used.
        // If not, default to GaussRound (if it can be used).
        if (!Ricochet.EnoughLevel)
        {
            return GaussRound.CanUse(out act, CanUseOption.MustUseEmpty);
        }

        // Third, check if GaussRound and Ricochet have the same number of charges.
        // If they do, prefer using GaussRound.
        if (GaussRound.CurrentCharges >= Ricochet.CurrentCharges)
        {
            return GaussRound.CanUse(out act, CanUseOption.MustUseEmpty);
        }

        // Fourth, check if Ricochet has more or an equal number of charges compared to GaussRound.
        // If so, prefer using Ricochet.
        if (Ricochet.CurrentCharges >= GaussRound.CurrentCharges)
        {
            return Ricochet.HasOneCharge && Ricochet.CanUse(out act, CanUseOption.MustUseEmpty);
        }
        // If none of the above conditions are met, default to using GaussRound.
        // This is a fallback in case other conditions fail to determine a clear action.
        return GaussRound.CanUse(out act, CanUseOption.MustUseEmpty);
    }

    #endregion Job Helper Methods

    #region Miscellaneous Helper Methods

    // Updates Status of other extra helper methods on every frame draw.
    protected override void UpdateInfo()
    {
        if (Player.IsInCombat())
        {
            ToolKitCheck();
            BurstActionCheck();
        }
    }

    // Checks if any major tool skill will almost come off CD.
    private void ToolKitCheck()
    {
        bool WillHaveDrill = !Drill.IsCoolingDown || Drill.WillHaveOneCharge(5f);
        bool WillHaveAirAnchor = !AirAnchor.IsCoolingDown || AirAnchor.WillHaveOneCharge(5f);
        bool WillHaveChainSaw = !ChainSaw.IsCoolingDown || ChainSaw.WillHaveOneCharge(5f);

        if (Player.Level >= 90)
        {
            // Player is level 90 or higher, check all tools
            WillhaveTool = WillHaveDrill || WillHaveAirAnchor || WillHaveChainSaw;
        }
        else if (Player.Level >= 76)
        {
            // Player is level 76 or higher but lower than 90, check Drill and Air Anchor
            WillhaveTool = WillHaveDrill || WillHaveAirAnchor;
        }
        else if (Player.Level >= 58)
        {
            // Player is level 58 or higher but lower than 76, check only Drill
            WillhaveTool = WillHaveDrill;
        }
        // Optionally, add an else clause for levels lower than 58 if needed
    }

    #endregion Miscellaneous Helper Methods
}