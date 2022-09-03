namespace RiichiNET.Components;

using Naki = Enums.Naki;
using Value = Enums.Value;
using Seat = Enums.Seat;

public readonly struct Meld
{
    public Naki Naki { get; }
    public Value Value { get; }
    public Seat Origin { get; } 
    public (bool, Naki) Akadora { get; }

    public Meld(Naki naki, Value value, Seat origin, (bool, Naki) akadora)
    {
        this.Naki = naki;
        this.Value = value;
        this.Origin = origin;
        this.Akadora = akadora;
    }
}
