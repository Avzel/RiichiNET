namespace RiichiNET.Core.Scoring;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
    private WinningHand final = null!;
    private static Mutex mut = new Mutex();

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

		foreach (WinningHand wh in _player.WinningHands)
		{
            YakuCalcInParallel(wh);
			if (_current.Any() || _yakuman.Any()) return true;
        }
        return false;
    }

	private void OverwriteBest(WinningHand wh)
	{
		if (_current.Any() && _current.Sum(x => (int)x) > _best.Sum(x => (int)x))
		{
			_best.Clear();
			foreach (Yaku yaku in _current) _best.Add(yaku);
			_current.Clear();
            final = wh;
        }
    }

	internal void DetermineYaku()
	{
		if (!_player.IsComplete()) return;
        foreach (WinningHand wh in _player.WinningHands)
		{
            YakuCalcInParallel(wh);
            OverwriteBest(wh);
			if (_yakuman.Contains(Yaku.ChuurenPoutou)) break;
        }
        if (final != null)
        {
            _player.WinningHands.Clear();
            _player.WinningHands.Add(final);
        }
        _player.YakuList =  _yakuman.Any() ? _yakuman : _best;
    }

	private void YakuCalcInParallel(WinningHand wh)
	{
        Parallel.Invoke
        (
            () => { Riichi(); }, 
			() => { RyanRiichi(); }, 
			() => { MenzenchinTsumohou(); }, 
			() => { Chankan(); }, 
			() => { Ippatsu(); }, 
			() => { RinshanKaihou(); }, 
			() => { HaiteiRaoyue(); }, 
			() => { HouteiYaoyui(); }, 

			() => { Pinfu(wh); }, 
			() => { TanYaochuu(wh); }, 
			() => { Peikou(wh); }, 
			() => { Yakuhai(wh); }, 
			() => { SanshokuDoujun(wh); }, 
			() => { IkkiTsuukan(wh); }, 
			() => { HonChantaiYaochuu(wh); }, 
			() => { ChiiToitsu(wh); }, 
			() => { ToitoiHou(wh); }, 
			() => { Ankou(wh); }, 
			() => { SanshokuDoukou(wh); }, 
			() => { Kantsu(wh); }, 
			() => { HonRoutou(wh); }, 
			() => { Sangen(wh); }, 
			() => { JunChantaiYaochuu(wh); }, 
			() => { Iisou(wh); }, 
			() => { KokushiMusou(wh); }, 
			() => { Suushii(wh); }, 
			() => { TsuuIisou(wh); }, 
			() => { ChinRoutou(wh); }, 
			() => { RyuuIisou(wh); }, 
			() => { ChuurenPoutou(wh); }, 

			() => { TenHou(); }, 
			() => { RenHou(); }, 
			() => { ChiiHou(); }
        );
    }

	private void EditYaku(Yaku yaku, Func<Yaku, bool> Edit)
	{
        mut.WaitOne();
		Edit(yaku);
        mut.ReleaseMutex();
    }

	private void Riichi()
	{
        if (_player.IsRiichi()) EditYaku(Yaku.Riichi, _current.Add);
    }

	private void RyanRiichi()
	{
		if (
			_player.IsRiichi() && 
			_table.Calls.First().type == Naki.Riichi && 
			_table.Calls.First().caller == _player && 
			_table.Calls.First().elapsed < 4

		) EditYaku(Yaku.RyanRiichi, _current.Add);
    }

	private void MenzenchinTsumohou()
	{
        if (!_player.IsOpen() && _table.State == State.Draw)
        {
            EditYaku(Yaku.MenzenchinTsumohou, _current.Add);
        }
    }

	private void Chankan()
	{
		if (_table.State == State.Call && _table.GetPlayer() != _player)
		{
            EditYaku(Yaku.Chankan, _current.Add);
        }
	}

	private void Ippatsu()
	{
		if (
			_player.IsRiichi() &&
			_table.Calls.Last().type == Naki.Riichi && 
			_table.Calls.Last().caller == _player && 
			_table.Elapsed - _table.Calls.Last().elapsed < 5

		) EditYaku(Yaku.Ippatsu, _current.Add);
    }

	private void RinshanKaihou()
	{
		if (_table.State == State.Call && _table.GetPlayer() == _player)
		{
            EditYaku(Yaku.RinshanKaihou, _current.Add);
        }
	}

	private void HaiteiRaoyue()
	{
		if (_table.State == State.Draw && _table.RoundIsOver())
		{
            EditYaku(Yaku.HaiteiRaoyue, _current.Add);
        }
	}

	private void HouteiYaoyui()
	{
		if (_table.State == State.Discard && _table.RoundIsOver())
		{
            EditYaku(Yaku.HouteiYaoyui, _current.Add);
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


		) EditYaku(Yaku.Pinfu, _current.Add);
    }

	private void TanYaochuu(WinningHand wh)
	{
		foreach (Meld meld in wh.GetAllMelds()) if (meld.HasYaoChuu()) return;
        EditYaku(Yaku.TanYaochuu, _current.Add);
    }

	private void Peikou(WinningHand wh)
	{
		if (!_player.IsOpen())
		{
			ObjectCounter<Meld> shuntsu = new ObjectCounter<Meld>();
			foreach (Meld meld in wh.GetMelds(Mentsu.Shuntsu))
			{
				shuntsu.Draw(meld);

				if (shuntsu[meld] == 2) EditYaku(Yaku.Iipeikou, _current.Add);
				if (shuntsu[meld] == 4)
				{
					EditYaku(Yaku.Iipeikou, _current.Remove);
					EditYaku(Yaku.RyanPeikou, _current.Add);
				}
			}
		}
    }

	private void Yakuhai(WinningHand wh)
	{
		foreach (Meld meld in wh.GetMelds(Mentsu.Koutsu, Mentsu.Kantsu))
		{
			if (meld.Contains(Value.DG)) 
				EditYaku(Yaku.YakuhaiHatsu, _current.Add);
			else if (meld.Contains(Value.DW)) 
				EditYaku(Yaku.YakuhaiHaku, _current.Add);
			else if (meld.Contains(Value.DR)) 
				EditYaku(Yaku.YakuhaiChun, _current.Add);
			else if (meld.Contains(_player.Wind.WindToValue())) 
				EditYaku(Yaku.YakuhaiJikaze, _current.Add);
			else if (meld.Contains(_table.Wind.WindToValue())) 
				EditYaku(Yaku.YakuhaiBakaze, _current.Add);
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
				if (_player.IsOpen()) EditYaku(Yaku.SanshokuDoujunOpen, _current.Add);
				else EditYaku(Yaku.SanshokuDoujunClosed, _current.Add);
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
					if (_player.IsOpen()) EditYaku(Yaku.IkkiTsuukanOpen, _current.Add);
					else EditYaku(Yaku.IkkiTsuukanClosed, _current.Add);
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
			if (_player.IsOpen()) EditYaku(Yaku.HonChantaiYaochuuOpen, _current.Add);
			else EditYaku(Yaku.HonChantaiYaochuuClosed, _current.Add);
		}
    }

	private void ChiiToitsu(WinningHand wh)
	{
		if (wh.Count(Mentsu.Jantou) == 7) EditYaku(Yaku.ChiiToitsu, _current.Add);
    }

	private void ToitoiHou(WinningHand wh)
	{
		if (wh.Count(Mentsu.Shuntsu, Mentsu.Koutsu, Mentsu.Kantsu) == 4)
			EditYaku(Yaku.ToitoiHou, _current.Add);
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
		if (count == 3) EditYaku(Yaku.SanAnkou, _current.Add);
		else if (count == 4)
        {
			if (_player.WinningTiles.Count() == 1) 
				EditYaku(Yaku.SuuAnkouSingleWait, _yakuman.Add);
            else EditYaku(Yaku.SuuAnkou, _yakuman.Add);
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
		
		if (sd.Any()) EditYaku(Yaku.SanshokuDoukou, _current.Add);
    }

	private void Kantsu(WinningHand wh)
	{
        int count = wh.Count(Mentsu.Kantsu);
        if (count == 3) EditYaku(Yaku.SanKantsu, _current.Add);
		else if (count == 4) EditYaku(Yaku.SuuKantsu, _yakuman.Add);
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
		if (honor && terminal) EditYaku(Yaku.HonRoutou, _current.Add);
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
                    EditYaku(Yaku.ShouSangen, _current.Add);
				else EditYaku(Yaku.DaiSangen, _yakuman.Add);
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
            if (_player.IsOpen()) EditYaku(Yaku.JunChantaiYaochuuOpen, _current.Add);
			else EditYaku(Yaku.JunChantaiYaochuuClosed, _current.Add);
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
			if (_player.IsOpen()) EditYaku(Yaku.HonIisouOpen, _current.Add);
			else EditYaku(Yaku.HonIisouClosed, _current.Add);
        }
		else if (!honor && suits.Count() == 1)
		{
			if (_player.IsOpen()) EditYaku(Yaku.ChinIisouOpen, _current.Add);
			else EditYaku(Yaku.ChinIisouClosed, _current.Add);
		}
    }

	private void KokushiMusou(WinningHand wh)
	{
		if (wh.GetAllMelds().Count() == 1) EditYaku(Yaku.KokushiMusou, _yakuman.Add);
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
                    EditYaku(Yaku.ShouSuushii, _yakuman.Add);
				else EditYaku(Yaku.DaiSuuShii, _yakuman.Add);
            }
        }
	}

	private void TsuuIisou(WinningHand wh)
	{
		foreach (Meld meld in wh.GetAllMelds())
		{
			if (!meld.OnlyHonors()) return;
        }
        EditYaku(Yaku.TsuuIisou, _yakuman.Add);
    }

	private void ChinRoutou(WinningHand wh)
	{
		foreach (Meld meld in wh.GetAllMelds())
		{
			if (!meld.OnlyTerminals()) return;
        }
        EditYaku(Yaku.ChinRoutou, _yakuman.Add);
	}

	private void RyuuIisou(WinningHand wh)
	{
		foreach (Meld meld in wh.GetAllMelds())
		{
			if (!meld.OnlyGreens()) return;
        }
        EditYaku(Yaku.RyuuIisou, _yakuman.Add);
	}

	private void TenHou()
	{
		if (
			_table.State == State.Draw && 
			_table.UninterruptedFirstRound() && 
			_table.GetDealer() == _player

		) EditYaku(Yaku.TenHou, _yakuman.Add);
    }

	private void RenHou()
	{
		if (
			_table.State == State.Discard &&
			_table.UninterruptedFirstRound() &&
			!_player.Graveyard.Any()

		) EditYaku(Yaku.RenHou, _yakuman.Add);
    }

	private void ChiiHou()
	{
		if (
			_table.State == State.Draw &&
			_table.UninterruptedFirstRound() &&
			_table.GetDealer() != _player

		) EditYaku(Yaku.ChiiHou, _yakuman.Add);
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
				if (_player.WinningTiles.Count() == 9) 
					EditYaku(Yaku.JunseiChuurenPoutou, _yakuman.Add);
				else EditYaku(Yaku.ChuurenPoutou, _yakuman.Add);
            }
        }
    }
}
