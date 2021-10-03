import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter as Router, Switch, Route } from "react-router-dom";

import Layout from './components/layout';

import Index from "./pages/index";
import Game from "./pages/game";
import Play from './pages/play';
import CreateGame from './pages/create-game';
import JoinGame from "./pages/join-game";
import About from "./pages/about";

ReactDOM.render(
	<Router>
		<Layout>
			<Switch>
				<Route exact path="/" component={Index}/>
				<Route exact path="/play" component={Play}/>
				<Route exact path="/create-game" component={CreateGame}/>
				<Route exact path="/join-game" component={JoinGame}/>
				<Route exact path="/join-game/:gameCode" component={JoinGame}/>
				<Route exact path="/game/:gameCode" component={Game}/>
				<Route exact path="/about" component={About}/>
			</Switch>
		</Layout>
	</Router>,
	document.getElementById('root')
);
