using ImGuiNET;
using KirboRotations.Configurations;
using KirboRotations.UI;
using RotationSolver.RotationBasics.Actions;
using RotationSolver.RotationBasics.Attributes;
using RotationSolver.RotationBasics.Configuration.RotationConfig;
using RotationSolver.RotationBasics.Data;
using RotationSolver.RotationBasics.Rotations.Basic;

namespace KirboRotations.PvP.Tank;

[BetaRotation]
[RotationDesc(ActionID.PrimalRend)]
public class WAR_KirboPvP : WAR_Base
{
    #region Rotation Info
    public override string GameVersion => "6.51";
    public override string RotationName => $"{RotationConfigs.USERNAME}'s {ClassJob.Abbreviation} [{Type}]";
    public override CombatType Type => CombatType.PvP;
    #endregion Rotation Info

    #region Debug window
    public override bool ShowStatus => true;
    public override void DisplayStatus()
    {
        RotationConfigs CompatibilityAndFeatures = new();
        CompatibilityAndFeatures.AddContentCompatibilityForPvP(PvPContentCompatibility.Frontlines);
        CompatibilityAndFeatures.AddContentCompatibilityForPvP(PvPContentCompatibility.CrystalineConflict);
        CompatibilityAndFeatures.AddFeaturesForPvP(PvPFeatures.HasUserConfig);
        try
        {
            PvPDebugWindow.DisplayPvPTab();
            ImGui.SameLine();
            PvPDebugWindow.DisplayPvPRotationTabs(RotationName, CompatibilityAndFeatures);
        }
        catch (Exception ex)
        {
            Serilog.Log.Warning($"{ex}");
        }
    }
    #endregion Debug window

    protected override IRotationConfigSet CreateConfiguration() => base.CreateConfiguration()
        .SetBool(CombatType.PvP, "LBInPvP", true, "Use the LB in PvP when Target is killable by it")
        .SetInt(CombatType.PvP, "PSValue", 30000, "How much HP does the enemy have for LB:PrimalScream to be done", 1, 100000)
        .SetInt(CombatType.PvP, "OSValue", 30000, "How much HP does the enemy have for Onslaught to be done", 1, 100000)
        .SetBool(CombatType.PvP, "GuardCancel", false, "Turn on if you want to FORCE RS to use nothing while ENEMIES in guard in PvP");

    protected override bool GeneralGCD(out IAction act)
    {
        act = null;

        #region PvP

        if (Configs.GetBool("LBInPvP") && HostileTarget && HostileTarget.CurrentHp < Configs.GetInt("PSValue") && PvP_PrimalScream.IsEnabled)
        {
            if (PvP_PrimalScream.CanUse(out act, CanUseOption.MustUse))
            {
                return true;
            }

            if (PvP_PrimalRend.CanUse(out act, CanUseOption.MustUse))
            {
                return true;
            }

            if (PvP_FellCleave.CanUse(out act, CanUseOption.MustUse))
            {
                return true;
            }

            if (PvP_Bloodwhetting.CanUse(out act, CanUseOption.MustUse))
            {
                return true;
            }

            if (PvP_ChaoticCyclone.CanUse(out act, CanUseOption.MustUse))
            {
                return true;
            }
        }

        if (PvP_Onslaught.CanUse(out act, CanUseOption.MustUse) && HostileTarget && HostileTarget.CurrentHp < Configs.GetInt("OSValue"))
        {
            return true;
        }

        if (PvP_Orogeny.CanUse(out act, CanUseOption.MustUse))
        {
            return true;
        }

        if (PvP_Blota.CanUse(out act, CanUseOption.MustUse))
        {
            return true;
        }

        if (PvP_Bloodwhetting.CanUse(out act, CanUseOption.MustUse) && InCombat)
        {
            return true;
        }

        if (PvP_FellCleave.CanUse(out act, CanUseOption.MustUse))
        {
            return true;
        }

        if (PvP_ChaoticCyclone.CanUse(out act, CanUseOption.MustUse))
        {
            return true;
        }

        if (PvP_PrimalRend.CanUse(out act, CanUseOption.MustUse))
        {
            return true;
        }

        if (PvP_StormsPath.CanUse(out act, CanUseOption.MustUse))
        {
            return true;
        }

        if (PvP_Maim.CanUse(out act, CanUseOption.MustUse))
        {
            return true;
        }

        if (PvP_HeavySwing.CanUse(out act, CanUseOption.MustUse))
        {
            return true;
        }

        return false;

        #endregion PvP
    }
}