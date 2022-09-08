namespace RiichiNET.Components.Groups;

using Enums;

internal sealed class MinKanShou: OpenGroup
{
    internal override Mentsu Mentsu { get => Mentsu.Kantsu; }
    internal override Naki Naki { get => Naki.ShouMinKan; }
    
    internal int AddedIndex { get; }

    internal MinKanShou(Tile tile, MinKou pon)
    {
        OrderedTiles = new List<Tile>(pon.OrderedTiles);
        CalledIndex = pon.CalledIndex;
        AddedIndex = CalledIndex + 1;

        if (pon.Akadora || tile.akadora) Akadora = true;

        OrderedTiles.Insert(AddedIndex, tile);
    }

    internal override List<Tile> GetSortedTiles()
    {
        return new List<Tile>(OrderedTiles);
    }
}
