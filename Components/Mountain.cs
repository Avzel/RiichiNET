namespace OpenRiichi.Components;

using OpenRiichi.Util;

public sealed class Mountain
{
    static readonly Random rand = new Random();

    readonly Queue<Tile> wall = new Queue<Tile>();
    readonly Tile[] deadWall = new Tile[14];

    readonly IDictionary<Value, int> doraList = new Dictionary<Value, int>();
    readonly IDictionary<Value, int> uraDoraList = new Dictionary<Value, int>();

    public Mountain(bool sanma, bool akadora)
    {
        SetMountain(sanma, akadora);
    }

    public void SetMountain(bool sanma, bool akadora)
    {
        CreateTiles(sanma, akadora);
        SplitDeadWall();
        FlipDora(sanma);
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
                    wall.Enqueue(new Tile(val));
                }
            }
        }
    }

    private void SplitDeadWall()
    {
        for (int i = deadWall.Count() - 1; i >= 0; i--)
        {
            deadWall[i] = wall.Dequeue();
        }
    }

    private void FlipDora(bool sanma)
    {
        // TODO
    }
}
