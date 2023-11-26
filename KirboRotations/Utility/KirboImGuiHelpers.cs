using ImGuiNET;
using System.Numerics;

namespace KirboRotations.Utility;

public static class KirboImGuiHelpers
{
    public static void ImGuiColoredText(string text1, string text2, Vector4 color)
    {
        ImGui.Text(text1);
        ImGui.SameLine();
        ImGui.PushStyleColor(ImGuiCol.Text, color);
        ImGui.Text(text2);
        ImGui.PopStyleColor();
    }
}
