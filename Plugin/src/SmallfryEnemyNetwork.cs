using UnityEngine;
using Unity.Netcode;

namespace SmallfryEnemy;

public partial class SmallfryEnemy : EnemyAI
{
    [ClientRpc]
    public void DoAnimationClientRpc(string animationName)
    {
        //Plugin.Logger.LogInfo($"Performing this animation {animationName}");
        creatureAnimator.SetTrigger(animationName);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DoAnimationServerRpc(string animationName)
    {
        //Plugin.Logger.LogInfo($"Asking the server for this animation {animationName}");
        DoAnimationClientRpc(animationName);
    }

    [ClientRpc]
    public void PlayVOClientRpc()
    {
        //Plugin.Logger.LogInfo("Playing smallfry VO");

        creatureVoice.pitch = Random.Range(1f, 1.3f);
        creatureVoice.PlayOneShot(vo[Random.Range(0, vo.Length)]);
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayVOServerRpc()
    {
        //Plugin.Logger.LogInfo("Asking the server for smallfry VO");
        PlayVOClientRpc();
    }

    [ClientRpc]
    public void SetPassiveVOClientRpc(bool Muted)
    {
        //Plugin.Logger.LogInfo($"Change passive VO to Muted:{Muted}");

        creatureSFX.mute = Muted;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPassiveVOServerRpc(bool Muted)
    {
        //Plugin.Logger.LogInfo($"Asking server to change passive VO to Muted:{Muted}");

        SetPassiveVOClientRpc(Muted);
    }
}
