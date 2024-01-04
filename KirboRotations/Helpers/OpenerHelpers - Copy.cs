using RotationSolver.Basic.Rotations;
using static KirboRotations.Custom.ExtraHelpers.GeneralHelpers;

namespace KirboRotations.Custom.JobHelpers.Openers;

public static class OpenerHelpers
{
    /// <summary>
    /// Flag used to indicate a state change
    /// </summary>
    private static bool _openerFlag = false;

    internal static bool OpenerFlag
    {
        get
        {
            Serilog.Log.Information($"{v} Getting OpenerFlag: {_openerFlag}");
            return _openerFlag;
        }
        set
        {
            if (_openerActionsAvailable != value)
            {
                Serilog.Log.Debug($"{v} Setting OpenerFlag from {_openerFlag} to {value}");
                _openerFlag = value;
            }
        }
    }

    private static bool _openerActionsAvailable = false;

    /// <summary>
    /// Checks if actions needed for the opener are available.
    /// </summary>
    internal static bool OpenerActionsAvailable
    {
        get
        {
            Serilog.Log.Information($"{v} Getting OpenerActionsAvailable: {_openerActionsAvailable}");
            return _openerActionsAvailable;
        }
        set
        {
            if (_openerActionsAvailable != value)
            {
                Serilog.Log.Information($"{v} Setting OpenerActionsAvailable from {_openerActionsAvailable} to {value}");
                _openerActionsAvailable = value;
            }
        }
    }

    /// <summary>
    /// Checks if actions needed for the opener available.
    /// </summary>
    internal static bool LvL70_Ultimate_OpenerActionsAvailable { get; set; } = false;

    /// <summary>
    /// Checks if actions needed for the opener available.
    /// </summary>
    internal static bool LvL80_Ultimate_OpenerActionsAvailable { get; set; } = false;

    /// <summary>
    /// Indicates wether or not the opener is currently in progress
    /// </summary>
    internal static bool OpenerInProgress { get; set; } = false;

    /// <summary>
    /// Keeps track of the opener step
    /// </summary>
    internal static int OpenerStep { get; set; } = 0;

    #region OpenerHasFailed
    private static bool _openerHasFailed = false;

    public static bool OpenerHasFailed
    {
        get
        {
            bool previousValue = false; // sus
            // Log only if the value changes
            if (_openerHasFailed != previousValue)
            {
                Serilog.Log.Information($"{v} Getting OpenerHasFailed: {_openerHasFailed}");
                previousValue = _openerHasFailed;
            }
            return _openerHasFailed;
        }
        set
        {
            _openerHasFailed = value;
        }
    }
    #endregion OpenerHasFailed

    #region OpenerHasFinished
    private static bool _openerHasFinished = false;

    /// <summary>
    /// Indicates whether or not the opener was finished successfully.
    /// </summary>
    internal static bool OpenerHasFinished
    {
        get
        {
            Serilog.Log.Information($"{v} Getting OpenerHasFinished: {_openerHasFinished}");
            return _openerHasFinished;
        }
        set
        {
            if (_openerHasFinished != value)
            {
                _openerHasFinished = value;
                Serilog.Log.Information($"{v} Setting OpenerHasFinished to {value}");
                if (_openerHasFinished)
                {
                    Serilog.Log.Information($"{v} OpenerHasFinished is now true");
                    OpenerComplete();
                }
            }
        }
    }
    #endregion OpenerHasFinished

    /// <summary>
    /// Called when either OpenerHasFinished or OpenerHasFailed becomes true.
    /// </summary>
    private static void OpenerComplete()
    {
        // Logic for OpenerComplete
        Serilog.Log.Information($"{v} Completed Opener");
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="lastaction"></param>
    /// <param name="nextaction"></param>
    /// <returns></returns>
    internal static bool OpenerController(bool lastaction, bool nextaction)
    {
        Serilog.Log.Information($"{v} OpenerController called with lastaction: {lastaction}, nextaction: {nextaction}");

        if (lastaction)
        {
            OpenerStep++;
            Serilog.Log.Information($"{v} OpenerStep incremented to {OpenerStep}. Returning false as the action.");
            return false;
        }

        Serilog.Log.Information($"{v} Returning nextaction value: {nextaction}");
        return nextaction;
    }

    /// <summary>
    /// Resets opener properties
    /// </summary>
    internal static void ResetOpenerProperties()
    {
        OpenerActionsAvailable = false;
        OpenerInProgress = false;
        OpenerStep = 0;
        OpenerHasFinished = false;
        OpenerHasFailed = false;
        Serilog.Log.Information($"{v} OpenerActionsAvailable = {OpenerActionsAvailable}");
        Serilog.Log.Information($"{v} OpenerInProgress = {OpenerInProgress} - Step: {OpenerStep}");
        Serilog.Log.Information($"{v} OpenerHasFinished = {OpenerHasFinished}");
        Serilog.Log.Information($"{v} OpenerHasFailed = {OpenerHasFailed}");
    }

    /// <summary>
    /// Handles the current state of the opener based on various condition.
    /// </summary>
    internal static void StateOfOpener()
    {
        if (!CustomRotation.InCombat)
        {
            _openerFlag = false;
            OpenerStep = 0;
            OpenerHasFinished = false;
            OpenerHasFailed = false;
        }
        if (OpenerHasFailed)
        {
            _openerFlag = true;
            OpenerInProgress = false;
        }
        if (OpenerHasFinished)
        {
            _openerFlag = true;
            OpenerInProgress = false;
        }
    }

    /// <summary>
    /// Calls the 'ResetBoolAfterDelay' if 'Flag' is true.
    /// </summary>
    internal static void OpenerFlagControl()
    {
        if (_openerFlag)
        {
            Serilog.Log.Debug($"{v} Opener Event");
            _openerFlag = false;
        }
    }
}