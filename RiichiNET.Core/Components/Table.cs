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
    internal readonly record struct Call(int elapsed, Seat caller, Seat callee, Naki type);

    private const int SEATS = 4;

    internal State State { get; private set; } = State.Draw;
    public Wind Wind { get; private set; } = Wind.East;
    public int Pool { get; private set; } = 0;
    public int Honba { get; private set; } = 0;
    private int _round = 0;
    private Seat _turn = 0;

    internal int Elapsed { get; private set; } = 0;
    internal LinkedList<Call> Calls { get; } = new LinkedList<Call>();
    private Tile _justDiscarded;

    internal Mountain Mountain = new Mountain();
    internal Player[] Players { get; } = new Player[4]
    {
        new Player(Seat.First),
        new Player(Seat.Second),
        new Player(Seat.Third),
        new Player(Seat.Fourth)
    };

    internal Table()
        => InitialDraw();

    public Player GetPlayer(Seat? seat=null)
        => seat == null ? Players[(int)_turn] : Players[(int)seat];

    internal Player GetDealer()
    {
        int seat = _round > SEATS - 1 ? _round - SEATS : _round;
        return Players[seat];
    }

    internal ISet<Player> GetOtherPlayers()
    {
        ISet<Player> others = new HashSet<Player>(Players);
        others.Remove(GetPlayer());
        return others;
    }

    internal bool UninterruptedFirstRound()
        => !Calls.Any() && Elapsed < SEATS;
    
    internal bool IsCallVictim(Player candidate)
    {
        foreach (Call call in Calls)
        {
            if (call.callee == candidate) return true;
        }
        return false;
    }

    private bool CanKan()
        => Mountain.DoraCount() <= SEATS && !Mountain.IsEmpty();

    private bool CanAgari(Player player, Value value)
    {
        if (player.IsFuriten() && State != State.Draw) return false;

        WinningHand baseCase = new WinningHand(player.Melds);
        TileCount includesValue = new TileCount(player.Hand);
        includesValue.Draw(value);

        HandEvaluator he = new HandEvaluator(includesValue, baseCase);
        YakuCalculator yc = new YakuCalculator(this, player);

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
            if (CanAgari(player, value)) player.Callables.Add(Naki.Agari, value);
        }
    }

    internal void SetIchijiFuriten(HashSet<Player> players)
    {
        foreach (Player player in players)
        {
            player.IchijiFuriten = true;
        }
    }

    private ISet<Player> CanCallOnDiscard()
    {
        ISet<Player> able = new HashSet<Player>();

        foreach (Player player in GetOtherPlayers())
        {
            if (player.Callables.Able(_justDiscarded))
            {
                able.Add(player);
            }
        }
        return able;
    }

    internal bool Draw(Seat? turn = null)
    {
        State = State.Draw;
        Tile tile = Mountain.Draw();
        if (tile != Value.None)
        {
            GetPlayer(turn).Draw(tile);
            RectifyCallables();
            return true;
        }
        else return false;
    }

    internal ISet<Player> Discard(Tile tile, bool riichi = false)
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
            int i = seat == _turn ? TileCount.MAX_HAND_SIZE : TileCount.MIN_HAND_SIZE;
            for (; i > 0; i--) Draw(seat);
        }
    }

    internal ISet<Player> FormMeld(Meld meld, Seat caller)
    {
        State = State.Call;
        Calls.AddLast(new Call(Elapsed, caller, _turn, meld.Naki));

        GetPlayer(caller).AddMeld(meld);
        if (caller != _turn) ChangeTurn(caller);

        ISet<Player> able = new HashSet<Player>();
        if (meld.Naki is Naki.ShouMinKan or Naki.AnKan)
            foreach (Player player in GetOtherPlayers())
            {
                if (player.Callables.Able(meld[0], Naki.Agari) &&
                (
                    meld.Naki == Naki.ShouMinKan ||
                    YakuCalculator.IsTenpaiForKokushi(player.Hand)
                ))
                { able.Add(player); }
            }
        return able;
    }

    internal void Rinshan()
    {
        Player player = GetPlayer();
        Tile tile = Mountain.Rinshan();
        player.Draw(tile);
        RectifyCallables();
    }

    internal ISet<Player> StartRiichi(Tile tile)
    {
        Discard(tile, riichi: true);

        Calls.AddLast(new Call(Elapsed, GetPlayer(), _turn, Naki.Riichi));

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
        => Mountain.IsEmpty();

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
        Tabulator.Ryuukyoku(this);
        Honba++;
        return GetDealer().IsTenpai() ? false : true;
    }

    private Player DetermineWinner(ISet<Player> winners)
    {
        if (winners.Count() == 1) return winners.First();

        Seat priorty = _turn;
        Player candidate;
        while (true)
        {
            priorty = priorty.Next<Seat>();
            candidate = GetPlayer(priorty);
            if (winners.Contains(candidate)) return candidate;
        }
    }

    internal bool Agari(ISet<Player> winners)
    {
        Player winner = DetermineWinner(winners);
        Tabulator t = new Tabulator(this, winner);
        t.Agari();

        Pool = 0;
        bool overthrow = true;
        if (winner == GetDealer())
        {
            Honba++;
            overthrow = false;
        }
        return overthrow;
    }

    private Seat GetNextDealer()
        => GetDealer().Seat.Next<Seat>();

    internal void NextRound(bool overthrow)
    {
        Mountain.Reset();
        State = State.Draw;
        Elapsed = 0;
        Calls.Clear();

        if (overthrow)
        {
            Wind = Wind.Next<Wind>();
            _round++;
            _turn = GetNextDealer();
            Honba = 0;
        }

        foreach (Player player in Players) player.NextRound(overthrow);

        InitialDraw();
    }
}
