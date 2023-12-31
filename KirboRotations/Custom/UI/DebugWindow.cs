using ImGuiNET;
using KirboRotations.Custom.Data;
using KirboRotations.Custom.ExtraHelpers;
using static KirboRotations.Custom.ExtraHelpers.GeneralHelpers;

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
                    ImGuiExtra.AddTableRowColorLast("Rotation Athor", $"{RotationName}", EColor.ParsedPink);
                    ImGuiExtra.AddTableRowColorLast("RotationVersion", $"{rotationData.RotationVersion}", EColor.ParsedGold);
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
                        ImGuiExtra.AddTableRowColorLast("Rotation Opener", rotationOpener, EColor.ParsedOrange);
                    }
                    else
                    {
                        ImGuiExtra.AddTableRow("Rotation Opener", "No Openers Configured");
                    }
                    ImGuiExtra.AddTableRow("OpenerActionsAvailable", OpenerHelpers.OpenerActionsAvailable);
                    ImGuiExtra.AddTableRow("OpenerHasFinished", OpenerHelpers.OpenerHasFinished);
                    ImGuiExtra.AddTableRow("OpenerHasFailed", OpenerHelpers.OpenerHasFailed);
                    ImGuiExtra.AddTableRow("OpenerInProgress", OpenerHelpers.OpenerInProgress);
                    ImGuiExtra.AddTableRow("Openerstep", OpenerHelpers.OpenerStep.ToString());

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGuiExtra.DisplayResetButton("Reset");                    
                    ImGuiExtra.Tooltip($"Press this button if\n the rotation gets stuck during the Opener!\n All Opener related properties\n and the opener step will be resetted");
                    ImGui.TableNextColumn();
                    ImGuiExtra.CopyCurrentValues("ClipBoard");
                    ImGuiExtra.Tooltip($"Click to copy the current value's of the properties to the clipboard.");
                    ImGui.EndTable();
                }

            });
            
            ImGuiExtra.TripleSpacing();
            ImGuiExtra.CollapsingHeaderWithContent("Burst Status", () =>
            {
                ImGuiExtra.Tooltip("Displays Burst information like:\n-Burst Available\n-Burst HasFailed");
                if (ImGui.BeginTable("burstStatusTable", 2))
                {
                    ImGui.TableSetupColumn("Description"); ImGui.TableSetupColumn("Value"); ImGui.TableHeadersRow();
                    ImGuiExtra.AddTableRow("In Burst", BurstHelpers.InBurst);
                    ImGuiExtra.AddTableRow("BurstFlag", BurstHelpers._burstFlag);                    
                    ImGui.EndTable();
                }
            });            

            ImGuiExtra.TripleSpacing();
            ImGuiExtra.CollapsingHeaderWithContent("Action Details", () =>
            {
                ImGuiExtra.Tooltip("Displays action information like:\n-LastAction Used\n-LastGCD Used\n-LastAbility Used");
                if (ImGui.BeginTable("actionTable", 2))
                {
                    ImGui.TableSetupColumn("Description"); ImGui.TableSetupColumn("Value"); ImGui.TableHeadersRow();
                    ImGui.EndTable();
                }
            });

        }
        catch (Exception ex)
        {
            Serilog.Log.Warning($"{v} {ErrorMsg} - DisplayStatus: {ex.Message}");
        }
    }

}