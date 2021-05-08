import React from "react";

import Piece from "./piece.js";

export default class Tile extends React.Component{
	constructor(props){
		super(props)
		this.position = this.props.position;
	}

	render(){
		return (
			<div style={{
				backgroundColor: this.props.colour === 0 ? "#a45f16": "#ffd27b",
				width: `${100 / this.props.boardSize}%`,
				height: `${100 / this.props.boardSize}%`
				}} className="tile"
				onClick={() => this.onClicked()}>
				{this.renderPiece()}
			</div>
		);
	}

	renderPiece(){
		if(this.props.piece){
			return(
				<Piece source={this.props.piece} selected={this.props.selected}/>
			)
		}
		return null;
	}

	onClicked(){
		this.props.onClicked(this);
	}
}
