using Lumina.Excel.GeneratedSheets;
using FFXIVClientStructs.FFXIV.Client.Game;
using RotationSolver.Basic.Configuration;
using Action = Lumina.Excel.GeneratedSheets.Action;
//using KirboRotations.Utility.Service;
using Dalamud.Plugin.Services;

namespace KirboRotations.Custom.Actions;

/// <summary>
/// <br>The action properties and information provided via <seealso cref="Lumina.Excel.GeneratedSheets.Action"/></br>
/// <br>'Action.Name' would be the skill name.</br>
/// <br>'Action.Range' Would get the max range of the skill</br>
/// <br>'Action.CooldownGroup'</br>
/// <br>'Action.MaxCharges'</br>
/// <br>'Action.ClassJob' Used with <seealso cref="LazyRow"/></br>
/// <br>'Action.CanTargetHostile' Would check if a certain skill could be used on enemies</br>
/// </summary>
public partial class BaseActionEx
{
    /// <summary>
    /// Represents the action associated with this instance.
    /// </summary>
    protected readonly Action _action;

    /// <summary>
    /// Represents the options associated with this action.
    /// </summary>
    readonly ActionOption _option;

    /// <summary>
    /// Is a heal action.
    /// </summary>
    public bool IsHeal => _option.HasFlag(ActionOption.HealFlag);

    /// <summary>
    /// Is a friendly action.
    /// </summary>
    public bool IsFriendly
    {
        get
        {
            if (_action.CanTargetFriendly) return true;
            if (_action.CanTargetHostile) return false;
            return _option.HasFlag(ActionOption.Friendly);
        }
    }

    /// <summary>
    /// Has a normal gcd action.
    /// </summary>
    public bool IsGeneralGCD => _option.HasFlag(ActionOption.GeneralGCD);

    /// <summary>
    /// Is a GCD action, that would start a new GCD window.
    /// </summary>
    public bool IsRealGCD => _option.HasFlag(ActionOption.RealGCD);

    /// <summary>
    /// The name of this action.
    /// </summary>
    public string Name => _action.Name;

    /// <summary>
    /// Description about this action.
    /// </summary>
    public virtual string Description => string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public static PluginConfig Config { get; set; } = new PluginConfig();

    /// <summary>
    /// Is Enabled.
    /// </summary>
    public bool IsEnabled
    {
        // The 'get' accessor returns whether this action is enabled or not.
        get => !Config.GlobalConfig.DisabledActions.Contains(ID);

        // The 'set' accessor allows changing the enabled status of this action.
        set
        {
            if (value)
            {
                // If the value is 'true' (enabling the action), remove its ID from the list of disabled actions.
                Config.GlobalConfig.DisabledActions.Remove(ID);
            }
            else
            {
                // If the value is 'false' (disabling the action), add its ID to the list of disabled actions.
                Config.GlobalConfig.DisabledActions.Add(ID);
            }
        }
    }

    /// <summary>
    /// An uint representing the Action ID
    /// </summary>
    public uint ID => _action.RowId;

    

    /// <summary>
    /// Adjusted action Id.
    /// </summary>
    //public uint AdjustedID => (uint)Service.GetAdjustedActionId((ActionID)ID);

    /// <summary>
    /// Icon Id.
    /// </summary>
    public uint IconID => ID == (uint)ActionID.Sprint ? 104u : _action.Icon;

    private byte CoolDownGroup { get; }

    /// <summary>
    /// Casting time.
    /// </summary>
    //public unsafe float CastTime => ActionManager.GetAdjustedCastTime(ActionType.Action, AdjustedID) / 1000f;

}