namespace OpenRiichi.Components;

using OpenRiichi.Util;

public sealed class Mountain
{
    static readonly Random rand = new Random();

    private readonly LinkedList<Tile> wall = new LinkedList<Tile>();
    readonly Tile[] deadWall = new Tile[18];

    readonly Dictionary<Value, int> doraList = new Dictionary<Value, int>();
    readonly Dictionary<Value, int> uraDoraList = new Dictionary<Value, int>();

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
        if (wall.Any()) wall.Clear();

        Array vals = Enum.GetValues(typeof(Value));

        int m5 = rand.Next(4);
        int p5 = rand.Next(4);
        int s5 = rand.Next(4);

        for (int i = 0; i < 4; i++)
        {
            vals.Shuffle(rand);

            foreach (int j in vals)
            {
                Value val = (Value) Enum.ToObject(typeof(Value), j);
                Tile tile = new Tile(val);

                if ((tile.value == Value.M5 && i == m5) ||
                    (tile.value == Value.P5 && i == p5) ||
                    (tile.value == Value.S5 && i == s5))
                {
                    tile.akadora = true;
                }

                wall.AddFirst(tile);
            }
        }
    }

    private void SplitDeadWall()
    {
        for (int i = 13; i >= 0; i--)
        {
            deadWall[i] = wall.Last();
            wall.RemoveLast();
        }

        for (int i = 14; i < 18; i++)
        {
            deadWall[i] = new Tile(Value.None);
        }

        if (doraList.Any()) doraList.Clear();
        if (uraDoraList.Any()) uraDoraList.Clear();
    }

    public void FlipDora()
    {
        int index = 5;
        int kanCount = 0;
        for (;;kanCount++)
        {
            if (deadWall[index].visible) index += 2;
            else break;
        }

        deadWall[index].visible = true;
        Value dora = deadWall[index].DoraValue();
        Value uraDora = deadWall[index - 1].DoraValue();

        doraList.Add(dora, doraList.GetValueOrDefault(dora) + 1);
        uraDoraList.Add(uraDora, uraDoraList.GetValueOrDefault(uraDora) + 1);

        if (kanCount > 0)
        {
            deadWall[13 + kanCount] = wall.First();
            wall.RemoveFirst();
        }
    }

    public Tile Draw()
    {
        if (wall.Any())
        {
            Tile tile = wall.Last();
            wall.RemoveLast();
            return tile;
        }
        else return new Tile(Value.None);
    }

    public Tile Rinshan()
    {
        Tile empty = new Tile(Value.None);

        for (int i = 0; i < 4; i++)
        {
            if (deadWall[i].value == Value.None) continue;

            else
            {
                Tile tile = deadWall[i];
                deadWall[i] = empty;
                return tile;
            }
        }

        return empty;
    }
}
