/*********************************
Smallfry IDLE state
Purpose:
    Search for targets and enter
    the chasing state
**********************************/

namespace SmallfryBrain;

using System.Collections.Generic;
using BrainInterfaces;
using SmallfryEnemy;
using GameNetcodeStuff;
using UnityEngine;

public class SmallfryState_Idle(SmallfryEnemy ThisAI) : IBrainState
{
    public List<int> ValidChangeStates => [
        (int)SmallfryBrainStates.ATTACKING,
        (int)SmallfryBrainStates.CHASING,
        (int)SmallfryBrainStates.DEAD,
        (int)SmallfryBrainStates.STUNNED,
    ];
    public SmallfryEnemy ThisAI = ThisAI;

    public void ChangeToThisState()
    {
        //Reset our target
        ThisAI.targetPlayer = null;
        //Disable walk cycle
        ThisAI.creatureAnimator.SetBool("Walk", false);
        //Stop sounds
        ThisAI.creatureSFX.volume = 0;
        //Disable movement
        ThisAI.agent.speed = 0;
    }

    public void AI_Interval()
    {
        var colliders = Physics.OverlapSphere(ThisAI.transform.position, 25, LayerMask.GetMask("Player"), QueryTriggerInteraction.Collide);
        foreach (Collider c in colliders)
        {
            if (c.gameObject.TryGetComponent(out PlayerControllerB player) && player.isPlayerControlled && !player.isPlayerDead)
            {
                //Target the player and enter the chasing state
                ThisAI.targetPlayer = player;
                ((IEnemyBrain)ThisAI.brain).TryChangeBrainToState((int)SmallfryBrainStates.CHASING);
            }
        }
    }
}
