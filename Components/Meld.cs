namespace RiichiNET.Components;

using Enums;

internal struct Meld
{
    internal Mentsu mentsu;
    internal Naki naki = Naki.None;
    internal Seat origin;
    internal Value value = Value.None;

    internal Naki? akadora = null;

    internal Meld(Mentsu mentsu, Seat origin)
    {
        this.mentsu = mentsu;
        this.origin = origin;
    }

    internal Meld(Mentsu mentsu, Seat origin, Value value, Naki naki)
    {
        this.mentsu = mentsu;
        this.origin = origin;

        this.value = value;
        this.naki = naki;
    }

    internal Meld(Mentsu mentsu, Seat origin, Value value, Naki naki, Naki akadora)
    {
        this.mentsu = mentsu;
        this.origin = origin;

        this.value = value;
        this.naki = naki;
        this.akadora = akadora;
    }
}
