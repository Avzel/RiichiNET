namespace RiichiNET.Core.Collections.Melds;

using System.Collections.Generic;
using System.Linq;

using RiichiNET.Core.Components;
using RiichiNET.Core.Enums;

public abstract class OpenMeld: Meld
{
    public override bool Open { get => true; }

    public IList<Tile> OrderedTiles { get; private protected set; } = new List<Tile>();
    internal int CalledIndex { get; private protected set; }

    public Tile GetCalledTile()
        => OrderedTiles[CalledIndex];

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

    public override bool Contains(Value value)
    {
        foreach (Tile tile in OrderedTiles)
        {
            if (tile == value) return true;
        }
        return false;
    }

    internal override Tile this[int i]
    {
        get
        {
            if (i < OrderedTiles.Count() - 1) return OrderedTiles[i];
            else return (Tile)Value.None;
        }
    }

    private protected void SetShuntsuAkadora(bool akadora)
    {
        if ((Akadora = akadora) == true)
        {
            if ((int)OrderedTiles[0].value % 5 == 0)
                OrderedTiles[0] = new Tile(OrderedTiles[0].value, true);
            else if ((int)OrderedTiles[1].value % 5 == 0)
                OrderedTiles[1] = new Tile(OrderedTiles[1].value, true);
            else if ((int)OrderedTiles[2].value % 5 == 0)
                OrderedTiles[2] = new Tile(OrderedTiles[2].value, true);
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
        if (akadora.exists) Akadora = true;

        if (kantsu) OrderedTiles[3] = (Tile)value;
        int offset = kantsu ? 1 : 0;

        switch(called)
        {
            case Direction.Left:
                CalledIndex = 0;
                break;
            case Direction.Up:
                CalledIndex = 1 + offset;
                break;
            case Direction.Right:
                CalledIndex = 2 + offset;
                break;
        }

        if (Akadora && akadora.taken)
        {
            OrderedTiles[CalledIndex] = new Tile(OrderedTiles[CalledIndex].value, true);
        }
        else if (Akadora && !akadora.taken)
        {
            if (CalledIndex == 0)
                OrderedTiles[1] = new Tile(OrderedTiles[1].value, true);
            else
                OrderedTiles[0] = new Tile(OrderedTiles[0].value, true);
        }
    }
}
