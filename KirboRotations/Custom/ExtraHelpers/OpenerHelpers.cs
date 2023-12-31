using System.Runtime.CompilerServices;
using static KirboRotations.Custom.ExtraHelpers.GeneralHelpers;

namespace KirboRotations.Custom.ExtraHelpers;

public static class OpenerHelpers
{
    #region Backing fields for properties
    private static bool _openerHasFailed = false;
    private static bool _openerHasFinished = false;
    private static int _openerStep = 0;
    private static bool _openerInProgress = false;
    private static bool _openerActionsAvailable = false;
    private static bool _lvl70UltimateOpenerActionsAvailable = false;
    private static bool _lvl80UltimateOpenerActionsAvailable = false;
    #endregion

    #region Properties with logging
    public static bool OpenerHasFailed
    {
        get => _openerHasFailed;
        set => SetWithLogging(ref _openerHasFailed, value, nameof(OpenerHasFailed));
    }

    public static bool OpenerHasFinished
    {
        get => _openerHasFinished;
        set => SetWithLogging(ref _openerHasFinished, value, nameof(OpenerHasFinished));
    }

    public static int OpenerStep
    {
        get => _openerStep;
        set => SetWithLogging(ref _openerStep, value, nameof(OpenerStep));
    }

    public static bool OpenerInProgress
    {
        get => _openerInProgress;
        set => SetWithLogging(ref _openerInProgress, value, nameof(OpenerInProgress));
    }

    public static bool OpenerActionsAvailable
    {
        get => _openerActionsAvailable;
        set => SetWithLogging(ref _openerActionsAvailable, value, nameof(OpenerActionsAvailable));
    }

    public static bool LvL70_Ultimate_OpenerActionsAvailable
    {
        get => _lvl70UltimateOpenerActionsAvailable;
        set => SetWithLogging(ref _lvl70UltimateOpenerActionsAvailable, value, nameof(LvL70_Ultimate_OpenerActionsAvailable));
    }

    public static bool LvL80_Ultimate_OpenerActionsAvailable
    {
        get => _lvl80UltimateOpenerActionsAvailable;
        set => SetWithLogging(ref _lvl80UltimateOpenerActionsAvailable, value, nameof(LvL80_Ultimate_OpenerActionsAvailable));
    }
    #endregion

    #region Methods
    public static void ResetOpenerProperties()
    {
        OpenerHasFailed = false;
        OpenerHasFinished = true;
        OpenerStep = 0;
        OpenerInProgress = false;
        // Do not reset OpenerActionsAvailable here
        LvL70_Ultimate_OpenerActionsAvailable = false;
        LvL80_Ultimate_OpenerActionsAvailable = false;
    }

    public static void StateOfOpener()
    {

        if (OpenerSequenceCompleted())
        {
            OpenerInProgress = false;
        }
        else
        {
            ResetOpenerProperties();
        }

    }

    private static bool OpenerSequenceCompleted()
    {
        return OpenerHasFinished || OpenerHasFailed;
    }

    private static void SetWithLogging<T>(ref T field, T value, string propertyName, [CallerMemberName] string caller = null)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            LogPropertyChange(propertyName, value, caller);
        }
    }

    internal static bool OpenerController(bool lastAction, bool nextAction, [CallerMemberName] string caller = null)
    {
        if (lastAction)
        {
            OpenerStep++; // Increment using the property
            Serilog.Log.Information($"{v} OpenerStep incremented to {OpenerStep} (Called by: {caller}).");
            return false;
        }

        return nextAction;
    }

    private static void LogPropertyChange<T>(string propertyName, T value, string caller)
    {
        Serilog.Log.Information($"{v} Property {propertyName} changed to: {value} (Called by: {caller})");
    }
    #endregion
}
