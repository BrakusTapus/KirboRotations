using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using ImGuiNET;
using KirboRotations.Custom.JobHelpers.Openers;
using KirboRotations.JobHelpers;

namespace KirboRotations.UI;

/// <summary>
///
/// </summary>
internal static class ImGuiExtra
{
    #region Spacing

    /// <summary>
    /// Simple way of adding some space between elements. (Uses 3 ImGui.Spacing's).
    /// </summary>
    public static void TripleSpacing()
    {
        ImGui.Spacing();
        ImGui.Spacing();
        ImGui.Spacing();
    }

    #endregion Spacing

    #region Color
    /*/// <summary>
    /// Converts RGB color to <see cref="Vector4"/> for ImGui
    /// </summary>
    /// <param name="col">Color in format 0xRRGGBB</param>
    /// <param name="alpha">Optional transparency value between 0 and 1</param>
    /// <returns>Color in <see cref="Vector4"/> format ready to be used with <see cref="ImGui"/> functions</returns>
    public static Vector4 Vector4FromRGB(this uint col, float alpha = 1.0f)
    {
        byte* bytes = (byte*)&col;
        return new Vector4((float)bytes[2] / 255f, (float)bytes[1] / 255f, (float)bytes[0] / 255f, alpha);
    }*/

    public static Vector4 Vector4FromRGB(this uint col, float alpha = 1.0f)
    {
        float red = col >> 16 & 0xFF;
        float green = col >> 8 & 0xFF;
        float blue = col & 0xFF;

        return new Vector4(red / 255f, green / 255f, blue / 255f, alpha);
    }

    #endregion Color

    #region Tooltip

    /// <summary>
    ///
    /// </summary>
    /// <param name="text"></param>
    private static void SetTooltip(string text)
    {
        ImGui.BeginTooltip();
        ImGui.TextUnformatted(text);
        ImGui.EndTooltip();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="s"></param>
    public static void Tooltip(string s)
    {
        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip(s);
        }
    }

    #endregion Tooltip

    #region Text

