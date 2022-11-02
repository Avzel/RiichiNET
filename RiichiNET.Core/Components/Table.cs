namespace RiichiNET.Core.Components;

using System;
using System.Collections.Generic;
using System.Linq;

using RiichiNET.Core.Collections;
using RiichiNET.Core.Collections.Melds;
using RiichiNET.Core.Enums;
using RiichiNET.Core.Scoring;
using RiichiNET.Util.Extensions;

public sealed class Table
{
    internal struct Call
    {
        internal int elapsed;
        internal Seat caller;
        internal Naki type;
    }

    internal State State { get; private set; } = State.Draw;
    public Wind Wind { get; private set; } = Wind.East;
    public int Pool { get; private set; } = 0;
    private int _round = 0;
    private Seat _turn = 0;

    internal int Elapsed { get; private set; } = 0;
    internal LinkedList<Call> Calls { get; } = new LinkedList<Call>();
    private Tile _justDiscarded;

    private Mountain _mountain = new Mountain();
    internal Player[] Players { get; } = new Player[4]
    {
        new Player(Seat.First),
        new Player(Seat.Second),
        new Player(Seat.Third),
        new Player(Seat.Fourth)
    };

    internal Table()
    {
        InitialDraw();
    }

    public Player GetPlayer(Seat? seat=null)
    {
        return seat == null ? Players[(int)_turn] : Players[(int)seat];
    }

    internal Player GetDealer()
    {
        int seat = _round > 3 ? _round - 4 : _round;
        return Players[seat];
    }

    internal HashSet<Player> GetOtherPlayers()
    {
        HashSet<Player> others = new HashSet<Player>(Players);
        others.Remove(GetPlayer());
        return others;
    }

    internal bool UninterruptedFirstRound()
    {
        return !Calls.Any() && Elapsed < 4;
    }

    private bool CanKan()
    {
        return _mountain.DoraCount() < 5 && !_mountain.IsEmpty();
    }

    private bool CanAgari(Player player, Value value)
    {
        if (State != State.Draw && player.IsFuriten()) return false;

        WinningHand baseCase = new WinningHand(player.Melds);
        TileCount includesValue = new TileCount(player.Hand);
        includesValue.Draw(value);

        HandEvaluator he = new HandEvaluator(includesValue, baseCase);
        YakuCalculator yc = new YakuCalculator();

        return yc.YakuExists();
    }

    private void RectifyCallables()
    {
        Player player = GetPlayer();
        if (!CanKan())
        {
            player.Callables.Clear(Naki.AnKan);
            player.Callables.Clear(Naki.ShouMinKan);
            player.Callables.Clear(Naki.DaiMinKan);
        }

        foreach (Value value in player.Callables[Naki.Agari])
        {
            if (!CanAgari(player, value)) player.Callables.Remove(Naki.Agari, value);
        }
    }

    internal void SetIchijiFuriten(HashSet<Player> players)
    {
        foreach (Player player in players)
        {
            player.IchijiFuriten = true;
        }
    }

    private HashSet<Player> CanCallOnDiscard()
    {
        HashSet<Player> able = new HashSet<Player>();

        foreach (Player player in GetOtherPlayers())
        {
            if (player.Callables.Able(_justDiscarded.value))
            {
                able.Add(player);
            }
        }
        return able;
    }

    internal bool Draw(Seat? turn = null)
    {
        State = State.Draw;
        Tile tile = _mountain.Draw();
        if (tile.value != Value.None)
        {
            GetPlayer(turn).Draw(tile);
            RectifyCallables();
            return true;
        }
        else return false;
    }

    internal HashSet<Player> Discard(Tile tile, bool riichi = false)
    {
        State = State.Discard;
        Player player = GetPlayer();
        if (riichi) player.DeclareRiichi(tile);
        else player.Discard(tile);
        _justDiscarded = tile;
        RectifyCallables();
        return CanCallOnDiscard();
    }

    private void InitialDraw()
    {
        foreach (Seat seat in Enum.GetValues(typeof(Seat)))
        {
            for (int i = seat == _turn ? 14 : 13; i > 0; i--) Draw(seat);
        }
    }

    internal HashSet<Player> FormMeld(Meld meld, Seat caller)
    {
        State = State.Call;
        Calls.AddLast(new Call 
        {
            elapsed = Elapsed, caller = caller, type = meld.Naki
        });

        GetPlayer(caller).AddMeld(meld);
        if (caller != _turn) ChangeTurn(caller);

        HashSet<Player> able = new HashSet<Player>();
        if (meld.Naki is Naki.ShouMinKan or Naki.AnKan)
            foreach (Player player in GetOtherPlayers())
            {
                if (player.Callables.Able(meld[0].value, Naki.Agari) &&
                (
                    meld.Naki == Naki.ShouMinKan ||
                    YakuCalculator.KokushuMusou(player.Hand)
                ))
                { able.Add(player); }
            }
        return able;
    }

    internal void Rinshan()
    {
        Player player = GetPlayer();
        Tile tile = _mountain.Rinshan();
        player.Draw(tile);
        RectifyCallables();
    }

    internal HashSet<Player> StartRiichi(Tile tile)
    {
        Discard(tile, riichi: true);

        Calls.AddLast(new Call 
        {
            elapsed = Elapsed, caller = GetPlayer().Seat, type = Naki.Riichi 
        });

        return CanCallOnDiscard();
    }

    internal void EndRiichi()
    {
        Player player = GetPlayer();
        player.ScoreChange -= Tabulator.RIICHI_COST;
        Pool += Tabulator.RIICHI_COST;
    }

    internal void NextTurn()
    {
        _turn = _turn.Next<Seat>();
        Elapsed++;
    }

    private void ChangeTurn(Seat seat)
    {
        _turn = seat;
        Elapsed++;
    }

    internal bool RoundIsOver()
    {
        return _mountain.IsEmpty();
    }

    internal bool GameIsOver()
    {
        foreach (Player player in Players)
        {
            if (player.IsDefeated()) return true;
        }
        return _round > 7 || false;
    }

    internal bool Ryuukyoku()
    {
        Tabulator.Ryuukyoku(Players);
        return GetDealer().IsTenpai() ? false : true;
    }

    internal bool Agari(HashSet<Player> winners)
    {
        foreach (Player winner in winners)
        {
            YakuCalculator yc = new YakuCalculator();
            winner.YakuList = yc.DetermineYaku();
        }
        Tabulator.Agari(winners);

        return winners.Contains(GetDealer());
    }

    private Seat DetermineNextDealer()
    {
        return GetDealer().Seat.Next<Seat>();
    }

    internal void NextRound(bool overthrow)
    {
        _mountain.Reset();
        State = State.Draw;
        Elapsed = 0;
        Calls.Clear();

        if (overthrow)
        {
            Wind = Wind.Next<Wind>();
            _round++;
            _turn = DetermineNextDealer();
        }
        foreach (Player player in Players) player.NextRound(overthrow);

        InitialDraw();
    }
}
