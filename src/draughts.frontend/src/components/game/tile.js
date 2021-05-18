import React from "react";

import Piece from "./piece.js";

import "../../styles/tile.css";

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
				${this.props.possibleForced ? "tile-possible-forced" : ""}
				${this.props.forced ? "tile-forced" : ""}
				${this.props.previous ? "tile-previous" : ""}
				${this.props.selected ? "tile-selected" : ""}
				${this.props.selectable ? "tile-selectable": ""}`}
				onClick={() => this.onClicked()}>
				{this.renderPiece()}
				{this.renderInside()}
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

	renderInside(){
		if(this.props.possible){
			return(
				<div className={`tile-inside tile-inside-possible ${this.props.possibleForced ? "tile-inside-possible-forced" : ""}`}/>
			)
		}
	}

	onClicked(){
		this.props.onClicked(this);
	}
}
