using System.Collections.Generic;

namespace BrainInterfaces;
public interface IBrainState
{
    public List<int> ValidChangeStates { get; }
    public void ChangeToThisState();
    public void AI_Interval();
}
