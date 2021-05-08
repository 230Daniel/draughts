import React from "react";

export default class Piece extends React.Component{
	render(){
		return (
			<div className={`piece ${this.props.source.colour === 0 ? "piece-white" : "piece-black"} ${this.props.selected ? "piece-selected" : ""}`}>
				<div className="piece-inside"/>
			</div>
		);
	}
}

