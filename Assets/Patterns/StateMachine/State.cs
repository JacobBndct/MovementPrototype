using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Patterns.StateMachine
{
    public abstract class State
    {
        private readonly List<StateTransition> _stateTransitions = new List<StateTransition>();
        public void AddTransition(StateTransition newTransition)
        {
            _stateTransitions.Add(newTransition);
        }
        public State TryTransitions()
        {
            foreach (StateTransition transition in _stateTransitions)
            {
                if (transition._Condition())
                {
                    return transition._newState;
                }
            }

            return null;
        }
        public virtual void EnterState()
        {
            Debug.Log("Entering " + StateName() + " state.");
        }
        public virtual void ExitState()
        {
            Debug.Log("Exiting " + StateName() + " state.");
        }

        public virtual void Update() { }
        public virtual void FixedUpdate() { }
        public virtual void LateUpdate() { }

        protected string StateName()
        {
            return GetType().Name;
        }
    }
}
