namespace OpenRiichi.Components;

public sealed class Tile
{
    public Value value { get; }
    public Wind wind { get; set; }
    public bool akadora { get; internal set; } = false;
    public bool visible { get; set; } = false;
    public bool called { get; set; } = false;

    public bool pon { get; set; } = false;
    public bool chii { get; set; } = false;
    public bool minKan { get; set; } = false;
    public bool anKan { get; set; } = false;
    public bool kuwaeta { get; set; } = false;

    public Tile(Value val)
    {
        this.value = val;
    }

    public static bool IsYaoChuu(Tile tile)
    {
        int val = (int) tile.value;

        return val > 60 || val % 10 == 1 || val % 10 == 9;
    }
}
