using Serilog.Core;
using Serilog.Events;

namespace KirboRotations.Custom.ExtraHelpers;

public class CustomLogEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var newSourceContext = $"[KirboRotations] {logEvent.Properties["SourceContext"].ToString()}";
        logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("SourceContext", newSourceContext));
    }
}
