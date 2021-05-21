import React from "react";
import Button from "../components/button";

export default class Play extends React.Component{
	render(){
		return (
			<div className="center">
				<h1 style={{marginBottom: "30px"}}>Play Draughts</h1>
				<Button to="/create-game" width="200px">Create a Game</Button>
				<Button to="/join-game" width="200px">Join a Game</Button>
			</div>
		);
	}

	async createGame(){
		var response = await fetch(`${window.BASE_URL}/game/create`, { method: "GET" });
		var gameCode = await response.json();
		await this.joinGame(gameCode);
	}

	async joinGame(gameCode){
		var response = await fetch(`${window.BASE_URL}/game/${gameCode}`, { method: "GET" });
		if (response.status === 404) {
			this.setState({invalidCode: true});
			return;
		}

		var game = await response.json();
		if (game.gameStatus !== 0) {
			this.setState({invalidCode: true});
			return;
		}
			
		this.setState({redirect: `/game/${gameCode}`});
	}
}
