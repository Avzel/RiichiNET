namespace RiichiNET.Components.Groups;

using Enums;

internal abstract class Group
{
    internal abstract Mentsu Mentsu { get; }
    private protected Value _value;

    internal Naki Naki { get; }
    internal Seat Origin { get; }
    internal Naki? Akadora { get; set; }

    internal Group(Tile tile, Seat origin, bool akadora=false, Naki naki=Naki.None)
    {
        _value = tile.value;
        Naki = naki;

        if (tile.akadora || akadora) Akadora = Naki.None;

        Origin = origin;
    }

    internal Group(Value value, Seat origin, bool akadora=false, Naki naki=Naki.None)
    {
        _value = value;
        Naki = naki;

        if (akadora) Akadora = Naki.None;

        Origin = origin;
    }

    internal abstract List<Tile> GetTiles();

    internal virtual bool HasYaoChuu()
    {
        return ((Tile)this._value).IsYaoChuu();
    }

    internal virtual bool OnlyHonors()
    {
        return ((Tile)this._value).IsHonor();
    }

    internal bool HasAkadora()
    {
        return Akadora != null;
    }

    internal bool OnlyGreens()
    {
        return ((Tile)this._value).IsGreen();
    }

    internal bool IsOpen()
    {
        return Naki == Naki.None || Naki == Naki.AnKan;
    }
}
