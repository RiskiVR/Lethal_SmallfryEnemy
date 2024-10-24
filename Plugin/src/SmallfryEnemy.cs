using System.Collections;
using GameNetcodeStuff;
using UnityEngine;
using Unity.Netcode.Components;

namespace SmallfryEnemy;

public class SmallfryEnemy : EnemyAI
{
    [SerializeField] AudioClip[] vo;
    [SerializeField] NetworkAnimator networkAnimator;
    float attackCooldown;
    enum States
    {
        Idle,
        Active
    }
    public override void Start()
    {
        base.Start();
        agent.speed = 0;
    }
    public override void Update()
    {
        base.Update();
        if (targetPlayer != null && targetPlayer.isPlayerDead) targetPlayer = null;
        if (attackCooldown > 0) attackCooldown -= Time.deltaTime;
    }
    public override void DoAIInterval()
    {
        base.DoAIInterval();
        switch (currentBehaviourStateIndex)
        {
            case (int)States.Idle: 
                Idle();
                break;
            case (int)States.Active:
                Active();
                break;
        }
    }
    public void Idle()
    {
        var colliders = Physics.OverlapSphere(transform.position, 25, LayerMask.GetMask("Player"), QueryTriggerInteraction.Collide);
        foreach (Collider c in colliders)
        {
            if (c.gameObject.TryGetComponent(out PlayerControllerB player) && player.isPlayerControlled && !player.isPlayerDead)
            {
                targetPlayer = player;
                agent.speed = 4;
                PlayVO();
                creatureSFX.volume = 1;
                creatureAnimator.SetBool("Walk", true);
                SwitchToBehaviourClientRpc((int)States.Active);
            }
        }
    }
    public void Active()
    {
        if (targetPlayer == null || Vector3.Distance(targetPlayer.transform.position, transform.position) > 25f)
        {
            targetPlayer = null;
            creatureAnimator.SetBool("Walk", false);
            creatureSFX.volume = 0;
            agent.speed = 0;
            SwitchToBehaviourClientRpc((int)States.Idle);
            return;
        }
        SetDestinationToPosition(targetPlayer.transform.position);
    }

    [ContextMenu("PlayVO")]
    public void PlayVO()
    {
        creatureVoice.pitch = Random.Range(1f, 1.3f);
        creatureVoice.PlayOneShot(vo[Random.Range(0, vo.Length)]);
    }
    
    public override void OnCollideWithPlayer(Collider other)
    {
        base.OnCollideWithPlayer(other);
        if (attackCooldown > 0f) return;
        if (isEnemyDead) return;
        PlayerControllerB player = MeetsStandardPlayerCollisionConditions(other);
        if (player == null || player != targetPlayer) return;
        Vector3 targetVector = (targetPlayer.transform.position - transform.position).normalized * 5;
        player.DamagePlayer(10, force: targetVector);
        PlayVO();
        creatureAnimator.SetInteger("AttackInt", Random.Range(0, 2));
        networkAnimator.SetTrigger("Attack");
        attackCooldown = 0.75f;
    }

    public override void HitEnemy(int force = 1, PlayerControllerB playerWhoHit = null, bool playHitSFX = false, int hitID = -1)
    {
        base.HitEnemy(force, playerWhoHit, playHitSFX, hitID);
        if (isEnemyDead) return;
        enemyHP -= force;
        PlayVO();
        if (enemyHP <= 0) KillEnemyOnOwnerClient();
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