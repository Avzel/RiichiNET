namespace RiichiNET.Components;

using Naki = Enums.Naki;
using Value = Enums.Value;
using Seat = Enums.Seat;

public struct Meld
{
    public Naki naki;
    public Value value;
    public Seat origin;
    public (bool, Naki) akadora;

    public Meld(Naki naki, Value value, Seat origin, (bool, Naki) akadora)
    {
        this.naki = naki;
        this.value = value;
        this.origin = origin;
        this.akadora = akadora;
    }
}
