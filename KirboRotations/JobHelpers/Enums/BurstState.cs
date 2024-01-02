namespace KirboRotations.JobHelpers.Enums;

internal enum BurstState
{
    PreBurst,       // Preparing for the burst phase, setting up conditions or buffs
    InBurst,        // Actively executing the burst rotation
    BurstFinished,  // The burst phase has completed successfully
    FailedBurst     // The burst phase failed to execute correctly or was interrupted
}