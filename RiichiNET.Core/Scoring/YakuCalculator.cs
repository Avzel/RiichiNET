namespace RiichiNET.Core.Scoring;

internal sealed class YakuCalculator
{
    internal YakuCalculator()
    {
        // TODO:
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
