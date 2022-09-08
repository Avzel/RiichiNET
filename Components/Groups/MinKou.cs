namespace RiichiNET.Components.Groups;

using Enums;

internal sealed class MinKou: OpenGroup
{
    internal override Mentsu Mentsu { get => Mentsu.Koutsu; }
    internal override Naki Naki { get => Naki.Pon; }

    internal MinKou(Value value, Direction called, (bool exists, bool taken) akadora=default)
    {
        OrderedTiles[0] = OrderedTiles[1] = OrderedTiles[2] = (Tile)value;
        if (akadora.exists) Akadora = true;

        switch(called)
        {
            case Direction.Left:
                CalledIndex = 0;
                break;
            case Direction.Up:
                CalledIndex = 1;
                break;
            case Direction.Right:
                CalledIndex = 2;
                break;
        }

        if (Akadora && akadora.taken)
        {
            OrderedTiles[CalledIndex] = new Tile(OrderedTiles[CalledIndex].value, true);
        }
        else if (Akadora && !akadora.taken)
        {
            if (CalledIndex == 0) OrderedTiles[1] = new Tile(OrderedTiles[1].value, true);
            else OrderedTiles[0] = new Tile(OrderedTiles[0].value, true);
        }
    }

    internal override List<Tile> GetSortedTiles()
    {
        return new List<Tile>(OrderedTiles);
    }
}
