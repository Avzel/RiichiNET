namespace RiichiNET.Components.Groups;

using Enums;

internal sealed class MinKanDai: OpenGroup
{
    internal override Mentsu Mentsu { get => Mentsu.Kantsu; }
    internal override Naki Naki { get => Naki.DaiMinKan; }

    internal MinKanDai(Value value, Direction called, (bool exists, bool taken) akadora=default)
    {
        OrderedTiles[0] = OrderedTiles[1] = OrderedTiles[2] = OrderedTiles[3] = (Tile)value;
        if (akadora.exists) Akadora = true;

        switch(called)
        {
            case Direction.Left:
                CalledIndex = 0;
                break;
            case Direction.Up:
                CalledIndex = 2;
                break;
            case Direction.Right:
                CalledIndex = 3;
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
