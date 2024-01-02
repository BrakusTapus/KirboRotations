namespace KirboRotations.Custom.Configurations.Enums;

/// <summary>
/// <br>NotCompatible: Indicates that the rotation is not compatible with any Ultimate content.</br>
/// <br>UCoB: Indicates compatibility with The Unending Coil of Bahamut.</br>
/// <br>UwU: Indicates compatibility with The Weapon's Refrain (Ultimate).</br>
/// <br>TEA: Indicates compatibility with The Epic of Alexander (Ultimate).</br>
/// <br>DSR: Indicates compatibility with The Dragonsong's Reprise (Ultimate).</br>
/// <br>TOP: Indicates compatibility with The The Omega Protocol (Ultimate).</br>
/// </summary>
[Flags]
internal enum UltimateCompatibility
{
    NotCompatible = 0,
    UCoB = 1,
    UwU = 2,
    TEA = 3,
    DSR = 4,
    TOP = 5,
}