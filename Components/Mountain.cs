namespace OpenRiichi.Components;

using OpenRiichi.Util;

public sealed class Mountain
{
    static readonly Random rand = new Random();

    readonly LinkedList<Tile> wall = new LinkedList<Tile>();
    readonly Tile[] deadWall = new Tile[18];

    readonly Dictionary<Value, int> doraList = new Dictionary<Value, int>();
    readonly Dictionary<Value, int> uraDoraList = new Dictionary<Value, int>();

    public Mountain(bool sanma, bool akadora)
    {
        SetMountain(sanma, akadora);
    }

    public void SetMountain(bool sanma, bool akadora)
    {
        CreateTiles(sanma, akadora);
        SplitDeadWall();
        FlipDora(sanma, 0);
    }

    private void CreateTiles(bool sanma, bool akadora)
    {
        if (wall.Any()) wall.Clear();

        Array vals = Enum.GetValues(typeof(Value));

        for (int i = 0; i < 4; i++)
        {
            vals.Shuffle(rand);

            foreach (int j in vals)
            {
                if (!sanma || !(j > 1 && j < 9))
                {

                    Value val = (Value) Enum.ToObject(typeof(Value), j);
                    wall.AddFirst(new Tile(val));
                }
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
    }

    private void FlipDora(bool sanma, int kanCount)
    {
        int index = sanma == false ? 5 : 9;
        index += kanCount * 2;

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
}
