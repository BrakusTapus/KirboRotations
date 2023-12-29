namespace KirboRotations.Utility.ImGuiEx;

using KirboRotations.Utility.Core;
using KirboRotations.Utility.GameAssists;
using KirboRotations.Utility.ImGuiEx.ImGuiMethods;

public static class ImGuiEx2
{

    #region Text

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

    #endregion

    #region Features

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

    #endregion

    #region Spacing

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
    /// Simple way of adding some space between elements. (Uses 3 ImGui.Spacing's).
    /// </summary>
    public static void TripleSpacing()
    {
        ImGui.Spacing();
        ImGui.Spacing();
        ImGui.Spacing();
    }

    /// <summary>
    /// Custom extension for a horizontal line
    /// </summary>
    public static void HorizontalLine()
    {
        ImGui.Separator();
    }

    #endregion

    #region Actions

    /// <summary>
    /// Easy way to display the action name and id of the last action that was used
    /// </summary>
    /// <param name="name"></param>
    /// <param name="actionid"></param>
    public static void DrawLastAction(ActionID name, uint actionid)
    {
        ImGui.Text($"'{name}' | ActionID: '{actionid}'");
    }

    private static void DrawLastAction(string title, ActionID actionname, uint actionid)
    {
        // DrawLastAction(DataCenter.LastAction, (uint)DataCenter.LastAction);
        DrawLastAction(actionname, (uint)actionid);
        ImGui.Separator();

        ImGui.Text($"{title}"); // Should be 'StatusProvide'
        ImGui.Indent();
        foreach (var status in PlayerData.Object.StatusList)
        {
            // Skip the 'Well Fed' buff with ID 48
            if (status.StatusId == 48) continue;

            var source = status.SourceId == PlayerData.Object.ObjectId ? "You" : KirboSvc.Objects.SearchById(status.SourceId) == null ? "None" : "Others";
            ImGui.Text($"ActionName: '{DataBase.LastAction}' | ActionID '{(uint)DataBase.LastAction}' | StatusName: '{status.GameData.Name}' | ID: '{status.StatusId}' | Source: '{source}'");
            ImGui.SameLine();

            ImGui.PushID((int)status.StatusId);
            var actioninfo = $"{DataBase.LastAction} = {(uint)DataBase.LastAction}";
            var statusinfo = $"{status.GameData.Name} = {status.StatusId}";
            var sourceinfo = $"Source: '{source}'";
            var providestatus = $"StatusProvide = new StatusID[] { status.GameData.Name },";
            if (ImGui.Button("Copy"))
            {
                ImGui.SetClipboardText($"{actioninfo}\n{statusinfo}\n{sourceinfo}\n{providestatus}");
                Notify.Success("copied to clipboard.");
            }
            ImGui.PopID();
        }
        ImGui.Unindent();

        ImGuiEx2.TripleSpacing();

        ImGui.Text($"{title}"); // Should be 'TargetStatus'
        ImGui.Indent();
        if (KirboSvc.Targets.Target is BattleChara b)
        {
            foreach (var status in b.StatusList)
            {
                var source = status.SourceId == PlayerData.Object.ObjectId ? "You" : KirboSvc.Objects.SearchById(status.SourceId) == null ? "None" : "Others";
                ImGui.Text($"ActionName: '{DataBase.LastAction}' | ActionID '{(uint)DataBase.LastAction}' | StatusName: '{status.GameData.Name}' | ID: '{status.StatusId}' | Source: '{source}'");
                ImGui.SameLine();

                // Copy Button Uses a unique ID for each button
                ImGui.PushID((int)status.StatusId); // Ensure status.StatusId is unique for each status
                var actioninfo = $"{DataBase.LastAction} = {(uint)DataBase.LastAction}";
                var statusinfo = $"{status.GameData.Name} = {status.StatusId}";
                var sourceinfo = $"Source: '{source}'";
                var targetstatus = $"TargetStatus = new StatusID[] { status.GameData.Name },";
                if (ImGui.Button("Copy"))
                {
                    ImGui.SetClipboardText($"{actioninfo}\n{statusinfo}\n{sourceinfo}\n{targetstatus}");
                    ECommons.ImGuiMethods.Notify.Success("copied to clipboard.");
                }
                ImGui.PopID(); // End of unique ID scope
            }
        }
        ImGui.Unindent();



    }

    #endregion

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

    #endregion

    #region Buttons

    /// <summary>
    /// Custom extension for a simple toggle button
    /// </summary>
    public static bool ToggleButton(string id, ref bool value)
    {
        if (value)
            ImGui.PushStyleColor(ImGuiCol.Button, ImGui.GetColorU32(ImGuiCol.ButtonActive));

        bool clicked = ImGui.Button(id);

        if (value)
            ImGui.PopStyleColor();

        if (clicked)
            value = !value;

        return clicked;
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

    #endregion

    #region colors

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

    #endregion

}