using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility;
using KirboRotations.Custom.ExtraHelpers;
using static KirboRotations.Custom.ExtraHelpers.GeneralHelpers;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace KirboRotations.Utility.ImGuiEx;

/// <summary>
/// 
/// </summary>
public static unsafe partial class ImGuiExtra
{
    /// <summary>
    /// 
    /// </summary>
    public static float GlobalScale { get; private set; }

    /// <summary>
    ///
    /// </summary>
    /// <param name="tooltip"></param>
    /// <param name="btn"></param>
    /// <param name="requireCtrl"></param>
    /// <returns></returns>
    public static bool HoveredAndClicked(string tooltip = null, ImGuiMouseButton btn = ImGuiMouseButton.Left, bool requireCtrl = false)
    {
        if (ImGui.IsItemHovered())
        {
            if (tooltip != null)
            {
                SetTooltip(tooltip);
            }
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
            return (!requireCtrl || ImGui.GetIO().KeyCtrl) && ImGui.IsItemClicked(btn);
        }
        return false;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="condition"></param>
    /// <returns></returns>
    public static bool ButtonCond(string name, Func<bool> condition)
    {
        var dis = !condition();
        if (dis) ImGui.BeginDisabled();
        var ret = ImGui.Button(name);
        if (dis) ImGui.EndDisabled();
        return ret;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="num"></param>
    /// <returns></returns>
    public static bool InputLong(string id, ref long num)
    {
        var txt = num.ToString();
        var ret = ImGui.InputText(id, ref txt, 50);
        long.TryParse(txt, out num);
        return ret;
    }

    /// <summary>
    /// Provides a button that can be used to switch <see langword="bool"/>? variables. Left click - to toggle between <see langword="true"/> and <see langword="null"/>, right click - to toggle between <see langword="false"/> and <see langword="null"/>.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="TrueColor">Color when <paramref name="value"/> is true</param>
    /// <param name="FalseColor">Color when <paramref name="value"/> is false</param>
    /// <param name="smallButton">Whether a button should be small</param>
    /// <returns></returns>
    public static bool ButtonCheckbox(string name, ref bool? value, Vector4? TrueColor = null, Vector4? FalseColor = null, bool smallButton = false)
    {
        TrueColor ??= EColor.Green;
        FalseColor ??= EColor.Red;
        var col = value;
        var ret = false;
        if (col == true)
        {
            ImGui.PushStyleColor(ImGuiCol.Button, TrueColor.Value);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, TrueColor.Value);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, TrueColor.Value);
        }
        else if (col == false)
        {
            ImGui.PushStyleColor(ImGuiCol.Button, FalseColor.Value);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, FalseColor.Value);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, FalseColor.Value);
        }
        if (smallButton ? ImGui.SmallButton(name) : ImGui.Button(name))
        {
            if (value == null || value == false)
            {
                value = true;
            }
            else
            {
                value = false;
            }
            ret = true;
        }
        if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
        {
            if (value == null || value == true)
            {
                value = false;
            }
            else
            {
                value = true;
            }
            ret = true;
        }
        if (col != null) ImGui.PopStyleColor(3);
        return ret;
    }

    /// <summary>
    /// Converts RGB color to <see cref="Vector4"/> for ImGui
    /// </summary>
    /// <param name="col">Color in format 0xRRGGBB</param>
    /// <param name="alpha">Optional transparency value between 0 and 1</param>
    /// <returns>Color in <see cref="Vector4"/> format ready to be used with <see cref="ImGui"/> functions</returns>
    public static Vector4 Vector4FromRGB(this uint col, float alpha = 1.0f)
    {
        byte* bytes = (byte*)&col;
        return new Vector4((float)bytes[2] / 255f, (float)bytes[1] / 255f, (float)bytes[0] / 255f, alpha);
    }

    /// <summary>
    /// Converts RGBA color to <see cref="Vector4"/> for ImGui
    /// </summary>
    /// <param name="col">Color in format 0xRRGGBBAA</param>
    /// <returns>Color in <see cref="Vector4"/> format ready to be used with <see cref="ImGui"/> functions</returns>
    public static Vector4 Vector4FromRGBA(this uint col)
    {
        byte* bytes = (byte*)&col;
        return new Vector4((float)bytes[3] / 255f, (float)bytes[2] / 255f, (float)bytes[1] / 255f, (float)bytes[0] / 255f);
    }

    /// <summary>
    /// Draws a button that acts like a checkbox.
    /// </summary>
    /// <param name="name">Button text</param>
    /// <param name="value">Value</param>
    /// <param name="smallButton">Whether button should be small</param>
    /// <returns>true when clicked, otherwise false</returns>
    public static bool ButtonCheckbox(string name, ref bool value, bool smallButton = false) => ButtonCheckbox(name, ref value, EColor.Red, smallButton);

    /// <summary>
    /// Draws a button that acts like a checkbox.
    /// </summary>
    /// <param name="name">Button text</param>
    /// <param name="value">Value</param>
    /// <param name="color">Active button color</param>
    /// <param name="smallButton">Whether button should be small</param>
    /// <returns>true when clicked, otherwise false</returns>
    public static bool ButtonCheckbox(string name, ref bool value, Vector4 color, bool smallButton = false)
    {
        var col = value;
        var ret = false;
        if (col)
        {
            ImGui.PushStyleColor(ImGuiCol.Button, color);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, color);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, color);
        }
        if (smallButton ? ImGui.SmallButton(name) : ImGui.Button(name))
        {
            value = !value;
            ret = true;
        }
        if (col) ImGui.PopStyleColor(3);
        return ret;
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="collection"></param>
    /// <param name="smallButton"></param>
    /// <returns></returns>
    public static bool CollectionButtonCheckbox<T>(string name, T value, HashSet<T> collection, bool smallButton = false) => CollectionButtonCheckbox(name, value, collection, EColor.Red, smallButton);

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="collection"></param>
    /// <param name="color"></param>
    /// <param name="smallButton"></param>
    /// <returns></returns>
    public static bool CollectionButtonCheckbox<T>(string name, T value, HashSet<T> collection, Vector4 color, bool smallButton = false)
    {
        var col = collection.Contains(value);
        var ret = false;
        if (col)
        {
            ImGui.PushStyleColor(ImGuiCol.Button, color);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, color);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, color);
        }
        if (smallButton ? ImGui.SmallButton(name) : ImGui.Button(name))
        {
            if (col)
            {
                collection.Remove(value);
            }
            else
            {
                collection.Add(value);
            }
            ret = true;
        }
        if (col) ImGui.PopStyleColor(3);
        return ret;
    }

    /// <summary>
    /// Draws two radio buttons for a boolean value.
    /// </summary>
    /// <param name="labelTrue">True choice radio button text</param>
    /// <param name="labelFalse">False choice radio button text</param>
    /// <param name="value">Value</param>
    /// <param name="sameLine">Whether to draw radio buttons on the same line</param>
    /// <param name="prefix">Will be invoked before each radio button draw</param>
    /// <param name="suffix">Will be invoked after each radio button draw</param>
    public static void RadioButtonBool(string labelTrue, string labelFalse, ref bool value, bool sameLine = false, Action prefix = null, Action suffix = null)
    {
        prefix?.Invoke();
        if (ImGui.RadioButton(labelTrue, value)) value = true;
        suffix?.Invoke();
        if (sameLine) ImGui.SameLine();
        prefix?.Invoke();
        if (ImGui.RadioButton(labelFalse, !value)) value = false;
        suffix?.Invoke();
    }

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

    /// <summary>
    ///
    /// </summary>
    /// <param name="text"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    public static bool CollapsingHeader(string text, Vector4? col = null)
    {
        if (col != null) ImGui.PushStyleColor(ImGuiCol.Text, col.Value);
        var ret = ImGui.CollapsingHeader(text);
        if (col != null) ImGui.PopStyleColor();
        return ret;
    }


    /// <summary>
    ///
    /// </summary>
    /// <param name="text"></param>
    /// <param name="affix"></param>
    /// <returns></returns>
    public static bool ButtonCtrl(string text, string affix = " (Hold CTRL)") => ButtonCtrl(text, null, affix);

    /// <summary>
    /// Button that is disabled unless CTRL key is held
    /// </summary>
    /// <param name="text">Button ID</param>
    /// <param name="affix">Button affix</param>
    /// <returns></returns>
    public static bool ButtonCtrl(string text, Vector2? size, string affix = " (Hold CTRL)")
    {
        var disabled = !ImGui.GetIO().KeyCtrl;
        if (disabled)
        {
            ImGui.BeginDisabled();
        }
        var name = string.Empty;
        if (text.Contains($"###"))
        {
            var p = text.Split($"###");
            name = $"{p[0]}{affix}###{p[1]}";
        }
        else if (text.Contains($"##"))
        {
            var p = text.Split($"##");
            name = $"{p[0]}{affix}##{p[1]}";
        }
        else
        {
            name = $"{text}{affix}";
        }
        var ret = size == null?ImGui.Button(name):ImGui.Button(name, size.Value);
        if (disabled)
        {
            ImGui.EndDisabled();
        }
        return ret;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="popupId"></param>
    /// <returns></returns>
    public static bool BeginPopupNextToElement(string popupId)
    {
        ImGui.SameLine(0, 0);
        var pos = ImGui.GetCursorScreenPos();
        ImGui.Dummy(Vector2.Zero);
        ImGui.SetNextWindowPos(pos, ImGuiCond.Appearing);
        return ImGui.BeginPopup(popupId);
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="label"></param>
    /// <param name="value"></param>
    /// <param name="collection"></param>
    /// <returns></returns>
    public static bool CollectionCheckbox<T>(string label, T value, HashSet<T> collection)
    {
        var x = collection.Contains(value);
        if (ImGui.Checkbox(label, ref x))
        {
            if (x)
            {
                collection.Add(value);
            }
            else
            {
                collection.Remove(value);
            }
            return true;
        }
        return false;
    }

    /// <summary>
    ///
    /// </summary>
    public record HeaderIconOptions
    {
        public Vector2 Offset { get; init; } = Vector2.Zero;
        public ImGuiMouseButton MouseButton { get; init; } = ImGuiMouseButton.Left;
        public string Tooltip { get; init; } = string.Empty;
        public uint Color { get; init; } = 0xFFFFFFFF;
        public bool ToastTooltipOnClick { get; init; } = false;
        public ImGuiMouseButton ToastTooltipOnClickButton { get; init; } = ImGuiMouseButton.Left;
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="label"></param>
    /// <param name="value"></param>
    /// <param name="collection"></param>
    /// <param name="inverted"></param>
    /// <returns></returns>
    public static bool CollectionCheckbox<T>(string label, T value, List<T> collection, bool inverted = false)
    {
        var x = collection.Contains(value);
        if (inverted) x = !x;
        if (ImGui.Checkbox(label, ref x))
        {
            if (inverted) x = !x;
            if (x)
            {
                collection.Add(value);
            }
            else
            {
                collection.RemoveAll(x => x.Equals(value));
            }
            return true;
        }
        return false;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="col"></param>
    /// <param name="r"></param>
    /// <param name="g"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static Vector4 MutateColor(ImGuiCol col, byte r, byte g, byte b)
    {
        return ImGui.GetStyle().Colors[(int)col] with { X = (float)r / 255f, Y = (float)g / 255f, Z = (float)b / 255f };
    }

    /// <summary>
    /// Displays ImGui.SliderFloat for internal int value.
    /// </summary>
    /// <param name="id">ImGui ID</param>
    /// <param name="value">Integer value</param>
    /// <param name="min">Minimal value</param>
    /// <param name="max">Maximum value</param>
    /// <param name="divider">Value is divided by divider before being presented to user</param>
    /// <returns></returns>
    public static bool SliderIntAsFloat(string id, ref int value, int min, int max, float divider = 1000)
    {
        var f = (float)value / divider;
        var ret = ImGui.SliderFloat(id, ref f, (float)min / divider, (float)max / divider);
        if (ret)
        {
            value = (int)(f * divider);
        }
        return ret;
    }

    internal unsafe static bool NoPaddingNoColorImageButton(nint handle, Vector2 size, string id = "")
    {
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, 0);
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, 0);
        ImGui.PushStyleColor(ImGuiCol.Button, 0);
        var result = NoPaddingImageButton(handle, size, id);
        ImGui.PopStyleColor(3);

        return result;
    }

    internal static bool NoPaddingImageButton(nint handle, Vector2 size, string id = "")
    {
        var padding = ImGui.GetStyle().FramePadding;
        ImGui.GetStyle().FramePadding = Vector2.Zero;

        ImGui.PushID(id);
        var result = ImGui.ImageButton(handle, size);
        ImGui.PopID();
        if (ImGui.IsItemHovered())
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
        }

        ImGui.GetStyle().FramePadding = padding;
        return result;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="key"></param>
    /// <param name="repeat"></param>
    /// <returns></returns>
    public static bool IsKeyPressed(int key, bool repeat)
    {
        byte repeat2 = (byte)(repeat ? 1 : 0);
        return ImGuiNative.igIsKeyPressed((ImGuiKey)key, repeat2) != 0;
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public static float GetWindowContentRegionWidth()
    {
        return ImGui.GetWindowContentRegionMax().X - ImGui.GetWindowContentRegionMin().X;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="pix"></param>
    /// <param name="accountForScale"></param>
    public static void Spacing(float pix = 10f, bool accountForScale = true)
    {
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + (accountForScale ? pix : pix * ImGuiHelpers.GlobalScale));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="f"></param>
    /// <returns></returns>
    public static float Scale(this float f)
    {
        return f * ImGuiHelpers.GlobalScale;
    }

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

    private static readonly Dictionary<string, float> CenteredLineWidths = new();

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="func"></param>
    public static void ImGuiLineCentered(string id, Action func)
    {
        if (CenteredLineWidths.TryGetValue(id, out var dims))
        {
            ImGui.SetCursorPosX(ImGui.GetContentRegionAvail().X / 2 - dims / 2);
        }
        var oldCur = ImGui.GetCursorPosX();
        func();
        ImGui.SameLine(0, 0);
        CenteredLineWidths[id] = ImGui.GetCursorPosX() - oldCur;
        ImGui.Dummy(Vector2.Zero);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="mod"></param>
    public static void SetNextItemFullWidth(int mod = 0)
    {
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X + mod);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="percent"></param>
    /// <param name="mod"></param>
    public static void SetNextItemWidth(float percent, int mod = 0)
    {
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X * percent + mod);
    }

    private static Dictionary<string, float> InputWithRightButtonsAreaValues = new();

    /// <summary>
    /// Convenient way to display stretched input with button or other elements on it's right side.
    /// </summary>
    /// <param name="id">Unique ID</param>
    /// <param name="inputAction">A single element that accepts transformation by ImGui.SetNextItemWidth method</param>
    /// <param name="rightAction">A line of elements on the right side. Can contain multiple elements but only one line.</param>
    public static void InputWithRightButtonsArea(string id, Action inputAction, Action rightAction)
    {
        if (InputWithRightButtonsAreaValues.ContainsKey(id))
        {
            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - InputWithRightButtonsAreaValues[id]);
        }
        inputAction();
        ImGui.SameLine();
        var cur1 = ImGui.GetCursorPosX();
        rightAction();
        ImGui.SameLine(0, 0);
        InputWithRightButtonsAreaValues[id] = ImGui.GetCursorPosX() - cur1 + ImGui.GetStyle().ItemSpacing.X;
        ImGui.Dummy(Vector2.Zero);
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="list"></param>
    /// <param name="overrideValues"></param>
    /// <param name="addFunction"></param>
    public static void InputList<T>(string name, List<T> list, Dictionary<T, string> overrideValues, Action addFunction)
    {
        var text = list.Count == 0 ? "- No values -" : (list.Count == 1 ? $"{(overrideValues != null && overrideValues.ContainsKey(list[0]) ? overrideValues[list[0]] : list[0])}" : $"- {list.Count} elements -");
        if (ImGui.BeginCombo(name, text))
        {
            addFunction();
            var rem = -1;
            for (var i = 0; i < list.Count; i++)
            {
                var id = $"{name}ECommonsDeleItem{i}";
                var x = list[i];
                ImGui.Selectable($"{(overrideValues != null && overrideValues.ContainsKey(x) ? overrideValues[x] : x)}");
                if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
                {
                    ImGui.OpenPopup(id);
                }
                if (ImGui.BeginPopup(id))
                {
                    if (ImGui.Selectable("Delete##ECommonsDeleItem"))
                    {
                        rem = i;
                    }
                    if (ImGui.Selectable("Clear (hold shift+ctrl)##ECommonsDeleItem")
                        && ImGui.GetIO().KeyShift && ImGui.GetIO().KeyCtrl)
                    {
                        rem = -2;
                    }
                    ImGui.EndPopup();
                }
            }
            if (rem > -1)
            {
                list.RemoveAt(rem);
            }
            if (rem == -2)
            {
                list.Clear();
            }
            ImGui.EndCombo();
        }
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

    /// <summary>
    /// Calculates the remaining vertical space in the current ImGui window.
    /// </summary>
    /// <returns>The height of the remaining vertical space in pixels.</returns>
    public static float CalculateRemainingVerticalSpace()
    {
        // Get the total window height
        Vector2 windowSize = ImGui.GetWindowSize();

        // Get the current cursor position
        Vector2 cursorPos = ImGui.GetCursorPos();

        // Calculate the remaining space
        float remainingSpace = windowSize.Y - cursorPos.Y;

        return remainingSpace;
    }

    /// <summary>
    /// Aligns text vertically to a standard size button.
    /// </summary>
    /// <param name="col">Color</param>
    /// <param name="s">Text</param>
    public static void TextV(Vector4? col, string s)
    {
        if (col != null) ImGui.PushStyleColor(ImGuiCol.Text, col.Value);
        ImGuiExtra.TextV(s);
        if (col != null) ImGui.PopStyleColor();
    }

    /// <summary>
    /// Aligns text vertically to a standard size button.
    /// </summary>
    /// <param name="s">Text</param>
    public static void TextV(string s)
    {
        var cur = ImGui.GetCursorPos();
        ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0);
        ImGui.Button("");
        ImGui.PopStyleVar();
        ImGui.SameLine();
        ImGui.SetCursorPos(cur);
        ImGui.TextUnformatted(s);
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
        ImGuiExtra.Text(col, s);
        ImGui.PopTextWrapPos();
    }

    public static void TextWrapped(uint col, string s)
    {
        ImGui.PushTextWrapPos();
        ImGuiExtra.Text(col, s);
        ImGui.PopTextWrapPos();
    }

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

    public static void InvisibleButton(int width = 0)
    {
        ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0);
        ImGui.Button(" ");
        ImGui.PopStyleVar();
    }

    public static bool IconButton(FontAwesomeIcon icon, string id = "IconButton", Vector2 size = default)
    {
        ImGui.PushFont(UiBuilder.IconFont);
        var result = ImGui.Button($"{icon.ToIconString()}##{icon.ToIconString()}-{id}", size);
        ImGui.PopFont();
        return result;
    }

    public static bool SmallIconButton(string icon, string id = "SmallIconButton")
    {
        ImGui.PushFont(UiBuilder.IconFont);
        var result = ImGui.SmallButton($"{icon}##{icon}-{id}");
        ImGui.PopFont();
        return result;
    }

    public static bool IconButton(string icon, string id = "IconButton")
    {
        ImGui.PushFont(UiBuilder.IconFont);
        var result = ImGui.Button($"{icon}##{icon}-{id}");
        ImGui.PopFont();
        return result;
    }

    public static Vector2 CalcIconSize(FontAwesomeIcon icon)
    {
        ImGui.PushFont(UiBuilder.IconFont);
        var result = ImGui.CalcTextSize($"{icon.ToIconString()}");
        ImGui.PopFont();
        return result;
    }

    public static float Measure(Action func, bool includeSpacing = true)
    {
        var pos = ImGui.GetCursorPosX();
        func();
        ImGui.SameLine(0, 0);
        var diff = ImGui.GetCursorPosX() - pos;
        ImGui.Dummy(Vector2.Zero);
        return diff + (includeSpacing ? ImGui.GetStyle().ItemSpacing.X : 0);
    }

    public static void InputHex(string name, ref uint hexInt)
    {
        var text = $"{hexInt:X}";
        if (ImGui.InputText(name, ref text, 8))
        {
            if (uint.TryParse(text.Replace("0x", ""), NumberStyles.HexNumber, null, out var num))
            {
                hexInt = num;
            }
        }
    }

    public static void InputHex(string name, ref byte hexByte)
    {
        var text = $"{hexByte:X}";
        if (ImGui.InputText(name, ref text, 2))
        {
            if (byte.TryParse(text, NumberStyles.HexNumber, null, out var num))
            {
                hexByte = num;
            }
        }
    }

    public static void InputUint(string name, ref uint uInt)
    {
        var text = $"{uInt}";
        if (ImGui.InputText(name, ref text, 16))
        {
            if (uint.TryParse(text, out var num))
            {
                uInt = num;
            }
        }
    }

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

    public static void Text(Vector4? col, string text)
    {
        if (col == null)
        {
            Text(text);
        }
        else
        {
            Text(col.Value, text);
        }
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

    public static unsafe bool BeginTabItem(string label, ImGuiTabItemFlags flags)
    {
        int num = 0;
        byte* ptr;
        if (label != null)
        {
            num = Encoding.UTF8.GetByteCount(label);
            ptr = Allocate(num + 1);
            int utf = GetUtf8(label, ptr, num);
            ptr[utf] = 0;
        }
        else
        {
            ptr = null;
        }

        byte* p_open2 = null;
        byte num2 = ImGuiNative.igBeginTabItem(ptr, p_open2, flags);
        if (num > 2048)
        {
            Free(ptr);
        }
        return num2 != 0;
    }

    internal static unsafe byte* Allocate(int byteCount)
    {
        return (byte*)(void*)Marshal.AllocHGlobal(byteCount);
    }

    internal static unsafe void Free(byte* ptr)
    {
        Marshal.FreeHGlobal((IntPtr)ptr);
    }

    internal static unsafe int GetUtf8(string s, byte* utf8Bytes, int utf8ByteCount)
    {
        fixed (char* chars = s)
        {
            return Encoding.UTF8.GetBytes(chars, s.Length, utf8Bytes, utf8ByteCount);
        }
    }

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
    /// <param name="color">The Vector4 color value for the second part of the text.</param>
    public static void ImGuiColoredText(string text1, string text2, Vector4 color)
    {
        ImGui.Text(text1);
        ImGui.SameLine();

        ImGui.PushStyleColor(ImGuiCol.Text, color);
        ImGui.Text(text2);
        ImGui.PopStyleColor();
    }

    /// <summary>
    /// Displays two parts of text using ImGui, with the second part (an integer) in a specified color.
    /// </summary>
    /// <param name="text1">The first part of the text, displayed in default color.</param>
    /// <param name="integer">The integer to display in the specified color.</param>
    /// <param name="color">The Vector4 color value for the integer part of the text.</param>
    public static void ImGuiColoredText(string text1, int integer, Vector4 color)
    {
        ImGui.Text(text1);
        ImGui.SameLine();

        ImGui.PushStyleColor(ImGuiCol.Text, color);
        ImGui.Text(integer.ToString());
        ImGui.PopStyleColor();
    }
    #endregion Text

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
    #endregion Spacing

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
    public static bool ColoredButton(string label, Vector2 size, Vector4 color, bool enabled = true)
    {
        Vector4 colorValue = color;
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
    /// Creates a Icon button that resets the Opener Value's
    /// </summary>
    internal static void DisplayResetButton(string label)
    {
        if (ImGuiExtra.IconButton(FontAwesomeIcon.Medkit, "Reset", default))
        {
            OpenerHelpers.ResetOpenerProperties();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="label"></param>
    internal static void CopyCurrentValues(string label)
    {
        if (ImGuiExtra.IconButton(FontAwesomeIcon.Clipboard, "ClipBoard", default))
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
    #endregion Buttons
}

[StructLayout(LayoutKind.Explicit)]
public struct ImGuiWindow
{
    [FieldOffset(0xC)] public ImGuiWindowFlags Flags;

    [FieldOffset(0xD5)] public byte HasCloseButton;

    // 0x118 is the start of ImGuiWindowTempData
    [FieldOffset(0x130)] public Vector2 CursorMaxPos;
}

public static unsafe partial class ImGuiExtra
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