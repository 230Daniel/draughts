import React from "react";
import { Redirect } from "react-router-dom";
import { HubConnectionBuilder } from "@aspnet/signalr";

import Board from "../components/game/board.js";

import "../styles/game.css";

export default class Game extends React.Component{
	constructor(props){
		super(props);
		this.state = {
			connection: null,
			playing: false,
			status: "Waiting for an opponent...",

			player: null,

			turn: null,
			board: null,
			forcedMoves: null, 
			previousMove: null
		}
		this.gameCode = this.props.match.params.gameCode;
		this.board = React.createRef();
	}

	render(){
		if (this.state.redirect) {
			return <Redirect to={this.state.redirect} />
		}
		if (!this.state.playing){
			return (
				<div>
					
					<h1>Game Page</h1>
					<h2>Game Code: {this.gameCode}</h2>
					<p>You are player {this.state.player + 1}</p>
					<p>{this.state.status}</p>
				</div>
			);
		}
		if(!this.state.board){
			return (
				<div>
					<h1>Loading...</h1>
				</div>
			)
		}
		return(
			<div className="game-container">
				<Board 
				board={this.state.board}
				player={this.state.player}
				waitingForMove={this.state.turn === this.state.player}
				forcedMoves={this.state.forcedMoves}
				previousMove={this.state.previousMove}
				submitMove={(current, destination) => this.submitMove(current, destination)}
				ref={this.board}/>
			</div>
		);
	}

	submitMove(current, destination){
		this.state.connection.invoke("submitMove", current, destination);
	}

	async componentDidMount(){
		var connection = new HubConnectionBuilder()
		.withUrl(`${window.BASE_URL}/gamehub`)
		.build();

		connection.on("gameStarted", (player) =>{
			console.log("Received dispatch GAME_STARTED\nplayer: %i", player);
			this.setState({playing: true, player: player})
		});

		connection.on("gameUpdated", (turn, board, forcedMoves, previousMove) => {
			console.log("Received dispatch GAME_UPDATED\nturn: %i\nboard: %O\nforcedMoves: %O\npreviousMove: %O", turn, board, forcedMoves, previousMove);

			if(this.board.current && previousMove.length > 0){
				this.board.current.animateMove(previousMove.slice(-1)[0][0], previousMove.slice(-1)[0][1]);
				setTimeout(() =>{
					this.setState({turn: turn, board: board, forcedMoves: forcedMoves, previousMove: previousMove});
				}, 500);
			} else{
				this.setState({turn: turn, board: board, forcedMoves: forcedMoves, previousMove: previousMove});
			}
			
		});

		connection.on("gameCanceled", () =>{
			console.log("Received dispatch GAME_CANCELED");
			this.setState({playing: false, status: "The game has been canceled."})
		});

		connection.on("gameEnded", (winner) =>{
			console.log("Received dispatch GAME_ENDED\nwinner: %i", winner);
			this.setState({playing: false, status: `The game has ended - Player ${winner + 1} won!`});
		});

		await connection.start();

		connection.invoke("joinGame", this.gameCode)
		.then((success) =>{
			if(!success){
				this.setState({redirect: "/"});
				return;
			}
		});

		this.setState({connection: connection});
	}

	componentWillUnmount(){
		this.state.connection.stop();
	}
}
