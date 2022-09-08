namespace RiichiNET.Components.Groups;

using Enums;

internal sealed class MinKou: OpenGroup
{
    internal override Mentsu Mentsu { get => Mentsu.Koutsu; }
    internal override Naki Naki { get => Naki.Pon; }

    internal MinKou(Value value, Direction called, (bool exists, bool taken) akadora=default)
    {
        SetKouKanTiles(false, value, called, akadora);
    }

    internal override List<Tile> GetSortedTiles()
    {
        return new List<Tile>(OrderedTiles);
    }
}
