namespace RiichiNET.Core.Components.Groups;

using System.Collections.Generic;

using RiichiNET.Core.Enums;

internal sealed class MinKanShou: OpenGroup
{
    internal override Mentsu Mentsu { get => Mentsu.Kantsu; }
    internal override Naki Naki { get => Naki.ShouMinKan; }

    internal MinKanShou(Tile tile, MinKou pon)
    {
        OrderedTiles = new List<Tile>(pon.OrderedTiles);
        CalledIndex = pon.CalledIndex;

        if (pon.Akadora || tile.akadora) Akadora = true;

        OrderedTiles.Insert(CalledIndex + 1, tile);
    }

    internal override List<Tile> GetSortedTiles()
    {
        return new List<Tile>(OrderedTiles);
    }
}
