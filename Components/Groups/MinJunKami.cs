namespace RiichiNET.Components.Groups;

using Enums;

internal class MinJunKami: OpenGroup
{
    internal override Mentsu Mentsu { get => Mentsu.Shuntsu; }
    internal override Naki Naki { get => Naki.ChiiKami; }

    internal MinJunKami(Value value, Direction called, bool akadora=false)
    {
        CalledIndex = 0;

        OrderedTiles[0] = (Tile)value;
        OrderedTiles[1] = (Tile)value - 2;
        OrderedTiles[2] = (Tile)value - 1;

        Akadora = akadora;

        if (Akadora)
        {
            if ((int)OrderedTiles[0].value % 5 == 0) OrderedTiles[0] = new Tile(OrderedTiles[0].value, true);
            else if ((int)OrderedTiles[1].value % 5 == 0) OrderedTiles[1] = new Tile(OrderedTiles[1].value, true);
            else if ((int)OrderedTiles[2].value % 5 == 0) OrderedTiles[2] = new Tile(OrderedTiles[2].value, true);
        }
    }

    internal override List<Tile> GetSortedTiles()
    {
        return new List<Tile>()
        {
            OrderedTiles[1],
            OrderedTiles[2],
            OrderedTiles[0]
        };
    }
}
