using ImGuiNET;
using KirboRotations.Custom.Configurations;
using KirboRotations.Custom.Configurations.Enums;
using KirboRotations.Custom.Data;
using KirboRotations.Custom.UI;
using KirboRotations.PvE.Ranged;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Attributes;
using RotationSolver.Basic.Configuration.RotationConfig;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Helpers;
using RotationSolver.Basic.Rotations.Basic;

namespace KirboRotations.JobHelpers;

internal class MCHHelper : PvE_MCH_Kirbo
{
    // Holds the remaining amount of Heat stacks
    internal static byte HeatStacks
    {
        get
        {
            byte stacks = Player.StatusStack(true, StatusID.Overheated);
            return stacks == byte.MaxValue ? (byte)5 : stacks;
        }
    }
}