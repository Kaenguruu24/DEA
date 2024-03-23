using System.Collections.Generic;
using Godot;
namespace Automata;

public class Simulation
{
    private List<State> States;
    private string CurrentInput;
    private State CurrentState;

    public void Step()
    {
        if (CurrentState == null || CurrentState.IsEndState()) { GD.Print("No state set. Cannot advance simulation."); return; }

        bool Found = false;
        foreach (Path path in CurrentState.GetPaths())
        {
            var EvaluationResult = path.Evaluate(CurrentInput);
            if (EvaluationResult)
            {
                if (Found) { GD.PrintErr("Ambiguous paths"); }

                Found = true;
                GD.Print("Found valid path. Checking for ambiguities...");
            }
        }
        if (Found)
        {
            GD.Print("No ambiguities found. Moving to next state...");
            Path ValidPath = CurrentState.GetPaths().Find(path => path.Evaluate(CurrentInput));

            CurrentState.SelfModulate = new Color(1, 1, 1); // Resetting color modulation

            CurrentInput = ValidPath.GetModifiedInput(CurrentInput);
            CurrentState = ValidPath.GetTarget();

            CurrentState.SelfModulate = new Color(0, 1, 0); // Setting color modulation

            if (CurrentState.IsEndState())
            { GD.PushWarning("End state reached"); }
        }
        else { if (CurrentState.GetPaths().Count == 0) GD.PrintErr("No valid paths found"); else GD.PrintErr("No path condition matched the input"); }
    }

    public Simulation(List<State> States, string StartInput)
    {
        this.States = States;
        this.CurrentInput = StartInput;
        this.CurrentState = States.Count > 0 ? States[0] : null;
    }

    public void SetStartState(State state) { CurrentState = state; CurrentState.SelfModulate = new Color(0, 1, 0); }

    public List<State> GetStates() { return States; }
    public void AddState(State state) { States.Add(state); }

    public string GetCurrentInput() { return CurrentInput; }
}