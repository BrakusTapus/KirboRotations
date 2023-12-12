using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic.Traits;

namespace KirboRotations.Utility;

public class KirboRotation : IKirboRotation
{
    public Job[] Jobs { get; }
    public ClassJob ClassJob => kService.GetSheet<ClassJob>().GetRow((uint)Jobs[0]);

    public string Name => ClassJob.Abbreviation;
}