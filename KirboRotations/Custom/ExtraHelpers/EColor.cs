using Dalamud.Interface.Colors;

namespace KirboRotations.Custom.ExtraHelpers;

/// <summary>
/// A set of fancy color for use in plugins. You can redefine them to match necessary style!
/// </summary>
public static class EColor
{
    // Color Collection
    public static Vector4 White = ImGuiExtra.Vector4FromRGB(0xFFFFFF);
    public static Vector4 Black = ImGuiExtra.Vector4FromRGB(0x000000);
    public static Vector4 RedBright = ImGuiExtra.Vector4FromRGB(0xFF0000);
    public static Vector4 Red = ImGuiExtra.Vector4FromRGB(0xAA0000);
    public static Vector4 Green = ImGuiExtra.Vector4FromRGB(0x00aa00);
    public static Vector4 GreenBright = ImGuiExtra.Vector4FromRGB(0x00ff00);
    public static Vector4 Blue = ImGuiExtra.Vector4FromRGB(0x0000aa);
    public static Vector4 BlueBright = ImGuiExtra.Vector4FromRGB(0x0000ff);
    public static Vector4 BlueSea = ImGuiExtra.Vector4FromRGB(0x0058AA);
    public static Vector4 LightBlue = ImGuiExtra.Vector4FromRGB(0xADD8E6);
    public static Vector4 BlueSky = ImGuiExtra.Vector4FromRGB(0x0085FF);
    public static Vector4 Yellow = ImGuiExtra.Vector4FromRGB(0xAAAA00);
    public static Vector4 YellowBright = ImGuiExtra.Vector4FromRGB(0xFFFF00);
    public static Vector4 Orange = ImGuiExtra.Vector4FromRGB(0xAA5400);
    public static Vector4 OrangeBright = ImGuiExtra.Vector4FromRGB(0xFF7F00);
    public static Vector4 Cyan = ImGuiExtra.Vector4FromRGB(0x00aaaa);
    public static Vector4 CyanBright = ImGuiExtra.Vector4FromRGB(0x00FFFF);
    public static Vector4 Violet = ImGuiExtra.Vector4FromRGB(0xAA00AA);
    public static Vector4 VioletBright = ImGuiExtra.Vector4FromRGB(0xFF00FF);
    public static Vector4 Purple = ImGuiExtra.Vector4FromRGB(0xAA0058);
    public static Vector4 PurpleBright = ImGuiExtra.Vector4FromRGB(0xFF0084);
    public static Vector4 Pink = ImGuiExtra.Vector4FromRGB(0xFF6FFF);
    public static Vector4 PinkLight = ImGuiExtra.Vector4FromRGB(0xFFABD6);
    public static Vector4 Fuchsia = ImGuiExtra.Vector4FromRGB(0xAD0066);

    // Dalamud UI Colors
    public static Vector4 DalamudRed = ImGuiColors.DalamudRed;
    public static Vector4 DalamudGrey = ImGuiColors.DalamudGrey;
    public static Vector4 DalamudGrey2 = ImGuiColors.DalamudGrey2;
    public static Vector4 DalamudGrey3 = ImGuiColors.DalamudGrey3;
    public static Vector4 DalamudWhite = ImGuiColors.DalamudWhite;
    public static Vector4 DalamudWhite2 = ImGuiColors.DalamudWhite2;
    public static Vector4 DalamudOrange = ImGuiColors.DalamudOrange;
    public static Vector4 DalamudYellow = ImGuiColors.DalamudYellow;
    public static Vector4 DalamudViolet = ImGuiColors.DalamudViolet;

    // Job Role Colors
    public static Vector4 TankBlue = ImGuiColors.TankBlue;
    public static Vector4 HealerGreen = ImGuiColors.HealerGreen;
    public static Vector4 DPSRed = ImGuiColors.DPSRed;

    // FFLogs Parse Colors
    public static Vector4 ParsedGrey = ImGuiColors.ParsedGrey;
    public static Vector4 ParsedGreen = ImGuiColors.ParsedGreen;
    public static Vector4 ParsedBlue = ImGuiColors.ParsedBlue;
    public static Vector4 ParsedPurple = ImGuiColors.ParsedPurple;
    public static Vector4 ParsedOrange = ImGuiColors.ParsedOrange;
    public static Vector4 ParsedPink = ImGuiColors.ParsedPink;
    public static Vector4 ParsedGold = ImGuiColors.ParsedGold;
}
