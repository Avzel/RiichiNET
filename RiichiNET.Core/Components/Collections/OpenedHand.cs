namespace RiichiNET.Core.Components.Collections;

using System.Collections.Generic;

using RiichiNET.Core.Components.Collections.Melds;

internal sealed class OpenedHand
{
    private List<Meld> _hand;

    internal OpenedHand(List<Meld>? original = default)
    {
        if (original != null) _hand = new List<Meld>(original);
        else _hand = new List<Meld>();
    }

    
}
