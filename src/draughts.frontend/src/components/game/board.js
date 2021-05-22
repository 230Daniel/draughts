import React from "react";

import Tile from "./tile";
import PieceAnimator from "./piece-animator";

import "../../styles/board.css";

const BOARD_SIZE = 8;

export default class Board extends React.Component{
	constructor(props){
		super(props);
		this.state = {
			selectedTile: null
		}
		this.pieceAnimator = React.createRef();
	}

	render(){
		return(
			<div className="board">
				{this.renderTiles()}
				{<PieceAnimator
				boardSize={BOARD_SIZE}
				player={this.props.player}
				ref={this.pieceAnimator}/>}
			</div>
		);
	}

	renderTiles(){
		var tiles = [];

		var selectedPossibleMoves = [];

		if(this.state.selectedTile){
			selectedPossibleMoves = this.props.possibleMoves.filter(x => coordinatesAreEqual([x.origin.x, x.origin.y], this.state.selectedTile));
		}
		
		var possibleMovesAreForced = selectedPossibleMoves.some(x => x.isJumping);

		var i = 0;
		for(let j = 0; j < BOARD_SIZE; j++){
			for(let k = 0; k < BOARD_SIZE; k++){
				i++;
				let y = this.props.player === 0 ? j : BOARD_SIZE - j - 1;
				let x = this.props.player === 0 ? k : BOARD_SIZE - k - 1;

				var selected = coordinatesAreEqual(this.state.selectedTile, [x, y]);
				var forced = this.props.possibleMoves.some(m => m.isJumping && coordinatesAreEqual([m.origin.x, m.origin.y], [x, y]));
				var previous = this.props.previousMove.some(m => m.some(c => coordinatesAreEqual(c, [x, y])));
				var possible = selectedPossibleMoves.some(m => coordinatesAreEqual([m.destination.x, m.destination.y], [x, y]));
				var possibleForced = possible && possibleMovesAreForced;
				var piece = this.props.board[x][y].piece;
				var selectable = piece && 
								 this.props.waitingForMove && 
								 piece.colour === this.props.player &&
								 this.props.possibleMoves.some(m => coordinatesAreEqual([m.origin.x, m.origin.y], [x, y]));

				tiles.push((<Tile 
					boardSize={BOARD_SIZE} 
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

			if(tile.props.piece.colour === this.props.player && this.props.possibleMoves.some(m => coordinatesAreEqual([m.origin.x, m.origin.y], tile.position))){
				this.setState({
					selectedTile: tile.position
				});
			}
		}
	}

	animateMove(before, after){
		var tile = this.props.board[before[0]][before[1]];
		var piece = {...tile.piece};
		tile.piece = null;
		this.setState({});
		this.pieceAnimator.current.animateMove(piece, before, after);
	}
}

function coordinatesAreEqual(expected, actual) {
    return expected && actual && expected[0] === actual[0] && expected[1] === actual[1];
}
