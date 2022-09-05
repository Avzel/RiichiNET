namespace RiichiNET.Components;

using Enums;

internal struct Meld
{
    internal Naki naki;
    internal Value value;
    internal Seat origin;
    internal Naki akadora;

    internal Meld(Naki naki, Value value, Seat origin, Naki akadora)
    {
        this.naki = naki;
        this.value = value;
        this.origin = origin;
        this.akadora = akadora;
    }
}
