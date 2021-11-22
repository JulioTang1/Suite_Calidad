Permisos_Recurso.comunicaciones = new Object();

Permisos_Recurso.comunicaciones.crearPerfil = function(data){
	Permisos_Recurso.gestionDOM.crearPerfil(data);
}

Permisos_Recurso.comunicaciones.RegistroNewPerfil = function(data, Info){
	if(data == 1){
		Permisos_Recurso.gestionDOM.RegistroNewPerfil();
	}
	else
	{
		gestionModal.alertaConfirmacionHTML2(
			primario.aplicacion, 
			`El Perfil que ingreso ya existe`, 
			'warning', 
			'Aceptar', 
			'#f8bb86', 
			Permisos_Recurso.gestionDOM.Perfil_existe,
			Info
		);
	}
}

/************************************************************************************************/ 

Permisos_Recurso.comunicaciones.Editar_Perfil = function(data){
	Permisos_Recurso.gestionDOM.Editar_Perfil(data);
}

Permisos_Recurso.comunicaciones.updatePerfilMsv = function(data){
	Permisos_Recurso.gestionDOM.updatePerfilMsv(data);
}

/************************************************************************************************/ 