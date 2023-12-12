using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Statuses;
using FFXIVClientStructs.FFXIV.Client.Game;
using static FFXIVClientStructs.FFXIV.Client.Game.Conditions;
using static FFXIVClientStructs.FFXIV.Client.Game.ActionManager;
using KirboRotations.Utility.ImGuiEx;
using static KirboRotations.Utility.Data.StatusID_Buffs;
using static KirboRotations.Utility.Data.StatusID_DeBuffs;

namespace KirboRotations.Utility.ExtraHelpers;

public static class Methods
{
    /// <summary>
    /// Calls the 'ResetBoolAfterDelay' if 'Flag' is true.
    /// </summary>
    internal static void FlagControl()
    {
        if (_openerFlag)
        {
            Serilog.Log.Debug($"Opener Event");
            _openerFlag = false;
        }
        if (_burstFlag)
        {
            Serilog.Log.Debug($"Burst Event");
            _burstFlag = false;
        }
    }

    /// <summary> Checks if the player is in a PVP enabled zone. </summary>
    /// <returns> A value indicating whether the player is in a PVP enabled zone. </returns>
    internal static bool InPvP() => GameMain.IsInPvPArea() || GameMain.IsInPvPInstance();

    #region Opener

    /// <summary>
    /// Flag used to indicate a state change
    /// </summary>
    internal static bool _openerFlag { get; set; } = false;

    /// <summary>
    /// Checks if actions needed for the opener available.
    /// </summary>
    internal static bool OpenerActionsAvailable { get; set; } = false;

    /// <summary>
    /// Checks if actions needed for the opener available.
    /// </summary>
    internal static bool LvL70_Ultimate_OpenerActionsAvailable { get; set; } = false;

    /// <summary>
    /// Checks if actions needed for the opener available.
    /// </summary>
    internal static bool LvL80_Ultimate_OpenerActionsAvailable { get; set; } = false;

    /// <summary>
    /// Indicates wether or not the opener is currently in progress
    /// </summary>
    internal static bool OpenerInProgress { get; set; } = false;

    /// <summary>
    /// Keeps track of the opener step
    /// </summary>
    internal static int OpenerStep { get; set; } = 0;

    /// <summary>
    /// Indicates wether or not the opener was finished succesfully
    /// </summary>
    internal static bool OpenerHasFinished { get; set; } = false;

    /// <summary>
    /// Indicates wether or not the opener has failed
    /// </summary>
    internal static bool OpenerHasFailed { get; set; } = false;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lastaction"></param>
    /// <param name="nextaction"></param>
    /// <returns></returns>
    internal static bool OpenerController(bool lastaction, bool nextaction)
    {
        if (lastaction)
        {
            OpenerStep++;
            return false;
        }
        return nextaction;
    }

    /// <summary>
    /// Resets opener properties
    /// </summary>
    internal static void ResetOpenerProperties()
    {
        OpenerActionsAvailable = false;
        OpenerInProgress = false;
        OpenerStep = 0;
        OpenerHasFinished = false;
        OpenerHasFailed = false;
        Serilog.Log.Debug($"OpenerActionsAvailable = {OpenerActionsAvailable}");
        Serilog.Log.Debug($"OpenerInProgress = {OpenerInProgress} - Step: {OpenerStep}");
        Serilog.Log.Debug($"OpenerHasFinished = {OpenerHasFinished}");
        Serilog.Log.Debug($"OpenerHasFailed = {OpenerHasFailed}");
    }

    /// <summary>
    /// Handles the current state of the opener based on various condition.
    /// </summary>
    internal static void StateOfOpener()
    {
        if (!CustomRotation.InCombat)
        {
            _openerFlag = false;
            OpenerStep = 0;
            OpenerHasFinished = false;
            OpenerHasFailed = false;
        }
        if (OpenerHasFailed)
        {
            _openerFlag = true;
            OpenerInProgress = false;
        }
        if (OpenerHasFinished)
        {
            _openerFlag = true;
            OpenerInProgress = false;
        }
    }
    #endregion

    #region Burst

    /// <summary>
    /// Flag used to indicate a state change
    /// </summary>
    internal static bool _burstFlag { get; set; } = false;

    /// <summary>
    /// Keeps track of the opener step
    /// </summary>
    internal static int BurstStep { get; set; } = 0;

    /// <summary>
    /// Property to hold the availability of BurstWindow
    /// </summary>
    internal static bool BurstActionsAvailable { get; set; } = false;

    /// <summary>
    /// Is burst in progress
    /// </summary>
    internal static bool BurstInProgress { get; set; } = false;

    /// <summary>
    /// Is in Burst (should be when a jobs 2min action was used)
    /// </summary>
    internal static bool InBurst { get; set; } = false;

    /// <summary>
    /// check if burst completed correctly
    /// </summary>
    internal static bool BurstHasFinished { get; set; } = false;

    /// <summary>
    /// check if burst failed.
    /// </summary>
    internal static bool BurstHasFailed { get; set; } = false;

    /// <summary>
    /// Controls the Burst Proceeds to next step if the LastAction matches the Burst Sequence
    /// </summary>
    /// <param name="lastaction"></param>
    /// <param name="nextaction"></param>
    /// <returns></returns>
    internal static bool BurstController(bool lastaction, bool nextaction)
    {
        if (lastaction)
        {
            BurstStep++;
            return false;
        }
        return nextaction;
    }

    /// <summary>
    /// Resets opener properties
    /// </summary>
    internal static void ResetBurstProperties()
    {
        BurstActionsAvailable = false;
        BurstInProgress = false;
        BurstStep = 0;
        BurstHasFinished = false;
        BurstHasFailed = false;
        Serilog.Log.Debug($"OpenerHasFailed = {InBurst}");
        Serilog.Log.Debug($"OpenerInProgress = {BurstInProgress} - Step: {BurstStep}");
        Serilog.Log.Debug($"OpenerHasFinished = {BurstHasFinished}");
    }

    /// <summary>
    /// Handles the current state of the opener based on various condition.
    /// </summary>
    internal static void BurstStatus()
    {
        if (!CustomRotation.InCombat)
        {
            _burstFlag = true;
            BurstStep = 0;
            BurstHasFinished = false;
            InBurst = false;
            BurstInProgress = false;
        }
        if (BurstHasFailed)
        {
            _burstFlag = true;
            BurstInProgress = false;
        }
        if (BurstHasFinished)
        {
            _burstFlag = true;
            BurstInProgress = false;
        }
    }

    #endregion

    #region Conditions
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
}