using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Patterns.StateMachine
{
    public class StateTransition
    {
        public State _newState;
        public Func<bool> _Condition;

        public StateTransition(State newState, Func<bool> condition)
        {
            _newState = newState;
            _Condition = condition;
        }
    }
}
