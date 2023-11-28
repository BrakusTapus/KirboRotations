namespace KirboRotations.Utility;

public static class ImGuiEx
{
    /// <summary>
    /// Displays two parts of text using ImGui, with the second part in a specified color.
    /// </summary>
    /// <param name="text1">The first part of the text, displayed in default color.</param>
    /// <param name="text2">The second part of the text, displayed in the specified color.</param>
    /// <param name="color">The color for the second part of the text.</param>
    public static void ImGuiColoredText(string text1, string text2, KirboColor colorEnum)
    {
        ImGui.Text(text1);
        ImGui.SameLine();

        Vector4 colorValue = ColorMap[colorEnum];
        ImGui.PushStyleColor(ImGuiCol.Text, colorValue);
        ImGui.Text(text2);
        ImGui.PopStyleColor();
    }

    /// <summary>
    /// Easy way to add a Tooltip to an ImGui element.
    /// </summary>
    /// <param name="text"></param>
    public static void Tooltip(string text)
    {
        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.TextUnformatted(text);
            ImGui.EndTooltip();
        }
    }

    /// <summary>
    /// Simple way of adding some space and a seperator in between elements.
    /// </summary>
    public static void SeperatorWithSpacing()
    {
        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();
    }

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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="label"></param>
    /// <param name="size"></param>
    /// <param name="color"></param>
    /// <param name="enabled"></param>
    /// <returns></returns>
    public static bool ColoredButton(string label, Vector2 size, KirboColor color, bool enabled = true)
    {
        Vector4 colorValue = ColorMap[color];
        ImGui.PushStyleColor(ImGuiCol.Button, colorValue);
        bool result = Button(label, size, enabled);
        ImGui.PopStyleColor();
        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="label"></param>
    /// <param name="size"></param>
    /// <param name="enabled"></param>
    /// <returns></returns>
    public static bool Button(string label, Vector2 size, bool enabled = true)
    {
        if (!enabled)
        {
            ImGui.PushStyleVar(ImGuiStyleVar.Alpha, ImGui.GetStyle().Alpha * 0.5f);
            ImGui.Button(label, size);
            ImGui.PopStyleVar();
            return false;
        }
        return ImGui.Button(label, size);
    }

    /// <summary>
    /// Holds Vector4 value's for various colors.
    /// </summary>
    public enum KirboColor
    {
        Red,
        Green,
        Blue,
        Yellow,
        LightBlue
    }

    /// <summary>
    /// The actual Vector4 values used for KirboColor
    /// </summary>
    public static readonly Dictionary<KirboColor, Vector4> ColorMap = new Dictionary<KirboColor, Vector4>
    {
        { KirboColor.Red, new Vector4(1.0f, 0.0f, 0.0f, 1.0f) },
        { KirboColor.Green, new Vector4(0.0f, 1.0f, 0.0f, 1.0f) },
        { KirboColor.Blue, new Vector4(0.0f, 0.0f, 1.0f, 1.0f) },
        { KirboColor.Yellow, new Vector4(1.0f, 1.0f, 0.0f, 1.0f) },
        { KirboColor.LightBlue, new Vector4(0.68f, 0.85f, 1.0f, 1.0f) },
    };

}