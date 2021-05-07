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
			board: null,
			progress: 15
		}
		this.gameCode = this.props.match.params.gameCode;
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
				<Board board={this.state.board}/>
			</div>
		);
	}

	takeTurn(){
		this.state.connection.invoke("takeTurn");
	}

	async componentDidMount(){
		var connection = new HubConnectionBuilder()
		.withUrl(`${window.BASE_URL}/gamehub`)
		.build();

		connection.on("gameStarted", (player) =>{
			this.setState({playing: true, status: `You are player ${player}`})
		});

		connection.on("gameCanceled", () =>{
			this.setState({playing: false, status: "The game has been canceled."})
		});

		connection.on("boardUpdated", (board) => {
			this.setState({board: board});
		});

		connection.on("turnChanged", (turn) => {
			this.setState({turn: turn});
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
