using KirboRotations.Utility.Core;
using Serilog.Events;

namespace KirboRotations.Utility.Logging;

public static class KirboLog
{
    public static void Error(string s)
    {
        KirboSvc.Log.Error($"{s}");
        KirboSvc.Framework?.RunOnFrameworkThread(delegate
        {
            KirboInternalLog.Messages.PushBack(new(s, LogEventLevel.Error));
        });
    }
}
