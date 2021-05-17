import React from "react";

export default class Piece extends React.Component{
	render(){
		return (
			<div className={`piece 
			${this.props.source.colour === 0 ? "piece-white" : "piece-black"} 
			${this.props.source.isKing ? "piece-king" : ""} 
			${this.props.selected ? "piece-selected" : ""}
			${this.props.forced ? "piece-forced" : ""}`}>
				<div className="piece-inside"/>
			</div>
		);
	}
}

