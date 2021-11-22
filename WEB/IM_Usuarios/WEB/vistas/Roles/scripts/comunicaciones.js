Roles.comunicaciones = new Object();

Roles.comunicaciones.CrearRol = function(data){
	Roles.gestionDOM.CrearRol(data);
}

Roles.comunicaciones.RegistroNewRol = function(data, Info){
	if(data == 1){
		Roles.gestionDOM.RegistroNewRol();
	}
	else
	{
		gestionModal.alertaConfirmacionHTML2(
			primario.aplicacion, 
			`El Rol que ingreso ya existe`, 
			'warning', 
			'Aceptar', 
			'#f8bb86', 
			Roles.gestionDOM.Rol_existe,
			Info
		);
	}
}

Roles.comunicaciones.Editar_Modulo = function(data){
	Roles.gestionDOM.Editar_Modulo(data);
}

Roles.comunicaciones.updateRolAppMsv = function(data){
	Roles.gestionDOM.updateRolAppMsv(data);
}

/************************************************************************************************/ 

Roles.comunicaciones.Editar_Perfil = function(data){
	Roles.gestionDOM.Editar_Perfil(data);
}

Roles.comunicaciones.updateRolPerfilMsv = function(data){
	Roles.gestionDOM.updateRolPerfilMsv(data);
}

/************************************************************************************************/ 

Roles.comunicaciones.Editar_Menu = function(data, info){
	Roles.gestionDOM.Editar_Menu(data, info);
}

Roles.comunicaciones.updateRolMenuMsv = function(data, DM){
	Roles.gestionDOM.updateRolMenuMsv(data, DM);
}