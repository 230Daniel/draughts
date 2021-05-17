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
				width: `${100 / this.props.boardSize}%`,
				height: `${100 / this.props.boardSize}%`
				}} 
				className={`tile
				${this.props.colour ? "tile-white" : "tile-black"}
				${this.props.possible ? "tile-possible" : ""}
				${this.props.forced ? "tile-forced" : ""}`}
				onClick={() => this.onClicked()}>
				{this.renderPiece()}
			</div>
		);
	}

	renderPiece(){
		if(this.props.piece){
			return(
				<Piece source={this.props.piece} selected={this.props.selected} forced={this.props.forced}/>
			)
		}
		return null;
	}

	onClicked(){
		this.props.onClicked(this);
	}
}
