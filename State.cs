using Godot;
using System.Collections.Generic;
namespace Automata;

public partial class State : Button
{
    private List<Path> Paths = new List<Path>();
    private bool IsValidEndState = false;

    private string DisplayName = "Q";
    
    
    public bool IsInitialized = false;


    // Internal use
    public State(List<Path> Paths, bool IsValidEndState = false)
    {
        this.Paths = Paths;
        this.IsValidEndState = IsValidEndState;
    }
    public State() { }

    public void Initialize()
    {
        IsInitialized = true;
        Text = DisplayName;
    }

    public void AddPath(Path path) { Paths.Add(path); }

    public List<Path> GetPaths() { return Paths; }
    public bool IsEndState() { return IsValidEndState; }
    public void SetEndState(bool value) { IsValidEndState = value; }
}