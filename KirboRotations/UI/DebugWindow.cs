using System.Numerics;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Utility;
using ImGuiNET;
using KirboRotations.Configurations;
using KirboRotations.Extensions;
using KirboRotations.Helpers;
using KirboRotations.Helpers.JobHelpers;
using KirboRotations.PvE.Beta;
using KirboRotations.PvE.Ranged;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Helpers;
using RotationSolver.Basic.Rotations;

namespace KirboRotations.UI;

internal class DebugWindow : MCH_KirboBeta
{

    #region Debug Window for PvE Rotations
    private static bool _showImGuiDemoWindow = false;
    internal static void DisplayRotationTabs(string RotationName, RotationConfigs compatibilityAndFeatures)
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
                if (ImGui.BeginChild("ChildWindowTabBar", new Vector2(windowWidth, 300), true))
                {
                    if (ImGui.BeginTabBar("RotationTabBar"))
                    {
                        if (ImGui.BeginTabItem("General Info"))
                        {
                            DisplayGeneralInfoTab(RotationName.ToString());
                            ImGui.EndTabItem();
                        }
                        ImGuiExtra.Tooltip("Displays General information like:\n-Rotation Name\n-Player's Health\n-InCombat Status");

                        if (ImGui.BeginTabItem("Opener Status"))
                        {
                            DisplayRotationStatusTab(compatibilityAndFeatures);
                            ImGui.EndTabItem();
                        }
                        ImGuiExtra.Tooltip("Displays Rotation information like:\n-Opener Available\n-Burst HasFailed");

                        if (ImGui.BeginTabItem("Burst Status"))
                        {
                            DisplayBurstStatusTab();
                            ImGui.EndTabItem();
                        }
                        ImGuiExtra.Tooltip("Displays Burst information like:\n-Burst Available\n-Burst HasFailed");

                        if (ImGui.BeginTabItem("Compatibility and Features"))
                        {
                            DisplayCompatibilityAndFeaturesTab(compatibilityAndFeatures);
                            ImGui.EndTabItem();
                        }
                        ImGuiExtra.Tooltip("Displays Compatibility information like:\n-Ultimates\n-Content\n-Features");

                        ImGui.EndTabBar();
                    }
                }
                ImGui.EndChild();

