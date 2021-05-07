import React from "react";

export default class Piece extends React.Component{
	constructor(props){
		super(props)
	}

	render(){
		return (
			<div className={`piece ${this.props.source.colour === 0 ? "piece-white" : "piece-black"}`}>
				<div className="piece-inside"/>
			</div>
		);
	}
}

