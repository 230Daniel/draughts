﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Draughts.Api.Extensions;
using Draughts.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Draughts.Api.Game
{
    public class TwoPlayerGame : IGame
    {
        public string GameCode { get; }
        public GameStatus GameStatus { get; set; }
        public List<User> Players { get; }
        public Board Board { get; }
        public GameCreateOptions Options { get; }

        int _turnNumber;
        List<(Position, Position)> _moves;
        int _currentMoveCount;
        
        User NextPlayer => Players[_turnNumber % Players.Count];
        IHubContext<GameHub> _hub;
        IClientProxy PlayersConnection => _hub.Clients.Clients(Players.Select(x => x.ConnectionId));
        IClientProxy Player1Connection => _hub.Clients.Clients(Players[0].ConnectionId);
        IClientProxy Player2Connection => _hub.Clients.Clients(Players[1].ConnectionId);

        public TwoPlayerGame(string gameCode, GameCreateOptions options, IHubContext<GameHub> hub)
        {
            GameCode = gameCode;
            Options = options;
            GameStatus = GameStatus.Waiting;
            Players = new();
            Board = new();
            _hub = hub;
            _moves = new();
        }

        public async Task AddPlayerAsync(User player)
        {
            player.OnDisconnected += OnPlayerDisconnected;
            Players.Add(player);
            
            if (Players.Count == 2)
            {
                GameStatus = GameStatus.Playing;
                await Player1Connection.SendAsync("GameStarted", 0);
                await Player2Connection.SendAsync("GameStarted", 1);
                await PlayersConnection.SendAsync("GameUpdated", 
                    _turnNumber % 2, 
                    Board, 
                    Board.GetForcedMoves(PieceColour.White).AsTransportable(),
                    _moves.TakeLast(_currentMoveCount + 1));
            }
            else
            {
                await Player1Connection.SendAsync("WaitingForOpponent");
            }
        }

        public async Task CancelAsync()
        {
            GameStatus = GameStatus.Canceled;
            await PlayersConnection.SendAsync("GameCanceled");
        }

        public async Task SubmitMove(User player, Position before, Position after)
        {
            if(NextPlayer != player) return;
            PieceColour pieceColour = Players.IndexOf(player) == 0 ? PieceColour.White : PieceColour.Black;

            MoveResult moveResult = Board.Move(before, after);
            if (moveResult.IsValid)
            {
                _moves.Add((before, after));
                _currentMoveCount++;
                
                Board.PromoteKings();
                Board.ApplyPossibleMoves();

                if (moveResult.IsFinished)
                    _turnNumber++;

                PieceColour nextPieceColour = moveResult.IsFinished ? pieceColour == PieceColour.White? PieceColour.Black : PieceColour.White : pieceColour;
                List<(Position, Position)> newForcedMoves = Board.GetForcedMoves(nextPieceColour);
                if (!moveResult.IsFinished) newForcedMoves.RemoveAll(x => x.Item1 != moveResult.PositionToMoveAgain);
                
                await PlayersConnection.SendAsync("GameUpdated", 
                    _turnNumber % 2, 
                    Board, 
                    newForcedMoves.AsTransportable(),
                    _moves.TakeLast(_currentMoveCount).AsTransportable());

                if (moveResult.IsFinished)
                    _currentMoveCount = 0;
                
                if (Board.GetIsWon(out PieceColour? winner))
                {
                    GameStatus = GameStatus.Ended;
                    await PlayersConnection.SendAsync("GameEnded", winner);
                }
            }
        }

        void OnPlayerDisconnected(object sender, EventArgs e)
        {
            _ = CancelAsync();
        }

        public TwoPlayerGame()
        {
            Players = new List<User>();
        }
    }

    public enum GameStatus
    {
        Waiting,
        Playing,
        Canceled,
        Ended
    }
}