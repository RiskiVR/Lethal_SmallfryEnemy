/*********************************
Smallfry ATTACK state
Purpose:
    Hit an enemy once for 10
    damage then return to the
    chasing state
**********************************/

namespace SmallfryBrain;

using System.Collections.Generic;
using BrainInterfaces;
using SmallfryEnemy;
using GameNetcodeStuff;
using UnityEngine;

public class SmallfryState_Attacking(SmallfryEnemy ThisAI) : IBrainState
{

    internal PlayerControllerB? attackTarget;

    public List<int> ValidChangeStates => [
        (int)SmallfryBrainStates.DEAD,
        (int)SmallfryBrainStates.STUNNED,
        (int)SmallfryBrainStates.CHASING,
        (int)SmallfryBrainStates.IDLE,
    ];
    public SmallfryEnemy ThisAI = ThisAI;

    public void ChangeToThisState()
    {
        if (attackTarget == null)
            return;

        //Stop repeat attacks
        ThisAI.attackCooldown = 0.75f;

        attackTarget.DamagePlayer(10);

        //TODO: Add a sound RPC to sync this
        ThisAI.creatureVoice.PlayOneShot(ThisAI.vo[Random.Range(0, ThisAI.vo.Length)]);
        ThisAI.creatureAnimator.SetInteger("AttackInt", Random.Range(0, 2));
        //TODO: Run animator RPC from main branch here
    }

    public void AI_Interval()
    {
        //Return to chasing once the attack is complete
        ((IEnemyBrain)ThisAI.brain).TryChangeBrainToState((int)SmallfryBrainStates.CHASING);
    }

    internal void SetAttackTarget(PlayerControllerB Target)
    {
        attackTarget = Target;
    }
}
