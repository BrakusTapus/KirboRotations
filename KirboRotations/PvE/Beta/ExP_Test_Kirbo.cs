using ImGuiNET;
using KirboRotations.Custom.Configurations;
using KirboRotations.Custom.Configurations.Enums;
using KirboRotations.Custom.JobHelpers;
using KirboRotations.Custom.JobHelpers.Openers;
using KirboRotations.Custom.UI;
using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Attributes;
using RotationSolver.Basic.Configuration.RotationConfig;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Helpers;
using RotationSolver.Basic.Rotations.Basic;

namespace KirboRotations.PvE.Beta;

[RotationDesc(ActionID.None)]
[SourceCode(Path = "%Branch/FilePath to your sourse code% eg. main/DefaultRotations/Melee/NIN_Default.cs%")]
[LinkDescription("%Link to the pics or just a link%", "%Description about your rotation.%")]
[YoutubeLink(ID = "%If you got a youtube video link, please add here, just video id!%")]
[BetaRotation]
internal class Test_Kirbo : MCH_Base
{
    #region Rotation Info

    public override string GameVersion => "6.51";
    public override string RotationName => $"{USERNAME}'s {ClassJob.Abbreviation} [{Type}]";
    public override MedicineType MedicineType => MedicineType.Dexterity;
    public override CombatType Type => CombatType.Both;

    #endregion Rotation Info

    internal static MCHOpenerLogic MCHOpener = new();

    #region New PvE IBaseActions

    // WIP

    #endregion New PvE IBaseActions

    #region Debug window

    public override bool ShowStatus => true;
    public override void DisplayStatus()
    {
        base.DisplayStatus();
    }

    #endregion Debug window

    #region Action Related Properties

    public override bool CanHealAreaSpell => base.CanHealAreaSpell;
    public override bool CanHealAreaAbility => base.CanHealAreaAbility;
    public override bool CanHealSingleSpell => base.CanHealSingleSpell;
    public override bool CanHealSingleAbility => base.CanHealSingleAbility;

    #endregion Action Related Properties

    #region Rotation Config

    protected override IRotationConfigSet CreateConfiguration() => base.CreateConfiguration()
        .SetCombo(CombatType.PvE, "SampleText", 0, "SampleText", "SampleText2");

    #endregion Rotation Config

    #region Countdown Logic

    protected override IAction CountDownAction(float remainTime)
    {
        return base.CountDownAction(remainTime);
    }

    #endregion Countdown Logic

    #region Opener Logic

    // WIP

    #endregion Opener Logic

    #region GCD Logic

    protected override bool EmergencyGCD(out IAction act)
    {
        return base.EmergencyGCD(out act);
    }

    #endregion GCD Logic

    #region oGCD Logic

    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
    {
        return base.EmergencyAbility(nextGCD, out act);
    }

    #endregion oGCD Logic

    #region Job Helper Methods

    private bool TestRotation(IAction nextGCD, out IAction act)
    {
        act = null;

        return false;
    }
    private void BurstActionCheck()
    {
        BurstHelpers.InBurst = Player.HasStatus(true, StatusID.Wildfire);
    }

    #endregion Job Helper Methods

    #region Miscellaneous Helper Methods

    protected override void UpdateInfo()
    {
        base.UpdateInfo();
    }
    public override void OnTerritoryChanged()
    {
        base.OnTerritoryChanged();
    }

    #endregion Miscellaneous Helper Methods
}