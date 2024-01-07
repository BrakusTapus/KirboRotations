using RotationSolver.RotationBasics.Actions;

namespace KirboRotations.Helpers;

internal interface ICustomRotation
{
    IAction CountDownAction(float remainTime);

    bool EmergencyAbility(IAction nextGCD, out IAction act);

    bool GeneralGCD(out IAction act);

}