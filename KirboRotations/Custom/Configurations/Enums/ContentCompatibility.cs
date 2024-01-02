namespace KirboRotations.Custom.Configurations.Enums;

/// <summary>
/// <br>NotCompatible: Indicates that the rotation is not compatible with any content.</br>
/// <br>Other flags represent compatibility with specific types of content such as dungeons, trials, raids, etc.</br>
/// </summary>
[Flags]
internal enum ContentCompatibility
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
    Hunts = 15,
}