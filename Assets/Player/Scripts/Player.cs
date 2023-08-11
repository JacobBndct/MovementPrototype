using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //single source of truth
    PlayerStateMachine _stateMachine;

    bool isGrounded;
    int maxJumps;
    int currentJumps;


}
