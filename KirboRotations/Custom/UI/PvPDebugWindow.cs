using KirboRotations.Custom.Data;
using KirboRotations.Custom.ExtraHelpers;
using KirboRotations.Custom.Utility.ImGuiEx;
using KirboRotations.Utility.ImGuiEx;
using static KirboRotations.Custom.ExtraHelpers.GeneralHelpers;

namespace KirboRotations.Custom.UI;

public class PvPDebugWindow
{
    /// <summary>
    /// Displays "Error Caught"
    /// </summary>
    public static string ErrorMsg => "Error Caught";

    /// <summary>
    /// WIP.
    /// </summary>
    /// <param name="RotationName"></param>
    /// <param name="RotationVersion"></param>
    /// <param name="rotationData"></param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void DisplayPvPDebugWindow(string RotationName, string RotationVersion, RotationData rotationData)
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
                ImGuiExtra.CopyCurrentValues("PvPClipboard");

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