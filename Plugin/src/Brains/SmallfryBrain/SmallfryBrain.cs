using System.Collections.Generic;
using BrainInterfaces;

namespace SmallfryBrain;
public class SmallfryEnemyBrain : IEnemyBrain
{
    public SmallfryEnemy.SmallfryEnemy ThisAI { get; }
    public IBrainState CurrentState { get => currentState; set => currentState = value; }
    protected IBrainState currentState;
    public Dictionary<int, IBrainState> AllStates => _allStates;
    protected Dictionary<int, IBrainState> _allStates;

    public SmallfryEnemyBrain(SmallfryEnemy.SmallfryEnemy ThisAI)
    {
        this.ThisAI = ThisAI;
        currentState = new SmallfryState_Idle(ThisAI);
        _allStates = new()
        {
            {(int)SmallfryBrainStates.IDLE, CurrentState},
            {(int)SmallfryBrainStates.CHASING, new SmallfryState_Chase(ThisAI)},
            {(int)SmallfryBrainStates.ATTACKING, new SmallfryState_Attacking(ThisAI)},
            {(int)SmallfryBrainStates.DEAD, new SmallfryState_Dead()}
        };
    }
}
