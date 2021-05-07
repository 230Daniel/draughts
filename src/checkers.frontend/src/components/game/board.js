import React from "react";

import Tile from "./tile.js";

import "../../styles/board.css";

export default class Board extends React.Component{
	constructor(props){
		super(props);
	}

	render(){
		return(
			<div className="board">
				{this.renderTiles()}
			</div>
		);
	}

	renderTiles(){
		var boardSize = 8;
		var tiles = [];

		for(var i = 0; i < boardSize; i++){
			for(var j = 0; j < boardSize; j++){

				// If a piece exists with coordinates (j, i) we should pass it to the tile to display.
				var piece = this.props.board.pieces.find(x => x.position.x === j && x.position.y === i);

				tiles.push((<Tile boardSize={boardSize} colour={(i+j) % 2 == 0} piece={piece}/>))
			}
		}

		return tiles;
	}
}
