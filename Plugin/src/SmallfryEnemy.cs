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
        var colliders = Physics.OverlapSphere(transform.position, 50, LayerMask.GetMask("Player"), QueryTriggerInteraction.Collide);
        foreach (Collider c in colliders)
        {
            if (c.gameObject.TryGetComponent(out PlayerControllerB player) && player.isPlayerControlled && !player.isPlayerDead)
            {
                targetPlayer = player;
                agent.speed = 7;
                creatureVoice.PlayOneShot(vo[Random.Range(0, vo.Length)]);
                creatureSFX.volume = 1;
                creatureAnimator.SetBool("Walk", true);
                SwitchToBehaviourClientRpc((int)States.Active);
            }
        }
    }
    public void Active()
    {
        if (targetPlayer == null)
        {
            creatureAnimator.SetBool("Walk", false);
            creatureSFX.volume = 0;
            agent.speed = 0;
            SwitchToBehaviourClientRpc((int)States.Idle);
            return;
        }
        SetDestinationToPosition(targetPlayer.transform.position);
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
        creatureVoice.PlayOneShot(vo[Random.Range(0, vo.Length)]);
        creatureAnimator.SetInteger("AttackInt", Random.Range(0, 2));
        networkAnimator.SetTrigger("Attack");
        attackCooldown = 0.75f;
    }
}