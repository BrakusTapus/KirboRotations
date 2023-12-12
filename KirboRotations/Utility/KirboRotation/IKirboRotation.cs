using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommons.ExcelServices;
using Lumina.Excel.GeneratedSheets;

namespace KirboRotations.Utility.KirboRotation;

internal interface IKirboRotation
{
    /// <summary>
    /// The class job about this rotation.
    /// </summary>
    ClassJob ClassJob { get; }

    /// <summary>
    /// All jobs.
    /// </summary>
    Job[] Jobs { get; }

}
