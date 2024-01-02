using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Attributes;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Helpers;
using RotationSolver.Basic.Rotations.Basic;

namespace KirboRotations.PvE.Magical;

[SourceCode(Path = "main/KirboRotations/Magical/BLU_Extra.cs")]
internal sealed class PvE_BLU_Extra : BLU_Base
{
    public override CombatType Type => CombatType.PvE;

    public override string GameVersion => "6.51";

    public override string RotationName => "Kirbo's Extra";

    public override string Description => "This is a simplified version for me (ArchiTed) using, \nwhich doesn't contain all actions.";

    protected override bool AttackAbility(out IAction act)
    {
        act = null;
        return false;
    }

    protected override bool GeneralGCD(out IAction act)
    {
        if (ChocoMeteor.CanUse(out act)) return true;
        if (DrillCannons.CanUse(out act)) return true;

        if (TripleTrident.OnSlot && TripleTrident.RightType && TripleTrident.WillHaveOneChargeGCD(OnSlotCount(Whistle, Tingle), 0))
        {
            if ((TripleTrident.CanUse(out _, CanUseOption.MustUse) || !HasHostilesInRange) && Whistle.CanUse(out act)) return true;

            if (!Player.HasStatus(true, StatusID.Tingling)
                && Tingle.CanUse(out act, CanUseOption.MustUse)) return true;
            if (OffGuard.CanUse(out act)) return true;

            if (TripleTrident.CanUse(out act, CanUseOption.MustUse)) return true;
        }
        if (ChocoMeteor.CanUse(out act, HasCompanion ? CanUseOption.MustUse : CanUseOption.None)) return true;

        if (SonicBoom.CanUse(out act)) return true;
        if (DrillCannons.CanUse(out act, CanUseOption.MustUse)) return true;

        return false;
    }
}