namespace KirboRotations.Utility.ExtraHelpers;

public static class DescriptionHelpers
{
    /// <summary>
    /// Displays "[v1.0.0.15]"
    /// </summary>
    public static string RotationVersion => "[v1.0.0.15]";

    public static string GetUltimateCompatibilityDescription(UltimateCompatibility compatibility)
    {
        List<string> compatibilityDescriptions = new List<string>();

        if ((compatibility & UltimateCompatibility.UCoB) != 0)
        {
            compatibilityDescriptions.Add("Compatible for The Unending Coil of Bahamut (UCoB).");
        }

        if ((compatibility & UltimateCompatibility.UwU) != 0)
        {
            compatibilityDescriptions.Add("Compatible for The Weapon's Refrain (Ultimate) (UwU).");
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
            return "Not recommended for any content.";
        }

        return string.Join(", ", compatibilityDescriptions);
    }
}