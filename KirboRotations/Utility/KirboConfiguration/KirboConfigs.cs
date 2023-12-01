using Dalamud.Configuration;
using Dalamud.Utility;
using KirboRotations.Utility.Core;

namespace KirboRotations.Utility.KirboConfiguration;

public class KirboConfigs
{
}

[Serializable]
public class JobConfig
{
    public string RotationChoice { get; set; }
    public string PvPRotationChoice { get; set; }
    public DictionConfig<JobConfigFloat, float> Floats { get; set; } = new();

    public DictionConfig<JobConfigInt, int> Ints { get; set; } = new();
    public Dictionary<string, Dictionary<string, string>> RotationsConfigurations { get; set; } = new();
}


public enum JobConfigInt : byte
{

}

public enum JobConfigFloat : byte
{

}

[Serializable]
public class DictionConfig<TConfig, TValue> where TConfig : struct, Enum
{

}

[AttributeUsage(AttributeTargets.Field)]
public class DefaultAttribute : Attribute
{
    public object Default { get; set; }
    public object Min { get; set; }
    public object Max { get; set; }

    public DefaultAttribute(object @default, object min = null, object max = null)
    {
        Default = @default;
        Min = min;
        Max = max;
    }
}

public enum PluginConfigBool : byte
{
    [Default(false)] PoslockCasting,
}
