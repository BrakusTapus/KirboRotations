using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using KirboRotations.Utility.Core;
using RotationSolver.Basic.Configuration;

namespace KirboRotations.Utility.Data.Actions;

internal static class ActionUpdater
{
    internal static DateTime AutoCancelTime { get; set; } = DateTime.MinValue;

    static RandomDelay _GCDDelay = new(() =>   (0, 0));

    internal static IAction NextAction { get; set; }
    internal static IBaseAction NextGCDAction { get; set; }
    internal static IAction WrongAction { get; set; }
    static readonly Random _wrongRandom = new();

    internal static void ClearNextAction()
    {
        SetAction(0);
        WrongAction = NextAction = NextGCDAction = null;
    }

    internal static void UpdateNextAction()
    {
        PlayerCharacter localPlayer = Player.Object;
        var customRotation = DataBase.RightNowRotation;

        try
        {
            if (localPlayer != null && customRotation != null
                && customRotation.TryInvoke(out var newAction, out var gcdAction))
            {
                /*if (KirboService.Config.GetValue(PluginConfigFloat.MistakeRatio) > 0)
                {
                    var actions = customRotation.AllActions.Where(a =>
                    {
                        if (a.ID == newAction?.ID) return false;
                        if (a is IBaseAction action)
                        {
                            return !action.IsFriendly && action.IsInMistake
                            && action.ChoiceTarget != TargetFilter.FindTargetForMoving
                            && action.CanUse(out _, CanUseOption.MustUseEmpty | CanUseOption.IgnoreClippingCheck);
                        }
                        return false;
                    });

                    var count = actions.Count();
                    WrongAction = count > 0 ? actions.ElementAt(_wrongRandom.Next(count)) : null;
                }*/

                NextAction = newAction;

                if (gcdAction is IBaseAction GcdAction)
                {
                    if (NextGCDAction != GcdAction)
                    {
                        NextGCDAction = GcdAction;
                    }
                }
                return;
            }
        }
        catch (Exception ex)
        {
            Svc.Log.Error(ex, "Failed to update next action.");
        }

        WrongAction = NextAction = NextGCDAction = null;
    }

    private static void SetAction(uint id) => Svc.PluginInterface.GetOrCreateData("Avarice.ActionOverride", () => new List<uint>() { id })[0] = id;

    internal unsafe static void UpdateActionInfo()
    {
        SetAction(NextGCDAction?.AdjustedID ?? 0);
        UpdateWeaponTime();
        UpdateCombatTime();
        UpdateSlots();
        UpdateMoving();
        UpdateMPTimer();
    }
    private unsafe static void UpdateSlots()
    {
        for (int i = 0; i < DataBase.BluSlots.Length; i++)
        {
            DataBase.BluSlots[i] = ActionManager.Instance()->GetActiveBlueMageActionInSlot(i);
        }
        for (ushort i = 0; i < DataBase.DutyActions.Length; i++)
        {
            DataBase.DutyActions[i] = ActionManager.GetDutyActionId(i);
        }
    }

    static DateTime _stopMovingTime = DateTime.MinValue;
    private unsafe static void UpdateMoving()
    {
        var last = DataBase.IsMoving;
        DataBase.IsMoving = AgentMap.Instance()->IsPlayerMoving > 0;
        if (last && !DataBase.IsMoving)
        {
            _stopMovingTime = DateTime.Now;
        }
        else if (DataBase.IsMoving)
        {
            _stopMovingTime = DateTime.MinValue;
        }

        if (_stopMovingTime == DateTime.MinValue)
        {
            DataBase.StopMovingRaw = 0;
        }
        else
        {
            DataBase.StopMovingRaw = (float)(DateTime.Now - _stopMovingTime).TotalSeconds;
        }
    }

    static DateTime _startCombatTime = DateTime.MinValue;
    private static void UpdateCombatTime()
    {
        var last = DataBase.InCombat;
        DataBase.InCombat = Svc.Condition[ConditionFlag.InCombat];
        if (!last && DataBase.InCombat)
        {
            _startCombatTime = DateTime.Now;
        }
        else if (last && !DataBase.InCombat)
        {
            _startCombatTime = DateTime.MinValue;

            if (true)
            {
                AutoCancelTime = DateTime.Now.AddSeconds(30);
            }
        }

        if (_startCombatTime == DateTime.MinValue)
        {
            DataBase.CombatTimeRaw = 0;
        }
        else
        {
            DataBase.CombatTimeRaw = (float)(DateTime.Now - _startCombatTime).TotalSeconds;
        }
    }

