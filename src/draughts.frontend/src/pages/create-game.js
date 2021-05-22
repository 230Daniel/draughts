import React from "react";
import { Redirect } from "react-router-dom";
import Loading from "../components/loading";

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
			depth: 4,
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
						<select defaultValue={this.state.opponent} onChange={(e) => { this.setState({opponent: parseInt(e.target.value)}); }}>
							<option value="0">Player</option>
							<option value="1">Computer</option>
						</select>
					</div>
					{this.state.opponent === 1 &&
						<div className="menu-box-input">
							<span>Algorithm</span>
							<select defaultValue={this.state.algorithm} onChange={(e) => { this.setState({algorithm: parseInt(e.target.value)}); }}>
							<option value="0">MiniMax</option>
							<option value="1">Random Moves</option>
							</select>
						</div>
					}
					{this.state.opponent === 1 && this.state.algorithm === 0 &&
						<div className="menu-box-input">
							<span>Depth</span>
							<select defaultValue={this.state.depth} onChange={(e) => { this.setState({depth: parseInt(e.target.value)}); }}>
							<option value="1">1 move</option>
							<option value="2">2 moves</option>
							<option value="3">3 moves</option>
							<option value="4">4 moves</option>
							<option value="5">5 moves</option>
							<option value="6">6 moves</option>
							<option value="7">7 moves</option>
							<option value="8">8 moves</option>
							</select>
						</div>
					}
				</div>
				<div className="menu-box-section">
					<div className="menu-box-input">
						<span>Side</span>
						<select defaultValue={this.state.side} onChange={(e) => { this.setState({side: parseInt(e.target.value)}); }}>
							<option value="0">Random</option>
							<option value="1">White</option>
							<option value="2">Black</option>
						</select>
					</div>
				</div>
				<div className="menu-box-section">
					<div className="menu-box-input">
						<span>Variant</span>
						<select defaultValue={this.state.variant} onChange={(e) => { this.setState({variant: parseInt(e.target.value)}); }}>
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
			var gameCode = await (await fetch(`${window.BASE_URL}/game/create`, { method: "POST", body: JSON.stringify(this.state), headers: {"Content-Type": "application/json"} })).json();
		} catch (e) {
			this.setState({error: true, errorMessage: "Failed to create the game"});
			console.log("error: %O", e);
			return; 
		}

		this.setState({redirect: `/game/${gameCode}`});
	}
}
