namespace KirboRotations.Utility.ExtraHelpers;

/// <summary>
/// Provides methods for generating rotation compatibility descriptions.
/// </summary>
public static class DescriptionHelpers
{
    /// <summary>
    /// Displays "[vx.x.x.x]"
    /// </summary>
    public static string RotationVersion => "[v1.0.0.16]";

    /// <summary>
    /// Gets a user-friendly description of rotation compatibility with Ultimate fights.
    /// </summary>
    /// <param name="compatibility">The Ultimate compatibility flags for the rotation.</param>
    /// <returns>A description of the rotation's compatibility with Ultimate fights.</returns>
    public static string GetUltimateCompatibilityDescription(UltimateCompatibility compatibility)
    {
        List<string> compatibilityDescriptions = new List<string>();

        if ((compatibility & UltimateCompatibility.UCoB) != 0)
        {
            compatibilityDescriptions.Add("Compatible for The Unending Coil of Bahamut (UCoB).");
        }

        if ((compatibility & UltimateCompatibility.UwU) != 0)
        {
            compatibilityDescriptions.Add("Compatible for The Weapon's Refrain (UwU).");
        }

        if ((compatibility & UltimateCompatibility.TEA) != 0)
        {
            compatibilityDescriptions.Add("Compatible for The Epic of Alexander (TEA).");
        }

        if ((compatibility & UltimateCompatibility.DSR) != 0)
        {
            compatibilityDescriptions.Add("Compatible for The Epic of Alexander (DSR).");
        }

        if ((compatibility & UltimateCompatibility.TOP) != 0)
        {
            compatibilityDescriptions.Add("Compatible for The Epic of Alexander (TOP).");
        }

        if (compatibilityDescriptions.Count == 0)
        {
            return "Not recommended for any Ultimate fights.";
        }

        return string.Join("\n -", compatibilityDescriptions);
    }

    /// <summary>
    /// Gets a user-friendly description of rotation compatibility with different types of in-game content.
    /// </summary>
    /// <param name="compatibility">The content compatibility flags for the rotation.</param>
    /// <returns>A description of the rotation's compatibility with in-game content.</returns>
    public static string GetContentCompatibilityDescription(ContentCompatibility compatibility)
    {
        List<string> compatibilityDescriptions = new List<string>();

        if ((compatibility & ContentCompatibility.DutyRoulette) != 0)
        {
            compatibilityDescriptions.Add("Duty Roulette.");
        }

        if ((compatibility & ContentCompatibility.Dungeons) != 0)
        {
            compatibilityDescriptions.Add("Dungeons.");
        }

        if ((compatibility & ContentCompatibility.Guildhests) != 0)
        {
            compatibilityDescriptions.Add("Guildhests.");
        }

        if ((compatibility & ContentCompatibility.Trials) != 0)
        {
            compatibilityDescriptions.Add("Trials.");
        }

        if ((compatibility & ContentCompatibility.ExtremeTrials) != 0)
        {
            compatibilityDescriptions.Add("ExtremeTrials.");
        }

        if ((compatibility & ContentCompatibility.NormalRaids) != 0)
        {
            compatibilityDescriptions.Add("NormalRaids.");
        }

        if ((compatibility & ContentCompatibility.AllianceRaids) != 0)
        {
            compatibilityDescriptions.Add("AllianceRaids.");
        }

        if ((compatibility & ContentCompatibility.SavageRaids) != 0)
        {
            compatibilityDescriptions.Add("SavageRaids.");
        }

        if ((compatibility & ContentCompatibility.FATEs) != 0)
        {
            compatibilityDescriptions.Add("FATEs.");
        }

        if ((compatibility & ContentCompatibility.TreasureHunt) != 0)
        {
            compatibilityDescriptions.Add("TreasureHunt.");
        }

        if ((compatibility & ContentCompatibility.DeepDungeons) != 0)
        {
            compatibilityDescriptions.Add("DeepDungeons.");
        }

        if ((compatibility & ContentCompatibility.Eureka) != 0)
        {
            compatibilityDescriptions.Add("Eureka.");
        }

        if ((compatibility & ContentCompatibility.VariantDungeons) != 0)
        {
            compatibilityDescriptions.Add("VariantDungeons.");
        }

        if ((compatibility & ContentCompatibility.Criterion) != 0)
        {
            compatibilityDescriptions.Add("Criterion.");
        }

        if (compatibilityDescriptions.Count == 0)
        {
            return "Not recommended for any specific content.";
        }

        return string.Join("\n -", compatibilityDescriptions);
    }

    /// <summary>
    /// Gets a user-friendly description of rotation features and optimizations.
    /// </summary>
    /// <param name="features">The feature flags indicating rotation features.</param>
    /// <returns>A description of the rotation's features.</returns>
    public static string GetFeaturesDescription(Features features)
    {
        List<string> featureDescriptions = new List<string>();

        if ((features & Features.UseTincture) != 0)
        {
            featureDescriptions.Add("Uses tinctures.");
        }

        if ((features & Features.SavageOptimized) != 0)
        {
            featureDescriptions.Add("Optimized for Savage.");
        }

        if ((features & Features.HasUserConfig) != 0)
        {
            featureDescriptions.Add("User-configurable settings.");
        }

        if (featureDescriptions.Count == 0)
        {
            return "None.";
        }

        return string.Join("\n -", featureDescriptions);
    }



}