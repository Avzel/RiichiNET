namespace RiichiNET.Core.Scoring;

using System.Collections.Generic;
using System.Linq;

using RiichiNET.Core.Collections;
using RiichiNET.Core.Collections.Melds;
using RiichiNET.Core.Components;
using RiichiNET.Core.Enums;

internal sealed class YakuCalculator
{
    private readonly ISet<Yaku> _best = new HashSet<Yaku>();
    private readonly ISet<Yaku> _current = new HashSet<Yaku>();
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
		foreach (Tile tile in graveyard)
		{
			if (!tile.IsYaoChuu()) return false;
        }
        return true;
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

	private void OverwriteBest()
	{
		if (_current.Any() && _current.Sum(x => (int)x) > _best.Sum(x => (int)x))
		{
			_best.Clear();
			foreach (Yaku yaku in _current) _best.Add(yaku);
		}
    }

	internal ISet<Yaku> DetermineYaku()
	{
        ISet<Yaku> yaku = new HashSet<Yaku>();

		foreach (WinningHand wh in _player.WinningHands)
		{
            // TODO:

            OverwriteBest();
            _current.Clear();
        }
        return yaku;
    }

	// Yaku:

	private void Riichi()
	{
        if (_player.IsRiichi()) _current.Add(Yaku.Riichi);
    }

	private void RyanRiichi()
	{
		if (
			_player.IsRiichi() && 
			_table.Calls.First().type == Naki.Riichi && 
			_table.Calls.First().caller == _player && 
			_table.Calls.First().elapsed < 4

		) _current.Add(Yaku.RyanRiichi);
    }

	private void MenzenchinTsumohou()
	{
        if (!_player.IsOpen() && _table.State == State.Draw)
        {
            _current.Add(Yaku.MenzenchinTsumohou);
        }
    }

	private void Chankan()
	{
		if (_table.State == State.Call && _table.GetPlayer() != _player)
		{
            _current.Add(Yaku.Chankan);
        }
	}

	private void Ippatsu()
	{
		if (
			_player.IsRiichi() &&
			_table.Calls.Last().type == Naki.Riichi && 
			_table.Calls.Last().caller == _player && 
			_table.Elapsed - _table.Calls.Last().elapsed < 5

		) _current.Add(Yaku.Ippatsu);
    }

	private void RinshanKaihou()
	{
		if (_table.State == State.Call && _table.GetPlayer() == _player)
		{
            _current.Add(Yaku.RinshanKaihou);
        }
	}

	private void HaiteiRaoyue()
	{
		if (_table.State == State.Draw && _table.RoundIsOver())
		{
            _current.Add(Yaku.HaiteiRaoyue);
        }
	}

	private void HouteiYaoyui()
	{
		if (_table.State == State.Discard && _table.RoundIsOver())
		{
            _current.Add(Yaku.HouteiYaoyui);
        }
	}

	private void Pinfu(WinningHand wh)
	{
		if (
			!_player.IsOpen() && 
			wh.GetMelds(Mentsu.Shuntsu).Count() == 4 && 
			_player.WinningTiles.Count() > 1 && 
			!wh.GetMelds(Mentsu.Jantou).First().OnlyDragons() &&
			!wh.Contains(Mentsu.Jantou, _table.Wind.WindToValue()) &&
			!wh.Contains(Mentsu.Jantou, _player.Wind.WindToValue())


		) _current.Add(Yaku.Pinfu);
    }

	private void TanYaochuu(WinningHand wh)
	{
		foreach (Meld meld in wh.GetAllMelds())
		{
			if (meld.HasYaoChuu()) return;
        }
        _current.Add(Yaku.TanYaochuu);
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
		if (
			_table.State == State.Draw && 
			_table.UninterruptedFirstRound() && 
			_table.GetDealer() == _player

		) _current.Add(Yaku.TenHou);
    }

	private void RenHou()
	{
		if (
			_table.State == State.Discard &&
			_table.UninterruptedFirstRound() &&
			!_player.Graveyard.Any()

		) _current.Add(Yaku.RenHou);
    }

	private void ChiiHou()
	{
		if (
			_table.State == State.Draw &&
			_table.UninterruptedFirstRound() &&
			_table.GetDealer() != _player

		) _current.Add(Yaku.ChiiHou);
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
