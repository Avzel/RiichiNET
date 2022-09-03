namespace RiichiNET.Components;

using RiichiNET.Util;
using Enums;

public sealed class Mountain
{
    static readonly Random Rand = new Random();

    private readonly LinkedList<Tile> _wall = new LinkedList<Tile>();
    private readonly Tile[] _deadWall = new Tile[18];

    public Dictionary<Value, int> DoraList { get; } = new Dictionary<Value, int>();
    public Dictionary<Value, int> UraDoraList { get; } = new Dictionary<Value, int>();

    public Mountain()
    {
        SetMountain();
    }

    public void SetMountain()
    {
        CreateTiles();
        SplitDeadWall();
        FlipDora();
    }

    private void CreateTiles()
    {
        if (_wall.Any()) _wall.Clear();

        Array vals = Enum.GetValues(typeof(Value));

        int m5 = Rand.Next(4);
        int p5 = Rand.Next(4);
        int s5 = Rand.Next(4);

        for (int i = 0; i < 4; i++)
        {
            vals.Shuffle(Rand);

            foreach (int j in vals)
            {
                Value val = (Value) Enum.ToObject(typeof(Value), j);
                Tile tile = new Tile(val);

                if ((tile.Value == Value.M5 && i == m5) ||
                    (tile.Value == Value.P5 && i == p5) ||
                    (tile.Value == Value.S5 && i == s5))
                {
                    tile.Akadora = true;
                }

                _wall.AddFirst(tile);
            }
        }
    }

    private void SplitDeadWall()
    {
        for (int i = 13; i >= 0; i--)
        {
            _deadWall[i] = _wall.Last();
            _wall.RemoveLast();
        }

        for (int i = 14; i < 18; i++)
        {
            _deadWall[i] = new Tile(Value.None);
        }

        if (DoraList.Any()) DoraList.Clear();
        if (UraDoraList.Any()) UraDoraList.Clear();
    }

    public void FlipDora()
    {
        int kanCount = DoraList.Values.Sum();
        int index = 5 + (kanCount * 2);

        Value dora = _deadWall[index].DoraValue();
        Value uraDora = _deadWall[index - 1].DoraValue();

        DoraList.Add(dora, DoraList.GetValueOrDefault(dora) + 1);
        UraDoraList.Add(uraDora, UraDoraList.GetValueOrDefault(uraDora) + 1);

        if (kanCount > 0)
        {
            _deadWall[13 + kanCount] = _wall.First();
            _wall.RemoveFirst();
        }
    }

    public Tile Draw()
    {
        if (_wall.Any())
        {
            Tile tile = _wall.Last();
            _wall.RemoveLast();
            return tile;
        }
        else return new Tile(Value.None);
    }

    public Tile Rinshan()
    {
        int index = DoraList.Values.Sum();
        Tile tile = _deadWall[index];

        if (tile.Value != Value.None)
        {
            _deadWall[index] = new Tile(Value.None);
        }

        return tile;
    }
}
