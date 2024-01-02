namespace KirboRotations.Custom.Configurations.Enums;

/// <summary>
/// <br>UseTincture: Indicates that the rotation uses tinctures.</br>
/// <br>SavageOptimized: Indicates that the rotation is optimized for Savage content.</br>
/// <br>HasUserConfig: Indicates that the rotation has user-configurable settings.</br>
/// </summary>
[Flags]
internal enum Features
{
    None = 0,
    UseTincture = 1,
    SavageOptimized = 2,
    HasUserConfig = 3,
}