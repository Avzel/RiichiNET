namespace RiichiNET.Components.Groups;

using RiichiNET.Enums;

internal sealed class MinKanDai: OpenGroup
{
    internal override Mentsu Mentsu { get => Mentsu.Kantsu; }
    internal override Naki Naki { get => Naki.DaiMinKan; }

    internal MinKanDai(Value value, Direction called, (bool exists, bool taken) akadora=default)
    {
        SetKouKanTiles(true, value, called, akadora);
    }

    internal override List<Tile> GetSortedTiles()
    {
        return new List<Tile>(OrderedTiles);
    }
}
