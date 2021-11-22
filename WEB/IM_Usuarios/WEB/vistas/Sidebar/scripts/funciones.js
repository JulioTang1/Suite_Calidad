
Sidebar.funciones = new Object();

// dependiendo el rol que tenga el usuario se carga las opciones del sidebar dinamicamente y se les da un
// acceso, editar o visualizar dependiendo el usuario
Sidebar.funciones.SidebarDynamic = function() {

	var data = {
		id_rol: Login.id_rol
	};

	query.callAjax(
		Sidebar.urlConexion,
		"acceso_menu",
		data,
		Sidebar.comunicaciones.Nivel_acceso
	);
}

// se guarda en dos vectores el id del menu y el nivel de acceso que tiene el usuario a ese menu
Sidebar.funciones.Nivel_acceso = function(resultado){

	for(x in resultado){
		Sidebar.menus_ID[x] = resultado[x].id_menu;
		Sidebar.Acceso[x]	= resultado[x].id_acceso;
	}

	var data = {
		id_app:	Login.id_app,
		id_rol: Login.id_rol
	};

	query.callAjax(
		Sidebar.urlConexion, 
		"sidebar_dinamico", 
		data, 
		Sidebar.gestionDOM.SidebarDynamic);
}

Sidebar.funciones.T_menu = function(menu_id){
	var data = {
		id_menu: menu_id
	}
	query.callAjax(Sidebar.urlConexionSession,"InsertSessionMenu",data,function(){});
}


Sidebar.funciones.NombreVistaTopBar = function(menu){
	var message = {
		mensaje: 	"TopBar",
		name_menu: 	menu
	};
	primario.window_source.postMessage(message,primario.window_origin);

}



