using ECommons.ExcelServices;
using KirboRotations.Utility.Service;
using Lumina.Excel.GeneratedSheets;

namespace KirboRotations.Utility.KirboRotation;

public class KirboRotation : IKirboRotation
{
    public Job[] Jobs { get; }
    public ClassJob ClassJob => kService.GetSheet<ClassJob>().GetRow((uint)Jobs[0]);

    public string Name => ClassJob.Abbreviation;
}