    private static unsafe void UpdateWeaponTime()
    {
        var player = Player.Object;
        if (player == null) return;

        var instance = ActionManager.Instance();

        var castTotal = player.TotalCastTime;

        var weaponTotal = instance->GetRecastTime(ActionType.Action, 11);
        if (castTotal > 0) castTotal += 0.1f;
        if (player.IsCasting) weaponTotal = Math.Max(castTotal, weaponTotal);

        DataBase.WeaponElapsed = instance->GetRecastTimeElapsed(ActionType.Action, 11);
        DataBase.WeaponRemain = DataBase.WeaponElapsed == 0 ? player.TotalCastTime - player.CurrentCastTime
            : Math.Max(weaponTotal - DataBase.WeaponElapsed, player.TotalCastTime - player.CurrentCastTime);

        //Casting time.
        if (DataBase.WeaponElapsed < 0.3) DataBase.CastingTotal = castTotal;
        if (weaponTotal > 0 && DataBase.WeaponElapsed > 0.2) DataBase.WeaponTotal = weaponTotal;
    }

    static uint _lastMP = 0;
    static DateTime _lastMPUpdate = DateTime.Now;

    internal static float MPUpdateElapsed => (float)(DateTime.Now - _lastMPUpdate).TotalSeconds % 3;

    private static void UpdateMPTimer()
    {
        var player = Player.Object;
        if (player == null) return;

        //不是黑魔不考虑啊
        if (player.ClassJob.Id != (uint)ECommons.ExcelServices.Job.BLM) return;

        //有醒梦，就算了啊
        if (player.HasStatus(true, StatusID.LucidDreaming)) return;

        if (_lastMP < player.CurrentMp)
        {
            _lastMPUpdate = DateTime.Now;
        }
        _lastMP = player.CurrentMp;
    }

    internal unsafe static bool CanDoAction()
    {
        if (Svc.Condition[ConditionFlag.OccupiedInQuestEvent]
            || Svc.Condition[ConditionFlag.OccupiedInCutSceneEvent]
            || Svc.Condition[ConditionFlag.Occupied33]
            || Svc.Condition[ConditionFlag.Occupied38]
            || Svc.Condition[ConditionFlag.Jumping61]
            || Svc.Condition[ConditionFlag.BetweenAreas]
            || Svc.Condition[ConditionFlag.BetweenAreas51]
            || Svc.Condition[ConditionFlag.Mounted]
            //|| Svc.Condition[ConditionFlag.SufferingStatusAffliction] //Because of BLU30!
            || Svc.Condition[ConditionFlag.SufferingStatusAffliction2]
            || Svc.Condition[ConditionFlag.RolePlaying]
            || Svc.Condition[ConditionFlag.InFlight]
            || ActionManager.Instance()->ActionQueued && NextAction != null
                && ActionManager.Instance()->QueuedActionId != NextAction.AdjustedID
            || Player.Object.CurrentHp == 0) return false;

        var maxAhead = Math.Max(DataBase.MinAnimationLock - DataBase.Ping, 0.08f);
        var ahead = Math.Min(maxAhead,0.08f);

        //GCD
        var canUseGCD = DataBase.WeaponRemain <= ahead;
        if (_GCDDelay.Delay(canUseGCD))
        {
            //return RSCommands.CanDoAnAction(true);
        }
        if (canUseGCD) return false;

        var nextAction = NextAction;
        if (nextAction == null) return false;

        var timeToNext = DataBase.ActionRemain;

        ////No time to use 0gcd
        //if (timeToNext + nextAction.AnimationLockTime
        //    > DataCenter.WeaponRemain) return false;

        //Skip when casting
        if (DataBase.WeaponElapsed <= DataBase.CastingTotal) return false;

        //The last one.
        if (timeToNext + nextAction.AnimationLockTime + DataBase.Ping + DataBase.MinAnimationLock > DataBase.WeaponRemain)
        {
            if (DataBase.WeaponRemain > nextAction.AnimationLockTime + DataBase.Ping) return false;

            //return RSCommands.CanDoAnAction(false);
        }
        else if (timeToNext < ahead)
        {
            //return RSCommands.CanDoAnAction(false);
        }

        return false;
    }
}
