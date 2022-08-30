namespace OpenRiichi.Components;

public sealed class Mountain
{
    static readonly Random rand = new Random();

    readonly List<Tile> wall = new List<Tile>();
    readonly List<Tile> deadWall = new List<Tile>();

    readonly IDictionary<Value, int> doraList = new Dictionary<Value, int>();
    readonly IDictionary<Value, int> uraDoraList = new Dictionary<Value, int>();

    public Mountain(bool sanma, bool akadora)
    {
        PrepareWall(sanma, akadora);
    }

    public void PrepareWall(bool sanma, bool akadora)
    {
        CreateTiles(sanma, akadora);
        ShuffleWall();
        DetermineWinds();
    }

    private void CreateTiles(bool sanma, bool akadora)
    {
        if (wall.Any()) wall.Clear();

        for (int i = 0; i < 4; i++) {

            foreach (int j in Enum.GetValues(typeof(Value))) {

                if (!sanma || !(j > 1 && j < 9)) {

                    Value val = (Value) Enum.ToObject(typeof(Value), j);
                    wall.Add(new Tile(val));
                }
            }
        }
    }

    private void ShuffleWall()
    {
        for (int i = wall.Count - 1; i > 0; i--) {

            int swap = Mountain.rand.Next(i + 1);
            Tile temp = wall[i];
            wall[i] = wall[swap];
            wall[swap] = temp;
        }
    }

    private void DetermineWinds()
    {
        int group = wall.Count / 4;
        int quadrant = rand.Next(1, 5);
        int offset = (new[] {-1, 1})[rand.Next(2)];
        int split = quadrant * rand.Next(group);
        if (split % 2 != 0) split += offset;

        for (int i = 0; i < 4; i++) {

            for (int j = group; j > 0; j--) {

                if (split == 0) split = wall.Count - 1;

                wall[split].wind = (Wind) Enum.ToObject(typeof(Wind), i);
                split--;
            }
        }
    }

    
}
