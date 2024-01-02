namespace KirboRotations.Custom.Configurations.Enums;

/// <summary>
/// <br>NotCompatible: Indicates that the rotation is not compatible with any content.</br>
/// <br>Compatible: Indicates that the rotation is compatible with some content.</br>
/// <br>Untested: Indicates that the compatibility with certain content has not been tested.</br>
/// </summary>
[Flags]
internal enum Compatibility
{
    NotCompatible = 0,
    Compatible = 1,
    Untested = 2,
}