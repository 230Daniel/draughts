import React from "react";
import { Link } from "react-router-dom";

class Footer extends React.Component{
	render(){
		return(
			<div className="footer">
				<div className="container">
					<div className="footer-left">
						© Daniel Baynton 2021 <br/>
					</div>
					<div className="footer-right">
						<Link className="link" to="/about">About</Link>
					</div>
				</div>
			</div>
		);
	}
}

export default Footer;
