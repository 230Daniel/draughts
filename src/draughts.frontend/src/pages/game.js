import React from "react";
import { Redirect } from "react-router-dom";
import { HubConnectionBuilder } from "@aspnet/signalr";

import Board from "../components/game/board.js";
import Loading from "../components/loading";

import "../styles/game.css";

export default class Game extends React.Component{
	constructor(props){
		super(props);
		this.state = {
			message: null,
			playing: false,
			loading: true,
			exit: false,
			waitingForOpponent: false,
			
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
		if(this.state.redirect){
			return (
				<Redirect to={this.state.redirect}/>
			)
		}
		if(!this.state.playing || this.state.loading || this.state.error || this.state.waitingForOpponent || this.state.exit){
			return(
				<div className="menu-box-centerer">
					<div className="menu-box">
						{this.renderMenuBoxContent()}
					</div>
				</div>
			);
		}
		return(
			<div className="game-container">
				<Board 
				board={this.state.board}
				player={this.state.player}
				waitingForMove={this.state.turn === this.state.player}
				possibleMoves={this.state.possibleMoves}
				previousMove={this.state.previousMove}
				submitMove={(current, destination) => this.submitMove(current, destination)}
				ref={this.board}/>
			</div>
		);
	}

	renderMenuBoxContent(){
		if(this.state.exit){
			return (
				<>
					<span className="menu-box-title">{this.state.message}</span>
					<div className="center">
						<button className="menu-box-button" onClick={() => {
							this.setState({redirect: "/play"})
						}}>Back</button>
					</div>
				</>
			);
		}
		if(this.state.error){
			return (
				<>
					<span className="menu-box-title">Error</span>
					<span className="menu-box-message">{this.state.errorMessage}</span>
					<div className="center">
						<button className="menu-box-button" onClick={() => {
							this.setState({redirect: "/play"})
						}}>Back</button>
					</div>
				</>
			);
		}
		if(this.state.waitingForOpponent){
			return(
				<>
					<span className="menu-box-title">Waiting for opponent...</span>
					<span className="menu-box-message">Invite a friend using this game code</span>
					<div className="menu-box-section">
						<div className="menu-box-input menu-box-input-simple">
							<input value={this.gameCode} style={{textAlign: "center", textTransform: "uppercase"}} readOnly/>
						</div>
					</div>
					<div className="center">
						<button className="menu-box-button" onClick={() => {
							this.setState({redirect: "/play"})
						}}>Back</button>
					</div>
					
				</>
			);
		}
		if(this.state.loading || (!this.state.waitingForOpponent && !this.state.playing)){
			return (
				<>
					<span className="menu-box-title">{this.state.message}</span>
					<Loading/>
				</>
			);
		}
	}

	submitMove(current, destination){
		window.connection.invoke("submitMove", current, destination);
	}

	async componentDidMount(){

		window.connection = new HubConnectionBuilder()
		.withUrl(`${window.BASE_URL}/gamehub`)
		.build();

		this.setState({message: "Connecting..."});

		try{
			await window.connection.start();
		} catch (e) {
			this.setState({error: true, errorMessage: "Failed to connect to the server"});
			console.log("error: %O", e);
			return;
		}

		this.setState({message: "Getting ready..."});

		window.connection.on("waitingForOpponent", () =>{
			console.log("Received dispatch WAITING_FOR_OPPONENT");
			this.setState({waitingForOpponent: true})
		});

		window.connection.on("gameStarted", (player) =>{
			console.log("Received dispatch GAME_STARTED\nplayer: %i", player);
			this.setState({waitingForOpponent: false, player: player, message: "Loading..."})
		});

		window.connection.on("gameUpdated", (turn, board, possibleMoves, previousMove) => {
			console.log("Received dispatch GAME_UPDATED\nturn: %i\nboard: %O\npossibleMoves: %O\npreviousMove: %O", turn, board, possibleMoves, previousMove);

			if(this.board.current && previousMove.length > 0){
				this.board.current.animateMove({ ...previousMove.slice(-1)[0][0]}, {...previousMove.slice(-1)[0][1]});
				setTimeout(() =>{
					this.setState({loading: false, playing: true, turn: turn, board: board.tiles, possibleMoves: possibleMoves, previousMove: previousMove});
				}, 500);
			} else{
				this.setState({loading: false, playing: true, turn: turn, board: board.tiles, possibleMoves: possibleMoves, previousMove: previousMove});
			}
			
		});

		window.connection.on("gameCanceled", () =>{
			console.log("Received dispatch GAME_CANCELED");
			this.setState({error: true, errorMessage: "Your opponent has disconnected"});
		});

		window.connection.onclose(() =>{
			this.setState({error: true, errorMessage: "Lost connection to the server"});
		});

		window.connection.on("gameEnded", (won) =>{
			console.log("Received dispatch GAME_ENDED\nwon: %i", won);

			setTimeout(() =>{
				this.setState({exit: true, message: won ? "Victory!" : "Defeat!"});
			}, 1000);
		});

		this.setState({message: "Joining game..."});

		try{
			var game = await window.connection.invoke("joinGame", this.gameCode);
			if(!game){
				this.setState({error: true, errorMessage: "Failed to join the game"});
				return;
			}
		} catch (e) {
			this.setState({error: true, errorMessage: "Failed to join the game"});
			console.log("error: %O", e);
			return;
		}
	}

	componentWillUnmount(){
		window.connection?.stop();
	}
}
