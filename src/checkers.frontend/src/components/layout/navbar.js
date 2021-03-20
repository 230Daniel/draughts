import React from "react";
import { Link } from "react-router-dom";
import { Navbar, Nav } from "react-bootstrap";

export default class MyNavbar extends React.Component{
	render(){
		return(
			<div>
				<Navbar variant="dark" expand="lg">
					<div className="container">
						<Navbar.Brand to="/" as={Link}>
							<img src="/checkers.png" width="30px" className="d-inline-block align-middle" alt=""/>
							Checkers
						</Navbar.Brand>
						<Navbar.Toggle aria-controls="my-navbar" />
						<Navbar.Collapse id="my-navbar">
							<Nav className="mr-auto">
								<Nav.Link to="/play" as={Link}>Play</Nav.Link>
							</Nav>
						</Navbar.Collapse>
					</div>
				</Navbar>
			</div>
		);
	}
}
