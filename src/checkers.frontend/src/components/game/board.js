import React from "react";

import Tile from "./tile.js";

import "../../styles/board.css";

export default class Board extends React.Component{
	constructor(props){
		super(props);
		this.state = {
			selectedTile: null
		}
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

		for(var y = this.getInitialTile(boardSize); this.compareValue(y, boardSize); y += this.changeValue()){
			for(var x = this.getInitialTile(boardSize); this.compareValue(x, boardSize); x += this.changeValue()){

				var selected = coordinatesAreEqual(this.state.selectedTile, [x, y]);

				// If a piece exists with coordinates (x, y) we should pass it to the tile to display.
				var piece = this.props.board.pieces.find(p => p.position.x === x && p.position.y === y);

				tiles.push((<Tile 
					boardSize={boardSize} 
					colour={(y+x) % 2 === 0} 
					piece={piece} 
					selected={selected}
					position={[x, y]}
					onClicked={(tile) => this.onTileClicked(tile)}/>))
			}
		}

		return tiles;
	}

	getInitialTile(boardSize){
		return this.props.player === 0 ? 0 : boardSize - 1;
	}

	changeValue(){
		return this.props.player === 0 ? 1 : -1;
	}

	compareValue(value, boardSize){
		return this.props.player === 0 ? value < boardSize : value >= 0;
	}

	onTileClicked(tile){
		if(!this.props.waitingForMove) return;

		if(coordinatesAreEqual(this.state.selectedTile, tile.position)){

			// The selected tile was clicked again
			// We should reset the selected tile

			this.setState({
				selectedTile: null
			});

		} else if(this.state.selectedTile) {

			// A tile is already selected and another tile has been clicked
			// We should validate and then submit the move

			this.setState({
				selectedTile: null
			});

			this.props.submitMove(this.state.selectedTile, tile.position);

		} else if(tile.props.piece) {

			// No tile is selected and one with a piece has been clicked
			// We should select this tile

			this.setState({
				selectedTile: tile.position
			});
		}
	}
}

function coordinatesAreEqual(expected, actual) {
    return expected && actual && expected[0] === actual[0] && expected[1] === actual[1];
}
