namespace RiichiNET.Components.Groups;

using Enums;

internal class MinJunNaka: OpenGroup
{
    internal override Mentsu Mentsu { get => Mentsu.Shuntsu; }
    internal override Naki Naki { get => Naki.ChiiNaka; }

    internal MinJunNaka(Value value, Direction called, bool akadora=false)
    {
        CalledIndex = 0;
        OrderedTiles[0] = (Tile)value;
        OrderedTiles[1] = (Tile)value - 1;
        OrderedTiles[2] = (Tile)value + 1;
        SetShuntsuAkadora(akadora);
    }

    internal override List<Tile> GetSortedTiles()
    {
        return new List<Tile>()
        {
            OrderedTiles[1],
            OrderedTiles[0],
            OrderedTiles[2]
        };
    }
}
