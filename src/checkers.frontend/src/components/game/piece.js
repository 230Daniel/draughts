import React from "react";

export default class Piece extends React.Component{
	render(){
		if(this.props.selected){
			console.log(this.props.source.possibleMoves.map(x => [x.x, x.y]));
		}
		return (
			<div className={`piece 
			${this.props.source.colour === 0 ? "piece-white" : "piece-black"} 
			${this.props.source.isKing ? "piece-king" : ""} 
			${this.props.selected ? "piece-selected" : ""}`}>
				<div className="piece-inside">
					{this.props.source.isKing &&
					<img src="/crown.svg"/>}
				</div>
			</div>
		);
	}
}

