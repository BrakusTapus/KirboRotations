using Serilog.Events;

namespace KirboRotations.Utility.Logging;

public record struct KirboInternalLogMessage
{
    public string Message;
    public LogEventLevel Level;

    public KirboInternalLogMessage(string Message, LogEventLevel Level = LogEventLevel.Information)
    {
        this.Message = Message;
        this.Level = Level;
    }
}
