using System.Collections.Generic;

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
        CurrentState = AllStates[IncomingState];
        CurrentState.ChangeToThisState();
        return true;
    }
    bool IsValidFromThisState(int IncomingState)
    {
        if (!AllStates.TryGetValue(IncomingState, out IBrainState state))
            return false;

        if (!CurrentState.ValidChangeStates.Contains(IncomingState))
            return false;

        return true;
    }
}
