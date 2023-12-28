using RotationSolver.Basic.Helpers;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace KirboRotations.Custom.ExtraHelpers;

public static class RotationTestHelper
{
    private static float CountDownTime => Countdown.TimeRemaining;
    private static System.Timers.Timer _rotationTimer;
    private const int RotationDuration = 10 * 60 * 1000; // 10 minutes in milliseconds

    static RotationTestHelper()
    {
        _rotationTimer = new System.Timers.Timer(RotationDuration);
        _rotationTimer.Elapsed += OnRotationTimerElapsed;
        _rotationTimer.AutoReset = false; // Timer runs only once
    }

    public static void StartRotationTimer()
    {
        // Logic to start the Rotation Timer
        // This should be called when the countdown reaches 0
        if (CountDownTime < 1 && CountDownTime > 0)
        {
            // Start the timer
            _rotationTimer.Start();
        }
    }

    private static void OnRotationTimerElapsed(object sender, ElapsedEventArgs e)
    {
        // Stop the rotation
        StopRotation();

        // Optionally, stop and dispose of the timer if it's no longer needed
        _rotationTimer.Stop();
        _rotationTimer.Dispose();
    }

    private static void StopRotation()
    {
        // Implement the logic to stop the rotation
        // This might involve changing a state, sending a command, etc.
        Log.Information("Rotation has been stopped after 10 minutes");
    }
}
