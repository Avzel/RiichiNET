namespace RiichiNET.Util.Extensions;

using System;

public static class ArrayExtensions
{
    public static void Shuffle(this Array arr, Random rand)
    {
        for (int i = arr.GetLength(0) - 1; i > 0; i--)
        {
            int swap = rand.Next(i + 1);
            Object? temp = arr.GetValue(i);
            arr.SetValue(arr.GetValue(swap), i);
            arr.SetValue(temp, swap);
        }
    }
}
