namespace RiichiNET.Core.Scoring;

using System;
using System.Collections.Generic;
using System.Linq;

using RiichiNET.Core.Collections;
using RiichiNET.Core.Collections.Melds;
using RiichiNET.Core.Components;
using RiichiNET.Core.Enums;

internal sealed class Tabulator
{
    internal const int STARTING_SCORE = 25000;
    internal const int DEFEAT = 0;
    internal const int RIICHI_COST = 1000;
    internal const int YAKUMAN = 13;

    private readonly Table _table;
    private readonly Player _winner;

    internal Tabulator(Table table, Player winner)
    {
        _table = table;
        _winner = winner;
    }

    private static int Round(int value, int nearest)
        => value % nearest == 0 ? value : value + nearest - value % nearest;

    private int CalculateHan()
    {
        Mountain mountain = _table.Mountain;
        int han = _winner.YakuList.Sum(x => (int)x);
        foreach (Tile tile in _winner.Hand.Held())
        {
            if (tile.akadora) han++;
            if (mountain.DoraList.ContainsKey(tile))
            {
                han += _winner.Hand[tile] * mountain.DoraList[tile];
            }
            if (_winner.IsRiichi() && mountain.UraDoraList.ContainsKey(tile))
            {
                han += _winner.Hand[tile] * mountain.UraDoraList[tile];
            }
        }
        return han;
    }

    private int KouKanFu(Meld meld)
    {
        int fu = 0;
        if (meld.Mentsu == Mentsu.Koutsu) fu = 2;
        else if (meld.Mentsu == Mentsu.Kantsu) fu = 8;
        if (meld.Open) fu *= 2;
        if (meld.OnlyHonors()) fu *= 2;
        return fu;
    }

    private int JanFu(Meld meld)
    {
        int fu = 0;
        if (meld.Mentsu == Mentsu.Jantou && 
            (
                meld.OnlyDragons() ||
                meld.Contains(_winner.Wind.WindToValue()) ||
                meld.Contains(_table.Wind.WindToValue())
            )
        ) fu += 2;
        if (_table.Wind == _winner.Wind && meld.Contains(_table.Wind.WindToValue()))
            fu *= 2;

        return fu;
    }

    private int MachiFu()
        => _winner.WinningTiles.Count() == 1 ? 2 : 0;

    private int AgariFu(int fu)
    {
        if (_table.State != State.Draw && !_winner.IsOpen())
            fu += 10;
        else if (_table.State != State.Draw && _winner.IsOpen() && fu == 20)
            fu += 2;
        else if (_table.State == State.Draw && !_winner.YakuList.Contains(Yaku.Pinfu))
            fu += 2;
        return fu;
    }

    private int CalculateFu()
    {
        if (_winner.YakuList.Contains(Yaku.ChiiToitsu)) return 25;

        int fu = 20;
        foreach (Meld meld in _winner.WinningHands.First().GetAllMelds())
        {
            fu += KouKanFu(meld) + JanFu(meld);
        }
        fu += MachiFu() + AgariFu(fu);

        return Round(fu, 10);
    }

    private static int CalculateBaseScore(int han, int fu=0)
        => han switch
        {
            < 5 => Math.Min((int)Math.Round(fu * Math.Pow(2, han + 2)), 2000),
            5 => 2000,
            6 or 7 => 3000,
            > 7 and < 11 => 4000,
            11 or 12 => 6000,
            > 12 => (han/13) * 8000
        };

    private void Ron(int score)
    {
        score *= _winner == _table.GetDealer() ? 6 : 4;
        score = Round(score, 100);
        _winner.ScoreChange += score;
        _table.GetPlayer().ScoreChange -= score;
    }

    private void Tsumo(int score)
    {
        Player dealer = _table.GetDealer();
        score = _winner == dealer ? 
            3 * Round(score * 2, 100) :
            (2 * Round(score, 100)) + Round(score * 2, 100);

        foreach (Player player in _table.Players)
        {
            if (player == _winner) player.ScoreChange += score;

            else player.ScoreChange -= player == dealer ? 
                score / 2 : 
                _winner == dealer ? 
                    score / 3 : 
                    score / 4;
        }
    }

    private void Tabulate(int score)
    {
        if (_table.State != State.Draw)
            Ron(score);
        else Tsumo(score);
    }

    internal void Agari()
    {
        YakuCalculator yc = new YakuCalculator(_table, _winner);
        yc.DetermineYaku();
        if (_winner.YakuList.Any())
        {
            int han = CalculateHan();
            int fu = han < 5 ? CalculateFu() : 0;
            _winner.Points = (han, fu);
            int score = CalculateBaseScore(han, fu) + (300 * _table.Honba) + _table.Pool;
            Tabulate(score);
        }
    }

    internal static void Ryuukyoku(Table table)
    {
        int tenpai = 0;
        int score = 3000;
        ISet<Player> nagashiMangan = new HashSet<Player>();

        foreach (Player player in table.Players)
        {
            if (YakuCalculator.NagashiMangan(table, player))
                nagashiMangan.Add(player);
            if (player.IsTenpai()) tenpai++;
        }

        if (nagashiMangan.Any()) foreach (Player player in nagashiMangan)
        {
            Tabulator t = new Tabulator(table, player);
            t.Tsumo(CalculateBaseScore(5));
        }
        else if (tenpai is > 0 and < 4) foreach (Player player in table.Players)
        {
            player.ScoreChange += tenpai switch
            {
                1 => player.IsTenpai() ? score      : score / -3,
                2 => player.IsTenpai() ? score / 2  : score / -2,
                3 => player.IsTenpai() ? score / 3  : -score,
                _ => 0
            };
        }
    }
}
