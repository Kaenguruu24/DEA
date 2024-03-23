namespace Automata;

public class NFA
{
    private NFAState startState;
    private NFAState endState;

    public NFA()
    {
        startState = new NFAState(false);
        endState = new NFAState(true);
        startState.AddEpsilonTransition(endState);
    }

    public NFA(string symbol)
    {
        startState = new NFAState(false);
        endState = new NFAState(true);

        startState.AddTransition(symbol, endState);
    }

    public NFA(NFAState startState, NFAState endState)
    {
        this.startState = startState;
        this.endState = endState;
    }

    public NFAState GetEndState()
    { return endState; }
    public NFAState GetStartState()
    { return startState; }
}