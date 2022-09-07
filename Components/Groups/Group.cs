namespace RiichiNET.Components.Groups;

using Enums;

internal abstract class Group
{
    internal abstract Mentsu Mentsu { get; }
    private protected Value _value;
    private protected bool _open;

    internal virtual Naki Naki { get; set; }
    internal virtual Seat Origin { get; set; }
    internal virtual Naki? Akadora { get; set; }

    internal Group(Tile tile, bool akadora=false, bool open=false)
    {
        _value = tile.value;
        _open = open;

        if (tile.akadora || akadora) Akadora = Naki.None;
    }

    internal Group(Value value, bool akadora=false, bool open=false)
    {
        _value = value;
        _open = open;

        if (akadora) Akadora = Naki.None;
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
}
