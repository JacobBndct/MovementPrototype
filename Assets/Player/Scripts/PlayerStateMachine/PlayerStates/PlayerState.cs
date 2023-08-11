using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Patterns.StateMachine;

public abstract class PlayerState : State
{
    protected readonly Player _player;

    public PlayerState(Player player)
    {
        _player = player;
    }
}
