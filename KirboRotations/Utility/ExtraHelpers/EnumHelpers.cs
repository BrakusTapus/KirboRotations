namespace KirboRotations.Utility.ExtraHelpers;

/// <summary>
/// <br>NotCompatible: Indicates that the rotation is not compatible with any content.</br>
/// <br>Compatible: Indicates that the rotation is compatible with some content.</br>
/// <br>Untested: Indicates that the compatibility with certain content has not been tested.</br>
/// </summary>
[Flags]
public enum Compatibility
{
    NotCompatible = 0,
    Compatible = 1,
    Untested = 2,
}

/// <summary>
/// <br>UseTincture: Indicates that the rotation uses tinctures.</br>
/// <br>SavageOptimized: Indicates that the rotation is optimized for Savage content.</br>
/// <br>HasUserConfig: Indicates that the rotation has user-configurable settings.</br>
/// </summary>
[Flags]
public enum Features
{
    UseTincture = 0,
    SavageOptimized = 1,
    HasUserConfig = 2,
}

/// <summary>
/// <br>NotCompatible: Indicates that the rotation is not compatible with any Ultimate content.</br>
/// <br>UCoB: Indicates compatibility with The Unending Coil of Bahamut.</br>
/// <br>UwU: Indicates compatibility with The Weapon's Refrain (Ultimate).</br>
/// <br>TEA: Indicates compatibility with The Epic of Alexander (Ultimate).</br>
/// <br>DSR: Indicates compatibility with The Dragonsong's Reprise (Ultimate).</br>
/// <br>TOP: Indicates compatibility with The The Omega Protocol (Ultimate).</br>
/// </summary>
[Flags]
public enum UltimateCompatibility
{
    NotCompatible = 0,
    UCoB = 1,
    UwU = 2,
    TEA = 3,
    DSR = 4,
    TOP = 5,
}

/// <summary>
/// <br>NotCompatible: Indicates that the rotation is not compatible with any content.</br>
/// <br>Other flags represent compatibility with specific types of content such as dungeons, trials, raids, etc.</br>
/// </summary>
[Flags]
public enum ContentCompatibility
{
    NotCompatible = 0,
    DutyRoulette = 1,
    Dungeons = 2,
    Guildhests = 3,
    Trials = 4,
    ExtremeTrials = 5,
    NormalRaids = 6,
    AllianceRaids = 7,
    SavageRaids = 8,
    FATEs = 9,
    TreasureHunt = 10,
    DeepDungeons = 11,
    Eureka = 12,
    VariantDungeons = 13,
    Criterion = 14,
}
