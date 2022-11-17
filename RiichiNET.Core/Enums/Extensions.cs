namespace RiichiNET.Core.Enums;

using System;

internal static class Extensions
{
    internal static Value WindToValue(this Wind wind)
    {
        switch (wind)
        {
            case Wind.East:
                return Value.WE;
            case Wind.South:
                return Value.WS;
            case Wind.West:
                return Value.WW;
            case Wind.North:
                return Value.WN;
            default:
                throw new NotSupportedException();
        }
    }
}