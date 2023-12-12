using ECommons.DalamudServices;
using Lumina.Excel;

namespace KirboRotations.Utility.Service;

public class kService
{
    public const string USERNAME = "Kirbo";

    public static ExcelSheet<T> GetSheet<T>() where T : ExcelRow
    {
        return Svc.Data.GetExcelSheet<T>();
    }
}