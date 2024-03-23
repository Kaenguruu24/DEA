using Godot;

namespace Automata;

public partial class Connection : Line2D
{
    [Export]
    public Font ConditionFont;

    private Path AssociatedPath;
    private State Origin;

    public override void _Ready()
    {
        GetNode<LineEdit>("Config/RegexCondition").TextChanged += (text) => AssociatedPath.SetCondition(text);
        GetNode<Button>("Config/Close").Pressed += () => GetNode<CanvasLayer>("Config").Hide();
    }
    public override void _Process(double delta)
    {
        Vector2 mousePosition = GetGlobalMousePosition();
        Vector2 pointA = Origin.GlobalPosition;
        Vector2 pointB = AssociatedPath.GetTarget().GlobalPosition;

        Width = IsMouseOnLine(mousePosition, pointA, pointB, Width) ? 5 : 3;
        QueueRedraw();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed)
        {
            Vector2 mousePosition = GetGlobalMousePosition();
            Vector2 pointA = Origin.GlobalPosition;
            Vector2 pointB = AssociatedPath.GetTarget().GlobalPosition;

            if (IsMouseOnLine(mousePosition, pointA, pointB, Width))
            {
                GetNode<CanvasLayer>("Config").Show();
                GD.Print("SELECTED CONNECTION BETWEEN " + Origin.Name + " AND " + AssociatedPath.GetTarget().Name);
            }
        }
    }

    public override void _Draw()
    {
        if (AssociatedPath == null) return;
        DrawString(ConditionFont, ToLocal(Origin.GlobalPosition.MoveToward(AssociatedPath.GetTarget().GlobalPosition, 0.5f)), AssociatedPath.GetCondition(), HorizontalAlignment.Center, 200, 16);
    }

    public void SetAssociatedPath(Path path) { AssociatedPath = path; GetNode<LineEdit>("Config/RegexCondition").Text = path.GetCondition(); }
    public void SetOriginState(State state) { Origin = state; }

    public bool IsMouseOnLine(Vector2 mousePosition, Vector2 pointA, Vector2 pointB, float lineWidth)
    {
        mousePosition -= new Vector2(0, 8);

        float lineLength = pointA.DistanceTo(pointB);
        float triangleArea = Mathf.Abs((pointA.X - mousePosition.X) * (pointB.Y - mousePosition.Y) - (pointB.X - mousePosition.X) * (pointA.Y - mousePosition.Y));
        float distanceFromPointToLine = (2 * triangleArea) / lineLength;

        return Mathf.Abs((lineWidth / 2) - distanceFromPointToLine) < 15;
    }
}
