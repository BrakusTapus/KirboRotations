using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KirboRotations.Custom.Actions;

/// <summary>
/// Properties for the 'BaseAction' class
/// </summary>
public interface ICustomAction
{
    /// <summary>
    /// How long the action has been in cooldown for.
    /// </summary>
    public int CDTimeElapsed { get; set; }

    /// <summary>
    /// Time until next charge.
    /// </summary>
    public int CDTimeOneCharge { get; set; }
}