using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using KirboRotations.Utility.GameAssists;
using Lumina.Excel.GeneratedSheets;
using Action = Lumina.Excel.GeneratedSheets.Action;
using CharacterManager = FFXIVClientStructs.FFXIV.Client.Game.Character.CharacterManager;

namespace KirboRotations.Utility.Core;

internal static class DataBase
{
    internal static bool NoPoslock => KirboSvc.Condition[ConditionFlag.OccupiedInEvent]
        // || !KirboService.Config.GetValue(PluginConfigBool.PoslockCasting)
        //Key cancel.
        // || KirboSvc.KeyState[ConfigurationAssists.Keys[KirboService.Config.GetValue(PluginConfigInt.PoslockModifier) % ConfigurationAssists.Keys.Length]]
        //Gamepad cancel.
        || KirboSvc.GamepadState.Raw(Dalamud.Game.ClientState.GamePad.GamepadButtons.L2) >= 0.5f;

    #region Actions

    public static ActionID LastAction { get; private set; } = 0;

    public static ActionID LastGCD { get; private set; } = 0;

    public static ActionID LastAbility { get; private set; } = 0;

    #endregion

    #region Cooldown

    public const float MinAnimationLock = 0.6f;

    #endregion

    #region Combat

    public static bool InCombat { get; set; }

    #endregion

    #region Time

    /// <summary>
    /// 
    /// </summary>
    internal static float CombatTimeRaw { get; set; }

    /// <summary>
    /// Time left till next GCD
    /// </summary>
    public static float WeaponRemain { get; internal set; }

    /// <summary>
    /// Total GCD time
    /// </summary>
    public static float WeaponTotal { get; internal set; }

    /// <summary>
    /// ?
    /// </summary>
    public static float WeaponElapsed { get; internal set; }

    public static float GCDTime(uint gcdCount = 0, float offset = 0)
        => WeaponTotal * gcdCount + offset;

    /// <summary>
    /// Time to the next action
    /// </summary>
    public static unsafe float ActionRemain => *(float*)((IntPtr)ActionManager.Instance() + 0x8);

    public static float AbilityRemain
    {
        get
        {
            var gcdRemain = WeaponRemain;
            if (gcdRemain - MinAnimationLock - Ping <= ActionRemain)
            {
                return gcdRemain + MinAnimationLock + Ping;
            }
            return ActionRemain;
        }
    }

    public static float NextAbilityToNextGCD => WeaponRemain - ActionRemain;

    public static float CastingTotal { get; internal set; }

    #endregion

    #region Rotations

    public static ICustomRotation RightNowRotation { get; internal set; }

    public static uint[] BluSlots { get; internal set; } = new uint[24];

    public static uint[] DutyActions { get; internal set; } = new uint[2];

    #endregion

    #region Movement

    public static bool IsMoving { get; internal set; }

    internal static float StopMovingRaw { get; set; }

    #endregion

    #region System

    public static float Ping => Math.Min(RTT, FetchTime);
    public static float RTT { get; internal set; } = 0.1f;
    public static float FetchTime { get; private set; } = 0.1f;

    #endregion

    #region Target
    private static uint _hostileTargetId = GameObject.InvalidGameObjectId;
    internal static BattleChara HostileTarget
    {
        get
        {
            return KirboSvc.Objects.SearchById(_hostileTargetId) as BattleChara;
        }
        set
        {
            _hostileTargetId = value?.ObjectId ?? GameObject.InvalidGameObjectId;
        }
    }

    public static ObjectListDelay<BattleChara> HostileTargets { get; } = new ObjectListDelay<BattleChara>(() => (0,0));

    #endregion

}
