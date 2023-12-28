//using ECommons.MathHelpers;
using System;
using System.Numerics;

namespace KirboRotations.Custom.Utility.ImGuiEx;

public static class GradientColor
{
    public static Vector4 Get(Vector4 start, Vector4 end, int Milliseconds = 1000)
    {
        var delta = (end - start) / Milliseconds;
        var time = Environment.TickCount64 % (Milliseconds * 2);
        if (time < Milliseconds)
        {
            return start + delta * (time % Milliseconds);
        }
        else
        {
            return end - delta * (time % Milliseconds);
        }
    }
}
