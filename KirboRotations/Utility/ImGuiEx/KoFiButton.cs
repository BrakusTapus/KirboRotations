using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility;
using ECommons.DalamudServices;
using ImGuiNET;
using KirboRotations.Utility.GameAssists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace KirboRotations.Utility.ImGuiEx
{
    public static class KoFiButton
    {
        public static bool IsOfficialPlugin = false;
        public const string Text = "Support on Ko-fi";
        public static string DonateLink => "https://nightmarexiv.github.io/donate.html" + (IsOfficialPlugin ? "?official" : "");
        public static void DrawRaw()
        {
            DrawButton();
        }

        const uint ColorNormal = 0xFF942502;
        const uint ColorHovered = 0xFF942502;
        const uint ColorActive = 0xFF942502;
        const uint ColorText = 0xFFFFFFFF;

        public static void DrawButton()
        {
            ImGui.PushStyleColor(ImGuiCol.Button, ColorNormal);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, ColorHovered);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, ColorActive);
            ImGui.PushStyleColor(ImGuiCol.Text, ColorText);
            if (ImGui.Button(Text))
            {
                GenericAssists.ShellStart(DonateLink);
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
            }
            ImGui.PopStyleColor(4);
        }

        public static void RightTransparentTab()
        {
            var textWidth = ImGui.CalcTextSize(Text).X;
            var spaceWidth = ImGui.CalcTextSize(" ").X;
            ImGui.BeginDisabled();
            ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0f);
            if (ImGuiEx.BeginTabItem(" ".Repeat((int)MathF.Ceiling(textWidth / spaceWidth)), ImGuiTabItemFlags.Trailing))
            {
                ImGui.EndTabItem();
            }
            ImGui.PopStyleVar();
            ImGui.EndDisabled();
        }

        public static void DrawRight()
        {
            var cur = ImGui.GetCursorPos();
            ImGui.SetCursorPosX(cur.X + ImGui.GetContentRegionAvail().X - ImGuiHelpers.GetButtonSize(Text).X);
            DrawRaw();
            ImGui.SetCursorPos(cur);
        }
    }
}
