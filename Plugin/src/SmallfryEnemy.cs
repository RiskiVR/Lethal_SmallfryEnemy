using System.Collections;
using GameNetcodeStuff;
using UnityEngine;

namespace SmallfryEnemy;

public partial class SmallfryEnemy : EnemyAI
{
    [SerializeField] AudioClip[] vo;
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
        creatureSFX.mute = true;
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
        if (UpdateTargetSelection())
        {
            agent.speed = 4;
            PlayVOServerRpc();

            //Unmute passive noise
            SetPassiveVOServerRpc(false);
            creatureAnimator.SetBool("Walk", true);
            SwitchToBehaviourClientRpc((int)States.Active);
        }
    }
    protected bool UpdateTargetSelection()
    {
        var closestTarget = this.GetClosestPlayer();

        if (closestTarget == null)
        {
            targetPlayer = null;
            return false;
        }

        if (Vector3.Distance(this.transform.position, closestTarget.transform.position) < 25f)
        {
            targetPlayer = closestTarget;
            return true;
        }

        targetPlayer = null;
        return false;
    }
    public void Active()
    {
        if (targetPlayer == null || Vector3.Distance(transform.position, targetPlayer.transform.position) > 25f)
        {
            //This only fires if our target is null or too far away
            Plugin.Logger.LogInfo($"Player is null or too far {targetPlayer} | Abandoning chase");

            targetPlayer = null;
            creatureAnimator.SetBool("Walk", false);

            //Mute passive VO
            SetPassiveVOServerRpc(true);
            agent.speed = 0;
            SwitchToBehaviourClientRpc((int)States.Idle);
            return;
        }
        SetDestinationToPosition(targetPlayer.transform.position);
    }

    public override void OnCollideWithPlayer(Collider other)
    {
        base.OnCollideWithPlayer(other);

        //Plugin.Logger.LogInfo("Smallfry has collided with a player");

        //Check if the attack cooldown is running
        if (attackCooldown > 0f) return;
        //Check if smallfry is dead
        if (isEnemyDead) return;

        //Plugin.Logger.LogInfo("Attack is off cooldown and Smallfry is alive");

        //Get the player we collided with
        //Exit if its not the target or if is not a valid player
        PlayerControllerB collidePlayer = MeetsStandardPlayerCollisionConditions(other);
        if (collidePlayer == null) return;

        //Plugin.Logger.LogInfo($"Smallfry is damaging this player {collidePlayer.name}");

        //This is supposed to create a knock back vector to apply alongside the damage
        //But it seems to do nothing. Todo: look into PlayerControllerB.DamagePlayer
        Vector3 targetVector = (collidePlayer.transform.position - transform.position).normalized * 5;
        collidePlayer.DamagePlayer(10, force: targetVector);
        PlayVOServerRpc();

        //This picks a random attack animation and sets the animator.
        //It'll show on clients because its sync'd via the network animator
        //And this value is serializable across the network, yay!
        creatureAnimator.SetInteger("AttackInt", Random.Range(0, 2));

        //Plugin.Logger.LogInfo("Calling animation RPC");
        DoAnimationServerRpc("Attack");

        //Sets Smallfry's attack on cooldown for this long in seconds
        attackCooldown = 1f;

        //Plugin.Logger.LogInfo("All code in OnCollideWithPlayer has run");
    }

    public override void HitEnemy(int force = 1, PlayerControllerB playerWhoHit = null, bool playHitSFX = false, int hitID = -1)
    {
        base.HitEnemy(force, playerWhoHit, playHitSFX, hitID);
        if (isEnemyDead) return;
        enemyHP -= force;
        PlayVOServerRpc();
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
