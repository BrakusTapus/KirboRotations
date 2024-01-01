using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace KirboRotations.Custom.ExtraHelpers;

public static class GeneralHelpers
{
    public const string USERNAME = "Kirbo";

    private const string K = "[KirboRotations]";

    // Used so i can filter XLLog on rotation name
    public static string v = K;

    // 
    public static bool SaveAction { get; set; } = false;

    /// <summary> Checks if the player is in a PVP enabled zone. </summary>
    /// <returns> A value indicating whether the player is in a PVP enabled zone. </returns>
    internal static bool InPvP() => GameMain.IsInPvPArea() || GameMain.IsInPvPInstance();

    /// <summary>
    /// Whether the character is in combat.  This is an extension method of the class BattleChara
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static unsafe bool IsInCombat(this BattleChara obj)
    {
        return obj.Struct()->Character.InCombat;
    }

    /*  #region Conditions
        /// <summary>
        /// 
        /// </summary>
        internal static unsafe bool InNormalConditions => Conditions.Instance()->Flags[1];

        /// <summary>
        /// 
        /// </summary>
        public static unsafe bool IsUnconscious => Conditions.Instance()->Flags[2];

        /// <summary>
        /// 
        /// </summary>
        public static unsafe bool IsEmoting => Conditions.Instance()->Flags[3];

        /// <summary>
        /// 
        /// </summary>
        public static unsafe bool IsMounted => Conditions.Instance()->Flags[4];

        /// <summary>
        /// 
        /// </summary>
        public static unsafe bool IsCrafting => Conditions.Instance()->Flags[5];

        /// <summary>
        /// 
        /// </summary>
        public static unsafe bool IsGathering => Conditions.Instance()->Flags[6];

        /// <summary>
        /// Returns wether or not Combat is active
        /// </summary>
        internal static unsafe bool InCombat => Conditions.Instance()->Flags[26];

        /// <summary>
        /// 
        /// </summary>
        public static unsafe bool IsMeldingMateria => Conditions.Instance()->Flags[7];

        /// <summary>
        /// 
        /// </summary>
        public static unsafe bool IsOperatingSiegeMachine => Conditions.Instance()->Flags[8];

        /// <summary>
        /// 
        /// </summary>
        public static unsafe bool IsCarryingObject => Conditions.Instance()->Flags[9];

        /// <summary>
        /// 
        /// </summary>
        public static unsafe bool IsMounted2 => Conditions.Instance()->Flags[10];

        /// <summary>
        /// 
        /// </summary>
        public static unsafe bool IsInThatPosition => Conditions.Instance()->Flags[11];

        /// <summary>
        /// 
        /// </summary>
        public static unsafe bool IsChocoboRacing => Conditions.Instance()->Flags[12];

        /// <summary>
        /// 
        /// </summary>
        public static unsafe bool IsPlayingMiniGame => Conditions.Instance()->Flags[13];

        /// <summary>
        ///
        /// </summary>
        public static unsafe bool IsPlayingLordOfVerminion => Conditions.Instance()->Flags[14];

        /// <summary>
        /// 
        /// </summary>
        public static unsafe bool IsParticipatingInCustomMatch => Conditions.Instance()->Flags[15];

        /// <summary>
        /// 
        /// </summary>
        public static unsafe bool IsPerforming => Conditions.Instance()->Flags[16];
        #endregion
    */
}