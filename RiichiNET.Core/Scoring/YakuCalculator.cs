namespace RiichiNET.Core.Scoring;

using RiichiNET.Core.Collections;
using RiichiNET.Core.Components;

internal sealed class YakuCalculator
{
    internal YakuCalculator()
    {
        // TODO:
    }

	internal static bool KokushuMusou(TileCount hand)
	{
		if (hand.Length() is not 12 or 13) return false;
        foreach (Tile tile in hand.Tiles())
        {
            if (!tile.IsYaoChuu()) return false;
        }
        return true;
	}
}

/*
Special Yaku:

Ippatsu
    Player.IsRiichi() && Calls.Last().type == Naki.Riichi && Calls.Last().caller == Player && Table.Elapsed - Calls.Last().elapsed < 5

RyanRiichi
    Player.IsRiichi() && Calls.First().type == Naki.Riichi && Calls.First().caller == Player && Calls.First().elapsed < 4

MenzenchinTsumohou
	!Player.IsOpen() && Table.State == State.Draw

Chankan
	Table.State == State.Call && _turn != Player.Seat

RinshanKaihou
	Table.State == State.Call && _turn == Player.Seat

HaiteiRaoyue
	Table.State == State.Draw && Table.RoundIsOver()
	
HouteiYaoyui
	Table.State == State.Discard && Table.RoundIsOver()

TenHou
	Table.State == State.Draw && Table.UninterruptedFirstRound() Player == Table.GetDealer()

RenHou
	Table.State == State.Discard && Table.UninterruptedFirstRound() && Player.Graveyard.IsEmpty()

ChiiHou
	Table.State == State.Draw && Table.UninterruptedFirstRound() && Player != Table.GetDealer()

*/
