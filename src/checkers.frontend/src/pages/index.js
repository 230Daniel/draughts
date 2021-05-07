import React from "react";
import { Link } from "react-router-dom";
import Helmet from "react-helmet";

import "../styles/index.css";

export default class Index extends React.Component{
	render(){
		return(
			<>
				<Helmet>
					<title>Checkers</title>
				</Helmet>
				<div className="index-container">
					<div className="left">
						<img src="/checkers.png" alt=""/>
					</div>
					<div className="right">
						<div>
							<span className="title">Checkers</span><br/>
							<span className="subtitle">Because Chess would be too hard</span><br/>
							<Link className="subtitle link" to="/play">Find a Game ➔</Link>
						</div>
					</div>
				</div>
			</>
		);
	}
}
