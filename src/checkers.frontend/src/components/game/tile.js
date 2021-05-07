import React from "react";

import Piece from "./piece.js";

export default class Tile extends React.Component{
	constructor(props){
		super(props)
	}

	render(){
		return (
			<div style={{
				backgroundColor: this.props.colour == 0 ? "#a45f16": "#ffd27b",
				width: `${100 / this.props.boardSize}%`,
				height: `${100 / this.props.boardSize}%`
				}} className="tile">
				{this.renderPiece()}
			</div>
		);
	}

	renderPiece(){
		if(this.props.piece){
			return(
				<Piece source={this.props.piece}/>
			)
		}
		return null;
	}
}
