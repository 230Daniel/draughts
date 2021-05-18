import React from "react";

import "../../styles/piece.css";

export default class Piece extends React.Component{
	render(){
		return (
			<div className={`piece 
			${this.props.source.colour === 0 ? "piece-white" : "piece-black"} 
			${this.props.source.isKing ? "piece-king" : ""} 
			${this.props.selected ? "piece-selected" : ""}`}>
				<div className="piece-inside">
					{this.props.source.isKing &&
					<img src="/crown.svg" alt="king"/>}
				</div>
			</div>
		);
	}
}

