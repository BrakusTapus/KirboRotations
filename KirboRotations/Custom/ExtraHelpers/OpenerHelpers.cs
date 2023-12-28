namespace KirboRotations.Custom.ExtraHelpers;

public static class OpenerHelpers
{
    /// <summary>
    /// Flag used to indicate a state change
    /// </summary>
    internal static bool _openerFlag { get; set; } = false;

    /// <summary>
    /// Checks if actions needed for the opener available.
    /// </summary>
    internal static bool OpenerActionsAvailable { get; set; } = false;

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

    /// <summary>
    /// Indicates wether or not the opener was finished succesfully
    /// </summary>
    internal static bool OpenerHasFinished { get; set; } = false;

    /// <summary>
    /// Indicates wether or not the opener has failed
    /// </summary>
    internal static bool OpenerHasFailed { get; set; } = false;

    /// <summary>
    /// Displays "[RotationSolver] Completed Opener"
    /// </summary>
    public static string OpenerComplete => "[RotationSolver] Completed Opener";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lastaction"></param>
    /// <param name="nextaction"></param>
    /// <returns></returns>
    internal static bool OpenerController(bool lastaction, bool nextaction)
    {
        if (lastaction)
        {
            OpenerStep++;
            return false;
        }
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
        Serilog.Log.Debug($"OpenerActionsAvailable = {OpenerActionsAvailable}");
        Serilog.Log.Debug($"OpenerInProgress = {OpenerInProgress} - Step: {OpenerStep}");
        Serilog.Log.Debug($"OpenerHasFinished = {OpenerHasFinished}");
        Serilog.Log.Debug($"OpenerHasFailed = {OpenerHasFailed}");
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
            Serilog.Log.Debug($"Opener Event");
            _openerFlag = false;
        }
    }
}