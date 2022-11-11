namespace RiichiNET.Core.Scoring;

using System.Collections.Generic;
using System.Linq;

using RiichiNET.Core.Collections;
using RiichiNET.Core.Components;
using RiichiNET.Core.Enums;

internal sealed class YakuCalculator
{
    private readonly ISet<Yaku> _best = new HashSet<Yaku>();
    private readonly Table _table;
    private readonly Player _player;

    internal static bool IsTenpaiForKokushi(TileCount hand)
	{
		if (hand.Length() is not 12 or 13) return false;
        foreach (Tile tile in hand.Tiles())
        {
            if (!tile.IsYaoChuu()) return false;
        }
        return true;
	}

	internal static bool NagashiMangan(IList<Tile> graveyard)
	{
		// TODO:
        return false;
    }

	internal YakuCalculator(Table table, Player player)
	{
        _table = table;
        _player = player;
    }

	internal bool YakuExists()
	{
		if (!_player.IsComplete()) return false;

		// TODO:

        return false;
    }

	internal ISet<Yaku> DetermineYaku()
	{
        ISet<Yaku> yaku = new HashSet<Yaku>();

		foreach (WinningHand wh in _player.WinningHands)
		{
			// TODO:
		}
        return yaku;
    }

	// Yaku:

	private void Riichi()
	{
		// TODO:
	}

	private void RyanRiichi()
	{
		// TODO: Player.IsRiichi() && Calls.First().type == Naki.Riichi && Calls.First().caller == Player && Calls.First().elapsed < 4
	}

	private void MenzenchinTsumohou()
	{
		// TODO: !Player.IsOpen() && Table.State == State.Draw
	}

	private void Chankan()
	{
		// TODO: Table.State == State.Call && _turn != Player.Seat
	}

	private void Ippatsu()
	{
		// TODO: Player.IsRiichi() && Calls.Last().type == Naki.Riichi && Calls.Last().caller == Player && Table.Elapsed - Calls.Last().elapsed < 5
	}

	private void RinshanKaihou()
	{
		// TODO: Table.State == State.Call && _turn == Player.Seat
	}

	private void HaiteiRaoyue()
	{
		// TODO: Table.State == State.Draw && Table.RoundIsOver()
	}

	private void HouteiYaoyui()
	{
		// TODO: Table.State == State.Discard && Table.RoundIsOver()
	}

	private void Pinfu()
	{
		// TODO:
	}

	private void TanYaochuu()
	{
		// TODO:
	}

	private void Iipeikou()
	{
		// TODO:
	}

	private void YakuhaiHatsu()
	{
		// TODO:
	}

	private void YakuhaiHaku()
	{
		// TODO:
	}

	private void YakuhaiChun()
	{
		// TODO:
	}

	private void YakuhaiJikaze()
	{
		// TODO:
	}

	private void YakuhaiBakaze()
	{
		// TODO:
	}

	private void SanshokuDoujun()
	{
		// TODO:
	}

	private void IkkiTsuukan()
	{
		// TODO:
	}

	private void HonChantaiYaochuu()
	{
		// TODO:
	}

	private void ChiiToitsu()
	{
		// TODO:
	}

	private void ToitoiHou()
	{
		// TODO:
	}

	private void SanAnkou()
	{
		// TODO:
	}

	private void SanshokuDoukou()
	{
		// TODO:
	}

	private void SanKantsu()
	{
		// TODO:
	}

	private void HonRoutou()
	{
		// TODO:
	}

	private void ShouSangen()
	{
		// TODO:
	}

	private void JunChantaiYaochuu()
	{
		// TODO:
	}

	private void HonIisou()
	{
		// TODO:
	}

	private void RyanPeikou()
	{
		// TODO:
	}

	private void ChinIisou()
	{
		// TODO:
	}

	private void KokushiMusou()
	{
		// TODO:
	}

	private void SuuKantsu()
	{
		// TODO:
	}

	private void DaiSangen()
	{
		// TODO:
	}

	private void ShouSuushii()
	{
		// TODO:
	}

	private void TsuuIisou()
	{
		// TODO:
	}

	private void ChinRoutou()
	{
		// TODO:
	}

	private void RyuuIisou()
	{
		// TODO:
	}

	private void TenHou()
	{
		// TODO: Table.State == State.Draw && Table.UninterruptedFirstRound() Player == Table.GetDealer()
	}

	private void RenHou()
	{
		// TODO: Table.State == State.Discard && Table.UninterruptedFirstRound() && Player.Graveyard.IsEmpty()
	}

	private void ChiiHou()
	{
		// TODO: Table.State == State.Draw && Table.UninterruptedFirstRound() && Player != Table.GetDealer()
	}

	private void ChuurenPoutou()
	{
		// TODO:
	}

	private void SuuAnkou()
	{
		// TODO:
	}

	private void DaiSuuShii()
	{
		// TODO:
	}
}
