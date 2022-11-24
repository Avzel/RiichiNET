namespace RiichiNET.Core.Scoring;

using System.Collections.Generic;
using System.Linq;

using RiichiNET.Core.Collections;
using RiichiNET.Core.Collections.Melds;
using RiichiNET.Core.Components;
using RiichiNET.Core.Enums;
using RiichiNET.Util.Collections;

internal sealed class YakuCalculator
{
    private readonly ISet<Yaku> _best = new HashSet<Yaku>();
    private readonly ISet<Yaku> _current = new HashSet<Yaku>();
    private readonly ISet<Yaku> _yakuman = new HashSet<Yaku>();

    private readonly Table _table;
    private readonly Player _player;

    internal static bool IsTenpaiForKokushi(TileCount hand)
	{
		if (hand.Length() is not 12 or 13) return false;
        foreach (Tile tile in hand.Held())
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
		foreach (Meld meld in wh.GetAllMelds()) if (meld.HasYaoChuu()) return;
        _current.Add(Yaku.TanYaochuu);
    }

	private void Peikou(WinningHand wh)
	{
		if (!_player.IsOpen())
		{
			ObjectCounter<Meld> shuntsu = new ObjectCounter<Meld>();
			foreach (Meld meld in wh.GetMelds(Mentsu.Shuntsu))
			{
				shuntsu.Draw(meld);

				if (shuntsu[meld] == 2) _current.Add(Yaku.Iipeikou);
				if (shuntsu[meld] == 4)
				{
					_current.Remove(Yaku.Iipeikou);
					_current.Add(Yaku.RyanPeikou);
				}
			}
		}
    }

	private void Yakuhai(WinningHand wh)
	{
		foreach (Meld meld in wh.GetMelds(Mentsu.Koutsu, Mentsu.Kantsu))
		{
			if (meld.Contains(Value.DG)) 
				_current.Add(Yaku.YakuhaiHatsu);
			else if (meld.Contains(Value.DW)) 
				_current.Add(Yaku.YakuhaiHaku);
			else if (meld.Contains(Value.DR)) 
				_current.Add(Yaku.YakuhaiChun);
			else if (meld.Contains(_player.Wind.WindToValue())) 
				_current.Add(Yaku.YakuhaiJikaze);
			else if (meld.Contains(_table.Wind.WindToValue())) 
				_current.Add(Yaku.YakuhaiBakaze);
        }
    }

	private void SanshokuDoujun(WinningHand wh)
	{
        IList<Meld> shuntsu = wh.GetMelds(Mentsu.Shuntsu);
        if (wh.Count(Mentsu.Shuntsu) > 2) foreach (Meld meld in shuntsu)
		{
			Meld second = new AnJun(meld[0].NextSuit());
			Meld third = new AnJun(second[0].NextSuit());
			if (shuntsu.Contains(second) && shuntsu.Contains(third))
			{
				if (_player.IsOpen()) _current.Add(Yaku.SanshokuDoujunOpen);
				else _current.Add(Yaku.SanshokuDoujunClosed);
				break;
			}
		}
    }

	private void IkkiTsuukan(WinningHand wh)
	{
		IList<Meld> shuntsu = wh.GetMelds(Mentsu.Shuntsu);
		if (wh.Count(Mentsu.Shuntsu) > 2) foreach (Meld meld in shuntsu)
		{
			if (meld[0].IsTerminal())
			{
				Meld second = new AnJun(meld[0] + 3);
				Meld third = new AnJun(second[0] + 3);
				if (shuntsu.Contains(second) && shuntsu.Contains(third))
				{
					if (_player.IsOpen()) _current.Add(Yaku.IkkiTsuukanOpen);
					else _current.Add(Yaku.IkkiTsuukanClosed);
					break;
				}
			}
		}
	}

	private void HonChantaiYaochuu(WinningHand wh)
	{
        bool honor = false;
        bool terminal = false;
        foreach (Meld meld in wh.GetAllMelds())
		{
			if (!meld.HasYaoChuu()) return;
			if (!terminal && meld.HasTerminals()) terminal = true;
			else if (!honor && meld.OnlyHonors()) honor = true;
        }
		if (honor && terminal)
		{
			if (_player.IsOpen()) _current.Add(Yaku.HonChantaiYaochuuOpen);
			else _current.Add(Yaku.HonChantaiYaochuuClosed);
		}
    }

	private void ChiiToitsu(WinningHand wh)
	{
		if (wh.Count(Mentsu.Jantou) == 7) _current.Add(Yaku.ChiiToitsu);
    }

	private void ToitoiHou(WinningHand wh)
	{
		if (wh.Count(Mentsu.Shuntsu, Mentsu.Koutsu, Mentsu.Kantsu) == 4)
			_current.Add(Yaku.ToitoiHou);
    }

	private void Ankou(WinningHand wh)
	{
        int count = 0;
		foreach (Meld meld in wh.GetMelds(Mentsu.Koutsu, Mentsu.Kantsu))
		{
			if (
				!meld.Open && 
				!(_table.State == State.Discard && meld.Contains(_player.JustDrawn))

			) count++;
        }
		if (count == 3) _current.Add(Yaku.SanAnkou);
		else if (count == 4)
        {
			if (_player.WinningTiles.Count() == 1) _yakuman.Add(Yaku.SuuAnkouSingleWait);
            else _yakuman.Add(Yaku.SuuAnkou);
        }
    }

	private void SanshokuDoukou(WinningHand wh)
	{
		IEnumerable<Meld> koukan = wh.GetMelds(Mentsu.Koutsu, Mentsu.Kantsu);
        var sd = 	from item1 in koukan
					from item2 in koukan
					from item3 in koukan
					where Tile.SameValuesDifferentSuits(item1[0], item2[0], item3[0])
					select true;
		
		if (sd.Any()) _current.Add(Yaku.SanshokuDoukou);
    }