    public static void TextCentered(string text)
    {
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + ImGui.GetContentRegionAvail().X / 2 - ImGui.CalcTextSize(text).X / 2);
        Text(text);
    }

    public static void TextCentered(Vector4 col, string text)
    {
        ImGui.PushStyleColor(ImGuiCol.Text, col);
        TextCentered(text);
        ImGui.PopStyleColor();
    }

    public static void TextCentered(Vector4? col, string text)
    {
        if (col == null)
        {
            TextCentered(text);
        }
        else
        {
            TextCentered(col.Value, text);
        }
    }

    /// <summary>
    /// Custom extension for displaying a centered text
    /// </summary>
    /// <param name="text"></param>
    public static void CenteredText(string text)
    {
        float windowWidth = ImGui.GetWindowSize().X;
        float textWidth = ImGui.CalcTextSize(text).X;

        ImGui.SetCursorPosX((windowWidth - textWidth) * 0.5f);
        ImGui.Text(text);
    }

    /// <summary>
    /// Displays two parts of text using ImGui, with the second part in a specified color.
    /// </summary>
    /// <param name="text1">The first part of the text, displayed in default color.</param>
    /// <param name="text2">The second part of the text, displayed in the specified color.</param>
    /// <param name="color">The Vector4 color value for the second part of the text.</param>
    public static void ImGuiColoredText(string text1, string text2, Vector4 color)
    {
        ImGui.Text(text1);
        ImGui.SameLine();

        ImGui.PushStyleColor(ImGuiCol.Text, color);
        ImGui.Text(text2);
        ImGui.PopStyleColor();
    }

    public static void Text(string s)
    {
        ImGui.TextUnformatted(s);
    }

    public static void Text(Vector4 col, string s)
    {
        ImGui.PushStyleColor(ImGuiCol.Text, col);
        ImGui.TextUnformatted(s);
        ImGui.PopStyleColor();
    }

    public static void Text(uint col, string s)
    {
        ImGui.PushStyleColor(ImGuiCol.Text, col);
        ImGui.TextUnformatted(s);
        ImGui.PopStyleColor();
    }

    public static void TextWrapped(string s)
    {
        ImGui.PushTextWrapPos();
        ImGui.TextUnformatted(s);
        ImGui.PopTextWrapPos();
    }

    public static void TextWrapped(Vector4 col, string s)
    {
        ImGui.PushTextWrapPos(0);
        Text(col, s);
        ImGui.PopTextWrapPos();
    }

    public static void TextWrapped(uint col, string s)
    {
        ImGui.PushTextWrapPos();
        Text(col, s);
        ImGui.PopTextWrapPos();
    }

    #endregion Text

    #region GetParsedColor

    public static Vector4 GetParsedColor(int percent)
    {
        if (percent < 25)
        {
            return ImGuiColors.ParsedGrey;
        }
        else if (percent < 50)
        {
            return ImGuiColors.ParsedGreen;
        }
        else if (percent < 75)
        {
            return ImGuiColors.ParsedBlue;
        }
        else if (percent < 95)
        {
            return ImGuiColors.ParsedPurple;
        }
        else if (percent < 99)
        {
            return ImGuiColors.ParsedOrange;
        }
        else if (percent == 99)
        {
            return ImGuiColors.ParsedPink;
        }
        else if (percent == 100)
        {
            return ImGuiColors.ParsedGold;
        }
        else
        {
            return ImGuiColors.DalamudRed;
        }
    }

    #endregion GetParsedColor

    #region IconButton

    public static bool IconButton(FontAwesomeIcon icon, string id = "IconButton", Vector2 size = default)
    {
        ImGui.PushFont(UiBuilder.IconFont);
        var result = ImGui.Button($"{icon.ToIconString()}##{icon.ToIconString()}-{id}", size);
        ImGui.PopFont();
        return result;
    }

    /// <summary>
    /// Creates a Icon button that resets the Opener Value's
    /// </summary>
    internal static void DisplayResetButton(string label)
    {
        if (IconButton(FontAwesomeIcon.Medkit, "Reset", default))
        {
            OpenerHelpers.ResetOpenerProperties();
        }
    }

    /// <summary>
    /// supposed to be a button
    /// </summary>
    /// <param name="label"></param>
    internal static void CopyCurrentValues(string label)
    {
        if (IconButton(FontAwesomeIcon.Clipboard, "ClipBoard", default))
        {
            // Gather the current values
            string values = $"OpenerHasFailed: {OpenerHelpers.OpenerHasFailed}\n" +
                        $"OpenerHasFinished: {OpenerHelpers.OpenerHasFinished}\n" +
                        $"OpenerStep: {OpenerHelpers.OpenerStep}\n" +
                        $"OpenerInProgress: {OpenerHelpers.OpenerInProgress}\n" +
                        $"OpenerActionsAvailable: {OpenerHelpers.OpenerActionsAvailable}\n" +
                        $"Lvl70UltimateOpenerActionsAvailable: {OpenerHelpers.LvL70_Ultimate_OpenerActionsAvailable}\n" +
                        $"Lvl80UltimateOpenerActionsAvailable: {OpenerHelpers.LvL80_Ultimate_OpenerActionsAvailable}";

            // Copy to clipboard
            Clipboard.SetText(values);
        }
    }

    #endregion IconButton

    #region Table

    /// <summary>
    ///
    /// </summary>
    /// <param name="description"></param>
    /// <param name="value"></param>
    public static void AddTableRow(string description, string value)
    {
        ImGui.TableNextRow();
        ImGui.TableNextColumn(); ImGui.Text(description);
        ImGui.TableNextColumn(); ImGui.Text(value);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="description"></param>
    /// <param name="value"></param>
    public static void AddTableRow(string description, bool value)
    {
        Vector4 color = value ? EColor.GreenBright : EColor.RedBright; // Green for true, red for false
        string valueText = value.ToString();

        ImGui.TableNextRow();
        ImGui.TableNextColumn();
        ImGui.Text(description);
        ImGui.TableNextColumn();
        ImGui.TextColored(color, valueText);
    }

    /// <summary>
    /// The text in both collums wil be colored
    /// </summary>
    /// <param name="description"></param>
    /// <param name="value"></param>
    /// <param name="textColor"></param>
    public static void AddTableRow(string description, string value, Vector4 textColor)
    {
        ImGui.TableNextRow();
        ImGui.TableNextColumn();
        ImGui.TextColored(textColor, description);
        ImGui.TableNextColumn();
        ImGui.TextColored(textColor, value);
    }

    /// <summary>
    /// First Colum text will be white and text in the second colum will be whatever color is picked
    /// </summary>
    /// <param name="description"></param>
    /// <param name="value"></param>
    /// <param name="textColor"></param>
    public static void AddTableRowColorLast(string description, string value, Vector4 textColor)
    {
        ImGui.TableNextRow();
        ImGui.TableNextColumn();
        ImGui.Text(description);
        ImGui.TableNextColumn();
        ImGui.TextColored(textColor, value);
    }

    #endregion Table

    #region Structured Data

    /// <summary>
    /// Creates a collapsing header with a label.
    /// </summary>
    /// <param name="label"></param>
    /// <param name="contentAction"></param>
    /// <param name="defaultOpen"></param>
    /// <returns></returns>
    public static bool CollapsingHeaderWithContent(string label, Action contentAction, bool defaultOpen = false)
    {
        if (ImGui.CollapsingHeader(label, defaultOpen ? ImGuiTreeNodeFlags.DefaultOpen : ImGuiTreeNodeFlags.None))
        {
            contentAction();
            return true;
        }
        return false;
    }

    #endregion Structured Data
}

/*
[StructLayout(LayoutKind.Explicit)]
public struct ImGuiWindow
{
    [FieldOffset(0xC)] public ImGuiWindowFlags Flags;

    [FieldOffset(0xD5)] public byte HasCloseButton;

    // 0x118 is the start of ImGuiWindowTempData
    [FieldOffset(0x130)] public Vector2 CursorMaxPos;
}

public static unsafe class ImGuiExtra
{
    [LibraryImport("cimgui")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial nint igGetCurrentWindow();

    public static unsafe ImGuiWindow* GetCurrentWindow() => (ImGuiWindow*)igGetCurrentWindow();

    public static unsafe ImGuiWindowFlags GetCurrentWindowFlags() => GetCurrentWindow()->Flags;

    public static unsafe bool CurrentWindowHasCloseButton() => GetCurrentWindow()->HasCloseButton != 0;
}

public class KirboColor
{
    public string Name { get; private set; }
    public Vector4 Colorvalue { get; set; }

    public KirboColor(string name, Vector4 colorValue)
    {
        Name = name;
        Colorvalue = colorValue;
    }
}
*/