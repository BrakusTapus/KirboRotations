namespace KirboRotations.JobHelpers.Enums;

internal enum CombatPhase
{
    Opening,
    Sustained,
    Closing,
    Transition, // For phases between major fight segments
    Emergency  // For unexpected situations like sudden heavy damage or critical mechanics
}
