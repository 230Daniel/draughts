import React from "react";
import Loader from "react-loader-spinner";

export default class Loading extends React.Component{
	render(){
		return(
			<div className="center">
				<Loader type="ThreeDots" color="white" height={80} width={80}/>
			</div>
			);
	}
}