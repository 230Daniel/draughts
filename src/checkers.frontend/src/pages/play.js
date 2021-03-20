import React from "react";
import { Redirect } from "react-router-dom";

export default class Play extends React.Component{
	constructor(props){
		super(props);
		this.state = {
			gameCode: "",
			invalidCode: false
		}
	}

	render(){
		if (this.state.redirect) {
			return <Redirect to={this.state.redirect} />
		}
		return (
			<div>
				<h1>Home Page</h1>
				<button onClick={() => this.createGame()}>New Game</button><br/>
				<input placeholder="Game Code" value={this.state.gameCode} onChange={(obj) => this.setState({gameCode: obj.target.value})}></input>
				<button onClick={() => this.joinGame(this.state.gameCode)}>Join Game</button><br/>
				{this.state.invalidCode &&
					<span style={{color: "red"}}>Invalid game code</span>
				}
			</div>
		);
	}

	async createGame(){
		var response = await fetch("https://localhost:5001/game/create", { method: "GET" });
		var gameCode = await response.json();
		await this.joinGame(gameCode);
	}

	async joinGame(gameCode){
		var response = await fetch(`https://localhost:5001/game/${gameCode}`, { method: "GET" });
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
