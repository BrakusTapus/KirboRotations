using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using GameObject = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject;

namespace KirboRotations.Utility.Data.Actions;

public unsafe class ActionMethods
{
    private unsafe ActionManager* actionManager;

    public ActionMethods()
    {
        // Assuming ActionManager is already initialized in the game
        actionManager = ActionManager.Instance();
    }

    





    #region Combo Management
    public unsafe uint GetCurrentComboAction()
    {
        return actionManager->Combo.Action;
    }

    public unsafe float GetComboTimer()
    {
        return actionManager->Combo.Timer;
    }
    #endregion

    #region Blue Mage Actions Handling

    public unsafe void AssignBlueMageAction(int slot, uint actionId)
    {
        actionManager->AssignBlueMageActionToSlot(slot, actionId);
    }

    public unsafe uint GetActiveBlueMageActionInSlot(int slot)
    {
        return actionManager->GetActiveBlueMageActionInSlot(slot);
    }

    #endregion

    #region Utility Functions

    public static unsafe float GetActionRange(uint actionId)
    {
        return ActionManager.GetActionRange(actionId);
    }

    #endregion

    public static unsafe bool CanUseActionOnTarget(uint actionId, GameObject* target)
    {
        return ActionManager.CanUseActionOnTarget(actionId, target);
    }

    #region Action Usage and Queuing

    public unsafe void UseAction(uint actionId, ulong targetId)
    {
        ActionType actionType = ActionType.Action; // For demonstration, assume it's a regular action
        actionManager->UseAction(actionType, actionId, targetId);
    }

    public unsafe bool IsActionQueued()
    {
        return actionManager->ActionQueued;
    }

    public unsafe bool IsActionReady(uint actionId)
    {
        ActionType actionType = ActionType.Action; // For demonstration
        return actionManager->IsActionOffCooldown(actionType, actionId);
    }

    // Use a specific action on a target
    public unsafe void UseSpecificAction(uint actionId, ulong targetId)
    {
        ActionType actionType = ActionType.Action;
        actionManager->UseAction(actionType, actionId, targetId);
    }

    // Check if an action is currently queued
    public unsafe bool IsSpecificActionQueued()
    {
        return actionManager->ActionQueued;
    }

    // Get the recast time remaining for a specific action
    public unsafe float GetSpecificActionRecastTime(ActionID action)
    {
        ActionType actionType = ActionType.Action;
        return actionManager->GetRecastTime(actionType, (uint)action);
    }

    // Check if a specific action is off cooldown and ready to use
    public unsafe bool IsSpecificActionReady(uint actionId)
    {
        ActionType actionType = ActionType.Action;
        return actionManager->IsActionOffCooldown(actionType, actionId);
    }

    // Use an action at a specific location
    public unsafe void UseActionAtLocation(uint actionId, ulong targetId, FFXIVClientStructs.FFXIV.Common.Math.Vector3* location)
    {
        ActionType actionType = ActionType.Action;
        actionManager->UseActionLocation(actionType, actionId, targetId, location);
    }

    // Get the status of an action
    public unsafe uint GetSpecificActionStatus(uint actionId)
    {
        ActionType actionType = ActionType.Action;
        return actionManager->GetActionStatus(actionType, actionId, 0xE000_0000);
    }

    // Check if the recast timer for a specific action is active
    public unsafe bool IsSpecificRecastTimerActive(uint actionId)
    {
        ActionType actionType = ActionType.Action;
        return actionManager->IsRecastTimerActive(actionType, actionId);
    }

    // Get the adjusted ID of an action (considering any modifications)
    public unsafe uint GetAdjustedIdOfAction(uint actionId)
    {
        return actionManager->GetAdjustedActionId(actionId);
    }

    // Get the adjusted recast time for an action
    public unsafe int GetAdjustedRecastTimeOfAction(uint actionId)
    {
        ActionType actionType = ActionType.Action;
        return ActionManager.GetAdjustedRecastTime(actionType, actionId);
    }

    // Check if an action can be used on a specific target
    public static unsafe bool CanUseSpecificActionOnTarget(uint actionId, GameObject* target)
    {
        return ActionManager.CanUseActionOnTarget(actionId, target);
    }

    // Get the range of an action
    public static unsafe float GetRangeOfAction(uint actionId)
    {
        return ActionManager.GetActionRange(actionId);
    }

    #endregion

    #region Recast and Cooldown Management

    // Get the recast time remaining for a specific action
    public unsafe float GetActionRecastTime(uint actionId)
    {
        ActionType actionType = ActionType.Action;
        return actionManager->GetRecastTime(actionType, actionId);
    }

    // Check if a specific action is off cooldown and ready to use
    public unsafe bool IsActionOffCooldown(uint actionId)
    {
        ActionType actionType = ActionType.Action;
        return actionManager->IsActionOffCooldown(actionType, actionId);
    }

    // Get the elapsed recast time for a specific action
    public unsafe float GetActionRecastTimeElapsed(uint actionId)
    {
        ActionType actionType = ActionType.Action;
        return actionManager->GetRecastTimeElapsed(actionType, actionId);
    }

    // Check if the recast timer for a specific action is active
    public unsafe bool IsRecastTimerActive(uint actionId)
    {
        ActionType actionType = ActionType.Action;
        return actionManager->IsRecastTimerActive(actionType, actionId);
    }

    // Get the recast group for a specific action
    public unsafe int GetActionRecastGroup(uint actionId)
    {
        ActionType actionType = ActionType.Action;
        return actionManager->GetRecastGroup((int)actionType, actionId);
    }

    // Get additional recast group for a specific action (if any)
    public unsafe int GetAdditionalActionRecastGroup(uint actionId)
    {
        ActionType actionType = ActionType.Action;
        return actionManager->GetAdditionalRecastGroup(actionType, actionId);
    }

    // Get detailed information about a specific recast group
    public unsafe RecastDetail* GetRecastGroupDetail(int recastGroup)
    {
        return actionManager->GetRecastGroupDetail(recastGroup);
    }

    // Start a cooldown cycle for a specific action
    public unsafe nint StartActionCooldown(uint actionId)
    {
        ActionType actionType = ActionType.Action;
        return actionManager->StartCooldown(actionType, actionId);
    }

    // Get the adjusted recast time for an action
    public unsafe int GetAdjustedRecastTime(uint actionId)
    {
        ActionType actionType = ActionType.Action;
        return ActionManager.GetAdjustedRecastTime(actionType, actionId);
    }

    // Get the maximum number of charges for an action
    public static unsafe ushort GetMaxChargesOfAction(uint actionId)
    {
        return ActionManager.GetMaxCharges(actionId, 0); // 0 for current level
    }

    // Get the current number of charges for an action
    public unsafe uint GetCurrentChargesOfAction(uint actionId)
    {
        return actionManager->GetCurrentCharges(actionId);
    }

    #endregion


}
