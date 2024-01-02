namespace KirboRotations.JobHelpers.Enums;

internal enum PartyState
{
    FullHealth,
    ModerateDamage,
    Critical, // Multiple party members at low health
    ManDown  // One or more party members are incapacitated
}
