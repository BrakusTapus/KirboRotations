using System.Numerics;
using ImGuiNET;
using KirboRotations.Configurations;
using KirboRotations.Extensions;
using KirboRotations.Helpers;
using KirboRotations.Helpers.JobHelpers;
using KirboRotations.PvE.Beta;
using Lumina.Excel.GeneratedSheets2;
using RotationSolver.RotationBasics.Data;
using RotationSolver.RotationBasics.Helpers;

namespace KirboRotations.UI;
internal class PvPDebugWindow : MCH_KirboPvEBeta
{
    #region Debug Window for PvE Rotations
    private static bool _showImGuiDemoWindow = false;

    internal static void DisplayPvPRotationTabs(string RotationName, RotationConfigs compatibilityAndFeatures)
    {
        BaseEx.CheckPlayerStatus();
        try
        {
            if (BaseEx.LoggedIn)
            {
                // Get the size of the parent window
                float parentWindowWidth = ImGui.GetWindowWidth();
                float windowWidth = parentWindowWidth - (ImGui.GetStyle().WindowPadding.X * 20); // Adjust for padding

                // First child window
                var pos = ImGui.GetCursorScreenPos();
                ImGui.SetNextWindowPos(new Vector2(pos.X, pos.Y));
                ImGui.SetNextWindowSize(new Vector2(windowWidth, 300)); // Set width dynamically
                if (ImGui.BeginChild("PvPChildWindowTabBar", new Vector2(windowWidth, 300), true))
                {
                    if (ImGui.BeginTabBar("PvPRotationTabBar"))
                    {
                        if (ImGui.BeginTabItem("General PvP Info"))
                        {
                            DisplayGeneralPvPInfoTab(RotationName.ToString());
                            ImGui.EndTabItem();
                        }
                        ImGuiExtra.Tooltip("Displays General PvP information like:\n-Rotation Name\n-Player's Health\n-InCombat Status");

                        if (ImGui.BeginTabItem("PvP Burst Status"))
                        {
                            DisplayPVPBurstStatusTab();
                            ImGui.EndTabItem();
                        }
                        ImGuiExtra.Tooltip("Displays PvP Burst information like:\n-Burst Available\n-Burst HasFailed");

                        if (ImGui.BeginTabItem("Compatibility and Features (PvP)"))
                        {
                            DisplayPvPCompatibilityAndFeaturesTab(compatibilityAndFeatures);
                            ImGui.EndTabItem();
                        }
                        ImGuiExtra.Tooltip("Displays Compatibility information like:\n-Ultimates\n-Content\n-Features");

                        ImGui.EndTabBar();
                    }
                }
                ImGui.EndChild();

                PvPDebugWindow.DisplayPvPBottomBar();
            }
        }
        catch (Exception ex)
        {
            Serilog.Log.Warning($"{ex}");
        }
    }

    private static void DisplayPvPBottomBar()
    {
        BaseEx.CheckPlayerStatus();
        try
        {
            if (BaseEx.LoggedIn)
            {
                float bottomBarHeight = 50;
                float windowWidth = ImGui.GetWindowWidth();

                var pos = ImGui.GetCursorScreenPos();
                ImGui.SetNextWindowPos(new Vector2(pos.X, pos.Y));
                ImGui.SetNextWindowSize(new Vector2(windowWidth, bottomBarHeight));
                if (ImGui.BeginChild("BottomBar", new Vector2(windowWidth, bottomBarHeight), true))
                {
                    ImGui.Checkbox("ImGui Demo Window", ref _showImGuiDemoWindow);
                    // 3. Show the ImGui demo window. Most of the sample code is in ImGui.ShowDemoWindow(). Read its code to learn more about Dear ImGui!
                    if (_showImGuiDemoWindow)
                    {
                        // Normally user code doesn't need/want to call this because positions are saved in .ini file anyway.
                        // Here we just want to make the demo initial state a bit more friendly!
                        ImGui.SetNextWindowPos(new Vector2(650, 20), ImGuiCond.FirstUseEver);
                        ImGui.ShowDemoWindow(ref _showImGuiDemoWindow);
                    }
                    ImGuiExtra.Tooltip("This is a tooltip");

                    ImGui.SameLine();
                }
                ImGui.EndChild();
            }
        }
        catch (Exception ex)
        {
            Serilog.Log.Warning($"{ex}");
        }
    }

    private static void DisplayGeneralPvPInfoTab(string RotationName)
    {
        if (ImGui.BeginTable("generalInfoTable", 2))
        {
            ImGui.TableSetupColumn("Description"); ImGui.TableSetupColumn("Value"); ImGui.TableHeadersRow();
            ImGuiExtra.AddTableRowColorLast("Rotation Athor", $"{RotationName}", EColor.ParsedPink);
            ImGuiExtra.AddTableRowColorLast("RotationVersion", $"{RotationConfigs.RotationVersion}", EColor.ParsedGold);
            ImGuiExtra.AddTableRowColorLast("In PvP Are", $"{BattleCharaEx.InPvP()}", EColor.RedBright);
            ImGui.EndTable();
        }
    }

