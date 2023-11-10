using FFXIVClientStructs.FFXIV.Client.Game;

namespace KirboRotations.Utility;

public static class Methods
{
    public static bool Flag { get; internal set; } = false;

    internal static void ResetDebugFlag()
    {
        if (Flag)
        {
            ResetBoolAfterDelay();
         //PluginLog.Debug($"Resetting Flag > {Flag}");
        }
    }

    internal static void ResetBoolAfterDelay()
    {
        Thread.Sleep(300); // Delay for resetting the bool
        Flag = false; // Reset the bool
      //PluginLog.Debug($"flag is {Flag}");
    }

	/// <summary> Checks if the player is in a PVP enabled zone. </summary>
	/// <returns> A value indicating whether the player is in a PVP enabled zone. </returns>
	public static bool InPvP() => GameMain.IsInPvPArea() || GameMain.IsInPvPInstance();



}