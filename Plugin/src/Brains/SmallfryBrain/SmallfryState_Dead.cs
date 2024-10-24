/*********************************
Smallfry DEAD state
Purpose:
    Block all actions until we
    are disposed
**********************************/

namespace SmallfryBrain;

using System.Collections.Generic;
using BrainInterfaces;

public class SmallfryState_Dead() : IBrainState
{
    public List<int> ValidChangeStates => [];

    public void ChangeToThisState()
    {
        return;
    }

    public void AI_Interval()
    {
        return;
    }
}