    private static void DisplayPVPBurstStatusTab()
    {
        ImGuiExtra.Tooltip("Displays Burst information like:\n-Burst Available\n-Burst HasFailed");
        if (ImGui.BeginTable("burstStatusTable", 2))
        {
            ImGui.TableSetupColumn("Description"); ImGui.TableSetupColumn("Value"); ImGui.TableHeadersRow();
            ImGuiExtra.AddTableRow("In Burst", BurstHelpers.InBurst);
            ImGuiExtra.AddTableRow("BurstFlag", BurstHelpers.BurstFlag);
            ImGui.EndTable();
        }
    }

    private static void DisplayPvPCompatibilityAndFeaturesTab(RotationConfigs compatibilityAndFeatures)
    {
        if (ImGui.BeginTable("featuresTable", 2))
        {
            // Set up columns
            ImGui.TableSetupColumn("PvP Content Compatibilities");
            ImGui.TableSetupColumn("PvP Features");
            ImGui.TableHeadersRow();

            // Determine the maximum number of rows needed
            int maxRows = Math.Max(compatibilityAndFeatures.PvPContentCompatibilities.Count,compatibilityAndFeatures.PvPFeaturesList.Count);

            for (int i = 0; i < maxRows; i++)
            {
                ImGui.TableNextRow();

                // Content Compatibilities Column
                ImGui.TableSetColumnIndex(0);
                if (i < compatibilityAndFeatures.PvPContentCompatibilities.Count)
                {
                    ImGui.Text(compatibilityAndFeatures.PvPContentCompatibilities[i].ToString());
                }

                // Rotation Features Column
                ImGui.TableSetColumnIndex(1);
                if (i < compatibilityAndFeatures.PvPFeaturesList.Count)
                {
                    ImGui.Text(compatibilityAndFeatures.PvPFeaturesList[i].ToString());
                }
            }

            ImGui.EndTable();
        }
    }
    #endregion Debug Window for PvP Rotations

    #region Properties
    internal static bool FrontLine
    {
        get
        {
            return BattleCharaEx.InPvP() && PartyMembers.Count() >= 6 && PartyMembers.Count() <= 8;
        }
    }

    internal static bool CrystalineConflict
    {
        get
        {
            return BattleCharaEx.InPvP() && PartyMembers.Count() >= 2 && PartyMembers.Count() <= 5;
        }
    }
    #endregion

    #region Left Window
    internal static void DisplayPvPTab()
    {
        try
        {
            var pos = ImGui.GetCursorScreenPos();
            ImGui.SetNextWindowPos(new Vector2(pos.X, pos.Y));
            if (ImGui.BeginChild("PvP Child Window", new Vector2(155, 300), true))
            {
                ImGui.SetNextItemWidth(130); // Set the width of the table
                if (ImGui.BeginTable("generalPvPInfoTable", 2))
                {
                    ImGui.TableSetupColumn("Description", ImGuiTableColumnFlags.WidthFixed, 75);
                    ImGui.TableSetupColumn("Value", ImGuiTableColumnFlags.WidthFixed, 50); ImGui.TableHeadersRow();
                    ImGuiExtra.AddTableRowColorLast("Health", $"{Player.CurrentHp}", EColor.RedBright);
                    double healthPercentage = Player.GetHealthRatio() * 100;
                    string formattedHealthPercentage = healthPercentage.ToString("F2"); // "F2" format specifier for 2 decimal places
                    ImGuiExtra.AddTableRowColorLast("HPP", $"{formattedHealthPercentage} %%", EColor.OrangeBright);
                    ImGuiExtra.AddTableRowColorLast("Mana", $"{Player.CurrentMp}", EColor.VioletBright);
                    ImGui.Separator();
                    ImGuiExtra.AddTableRow("FrontLine", $"{FrontLine.ToString()}");
                    ImGuiExtra.AddTableRow("CC", $"{CrystalineConflict.ToString()}");

                    ImGui.EndTable();
                }
                ImGui.Separator();
                string targetName = Target != null ? Target.Name.ToString() : "No Target";
                ImGuiExtra.TextCentered($"Target: {targetName}");

                //ImGuiExtra.AddTableRow("HostileTarget Nearby", $"{NumberOfAllHostilesInRange.ToString()}");

            }
            ImGui.EndChild();
        }
        catch (Exception ex)
        {
            Serilog.Log.Warning($"{ex}");
        }
    }
    #endregion Left Window
}
