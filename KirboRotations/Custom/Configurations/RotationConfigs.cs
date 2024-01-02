using KirboRotations.Custom.Configurations.Enums;

namespace KirboRotations.Custom.Configurations;

internal class RotationConfigs
{
    public List<UltimateCompatibility> UltimateCompatibilities { get; set; }
    public List<ContentCompatibility> ContentCompatibilities { get; set; }
    public List<Features> FeaturesList { get; set; }
    public string RotationVersion { get; }

    public RotationConfigs()
    {
        UltimateCompatibilities = new List<UltimateCompatibility>();
        ContentCompatibilities = new List<ContentCompatibility>();
        FeaturesList = new List<Features>();
        RotationVersion = "v1.0.0.22";
    }

    public List<string> RotationOpeners { get; private set; } = new List<string>();
    public int CurrentRotationSelection { get; set; }

    public void SetRotationOpeners(params string[] openers)
    {
        RotationOpeners = openers.ToList();
    }

    public string GetCurrentRotationOpener()
    {
        if (RotationOpeners.Count == 0)
        {
            return "No Openers Available";
        }
        else if (CurrentRotationSelection >= 0 && CurrentRotationSelection < RotationOpeners.Count)
        {
            return RotationOpeners[CurrentRotationSelection];
        }
        return "Unknown";
    }

    // You can also add methods to easily add items to the lists
    public void AddUltimateCompatibility(UltimateCompatibility compatibility)
    {
        UltimateCompatibilities.Add(compatibility);
    }

    // You can also add methods to easily add items to the lists
    public void AddContentCompatibility(ContentCompatibility compatibility)
    {
        ContentCompatibilities.Add(compatibility);
    }

    // You can also add methods to easily add items to the lists
    public void AddFeatures(Features features)
    {
        FeaturesList.Add(features);
    }
}