using System.Collections.Generic;

namespace Automata;

public class NFAState
{
    private bool IsEnd;
    private List<NFAState> epsilonTransitions;
    private Dictionary<string, List<NFAState>> transitions;

    public NFAState(bool IsEnd)
    {
        this.IsEnd = IsEnd;
        epsilonTransitions = new List<NFAState>();
        transitions = new Dictionary<string, List<NFAState>>();
    }

    public void AddTransition(string symbol, NFAState state)
    {
        if (transitions.ContainsKey(symbol))
        { transitions[symbol].Add(state); }
        else
        { transitions.Add(symbol, new List<NFAState> { state }); }
    }

    public void AddEpsilonTransition(NFAState state)
    { epsilonTransitions.Add(state); }



    public List<NFAState> GetEpsilonTransitions()
    { return epsilonTransitions; }
    public Dictionary<string, List<NFAState>> GetTransitions()
    { return transitions; }
    public bool GetIsEnd()
    { return IsEnd; }
    public void SetIsEnd(bool IsEnd)
    { this.IsEnd = IsEnd; }
}