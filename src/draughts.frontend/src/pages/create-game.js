import React from "react";
import { Redirect } from "react-router-dom";
import Loading from "../components/loading";
import { HubConnectionBuilder } from "@aspnet/signalr";

import "../styles/menu-box.css";

export default class CreateGame extends React.Component{
	constructor(props){
		super(props);
		this.state = {
			redirect: null,
			loading: false,
			loadingStatus: null,
			error: false,
			errorMessage: null,

			opponent: 0,
			algorithm: 0,
			variant: 0,
			side: 0
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
							this.setState({loading: false, loadingStatus: null, error: false, errorMessage: null})
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
				</>
			);
		}
		return(
			<>
				<span className="menu-box-title">Game Options</span>
				<div className="menu-box-section">
					<div className="menu-box-input">
						<span>Opponent</span>
						<select defaultValue={this.state.opponent} onChange={(e) => { this.setState({opponent: e.target.value}); }}>
							<option value="0">Player</option>
							<option value="1">Computer</option>
						</select>
					</div>
					{this.state.opponent === "1" &&
						<div className="menu-box-input">
							<span>Algorithm</span>
							<select defaultValue={this.state.algorithm} onChange={(e) => { this.setState({algorithm: e.target.value}); }}>
								<option value="0">Random Moves</option>
							</select>
						</div>
					}
				</div>
				<div className="menu-box-section">
					<div className="menu-box-input">
						<span>Side</span>
						<select defaultValue={this.state.side} onChange={(e) => { this.setState({side: e.target.value}); }}>
							<option value="0">Random</option>
							<option value="1">White</option>
							<option value="1">Black</option>
						</select>
					</div>
				</div>
				<div className="menu-box-section">
					<div className="menu-box-input">
						<span>Variant</span>
						<select defaultValue={this.state.variant} onChange={(e) => { this.setState({variant: e.target.value}); }}>
							<option value="0">English Draughts</option>
						</select>
					</div>
				</div>
				<div className="center">
					<button onClick={() => this.createGame()} className="menu-box-button">Create Game</button>
				</div>
			</>
		);
	}

	async createGame(){
		this.setState({loading: true, loadingStatus: "Creating game..."});

		try {
			var gameCode = await (await fetch(`${window.BASE_URL}/game/create`, { method: "GET" })).json();
		} catch (e) {
			this.setState({error: true, errorMessage: "Failed to create the game"});
			console.log("error: %O", e);
			return; 
		}

		this.setState({loadingStatus: "Connecting to game..."});

		window.connection = new HubConnectionBuilder()
		.withUrl(`${window.BASE_URL}/gamehub`)
		.build();

		try{
			await window.connection.start();
		} catch (e) {
			this.setState({error: true, errorMessage: "Failed to connect to the server"});
			console.log("error: %O", e);
			return;
		}

		this.setState({redirect: `/game/${gameCode}`});
	}
}
