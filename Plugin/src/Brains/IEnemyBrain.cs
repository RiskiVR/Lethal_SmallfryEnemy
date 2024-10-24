using System.Collections.Generic;
using LethalLib;
using Unity.Netcode;

namespace BrainInterfaces;
internal interface IEnemyBrain
{
    public IBrainState CurrentState { get; set; }
    public Dictionary<int, IBrainState> AllStates { get; }
    bool TryChangeBrainToState(int IncomingState)
    {
        //This also checks if the state exists
        if (!IsValidFromThisState(IncomingState))
            return false;

        //State is valid and exists
        ChangeToStateServerRPC(IncomingState);
        return true;
    }
    bool IsValidFromThisState(int IncomingState)
    {
        if (!AllStates.TryGetValue(IncomingState, out IBrainState state))
        {
            Plugin.logger.LogWarning($"Unable to change to state in EnemyBrain, brain state is not in the dictionary! State ID: {IncomingState}");
            return false;
        }

        if (!CurrentState.ValidChangeStates.Contains(IncomingState))
        {
            Plugin.logger.LogWarning($"Unable to change to state in EnemyBrain, brain state is not a valid target for the current state! State ID: {IncomingState}");
            return false;
        }

        return true;
    }

    [ClientRpc]
    void ChangeToStateClientRPC(int IncomingState)
    {
        Plugin.logger.LogInfo($"Client changing brain state to {IncomingState}");
        CurrentState = AllStates[IncomingState];
        CurrentState.ChangeToThisState();
    }

    [ServerRpc(RequireOwnership = false)]
    void ChangeToStateServerRPC(int IncomingState)
    {
        Plugin.logger.LogInfo($"Requesting server changing brain state to {IncomingState}");
        ChangeToStateClientRPC(IncomingState);
    }
}
