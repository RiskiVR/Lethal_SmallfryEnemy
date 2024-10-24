using System.Collections;
using GameNetcodeStuff;
using UnityEngine;
using SmallfryBrain;
using BrainInterfaces;

namespace SmallfryEnemy;

public class SmallfryEnemy : EnemyAI
{
    public AudioClip[] vo;
    public SmallfryEnemyBrain brain;
    internal float attackCooldown;

    public override void Start()
    {
        base.Start();
        brain = new(this);
        agent.speed = 0;
    }
    public override void Update()
    {
        base.Update();
        if (attackCooldown > 0) attackCooldown -= Time.deltaTime;
    }
    public override void DoAIInterval() => brain.CurrentState.AI_Interval();

    public override void OnCollideWithPlayer(Collider other)
    {
        //Don't bother if we're dead but not disposed yet
        if (isEnemyDead)
            return;

        base.OnCollideWithPlayer(other);

        if (attackCooldown > 0f)
            return;

        PlayerControllerB player = MeetsStandardPlayerCollisionConditions(other);

        if (brain.AllStates[(int)SmallfryBrainStates.ATTACKING] is SmallfryState_Attacking attackState)
        {
            attackState.SetAttackTarget(player);
        }

    }

    public override void HitEnemy(int force = 1, PlayerControllerB playerWhoHit = null, bool playHitSFX = false, int hitID = -1)
    {
        base.HitEnemy(force, playerWhoHit, playHitSFX, hitID);
        Plugin.Logger.LogInfo($"Hit force: {force}");
        enemyHP -= force;
        creatureVoice.PlayOneShot(vo[Random.Range(0, vo.Length)]);
        if (enemyHP <= 0)
        {
            ((IEnemyBrain)brain).TryChangeBrainToState((int)SmallfryBrainStates.DEAD);
            KillEnemyOnOwnerClient();
        }
    }

    public override void KillEnemy(bool destroy = false)
    {
        base.KillEnemy(destroy);
        creatureSFX.volume = 0;
        StartCoroutine(AnimatorSlowSpeed());
        StartCoroutine(FallOverLol());
    }

    IEnumerator AnimatorSlowSpeed()
    {
        while (creatureAnimator.speed > 0)
        {
            creatureAnimator.speed -= 1f * Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator FallOverLol()
    {
        float rotateTime = 0f;
        while (rotateTime < 0.2f)
        {
            rotateTime += Time.deltaTime;
            transform.Rotate(-350f * Time.deltaTime, 0, 0);
            yield return null;
        }
    }
}
