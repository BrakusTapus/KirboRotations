using Dalamud.Game.ClientState.Objects;
using ImGuiNET;
using System.Numerics;
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
            Serilog.Log.Debug($"Resetting Flag > {Flag}");
        }
    }

    internal static void ResetBoolAfterDelay()
    {
        Thread.Sleep(300); // Delay for resetting the bool
        Flag = false; // Reset the bool
        Serilog.Log.Debug($"flag is {Flag}");
    }

    /// <summary> Checks if the player is in a PVP enabled zone. </summary>
    /// <returns> A value indicating whether the player is in a PVP enabled zone. </returns>
    internal static bool InPvP() => GameMain.IsInPvPArea() || GameMain.IsInPvPInstance();

    /// <summary>
    /// Displays two parts of text using ImGui, with the second part in a specified color.
    /// </summary>
    /// <param name="text1">The first part of the text, displayed in default color.</param>
    /// <param name="text2">The second part of the text, displayed in the specified color.</param>
    /// <param name="color">The color for the second part of the text.</param>
    public static void ImGuiColoredText(string text1, string text2, Vector4 color)
    {
        ImGui.Text(text1);
        ImGui.SameLine();
        ImGui.PushStyleColor(ImGuiCol.Text, color);
        ImGui.Text(text2);
        ImGui.PopStyleColor();
    }

}