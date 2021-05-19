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
	}

	render(){
		if(this.state.visible){
			return (
				<div className="piece-animator tile" 
				style={{
					width: `${100 / this.props.boardSize}%`,
					height: `${100 / this.props.boardSize}%`,
					top: `${100 / this.props.boardSize * this.state.top}%`,
					left: `${100 / this.props.boardSize * this.state.left}%`,
					transition: this.state.moving ? "top 0.5s, left 0.5s" : ""
					}} >
					<Piece source={this.state.piece}/>
				</div>
			);
		}
		return null;
	}

	componentDidUpdate(){
		setTimeout(() => {
			if(this.state.visible && !this.state.moving){
				this.setState({visible: true, moving: true, top: this.state.endingPosition[1], left: this.state.endingPosition[0]});
			} else if(this.state.moving){
				setTimeout(() => {
					this.setState({visible: false, moving: false});
				}, 500);
			}
		}, 50);
	}

	animateMove(piece, before, after){
		console.log("Animating move %O --> %O with piece %O", before, after, piece);
		this.setState({piece: piece, startingPosition: this.reorientateCoordinate(before), endingPosition: this.reorientateCoordinate(after)});
		this.setState({visible: true, moving: false, top: this.state.startingPosition[1], left: this.state.startingPosition[0]});
	}

	reorientateCoordinate(coordinate){
		if(this.props.player === 1){
			for(let i = 0; i < coordinate.length; i++){
				coordinate[i] = this.props.boardSize - coordinate[i] - 1;
			}
		}
		return coordinate;
	}
}

