namespace KirboRotations.Custom.ExtraHelpers;

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
    /// <param name="compatibilities">A list of content compatibilities for the rotation.</param>
    /// <returns>A description of the rotation's compatibility with Ultimate fights.</returns>
    public static string GetUltimateCompatibilityDescription(List<UltimateCompatibility> compatibilities)
    {
        List<string> ultimateCompatibilityDescriptions = new List<string>();

        if (compatibilities.Contains(UltimateCompatibility.UCoB))
        {
            ultimateCompatibilityDescriptions.Add(" - The Unending Coil of Bahamut (UCoB).");
        }

        if (compatibilities.Contains(UltimateCompatibility.UwU))
        {
            ultimateCompatibilityDescriptions.Add(" - The Weapon's Refrain (UwU).");
        }

        if (compatibilities.Contains(UltimateCompatibility.TEA))
        {
            ultimateCompatibilityDescriptions.Add(" - The Epic of Alexander (TEA).");
        }

        if (compatibilities.Contains(UltimateCompatibility.DSR))
        {
            ultimateCompatibilityDescriptions.Add(" - The Dragonsong's Reprise (DSR).");
        }

        if (compatibilities.Contains(UltimateCompatibility.TOP))
        {
            ultimateCompatibilityDescriptions.Add(" - The Omega Protocol (TOP).");
        }

        if (ultimateCompatibilityDescriptions.Count == 0)
        {
            return " - Not recommended for any Ultimate fights.";
        }

        return string.Join("\n", ultimateCompatibilityDescriptions);
    }

    /// <summary>
    /// Gets a user-friendly description of rotation compatibility with different types of in-game content.
    /// </summary>
    /// <param name="compatibilities">A list of content compatibilities for the rotation.</param>
    /// <returns>A description of the rotation's compatibility with in-game content.</returns>
    public static string GetContentCompatibilityDescription(List<ContentCompatibility> compatibilities)
    {
        List<string> compatibilityDescriptions = new List<string>();

        if (compatibilities.Contains(ContentCompatibility.DutyRoulette))
        {
            compatibilityDescriptions.Add(" - Duty Roulette");
        }

        if (compatibilities.Contains(ContentCompatibility.Dungeons))
        {
            compatibilityDescriptions.Add(" - Dungeons");
        }

        if (compatibilities.Contains(ContentCompatibility.Guildhests))
        {
            compatibilityDescriptions.Add(" - Guildhests");
        }

        if (compatibilities.Contains(ContentCompatibility.Trials))
        {
            compatibilityDescriptions.Add(" - Trials");
        }

        if (compatibilities.Contains(ContentCompatibility.ExtremeTrials))
        {
            compatibilityDescriptions.Add(" - ExtremeTrials");
        }

        if (compatibilities.Contains(ContentCompatibility.NormalRaids))
        {
            compatibilityDescriptions.Add(" - NormalRaids");
        }

        if (compatibilities.Contains(ContentCompatibility.AllianceRaids))
        {
            compatibilityDescriptions.Add(" - AllianceRaids");
        }

        if (compatibilities.Contains(ContentCompatibility.SavageRaids))
        {
            compatibilityDescriptions.Add(" - SavageRaids");
        }

        if (compatibilities.Contains(ContentCompatibility.FATEs))
        {
            compatibilityDescriptions.Add(" - FATEs");
        }

        if (compatibilities.Contains(ContentCompatibility.TreasureHunt))
        {
            compatibilityDescriptions.Add(" - TreasureHunt");
        }

        if (compatibilities.Contains(ContentCompatibility.DeepDungeons))
        {
            compatibilityDescriptions.Add(" - DeepDungeons");
        }

        if (compatibilities.Contains(ContentCompatibility.Eureka))
        {
            compatibilityDescriptions.Add(" - Eureka");
        }

        if (compatibilities.Contains(ContentCompatibility.VariantDungeons))
        {
            compatibilityDescriptions.Add(" - VariantDungeons");
        }

        if (compatibilities.Contains(ContentCompatibility.Criterion))
        {
            compatibilityDescriptions.Add(" - Criterion");
        }

        if (compatibilities.Contains(ContentCompatibility.Hunts))
        {
            compatibilityDescriptions.Add(" - Hunts");
        }

        if (compatibilityDescriptions.Count == 0)
        {
            return " - Not recommended for any specific content";
        }

        return string.Join("\n", compatibilityDescriptions);
    }

    /// <summary>
    /// Gets a user-friendly description of rotation features and optimizations.
    /// </summary>
    /// <param name="featuresList">A list of features indicating rotation features.</param>
    /// <returns>A description of the rotation's features and optimizations.</returns>
    public static string GetFeaturesDescription(List<Features> featuresList)
    {
        List<string> featureDescriptions = new List<string>();

        if (featuresList.Contains(Features.UseTincture))
        {
            featureDescriptions.Add(" - Uses tinctures");
        }

        if (featuresList.Contains(Features.SavageOptimized))
        {
            featureDescriptions.Add(" - Optimized for Savage");
        }

        if (featuresList.Contains(Features.HasUserConfig))
        {
            featureDescriptions.Add(" - User-configurable settings");
        }

        if (featureDescriptions.Count == 0)
        {
            return "This rotation currently has no special features.";
        }

        return string.Join("\n", featureDescriptions);
    }



}