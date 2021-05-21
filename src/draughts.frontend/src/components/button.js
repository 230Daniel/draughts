import React from "react";
import { Link } from "react-router-dom";

import "../styles/button.css";

export default class Button extends React.Component{
	render(){
		if(this.props.to){
			return (
				<Link className="button" to={this.props.to} style={{width: this.props.width}}>
					{this.props.children}
				</Link>
			);
		}
		return (
			<div className="button" style={{width: this.props.width}}>
				{this.props.children}
			</div>
		);
	}

	renderContent(){
		
	}
}