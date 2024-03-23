using System.Text.RegularExpressions;

namespace Automata;

public class Path
{
    private State Target;
    private string Condition;

    public Path(State Target, string Condition)
    {
        this.Target = Target;
        this.Condition = Condition;
    }

    public bool Evaluate(string Input)
    { return Input.StartsWith(Condition); }
    public string GetModifiedInput(string Input)
    { return Regex.Replace(Input, Condition, ""); }
    
    public State GetTarget()
    { return Target; }
    public string GetCondition()
    { return Condition; }
    public void SetCondition(string condition)
    { Condition = condition; }
}