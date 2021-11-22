Estado_Usuario.comunicaciones = new Object();

Estado_Usuario.comunicaciones.CargarEstado_Usuario = function(data){

	if(data.length > 0){
		Estado_Usuario.gestionDOM.CargarEstado_Usuario(data);
	}
}

/*******************************************************************************************************/ 

Estado_Usuario.comunicaciones.EditarUsuario = function(data){
	Estado_Usuario.gestionDOM.EditarUsuario(data);
}

Estado_Usuario.comunicaciones.updateStateUserMsv = function(data){
	Estado_Usuario.gestionDOM.updateStateUserMsv(data);
}
