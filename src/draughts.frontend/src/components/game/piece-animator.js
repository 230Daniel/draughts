import React from "react";

import AnimatedPiece from "./animated-piece";

export default class PieceAnimator extends React.Component{

	constructor(props){
		super(props);
		this.state = {
			
		};
		this.animatingPieces = [];
		this.i = 0;
	}

	render(){
		return(
			<>
				{this.animatingPieces}
			</>
		);
	}

	animateMove(piece, origin, destination){

		origin = this.reorientateCoordinate(origin);
		destination = this.reorientateCoordinate(destination);

		this.i++;
		this.animatingPieces.push((<AnimatedPiece key={this.i} piece={piece} origin={origin} destination={destination} boardSize={this.props.boardSize}/>));
		this.setState({});

		setTimeout(() =>{
			this.animatingPieces.pop();
		}, 1000);
	}

	reorientateCoordinate(coordinate){
		if(this.props.player === 1){
			return [this.props.boardSize - coordinate[0] - 1, this.props.boardSize - coordinate[1] - 1];
		}
		return coordinate;
	}
}

