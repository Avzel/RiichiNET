namespace RiichiNET.Core.Components.Collections.Melds;

using System.Collections.Generic;

using RiichiNET.Core.Enums;

internal abstract class OpenMeld: Meld
{
    internal override bool Open { get => true; }
    internal abstract Naki Naki { get; }

    internal List<Tile> OrderedTiles { get; private protected set; } = new List<Tile>();
    internal int CalledIndex { get; private protected set; }

    internal override bool HasYaoChuu()
    {
        return OrderedTiles[0].IsYaoChuu();
    }

    internal override bool OnlyHonors()
    {
        return OrderedTiles[0].IsHonor();
    }

    internal override bool OnlyGreens()
    {
        return OrderedTiles[0].IsGreen();
    }

    internal override bool Contains(Value value)
    {
        foreach (Tile tile in OrderedTiles)
        {
            if (tile.value == value) return true;
        }
        return false;
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

    private protected void SetKouKanTiles(bool kantsu, Value value, Direction called, (bool exists, bool taken) akadora=default)
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
