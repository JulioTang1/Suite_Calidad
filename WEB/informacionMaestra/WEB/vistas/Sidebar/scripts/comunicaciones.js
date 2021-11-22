
Sidebar.comunicaciones = new Object();

// funcion de retorno que me indica el nivel de acceso a cada menu que tiene el usuario,
// si puede editar o solo puede visualizar
Sidebar.comunicaciones.Nivel_acceso = function(resultado){
	if(resultado.length > 0){
		Sidebar.funciones.Nivel_acceso(resultado);
	}
	else
	{
		Sidebar.gestionDOM.errorConexion();
	}
}

