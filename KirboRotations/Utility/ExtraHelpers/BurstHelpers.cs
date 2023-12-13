namespace KirboRotations.Utility.ExtraHelpers;

public static class BurstHelpers
{
    /// <summary>
    /// Flag used to indicate a state change
    /// </summary>
    internal static bool _burstFlag { get; set; } = false;

    /// <summary>
    /// Keeps track of the opener step
    /// </summary>
    internal static int BurstStep { get; set; } = 0;

    /// <summary>
    /// Property to hold the availability of BurstWindow
    /// </summary>
    internal static bool BurstActionsAvailable { get; set; } = false;

    /// <summary>
    /// Is burst in progress
    /// </summary>
    internal static bool BurstInProgress { get; set; } = false;

    /// <summary>
    /// Is in Burst (should be when a jobs 2min action was used)
    /// </summary>
    internal static bool InBurst { get; set; } = false;

    /// <summary>
    /// check if burst completed correctly
    /// </summary>
    internal static bool BurstHasFinished { get; set; } = false;

    /// <summary>
    /// check if burst failed.
    /// </summary>
    internal static bool BurstHasFailed { get; set; } = false;

    /// <summary>
    /// Controls the Burst Proceeds to next step if the LastAction matches the Burst Sequence
    /// </summary>
    /// <param name="lastaction"></param>
    /// <param name="nextaction"></param>
    /// <returns></returns>
    internal static bool BurstController(bool lastaction, bool nextaction)
    {
        if (lastaction)
        {
            BurstStep++;
            return false;
        }
        return nextaction;
    }

    /// <summary>
    /// Resets opener properties
    /// </summary>
    internal static void ResetBurstProperties()
    {
        BurstActionsAvailable = false;
        BurstInProgress = false;
        BurstStep = 0;
        BurstHasFinished = false;
        BurstHasFailed = false;
        Serilog.Log.Debug($"OpenerHasFailed = {InBurst}");
        Serilog.Log.Debug($"OpenerInProgress = {BurstInProgress} - Step: {BurstStep}");
        Serilog.Log.Debug($"OpenerHasFinished = {BurstHasFinished}");
    }

    /// <summary>
    /// Handles the current state of the opener based on various condition.
    /// </summary>
    internal static void BurstStatus()
    {
        if (!CustomRotation.InCombat)
        {
            _burstFlag = true;
            BurstStep = 0;
            BurstHasFinished = false;
            InBurst = false;
            BurstInProgress = false;
        }
        if (BurstHasFailed)
        {
            _burstFlag = true;
            BurstInProgress = false;
        }
        if (BurstHasFinished)
        {
            _burstFlag = true;
            BurstInProgress = false;
        }
    }

    /// <summary>
    /// Calls the 'ResetBoolAfterDelay' if 'Flag' is true.
    /// </summary>
    internal static void BurstFlagControl()
    {
        if (_burstFlag)
        {
            Serilog.Log.Debug($"Burst Event");
            _burstFlag = false;
        }
    }
}