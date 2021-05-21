import React from "react";
import { Redirect } from "react-router-dom";
import Loading from "../components/loading";
import { HubConnectionBuilder } from "@aspnet/signalr";

import "../styles/menu-box.css";

export default class JoinGame extends React.Component{
	constructor(props){
		super(props);
		this.state = {
			redirect: null,
			loading: false,
			loadingStatus: null,
			error: false,
			errorMessage: null,

			gameCode: this.props.match.params.gameCode?.substring(0,6),
			validatingGameCode: false,
			gameCodeValid: false,
			gameCodeInvalid: false
		}
	}

	render(){
		if (this.state.redirect) {
			return <Redirect to={this.state.redirect} />
		}
		return (
			<div className="menu-box-centerer">
				<div className="menu-box">
					{this.renderMenuBoxContent()}
				</div>
			</div>
		);
	}

	renderMenuBoxContent(){
		if(this.state.error){
			return (
				<>
					<span className="menu-box-title">Error</span>
					<span className="menu-box-message">{this.state.errorMessage}</span>
					<div className="center">
						<button className="menu-box-button" onClick={() => {
							this.setState({loading: false, loadingStatus: null, error: false, errorMessage: null, validatingGameCode: false, gameCodeValid: false, gameCodeInvalid: false})
						}}>Back</button>
					</div>
				</>
			);
		}
		if(this.state.loading){
			return (
				<>
					<span className="menu-box-title">{this.state.loadingStatus}</span>
					<Loading/>
					<button className="menu-box-button" onClick={() => {
						this.setState({redirect: "/play"})
					}}>Back</button>
				</>
			);
		}
		return(
			<>
				<span className="menu-box-title">Enter Game Code</span>
				<div className="menu-box-section">
					<div className="menu-box-input menu-box-input-simple">
						<input style={{textAlign: "center", textTransform: "uppercase"}} value={this.state.gameCode} onChange={(e) => { this.gameCodeUpdated(e) }}/>
					</div>
				</div>
				{this.state.validatingGameCode &&
					<Loading height={50}/>
				}
				{this.state.gameCodeInvalid &&
					<span className="menu-box-message" style={{marginTop: "10px"}}>Invalid game code</span>
				}
				<div className="center inline">
					<button className="menu-box-button" style={{marginRight: "10px"}} onClick={() => {
						this.setState({redirect: "/play"})
					}}>Back</button>	
					{/*this.state.gameCodeValid && !this.state.validatingGameCode &&
						<button onClick={() => this.joinGame()} style={{marginLeft: "10px"}} disabled={!this.state.gameCodeValid} className="menu-box-button">Join Game</button>
					*/}
				</div>
			</>
		);
	}

	async gameCodeUpdated(e){
		var gameCode = e.target.value.substring(0,6).toLowerCase();
		if(gameCode === this.state.gameCode) return;
		this.setState({validatingGameCode: false, gameCodeValid: false, gameCode: gameCode});

		if(gameCode.length === 6){
			this.setState({validatingGameCode: true, gameCodeInvalid: false});
			try {
				var response = await fetch(`${window.BASE_URL}/game/${gameCode}`, { method: "GET" });
				if(response.status !== 200 && response.status !== 404){
					this.setState({error: true, errorMessage: "Failed to validate game code"});
					return;
				}
				var valid = response.status === 200;
				if(valid){
					var game = await response.json();
					if(game.gameStatus !== 0) valid = false;
				}
				this.setState({validatingGameCode: false, gameCodeInvalid: !valid, gameCodeValid: valid});
				if(valid){
					this.joinGame();
				}
			} catch (e) {
				this.setState({error: true, errorMessage: "Failed to validate game code"});
				console.log("error: %O", e);
				return;
			}
		}
	}

	async joinGame(){
		this.setState({loading: true, loadingStatus: "Fetching game..."});

		try {
			var game = await (await fetch(`${window.BASE_URL}/game/${this.state.gameCode}`, { method: "GET" })).json();
		} catch (e) {
			this.setState({error: true, errorMessage: "Invalid game code"});
			console.log("error: %O", e);
			return; 
		}

		this.setState({redirect: `/game/${this.state.gameCode}`});
	}
}
