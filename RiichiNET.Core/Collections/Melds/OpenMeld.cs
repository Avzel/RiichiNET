namespace RiichiNET.Core.Collections.Melds;

using System.Collections.Generic;
using System.Linq;

using RiichiNET.Core.Components;
using RiichiNET.Core.Enums;

public abstract class OpenMeld: Meld
{
    internal override bool Open { get => true; }

    public IList<Tile> OrderedTiles { get; private protected set; } = new List<Tile>();
    internal int CalledIndex { get; private protected set; }

    public Tile GetCalledTile()
        => OrderedTiles[CalledIndex];

    internal override IList<Tile> GetSortedTiles()
        => OrderedTiles.ToList().AsReadOnly();

    internal override bool HasYaoChuu()
        =>  GetSortedTiles()[0].IsYaoChuu() || 
            (Mentsu == Mentsu.Shuntsu &&  GetSortedTiles()[2].IsTerminal());

    internal override bool HasTerminals()
        =>  GetSortedTiles()[0].IsTerminal() || 
            (Mentsu == Mentsu.Shuntsu &&  GetSortedTiles()[2].IsTerminal());

    internal override bool OnlyHonors()
        => GetSortedTiles()[0].IsHonor();

    internal override bool OnlyGreens()
        => GetSortedTiles()[0].IsGreen();

    internal override bool OnlyDragons()
        => this[0].IsDragon();

    internal override bool OnlyWinds()
        => this[0].IsWind();

    internal override bool Contains(Value value)
    {
        foreach (Tile tile in OrderedTiles)
        {
            if (tile == value) return true;
        }
        return false;
    }

    private protected void SetShuntsuAkadora(bool akadora)
    {
        if ((Akadora = akadora) == true) for (int i = 0; i < OrderedTiles.Count(); i++)
        {
            if (OrderedTiles[i].IsFive())
            {
                OrderedTiles[i] = OrderedTiles[i] with { akadora = true };
            }
        }
    }

    private protected void SetKouKanTiles
    (
        bool kantsu, 
        Value value, 
        Direction called, 
        (bool exists, bool taken) akadora=default
    )
    {
        OrderedTiles[0] = OrderedTiles[1] = OrderedTiles[2] = (Tile)value;
        if (kantsu) OrderedTiles[3] = (Tile)value;
        if (akadora.exists) Akadora = true;

        int offset = kantsu ? 1 : 0;
        CalledIndex = called switch
        {
            Direction.Left => 0,
            Direction.Up => 1 + offset,
            Direction.Right => 2 + offset,
            _ => 0
        };

        if (Akadora) for (int i = 0; i < offset+3; i++)
        {
            if ((akadora.taken) == (i == CalledIndex))
            {
                OrderedTiles[i] = OrderedTiles[i] with { akadora = true };
                break;
            }
        }
    }
}
