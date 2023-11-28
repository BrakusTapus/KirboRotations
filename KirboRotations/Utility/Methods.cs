using FFXIVClientStructs.FFXIV.Client.Game;

namespace KirboRotations.Utility;

public static class Methods
{
    public static bool Flag { get; internal set; } = false;

    /// <summary>
    /// 
    /// </summary>
    internal static int Openerstep { get; set; }

    /// <summary>
    /// Indicates wether or not the opener was finished succesfully
    /// </summary>
    internal static bool OpenerHasFinished { get; set; }

    /// <summary>
    /// Indicates wether or not the opener has failed
    /// </summary>
    internal static bool OpenerHasFailed { get; set; }

    /// <summary>
    /// Indicates wether or not the opener is currently in progress
    /// </summary>
    internal static bool OpenerInProgress { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lastaction"></param>
    /// <param name="nextaction"></param>
    /// <returns></returns>
    internal static bool OpenerStep(bool lastaction, bool nextaction)
    {
        if (lastaction)
        {
            Methods.Openerstep++;
            return false;
        }
        return nextaction;
    }

    /// <summary>
    /// Resets opener properties
    /// </summary>
    internal static void ResetOpenerProperties()
    {
        Methods.Openerstep = 0;
        Methods.OpenerHasFinished = false;
        Methods.OpenerHasFailed = false;
        //OpenerActionsAvailable = false;
        Methods.OpenerInProgress = false;
        Serilog.Log.Debug($"Openerstep = {Methods.Openerstep}");
        Serilog.Log.Debug($"OpenerHasFinished = {Methods.OpenerHasFinished}");
        Serilog.Log.Debug($"OpenerHasFailed = {Methods.OpenerHasFailed}");
        //Serilog.Log.Debug($"OpenerActionsAvailable = {OpenerActionsAvailable}");
        Serilog.Log.Debug($"OpenerInProgress = {Methods.OpenerInProgress}");
    }

    /// <summary>
    /// Handles the current state of the opener based on various condition.
    /// </summary>
    internal static void StateOfOpener()
    {
        // Do NOT uncomment unless we have a way to get localplayer without ECommons
        //if (Player.Object.IsDead)
        //{
        //    Methods.OpenerHasFailed = false;
        //    Methods.OpenerHasFinished = false;
        //    Methods.Openerstep = 0;
        //}
        if (!CustomRotation.InCombat)
        {
            Methods.OpenerHasFailed = false;
            Methods.OpenerHasFinished = false;
            Methods.Openerstep = 0;

            // To-Do: Find a better way of handling the debugflag instead of resetting it every 60s            
            //if (!CustomRotation.InCombat && !Methods.Flag)
            //{
            //    Serilog.Log.Debug($"InCombat is {CustomRotation.InCombat} resetting properties: OpenerHasFailed: {OpenerHasFailed} | OpenerHasFinished: {OpenerHasFinished} | Openerstep: {Openerstep}");
            //    Methods.Flag = true;
            //    Thread.Sleep(5000);
            //    ResetDebugFlag();
            //}
        }
        if (Methods.OpenerHasFailed)
        {
            Methods.OpenerInProgress = false;
        }
        if (Methods.OpenerHasFinished)
        {
            Methods.OpenerInProgress = false;
        }
    }

    /// <summary>
    /// Calls the 'ResetBoolAfterDelay' if 'Flag' is true.
    /// </summary>
    internal static void ResetDebugFlag()
    {
        if (Flag)
        {
            ResetBoolAfterDelay();
            Serilog.Log.Debug($"Sending Reset request for 'Flag'");
        }
    }

    /// <summary>
    /// Resets 'Flag' back to false after 60s
    /// </summary>
    internal static void ResetBoolAfterDelay()
    {
        //Serilog.Log.Debug($"Resetting 'Flag' in 60s");
        //Thread.Sleep(60000); // Fairly certain this was causing issues
        Methods.Flag = false;
        Serilog.Log.Debug($"'Flag' is now {Methods.Flag}");
    }

    /// <summary> Checks if the player is in a PVP enabled zone. </summary>
    /// <returns> A value indicating whether the player is in a PVP enabled zone. </returns>
    internal static bool InPvP() => GameMain.IsInPvPArea() || GameMain.IsInPvPInstance();

}