	private void Kantsu(WinningHand wh)
	{
        int count = wh.Count(Mentsu.Kantsu);
        if (count == 3) _current.Add(Yaku.SanKantsu);
		else if (count == 4) _yakuman.Add(Yaku.SuuKantsu);
    }

	private void HonRoutou(WinningHand wh)
	{
        bool honor = false;
        bool terminal = false;
		foreach (Meld meld in wh.GetAllMelds())
		{
			if (!meld.OnlyYaoChuu()) return;
			if (!terminal && meld.OnlyTerminals()) terminal = true;
			else if (!honor && meld.OnlyHonors()) honor = true;
        }
		if (honor && terminal) _current.Add(Yaku.HonRoutou);
    }

	private void Sangen(WinningHand wh)
	{
        if (wh.Count(Mentsu.Koutsu, Mentsu.Kantsu) > 1)
        {
            HashSet<Value> values = new HashSet<Value>();
            foreach (Meld meld in wh.GetMelds(Mentsu.Jantou, Mentsu.Koutsu, Mentsu.Kantsu))
            {
                if (meld.OnlyDragons()) values.Add(meld[0]);
            }
            if (values.Count() == 3)
			{
				if (wh.GetMelds(Mentsu.Jantou)[0].OnlyDragons())
                    _current.Add(Yaku.ShouSangen);
				else _yakuman.Add(Yaku.DaiSangen);
            }
        }
    }

	private void JunChantaiYaochuu(WinningHand wh)
	{
        bool normal = false;
		foreach (Meld meld in wh.GetAllMelds())
		{
			if (!meld.HasTerminals()) return;
			if (!normal && !meld.OnlyTerminals()) normal = true;
        }
        if (normal)
        {
            if (_player.IsOpen()) _current.Add(Yaku.JunChantaiYaochuuOpen);
			else _current.Add(Yaku.JunChantaiYaochuuClosed);
        }
    }

	private void Iisou(WinningHand wh)
	{
        bool honor = false;
        HashSet<Suit> suits = new HashSet<Suit>();
		foreach (Meld meld in wh.GetAllMelds())
		{
			if (meld.OnlyHonors()) honor = true;
			else suits.Add(meld[0].GetSuit());
        }
		if (honor && suits.Count() == 1)
		{
			if (_player.IsOpen()) _current.Add(Yaku.HonIisouOpen);
			else _current.Add(Yaku.HonIisouClosed);
        }
		else if (!honor && suits.Count() == 1)
		{
			if (_player.IsOpen()) _current.Add(Yaku.ChinIisouOpen);
			else _current.Add(Yaku.ChinIisouClosed);
		}
    }

	private void KokushiMusou(WinningHand wh)
	{
		if (wh.GetAllMelds().Count() == 1) _yakuman.Add(Yaku.KokushiMusou);
    }

	private void Suushii(WinningHand wh)
	{
		if (wh.Count(Mentsu.Koutsu, Mentsu.Kantsu) > 1)
        {
            HashSet<Value> values = new HashSet<Value>();
            foreach (Meld meld in wh.GetMelds(Mentsu.Jantou, Mentsu.Koutsu, Mentsu.Kantsu))
            {
                if (meld.OnlyWinds()) values.Add(meld[0]);
            }
            if (values.Count() == 4)
			{
				if (wh.GetMelds(Mentsu.Jantou)[0].OnlyWinds())
                    _yakuman.Add(Yaku.ShouSuushii);
				else _yakuman.Add(Yaku.DaiSuuShii);
            }
        }
	}

	private void TsuuIisou(WinningHand wh)
	{
		foreach (Meld meld in wh.GetAllMelds())
		{
			if (!meld.OnlyHonors()) return;
        }
        _yakuman.Add(Yaku.TsuuIisou);
    }

	private void ChinRoutou(WinningHand wh)
	{
		foreach (Meld meld in wh.GetAllMelds())
		{
			if (!meld.OnlyTerminals()) return;
        }
        _yakuman.Add(Yaku.ChinRoutou);
	}

	private void RyuuIisou(WinningHand wh)
	{
		foreach (Meld meld in wh.GetAllMelds())
		{
			if (!meld.OnlyGreens()) return;
        }
        _yakuman.Add(Yaku.RyuuIisou);
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

	private void ChuurenPoutou(WinningHand wh)
	{
        if (!_player.IsOpen())
        {
            HashSet<Suit> suits = new HashSet<Suit>();
            foreach (Meld meld in wh.GetAllMelds())
            {
                if (meld.OnlyHonors()) return;
                else suits.Add(meld[0].GetSuit());
            }

            if (
                suits.Count() == 1 &&
                _player.Hand.ValueCount((int)suits.First() + 1) is 3 or 4 &&
                _player.Hand.ValueCount((int)suits.First() + 9) is 3 or 4 &&
                _player.Hand.ValueCount((int)suits.First() + 2) is 1 or 2 &&
                _player.Hand.ValueCount((int)suits.First() + 3) is 1 or 2 &&
                _player.Hand.ValueCount((int)suits.First() + 4) is 1 or 2 &&
                _player.Hand.ValueCount((int)suits.First() + 5) is 1 or 2 &&
                _player.Hand.ValueCount((int)suits.First() + 6) is 1 or 2 &&
                _player.Hand.ValueCount((int)suits.First() + 7) is 1 or 2 &&
                _player.Hand.ValueCount((int)suits.First() + 8) is 1 or 2
            ){
				if (_player.WinningTiles.Count() == 9) _yakuman.Add(Yaku.JunseiChuurenPoutou);
				else _yakuman.Add(Yaku.ChuurenPoutou);
            }
        }
    }
}
