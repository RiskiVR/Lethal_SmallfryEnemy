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
        ThisAI.targetPlayer = null;
        ThisAI.SwitchToBehaviourClientRpc((int)SmallfryBrainStates.IDLE);
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

                //Add this to CHASING AI_Interval
                /*
                ThisAI.agent.speed = 4;
                ThisAI.creatureVoice.PlayOneShot(ThisAI.vo[Random.Range(0, ThisAI.vo.Length)]);
                ThisAI.creatureSFX.volume = 1;
                ThisAI.creatureAnimator.SetBool("Walk", true);
                */
            }
        }
    }
}
