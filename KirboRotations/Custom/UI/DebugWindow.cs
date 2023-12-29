using KirboRotations.Custom.Data;
using KirboRotations.Custom.ExtraHelpers;
using KirboRotations.Utility.ImGuiEx;

namespace KirboRotations.Custom.UI;

public class DebugWindow
{
    /// <summary>
    /// Displays "Error Caught"
    /// </summary>
    public static string ErrorMsg => "Error Caught";

    public static void DisplayDebugWindow(string RotationName, string RotationVersion, RotationData rotationData)
    {
        try
        {
            ImGuiExtra.TripleSpacing();
            ImGuiExtra.CollapsingHeaderWithContent("General Info", () =>
            {
                ImGuiExtra.Tooltip("Displays General information like:\n-Rotation Name\n-Player's Health\n-InCombat Status");
                if (ImGui.BeginTable("generalInfoTable", 2))
                {
                    ImGui.TableSetupColumn("Description"); ImGui.TableSetupColumn("Value"); ImGui.TableHeadersRow();
                    ImGuiExtra.AddTableRow("Rotation", $"{RotationName}");
                    ImGuiExtra.AddTableRow("RotationVersion", $"{rotationData.RotationVersion}");
                    ImGui.EndTable();
                }

                if (ImGui.CollapsingHeader("Rotation Features"))
                {
                    if (ImGui.BeginTable("featuresTable", 3))
                    {
                        // Set up columns
                        ImGui.TableSetupColumn("Ultimate Compatibilities");
                        ImGui.TableSetupColumn("Content Compatibilities");
                        ImGui.TableSetupColumn("Rotation Features");
                        ImGui.TableHeadersRow();

                        // Determine the maximum number of rows needed
                        int maxRows = Math.Max(rotationData.UltimateCompatibilities.Count, Math.Max(rotationData.ContentCompatibilities.Count, rotationData.FeaturesList.Count));

                        for (int i = 0; i < maxRows; i++)
                        {
                            ImGui.TableNextRow();

                            // Ultimate Compatibilities Column
                            ImGui.TableSetColumnIndex(0);
                            if (i < rotationData.UltimateCompatibilities.Count)
                            {
                                ImGui.Text(rotationData.UltimateCompatibilities[i].ToString());
                            }

                            // Content Compatibilities Column
                            ImGui.TableSetColumnIndex(1);
                            if (i < rotationData.ContentCompatibilities.Count)
                            {
                                ImGui.Text(rotationData.ContentCompatibilities[i].ToString());
                            }

                            // Rotation Features Column
                            ImGui.TableSetColumnIndex(2);
                            if (i < rotationData.FeaturesList.Count)
                            {
                                ImGui.Text(rotationData.FeaturesList[i].ToString());
                            }
                        }

                        ImGui.EndTable();
                    }
                }

            });
            
            ImGuiExtra.TripleSpacing();
            ImGuiExtra.CollapsingHeaderWithContent("Rotation Status", () =>
            {
                ImGuiExtra.Tooltip("Displays Rotation information like:\n-Selected Rotation\n-Opener Status");

                if (ImGui.BeginTable("rotationStatusTable", 2))
                {
                    ImGui.TableSetupColumn("Description"); ImGui.TableSetupColumn("Value"); ImGui.TableHeadersRow();
                    if (rotationData.RotationOpeners.Count > 0)
                    {
                        string rotationOpener = rotationData.GetCurrentRotationOpener();
                        ImGuiExtra.AddTableRow("Rotation Opener", rotationOpener);
                    }
                    else
                    {
                        ImGuiExtra.AddTableRow("Rotation Opener", "No Openers Configured");
                    }
                    ImGuiExtra.AddTableRow("OpenerActionsAvailable", OpenerHelpers.OpenerActionsAvailable.ToString());
                    ImGuiExtra.AddTableRow("OpenerInProgress", OpenerHelpers.OpenerInProgress.ToString());
                    ImGuiExtra.AddTableRow("Openerstep", OpenerHelpers.OpenerStep.ToString());
                    ImGuiExtra.AddTableRow("OpenerHasFailed", OpenerHelpers.OpenerHasFailed.ToString());
                    ImGuiExtra.AddTableRow("OpenerHasFinished", OpenerHelpers.OpenerHasFinished.ToString());
                    ImGui.EndTable();
                }

                ImGuiExtra.DisplayResetButton("Reset Properties");
                ImGuiExtra.StartRotationTimer("Rotation Test");
                
            });

            /* Burst Status
            ImGuiExtra.TripleSpacing();
            ImGuiExtra.CollapsingHeaderWithContent("", () =>
            {
                ImGuiExtra.Tooltip("Displays Burst information like:\n-Burst Available\n-Burst HasFailed");
                if (ImGui.BeginTable("burstStatusTable", 2))
                {
                    ImGui.TableSetupColumn("Description"); ImGui.TableSetupColumn("Value"); ImGui.TableHeadersRow();
                    ImGuiExtra.AddTableRow("BurstStep", BurstHelpers.BurstStep.ToString());
                    ImGuiExtra.AddTableRow("BurstActionsAvailable", BurstHelpers.BurstActionsAvailable.ToString());
                    ImGuiExtra.AddTableRow("BurstInProgress", BurstHelpers.BurstInProgress.ToString());
                    ImGuiExtra.AddTableRow("BurstHasFailed", BurstHelpers.BurstHasFailed.ToString());
                    ImGuiExtra.AddTableRow("BurstHasFinished", BurstHelpers.BurstHasFinished.ToString());
                    // ... other Burst status ...
                    ImGui.EndTable();
                }
            });
            */

            ImGuiExtra.TripleSpacing();
            ImGuiExtra.CollapsingHeaderWithContent("Action Details", () =>
            {
                ImGuiExtra.Tooltip("Displays action information like:\n-LastAction Used\n-LastGCD Used\n-LastAbility Used");
                if (ImGui.BeginTable("actionTable", 2))
                {
                    ImGui.TableSetupColumn("Description"); ImGui.TableSetupColumn("Value"); ImGui.TableHeadersRow();
                    //ImGuiExtra.AddTableRow("GCD Remain", DataCenter.WeaponRemain.ToString());
                    //ImGuiExtra.AddTableRow("LastGCD", DataCenter.LastGCD.ToString());
                    //ImGuiExtra.AddTableRow("LastAbility", DataCenter.LastAbility.ToString());
                    ImGui.EndTable();
                }
            });
        }
        catch (Exception ex)
        {
            Serilog.Log.Warning($"{ErrorMsg} - DisplayStatus: {ex.Message}");
        }
    }

}