                DebugWindow.DisplayBottomBar();
            }
        }
        catch (Exception ex)
        {
            Serilog.Log.Warning($"{ex}");
        }
    }

    private static void DisplayBottomBar()
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

    private static void DisplayGeneralInfoTab(string RotationName)
    {
        if (ImGui.BeginTable("generalInfoTable", 2))
        {
            ImGui.TableSetupColumn("Description"); ImGui.TableSetupColumn("Value"); ImGui.TableHeadersRow();
            ImGuiExtra.AddTableRowColorLast("Rotation Athor", $"{RotationName}", EColor.ParsedPink);
            ImGuiExtra.AddTableRowColorLast("RotationVersion", $"{RotationConfigs.RotationVersion}", EColor.ParsedGold);
            ImGui.EndTable();
        }
    }

    private static void DisplayRotationStatusTab(RotationConfigs compatibilityAndFeatures)
    {
        if (ImGui.BeginTable("openerStatusTable", 2))
        {
            ImGui.TableSetupColumn("Description"); ImGui.TableSetupColumn("Value"); ImGui.TableHeadersRow();

            if (compatibilityAndFeatures.RotationOpeners.Count > 0)
            {
                string rotationOpener = compatibilityAndFeatures.GetCurrentRotationOpener();
                ImGuiExtra.AddTableRowColorLast("Selected Opener", rotationOpener, EColor.ParsedOrange);
            }
            else
            {
                ImGuiExtra.AddTableRow("Rotation Opener", "No Openers Configured");
            }
            if (!OpenerHelpers.LvL70_Ultimate_OpenerActionsAvailable && !OpenerHelpers.LvL80_Ultimate_OpenerActionsAvailable)
            {
                ImGuiExtra.AddTableRow("OpenerActionsAvailable", OpenerHelpers.OpenerActionsAvailable);
            }
            if (OpenerHelpers.LvL80_Ultimate_OpenerActionsAvailable)
            {
                ImGuiExtra.AddTableRow("LvL80_Ultimate_OpenerActionsAvailable", OpenerHelpers.LvL80_Ultimate_OpenerActionsAvailable);
            }
            if (OpenerHelpers.LvL70_Ultimate_OpenerActionsAvailable)
            {
                ImGuiExtra.AddTableRow("LvL70_Ultimate_OpenerActionsAvailable", OpenerHelpers.LvL70_Ultimate_OpenerActionsAvailable);
            }
            ImGuiExtra.AddTableRow("OpenerHasFinished", OpenerHelpers.OpenerHasFinished);
            ImGuiExtra.AddTableRow("OpenerHasFailed", OpenerHelpers.OpenerHasFailed);
            ImGuiExtra.AddTableRow("OpenerInProgress", OpenerHelpers.OpenerInProgress);

            string stateAsString = OpenerHelpers.CurrentOpenerState.ToString();
            ImGuiExtra.SpacingWithSeperator();


            ImGuiExtra.AddTableRow("CanOpener", MCHLogic.CanOpener.ToString());
            ImGuiExtra.AddTableRow("PrePullStep", MCHLogic.PrePullStep.ToString());
            ImGuiExtra.AddTableRow("Current OpenerState", MCHLogic.CurrentState.ToString());
            ImGuiExtra.AddTableRow("OpenerStep", MCHLogic.OpenerStep.ToString());
            ImGuiExtra.AddTableRow("HasStatus Reassemble", Player.HasStatus(true, StatusID.Reassemble).ToString());
            ImGuiExtra.AddTableRow("Player IsInCombat", Player.IsInCombat().ToString());



            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGuiExtra.DisplayResetButton("Reset");
            ImGuiExtra.Tooltip($"Press this button if\n the rotation gets stuck during the Opener!\n All Opener related properties\n and the opener step will be resetted");
            ImGui.TableNextColumn();
            ImGuiExtra.CopyCurrentValues("ClipBoard");
            ImGuiExtra.Tooltip($"Click to copy the current value's of the properties to the clipboard.");
            ImGui.EndTable();
        }
    }

    private static void DisplayBurstStatusTab()
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

    private static void DisplayCompatibilityAndFeaturesTab(RotationConfigs compatibilityAndFeatures)
    {
        if (ImGui.BeginTable("featuresTable", 3))
        {
            // Set up columns
            ImGui.TableSetupColumn("Ultimate Compatibilities");
            ImGui.TableSetupColumn("Content Compatibilities");
            ImGui.TableSetupColumn("Rotation Features");
            ImGui.TableHeadersRow();

            // Determine the maximum number of rows needed
            int maxRows = Math.Max(compatibilityAndFeatures.UltimateCompatibilities.Count,
                               Math.Max(compatibilityAndFeatures.ContentCompatibilities.Count,
                                        compatibilityAndFeatures.FeaturesList.Count));

            for (int i = 0; i < maxRows; i++)
            {
                ImGui.TableNextRow();

                // Ultimate Compatibilities Column
                ImGui.TableSetColumnIndex(0);
                if (i < compatibilityAndFeatures.UltimateCompatibilities.Count)
                {
                    ImGui.Text(compatibilityAndFeatures.UltimateCompatibilities[i].ToString());
                }

                // Content Compatibilities Column
                ImGui.TableSetColumnIndex(1);
                if (i < compatibilityAndFeatures.ContentCompatibilities.Count)
                {
                    ImGui.Text(compatibilityAndFeatures.ContentCompatibilities[i].ToString());
                }

                // Rotation Features Column
                ImGui.TableSetColumnIndex(2);
                if (i < compatibilityAndFeatures.FeaturesList.Count)
                {
                    ImGui.Text(compatibilityAndFeatures.FeaturesList[i].ToString());
                }
            }

            ImGui.EndTable();
        }
    }
    #endregion

    #region Job related
    internal static void DisplayMCHTab()
    {
        try
        {
            var pos = ImGui.GetCursorScreenPos();
            ImGui.SetNextWindowPos(new Vector2(pos.X, pos.Y));
            if (ImGui.BeginChild("Child Window", new Vector2(145, 300), true))
            {
                ImGui.SetNextItemWidth(130); // Set the width of the table
                if (ImGui.BeginTable("generalInfoTable", 2))
                {
                    ImGui.TableSetupColumn("Description", ImGuiTableColumnFlags.WidthFixed, 75);
                    ImGui.TableSetupColumn("Value", ImGuiTableColumnFlags.WidthFixed, 50); ImGui.TableHeadersRow();
                    ImGuiExtra.AddTableRow("HeatStacks", MCHLogic.HeatStacks);
                    ImGuiExtra.AddTableRow("InLv70Ult", MCHLogic.InLvL70Ultimate);
                    ImGuiExtra.AddTableRow("ToolSoon", MCHLogic.WillhaveTool);

                    ImGui.EndTable();
                }
            }
            ImGui.EndChild();
        }
        catch (Exception ex)
        {
            Serilog.Log.Warning($"{ex}");
        }
    }
    #endregion

}