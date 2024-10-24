/*********************************
Smallfry CHASE state
Purpose:
    Move towards a player and
    enter the attacking state
    when close enough to attack
**********************************/

namespace SmallfryBrain;

using System.Collections.Generic;
using BrainInterfaces;
using SmallfryEnemy;
using GameNetcodeStuff;
using UnityEngine;

public class SmallfryState_Chase(SmallfryEnemy ThisAI) : IBrainState
{
    public List<int> ValidChangeStates => [
        (int)SmallfryBrainStates.ATTACKING,
        (int)SmallfryBrainStates.IDLE,
        (int)SmallfryBrainStates.DEAD,
        (int)SmallfryBrainStates.STUNNED,
    ];
    public SmallfryEnemy ThisAI = ThisAI;

    public void ChangeToThisState()
    {
        ThisAI.SwitchToBehaviourClientRpc((int)SmallfryBrainStates.CHASING);
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
