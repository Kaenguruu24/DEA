using System.Collections;
using Godot;

namespace Automata;

public class Engine
{
    public Engine()
    { }

    public NFA ToNFA(string postfixExp)
    {
        if (postfixExp == "")
        { return new NFA(); }

        Stack stack = new Stack();

        foreach (char token in postfixExp)
        {
            if (token == '*')
            { stack.Push(closure((NFA)stack.Pop())); }
            else if (token == '|')
            {
                NFA right = (NFA)stack.Pop();
                NFA left = (NFA)stack.Pop();
                stack.Push(union(left, right));
            }
            else if (token == '.')
            {
                NFA right = (NFA)stack.Pop();
                NFA left = (NFA)stack.Pop();
                stack.Push(concat(left, right));
            }
            else
            { stack.Push(new NFA(token.ToString())); }
        }

        return (NFA)stack.Pop();
    }

    public string insertExplicitConcatOperator(string exp)
    {
        string output = "";

        for (int i = 0; i < exp.Length; i++)
        {
            char token = exp[i];
            output += token;

            if (token == '(' || token == '|')
            { continue; }

            if (i < exp.Length - 1)
            {
                char lookahead = exp[i + 1];
                if (lookahead == '*' || lookahead == '?' || lookahead == '+' || lookahead == '|' || lookahead == ')')
                { continue; }

                output += '.';
            }
        }

        return output;
    }

    public char peek(Stack stack)
    {
        if (stack.Count <= 2) return '\0'; // null character

        object obj = stack.Pop();
        object obj2 = stack.Pop();
        stack.Push(obj2);
        stack.Push(obj);
        return (char)obj2;
    }

    Godot.Collections.Dictionary<char, int> operatorPrecedence = new Godot.Collections.Dictionary<char, int>() { { '|', 0 }, { '.', 1 }, { '?', 2 }, { '*', 2 }, { '+', 2 } };

    public string toPostfix(string exp)
    {
        string output = "";
        Stack operatorStack = new Stack();

        foreach (char token in exp)
        {
            if (token == '.' || token == '|' || token == '*' || token == '?' || token == '+')
            {
                if (peek(operatorStack) == '\0') continue;
                
                while (operatorStack.Count > 0 && peek(operatorStack) != '(' && operatorPrecedence[peek(operatorStack)] >= operatorPrecedence[token])
                { output += operatorStack.Pop(); }

                operatorStack.Push(token);
            }
            else if (token == '(' || token == ')')
            {
                if (token == '(')
                { operatorStack.Push(token); }
                else
                {
                    while (peek(operatorStack) != '(')
                    { output += operatorStack.Pop(); }
                    operatorStack.Pop();
                }
            }
            else { output += token; }
        }

        while (operatorStack.Count > 0)
        { output += operatorStack.Pop(); }

        return output;
    }


    /*
        Mark accepting state of N(S) as not accepting.
        Add transition from it to start state of N(T).
        i denotes start state of N(S) and f denotes accepting state of N(T).
        Gives an NFA that recognizes all string concatenations vw where v belongs to L(S) and w belongs to L(T).
    */
    public NFA concat(NFA first, NFA second)
    {
        first.GetEndState().AddEpsilonTransition(second.GetStartState());
        first.GetEndState().SetIsEnd(false);

        return first;
    }

    /*
        Introduce start state i.
        Add Epsilon-trainsitions from it to start states of N(S) and N(T).
        Add transitions from end states of N(S) and N(T) to new f state.
        Mark as not accepting.
        Resulting NFA will recognize strings that either belong to L(S) or L(T).
    */
    public NFA union(NFA first, NFA second)
    {
        NFAState newStart = new NFAState(false);
        newStart.AddEpsilonTransition(first.GetStartState());
        newStart.AddEpsilonTransition(second.GetStartState());

        NFAState newEnd = new NFAState(true);

        first.GetEndState().AddEpsilonTransition(newEnd);
        second.GetEndState().AddEpsilonTransition(newEnd);

        first.GetEndState().SetIsEnd(false);
        second.GetEndState().SetIsEnd(false);

        NFAState startState = newStart;
        NFAState endState = newEnd;

        return new NFA(newStart, newEnd);
    }

    /*
        Introduce i as start and f as accepting state.
        Add Epsilon-transitions: i to f, i to start state of N(S).
        Connect accepting state of N(S) with f.
        Add transition from end state of N(S) to its start state.
        Mark end state of N(S) as intermediate.
    */
    public NFA closure(NFA nfa)
    {
        NFAState newStart = new NFAState(false);
        NFAState newEnd = new NFAState(true);

        newStart.AddEpsilonTransition(nfa.GetStartState());
        newStart.AddEpsilonTransition(newEnd);

        nfa.GetEndState().AddEpsilonTransition(nfa.GetStartState());
        nfa.GetEndState().AddEpsilonTransition(newEnd);

        nfa.GetEndState().SetIsEnd(false);

        NFAState startState = newStart;
        NFAState endState = newEnd;

        return new NFA(newStart, newEnd);
    }
}