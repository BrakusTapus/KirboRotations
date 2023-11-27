namespace KirboRotations.Utility;

public static class ImGuiEx
{
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
