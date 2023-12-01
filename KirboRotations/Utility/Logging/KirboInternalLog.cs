using Dalamud.Interface.Colors;
using KirboRotations.Utility.CircularBuffer;
using Serilog.Events;

namespace KirboRotations.Utility.Logging;

public class KirboInternalLog
{
    public static readonly CircularBuffer<KirboInternalLogMessage> Messages = new(1000);



}
