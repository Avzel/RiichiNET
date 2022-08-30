namespace OpenRiichi.Components;

public struct Tile
{
    public readonly Value value { get; }
    public Wind? wind { get; set; } = null;
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

    public static bool IsTerminal(Tile tile)
    {
        int val = (int) tile.value;

        return val == 64 || val == 83 || val % 10 == 1 || val % 10 == 9;
    }

    public static Value DoraValue(Tile tile)
    {
        int val = (int) tile.value;

        if (val == 64) val = 61;
        else if (val == 83) val = 81;
        else if (val % 10 == 9) val -= 8;
        else val ++;

        return (Value) Enum.ToObject(typeof(Value), val);
    }
}
