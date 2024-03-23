    using Godot;
using System.Collections.Generic;
using System.Linq;

namespace Automata;

public partial class Main : Node2D
{
    private Button DragStateObject = null;
    private Label DragConnectionObject = null;
    private Line2D ConnectionObject = null;

    private State[] SelectedStates = new State[2];
    private List<Vector2> linePoints = new List<Vector2>();

    private Simulation Sim;

    private string InitialInput = "Hallo";

    public override void _Ready()
    {
        Engine engine = new Engine();
        NFA testNFA = engine.ToNFA(engine.toPostfix(engine.insertExplicitConcatOperator("[a-z]*|[A-Z]*")));

        foreach (System.Collections.Generic.KeyValuePair<string, List<NFAState>> nextState in testNFA.GetStartState().GetTransitions())
        {
            GD.Print(nextState.Key);
            foreach (NFAState state in nextState.Value) { }
        }

        GetNode<Button>("New").Pressed += () =>
        {
            var newDragStateObject = GD.Load<PackedScene>("res://State.tscn").Instantiate<Button>();
            newDragStateObject.Name = "StateObj_" + GetNode<Node2D>("States").GetChildCount();
            GetNode<Node2D>("States").AddChild(newDragStateObject);

            newDragStateObject.Pressed += () => OnStatePressed((State)newDragStateObject);

            DragStateObject = newDragStateObject;
            Sim.AddState((State) newDragStateObject);
        };

        Sim = new Simulation(new List<State>(), InitialInput);

        GetNode<Button>("Step").Pressed += () => Sim.Step();

        GetNode<AnimatedSprite2D>("zweiter_versuch").Play();
        GetNode<AnimatedSprite2D>("zweiter_versuch2").Play();
    }

    public override void _Process(double delta)
    {
        if (DragStateObject != null)
        { DragStateObject.GlobalPosition = GetGlobalMousePosition() - DragStateObject.Size / 2; }
        if (ConnectionObject != null)
        {
            List<Vector2> MostCurrentList = linePoints.ToList();
            List<Vector2> SmoothedList = new List<Vector2>();

            MostCurrentList.Add(GetGlobalMousePosition());

            for (int i = 0; i < MostCurrentList.Count; i++)
            {
                if (i == 0 || i == MostCurrentList.Count - 1)
                {
                    SmoothedList.Add(MostCurrentList[i]);
                    continue;
                }

                Vector2 p0 = MostCurrentList[i - 1];
                Vector2 p1 = MostCurrentList[i];
                Vector2 p2 = MostCurrentList[i + 1];
            }

            ConnectionObject.Points = MostCurrentList.ToArray();
        }

        GetNode<Label>("CurrentInput").Text = Sim.GetCurrentInput();
        GetNode<Label>("PassedInput").Text = InitialInput.Replace(Sim.GetCurrentInput(), "");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton && mouseButton.IsPressed())
        {
            if (mouseButton.ButtonIndex == MouseButton.Right)
            {
                if (DragStateObject == null) return;
                DragStateObject.QueueFree();
                DragStateObject = null;
            }
            else if (mouseButton.ButtonIndex == MouseButton.Left)
            {
                if (ConnectionObject != null) { linePoints.Add(GetGlobalMousePosition()); }

                if (DragStateObject == null) return;
                DragStateObject = null;
            }
        } else if (@event is InputEventKey keyEvent && keyEvent.IsPressed())
        {
            if (keyEvent.Keycode == Key.S)
            {
                if (SelectedStates[0] != null) Sim.SetStartState(SelectedStates[0]);
                SelectedStates[0] = null;
                ConnectionObject?.QueueFree();
                ConnectionObject = null;
            } else if (keyEvent.Keycode == Key.E)
            {
                if (SelectedStates[0] != null) SelectedStates[0].SetEndState(true);
                SelectedStates[0] = null;
                ConnectionObject?.QueueFree();
                ConnectionObject = null;
            }
        }
    }

    private void OnStatePressed(State State)
    {
        if (!State.IsInitialized) { State.Initialize(); return; }

        if (SelectedStates[0] == null)
        {
            SelectedStates[0] = State;

            Line2D line = new Line2D { Name = "Line_" + GetNode<Node2D>("Lines").GetChildCount(), Width = 2 };
            linePoints.Add(State.GlobalPosition + State.Size / 2);
            GetNode<Node2D>("Lines").AddChild(line);
            ConnectionObject = line;
        }
        else if (SelectedStates[1] == null)
        {
            SelectedStates[1] = State;

            Path NewPath = new Path(SelectedStates[1], "H");

            SelectedStates[0].AddPath(NewPath);
            ConnectionObject.QueueFree();
            ConnectionObject = null;

            Connection line = GD.Load<PackedScene>("res://Connection.tscn").Instantiate<Connection>();
            line.SetAssociatedPath(NewPath);
            line.SetOriginState(SelectedStates[0]);
            GetNode<Node2D>("Lines").AddChild(line);


            line.Points = linePoints.ToArray().SkipLast(1).Append(State.GlobalPosition + State.Size / 2).ToArray();

            SelectedStates[0] = null;
            SelectedStates[1] = null;
            linePoints.Clear();
        }
    }

    private Vector2 GetClosestPointOnLine(Vector2 A, Vector2 B, Vector2 P)
    {
        Vector2 AP = P - A;
        Vector2 AB = B - A;
        float ab2 = AB.X * AB.X + AB.Y * AB.Y;
        float ap_ab = AP.X * AB.X + AP.Y * AB.Y;
        float t = ap_ab / ab2;
        if (t < 0.0f)
            t = 0.0f;
        else if (t > 1.0f)
            t = 1.0f;
        return A + AB * t;
    }
}
