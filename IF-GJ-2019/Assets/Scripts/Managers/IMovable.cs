namespace Assets.Scripts.Ui
{
    public interface IMovable
    {
        void MoveOneCell(MovementDirection direction);

        void MoveMaxCell(MovementDirection direction);
    }

    public enum MovementDirection
    {
        Up, Down, Left, Right
    }
}