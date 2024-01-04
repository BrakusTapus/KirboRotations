using KirboRotations.PvE.Beta;
using KirboRotations.PvE.Ranged;

namespace KirboRotations.Extensions;

internal class BaseEx : MCH_KirboBeta
{
    public static bool LoggedIn { get; set; }

    public static void CheckPlayerStatus()
    {
        if (Player == null)
        {
            LoggedIn = false;
        }
        else
        {
            LoggedIn = true;
        }
    }
}