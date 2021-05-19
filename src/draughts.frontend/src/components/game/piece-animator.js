import React from "react";

import Piece from "./piece";

import "../../styles/tile.css";
import "../../styles/piece-animator.css";

export default class PieceAnimator extends React.Component{

	constructor(props){
		super(props);
		this.state = {
			visible: false,
			moving: false,
			startingPosition: null,
			endingPosition: null,
			piece: null,

			top: 0,
			left: 0
		}
		this.currentlyUpdating = false;
	}

	render(){
		if(this.state.visible){
			return (
				<div className="piece-animator tile" 
				style={{
					width: `${100 / this.props.boardSize}%`,
					height: `${100 / this.props.boardSize}%`,
					top: `${100 / this.props.boardSize * this.state.top}%`,
					left: `${100 / this.props.boardSize * this.state.left}%`}}>
					<Piece source={this.state.piece}/>
				</div>
			);
		}
		return null;
	}

	componentDidUpdate(prevProps, prevState){
		if(this.currentlyUpdating) return;
		this.currentlyUpdating = true;
		console.log("update\n%O\n%O", prevState, this.state);
		setTimeout(() => {
			// Component has become visible
			if(!prevState.visible && this.state.visible){
				console.log("become visible");
				this.setState({moving: true, top: this.state.endingPosition[1], left: this.state.endingPosition[0]});
			}

			// Component has started moving
			if(!prevState.moving && this.state.moving){
				console.log("started moving");
				setTimeout(() => {
					this.setState({visible: false, moving: false});
				}, 500);
			}
			this.currentlyUpdating = false;
		}, 50);
	}

	animateMove(piece, before, after){

		var startingPosition = this.reorientateCoordinate(before);
		var endingPosition = this.reorientateCoordinate(after)

		this.setState({visible: true, moving: false, piece: piece, startingPosition: startingPosition, endingPosition: endingPosition, top: startingPosition[1], left: startingPosition[0]});
	}

	reorientateCoordinate(coordinate){
		if(this.props.player === 1){
			return [this.props.boardSize - coordinate[0] - 1, this.props.boardSize - coordinate[1] - 1];
		}
		return coordinate;
	}
}

