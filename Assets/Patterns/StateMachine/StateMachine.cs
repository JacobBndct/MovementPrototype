using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Patterns.StateMachine
{
    public class StateMachine : MonoBehaviour
    {
        private State _currentPlayerState = null;

        public void SwitchState(State newPlayerState)
        {
            _currentPlayerState?.ExitState();
            _currentPlayerState = newPlayerState;
            _currentPlayerState?.EnterState();
        }

        public void Update()
        {
            _currentPlayerState?.Update();
        }

        public void FixedUpdate()
        {
            _currentPlayerState?.FixedUpdate();
        }

        public void LateUpdate()
        {
            _currentPlayerState?.LateUpdate();

            State transitionState = _currentPlayerState?.TryTransitions();
            if (transitionState != null)
            {
                SwitchState(transitionState);
            }
        }
    }
}
