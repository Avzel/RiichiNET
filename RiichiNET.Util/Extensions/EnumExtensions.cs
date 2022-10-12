namespace RiichiNET.Util.Extensions;

using System;

public static class EnumExtensions
{
    public static T Next<T>(this T src) where T : struct
    {
        if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));
        T[] Arr = (T[])Enum.GetValues(src.GetType());
        int j = (Array.IndexOf<T>(Arr, src) + 1) % Arr.Length;
        return Arr[j];
    }

    public static T Previous<T>(this T src) where T : struct
    {
        if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));
        T[] Arr = (T[])Enum.GetValues(src.GetType());
        int j = (Array.IndexOf<T>(Arr, src) - 1) % Arr.Length;
        return Arr[j];
    }
}
