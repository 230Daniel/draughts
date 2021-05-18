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
		var selectedPiece = this.props.board.pieces.find(x => coordinatesAreEqual([x.position.x, x.position.y], this.state.selectedTile));

		var i = 0;
		for(let j = 0; j < boardSize; j++){
			for(let k = 0; k < boardSize; k++){
				i++;
				let y = this.props.player === 0 ? j : boardSize - j - 1;
				let x = this.props.player === 0 ? k : boardSize - k - 1;

				var selected = coordinatesAreEqual(this.state.selectedTile, [x, y]);
				var forced = this.props.forcedMoves.some(move => coordinatesAreEqual(move[0], [x, y]));
				var previous = this.props.previousMove.some(move => move.some(coordinate => coordinatesAreEqual(coordinate, [x, y])));
				var possible = selectedPiece?.possibleMoves.some(m => coordinatesAreEqual([m.x, m.y], [x, y]));
				var possibleForced = selectedPiece?.possibleMoves.some(m => coordinatesAreEqual([m.x, m.y], [x, y])) && this.props.forcedMoves.some(m => coordinatesAreEqual(m[1], [x, y]));;
				var piece = this.props.board.pieces.find(p => p.position.x === x && p.position.y === y);
				var selectable = piece && 
								 this.props.waitingForMove && 
								 piece.colour === this.props.player && 
								 piece.possibleMoves.length > 0;

				tiles.push((<Tile 
					boardSize={boardSize} 
					colour={(y+x) % 2 === 0} 
					piece={piece} 
					selected={selected}
					selectable={selectable}
					forced={forced}
					position={[x, y]}
					possible={possible}
					possibleForced={possibleForced}
					previous={previous}
					onClicked={(tile) => this.onTileClicked(tile)}
					key={i}/>))
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
			// We should submit the move

			this.setState({
				selectedTile: null
			});

			this.props.submitMove(this.state.selectedTile, tile.position);
			
		} else if(tile.props.piece) {

			// No tile is selected and one with a piece has been clicked
			// We should select this tile if it's a valid tile to select

			if(tile.props.piece.colour === this.props.player && tile.props.piece.possibleMoves.length > 0){
				this.setState({
					selectedTile: tile.position
				});
			}
		}
	}
}

function coordinatesAreEqual(expected, actual) {
    return expected && actual && expected[0] === actual[0] && expected[1] === actual[1];
}
