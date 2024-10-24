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
        //Allow the agent to move
        ThisAI.agent.speed = 4;
        //Play an alert sound
        ThisAI.creatureVoice.PlayOneShot(ThisAI.vo[Random.Range(0, ThisAI.vo.Length)]);
        ThisAI.creatureSFX.volume = 1;
        //Set the walk cycle on
        ThisAI.creatureAnimator.SetBool("Walk", true);
    }

    public void AI_Interval()
    {

        if (ThisAI.targetPlayer == null || ThisAI.targetPlayer.isPlayerDead || Vector3.Distance(ThisAI.targetPlayer.transform.position, ThisAI.transform.position) > 25f)
        {
            ((IEnemyBrain)ThisAI.brain).TryChangeBrainToState((int)SmallfryBrainStates.IDLE);
            return;
        }

        //Move towards the target player
        ThisAI.SetDestinationToPosition(ThisAI.targetPlayer.transform.position);
    }
}
