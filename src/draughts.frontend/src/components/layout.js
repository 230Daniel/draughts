import React from "react";

import Navbar from "./layout/navbar";
import Footer from "./layout/footer";

import "../styles/layout.css";

export default class Layout extends React.Component{
	render(){
		return(
			<>
				<main>
					<Navbar/>
					{this.props.children}
				</main>
				<footer>
					<Footer/>
				</footer>
			</>
		);
	}
}
