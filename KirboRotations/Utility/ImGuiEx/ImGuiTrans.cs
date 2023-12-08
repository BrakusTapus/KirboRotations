using ECommons;
using ImGuiNET;
using KirboRotations.Utility.GameAssists;
using System;
using System.Numerics;

namespace KirboRotations.Utility.ImGuiEx;

[Obsolete("Deprecated", true)]
public class ImGuiTrans
{
    public static void PushStyleColor(ImGuiCol target, Vector4 value)
    {
        ImGui.PushStyleColor(target, value with { W = ImGui.GetStyle().Colors[(int)target].W });
    }

    public static void PushTransparency(float v)
    {
        foreach (var c in Enum.GetValues<ImGuiCol>())
        {
            if (c == ImGuiCol.COUNT) continue;
            var col = ImGui.GetStyle().Colors[(int)c];
            ImGui.PushStyleColor(c, col with { W = col.W * v });
        }
    }

    public static void PopTransparency()
    {
        ImGui.PopStyleColor(Enum.GetValues<ImGuiCol>().Length - 1);
    }

    public static void WithTextColor(Vector4 col, Action func)
    {
        PushStyleColor(ImGuiCol.Text, col);
        GenericAssists.Safe(func);
        ImGui.PopStyleColor();
    }

    public static void Text(Vector4 col, string s)
    {
        PushStyleColor(ImGuiCol.Text, col);
        ImGui.TextUnformatted(s);
        ImGui.PopStyleColor();
    }

    public static void TextWrapped(Vector4 col, string s)
    {
        ImGui.PushTextWrapPos(0);
        Text(col, s);
        ImGui.PopTextWrapPos();
    }
}
