import React from "react";

import Piece from "./piece";

import "../../styles/tile.css";
import "../../styles/animated-piece.css";

export default class AnimatedPiece extends React.Component{

	constructor(props){
		super(props);
		this.state = {
			moving: false,
			finished: false
		};
	}

	render(){
		if(this.state.finished) return null;

		var top;
		var left;
		

		if(this.state.moving){
			top = this.props.destination[1];
			left = this.props.destination[0];
			
		} else {
			top = this.props.origin[1];
			left = this.props.origin[0];
		}

		return(
			<div 
			className="animated-piece tile" 
			style={{
				width: `${100 / this.props.boardSize}%`,
				height: `${100 / this.props.boardSize}%`,
				top: `${100 / this.props.boardSize * top}%`,
				left: `${100 / this.props.boardSize * left}%`}}>
				<Piece source={this.props.piece}/>
			</div>
		);
	}

	componentDidMount(){
		requestAnimationFrame(() => {
			requestAnimationFrame(() => {
				this.setState({ moving: true });
				setTimeout(() => {
					this.setState({ finished: true });	
				}, 500);
			});
		});

		requestAnimationFrame(() => requestAnimationFrame(() => this.setState({ moving: true })));
	}
}

