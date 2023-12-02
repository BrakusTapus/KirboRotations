using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using KirboRotations.Utility.Core;
using KirboRotations.Utility.ExcelServices;
using KirboRotations.Utility.GameAssists;
using Lumina.Excel.GeneratedSheets;

namespace KirboRotations.Utility.Rotations;

/// <summary>
/// Like RotationSolver.Basic.Rotations.CustomRotation but more accesible.
/// </summary>
public class UserRotations : IKirboTexture
{
    private static unsafe ActionManager* actionManager;

    static unsafe UserRotations()
    {       
        // Initialize ActionManager
        actionManager = ActionManager.Instance();

        // Optionally log the initialization
        Serilog.Log.Debug("Initialized ActionManager...");
    }


    #region Player
    /// <summary>
    /// This is the player.
    /// </summary>
    internal static PlayerCharacter Player => PlayerData.Object;

    public Job[] Jobs { get; }
    public ClassJob ClassJob => KirboService.GetSheet<ClassJob>().GetRow((uint)Jobs[0]);
    public string Name => ClassJob.Abbreviation + " - " + ClassJob.Name;

    #endregion

    #region Actions

    private unsafe delegate bool OnUseAction(ActionManager* manager, ActionType actionType, uint actionID, ulong targetID, uint a4, uint a5, uint a6, void* a7);

    private static Hook<OnUseAction> _useActionHook;

    private Queue<ActionInfo> _actions = new Queue<ActionInfo>();
    private const int MaxQueueSize = 512;

    // Modify this method to automatically queue actions when used
    internal unsafe void UseSpecificAction(ActionID actionId, ulong targetId)
    {
        ActionType actionType = ActionType.Action;
        actionManager->UseAction(actionType, (uint)actionId, targetId);
        QueueAction(actionId);
    }

    // Method to add an action to the queue
    public void QueueAction(ActionID actionId)
    {
        if (_actions.Count >= MaxQueueSize)
        {
            // Remove the oldest action if the queue is full
            _actions.Dequeue();
        }

        // Using ToString() to get the name of the enum member
        var actionName = actionId.ToString();

        _actions.Enqueue(new ActionInfo((uint)actionId, actionName));
    }

    // Method to get the next action from the queue
    public ActionInfo DequeueAction()
    {
        return _actions.Count > 0 ? _actions.Dequeue() : default;
    }

    // Method to peek at the next action without removing it
    public ActionInfo PeekNextAction()
    {
        return _actions.Count > 0 ? _actions.Peek() : default;
    }

    // Struct to hold action name and ID
    public struct ActionInfo
    {
        public uint ID;
        public string Name;

        public ActionInfo(uint id, string name)
        {
            ID = id;
            Name = name;
        }
    }

    internal static bool IsLastAction(bool isAdjust, params IAction[] actions)
    {
        return IsLastAction(GetIDFromActions(isAdjust, actions));
    }

    internal static bool IsLastAction(params ActionID[] ids)
    {
        return IsActionID(DataBase.LastAction, ids);
    }

    internal static bool IsLastGCD(bool isAdjust, params IAction[] actions)
    {
        return IsLastGCD(GetIDFromActions(isAdjust, actions));
    }

    internal static bool IsLastGCD(params ActionID[] ids)
    {
        return IsActionID(DataBase.LastGCD, ids);
    }

    internal static bool IsLastAbility(bool isAdjust, params IAction[] actions)
    {
        return IsLastAbility(GetIDFromActions(isAdjust, actions));
    }

    internal static bool IsLastAbility(params ActionID[] ids)
    {
        return IsActionID(DataBase.LastAbility, ids);
    }

    private static bool IsActionID(ActionID id, params ActionID[] ids)
    {
        return ids.Contains(id);
    }

    private static ActionID[] GetIDFromActions(bool isAdjust, params IAction[] actions)
    {
        return actions.Select((IAction a) => (ActionID)((!isAdjust) ? a.ID : a.AdjustedID)).ToArray();
    }

    /// <summary>
    /// 
    /// </summary>
    public static uint[] BluSlots { get; internal set; } = new uint[24];

    /// <summary>
    /// 
    /// </summary>
    public static uint[] DutyActions { get; internal set; } = new uint[2];

    #endregion Actions

    #region Time

    /// <summary>
    /// 
    /// </summary>
    protected static float WeaponRemain => DataBase.WeaponRemain;

    /// <summary>
    /// 
    /// </summary>
    protected static float WeaponTotal => DataBase.WeaponTotal;

    /// <summary>
    /// 
    /// </summary>
    protected static float WeaponElapsed => DataBase.WeaponElapsed;

    /// <summary>
    /// ?
    /// </summary>
    /// <param name="gcdCount"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    public static float GCDTime(uint gcdCount = 0, float offset = 0) => WeaponTotal * gcdCount + offset;

    /// <summary>
    /// Time to the next action
    /// </summary>
    public static unsafe float ActionRemain => *(float*)((IntPtr)ActionManager.Instance() + 0x8);

    /// <summary>
    /// 
    /// </summary>
    public static float AbilityRemain
    {
        get
        {
            var gcdRemain = WeaponRemain;
            if (gcdRemain - DataBase.MinAnimationLock - DataBase.Ping <= ActionRemain)
            {
                return gcdRemain + DataBase.MinAnimationLock + DataBase.Ping;
            }
            return ActionRemain;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static float NextAbilityToNextGCD => WeaponRemain - ActionRemain;

    /// <summary>
    /// 
    /// </summary>
    public static float CastingTotal { get; internal set; }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="GCD"></param>
    /// <returns></returns>
    internal static bool CombatElapsedLessGCD(int GCD)
    {
        return CombatElapsedLess(GCD * DataBase.WeaponTotal);
    }

    /// <summary>
    /// Whether the battle lasted less than <paramref name="time"/> seconds
    /// 
    /// </summary>
    /// <param name="time">time in second.</param>
    /// <returns></returns>
    protected static bool CombatElapsedLess(float time)
    {
        return CombatTime <= time;
    }

    /// <summary>
    /// The combat time.
    /// 
    /// </summary>
    public static float CombatTime
    {
        get
        {
            return DataBase.InCombat ? DataBase.CombatTimeRaw + DataBase.WeaponRemain : 0;
        }
    }

    #endregion

    #region Target
    /// <summary>
    /// The last attacked hostile target.
    /// </summary>
    internal static BattleChara HostileTarget => DataBase.HostileTarget;
    #endregion

}