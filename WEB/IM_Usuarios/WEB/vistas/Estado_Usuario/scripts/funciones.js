Estado_Usuario.funciones = new Object();

/*******************************************************************************************************/ 

Estado_Usuario.funciones.clickSelected = function(event){
    Estado_Usuario.eventoSelected = event;
}


Estado_Usuario.funciones.EditarUsuario = function(){

    gestionModal.alertaBloqueante(primario.aplicacion, 'Procesando...');
    Estado_Usuario.gestionDOM.borrarPopover();

    var id_EU = parseInt($(Estado_Usuario.ultimoElementoConPopoever)[0].attributes["id"].value);
    
    data = {
        id_EU: id_EU
    };

    query.callAjax(
	    Estado_Usuario.urlConexionVista,
	    'Rol_Estado',
	    data,
	    Estado_Usuario.comunicaciones.EditarUsuario
    );
}

Estado_Usuario.funciones.upd_EU = function(){

    var id_EU = parseInt($(Estado_Usuario.ultimoElementoConPopoever)[0].attributes["id"].value);
    var id_rol = parseInt($("#S_Rol").val());
    var id_estado = parseInt($("#S_Estado").val());
    
    data = {
        id_EU:      		id_EU,
        id_rol:     		id_rol,
        id_estado:  		id_estado,
		app_habilitada: 	$("#app").prop("checked") ? 1 : 0,
		contrasena_app: 	$("#contrasena").val()
    };

	gestionModal.alertaBloqueante(primario.aplicacion, 'Procesando...');

    query.callAjax(
	    Estado_Usuario.urlConexionVista,
	    'upd_EU',
	    data,
	    Estado_Usuario.gestionDOM.UPD_Success
    );
}

/*********************************************************************************************************/ 

Estado_Usuario.funciones.updateStateUser = function(){

    gestionModal.alertaBloqueante(primario.aplicacion, "Procesando...");
    
    var data = {
        id_app: Estado_Usuario.id_aplicacion
    }

	query.callAjax(
		Estado_Usuario.urlConexionVista, 
		"updateStateUser", 
		data,
		Estado_Usuario.gestionDOM.UPD_Success
	);
}

/*********************************************************************************************************/

Estado_Usuario.funciones.updateStateUserMsv = function(){

    gestionModal.alertaBloqueante(primario.aplicacion, 'Procesando...');
    Estado_Usuario.gestionDOM.borrarPopover();

    query.callAjax(
		Estado_Usuario.urlConexionVista,
	    'Rol_Estado_msv',
	    '',
	    Estado_Usuario.comunicaciones.updateStateUserMsv
    );
}

/*******************************************************************************************/
//Metodo para obtener la información de la tabla y hacer la petición para hacer
// el archivo de excel
Estado_Usuario.funciones.Excel_user = function(){

	gestionModal.alertaBloqueante(primario.aplicacion, "Procesando...");

	query.callAjax(
		Estado_Usuario.urlConexionVista, 
		"excel_info", 
		"",
		Estado_Usuario.funciones.queryStateExcel
	);
}

// se manda la consulta para verificar el estado del excel de la tabla D_Registro_Excel
Estado_Usuario.funciones.queryStateExcel = function(resultado){

	var data = {
		id: 	resultado[0].id
	}

	query.callAjax(
		Estado_Usuario.urlConexion,
		'excelState',
		data,
		Estado_Usuario.funciones.excelState
	);
}

// Cuando se consulta la tabla de registro, dependiendo si falla para, detiene el metodo que manda la consulta
// a la tabla de registro y le muestra el mensaje al usuario
// Fallo 
// true --> Fallo la generación del excel
// false --> No fallo o no ha fallado la generación del excel
// Estado 
// true --> Se esta generando el excel
// false --> Se termino de generar el excel
Estado_Usuario.funciones.excelState = function(resultado){
	if(resultado[0].Fallo == true){
		Estado_Usuario.funciones.ErrorConsulta();
	}
	else if(resultado[0].Estado == false)
	{
		Estado_Usuario.funciones.generarExcel(resultado[0].Nombre);
	}
	// Se retorna el id del registro que se acabo de generar para consulta en un 
	// intervalo de 250 ms si el archivo ya se genero
	else
	{
		setTimeout(Estado_Usuario.funciones.queryStateExcel, 250, resultado);
	}
}

// cuando se crea el archivo en el servidor, esta función se crea para poder descargarlo
// en el cliente
Estado_Usuario.funciones.generarExcel = function(nombre){
	//se crea una etiqueta a
	var downloadLink = document.createElement("a");
	//en el atributo href se le asigna la url de descarga
	downloadLink.href = "/apiServices/Registro_InfoUser/" + nombre;
	//se le da el link de descarga //en esta linea se cambian los datos de descarga
	downloadLink.download = nombre;
	//se añade al documento html la etiqueta a
	document.body.appendChild(downloadLink);
	//timeToLdr(false, 'bgInitLdr');     
	// se clickea esa etiqueta
	downloadLink.click();
	//se remueve el link de la etiqueta
	document.body.removeChild(downloadLink);
	gestionModal.cerrar();
}

Estado_Usuario.funciones.ErrorConsulta = function(){
	gestionModal.cerrar();	
	gestionModal.alertaConfirmacion(
		primario.aplicacion,
		"Ha ocurrido un error al generar el archivo, Vuelva a intentarlo más tarde",
		"error",
		"Ok",
		"#f27474",
		function(){}
	);
}

/*******************************************************************************************/

Estado_Usuario.funciones.upd_EU_msv = function(){

    var id_rol = parseInt($("#S_Rol").val());
    var id_estado = parseInt($("#S_Estado").val());

	gestionModal.alertaBloqueante(primario.aplicacion, 'Procesando...');
	
	var data = new FormData();

	var info = Estado_Usuario.$tabla.ajax.params();
	
	data.append("id_rol", id_rol);
	data.append("id_estado", id_estado);
	data.append("draw", info.draw);
	data.append("length", info.length);
	data.append("n_col", info.n_col);
	data.append("nombre", info.nombre);	
	data.append("replace", gestionDT.replace);
	data.append("start", info.start);

	for(x in info.columns)
	{
		data.append(`columns[${x}][data]`,info.columns[x].data);
		data.append(`columns[${x}][name]`,info.columns[x].name);
		data.append(`columns[${x}][orderable]`,info.columns[x].orderable);
		data.append(`columns[${x}][search][regex]`,info.columns[x].search.regex);
		data.append(`columns[${x}][search][value]`,info.columns[x].search.value);
		data.append(`columns[${x}][searchable]`,info.columns[x].searchable);
	}
	data.append("order[0][column]", info.order[0].column);
	data.append("order[0][dir]", info.order[0].dir);
	data.append("search[value]", info.search.value);
	data.append("search[regex]", info.search.regex);

	query.callAjaxDataTablesExcel(
		Estado_Usuario.urlConexionVista,
	    'upd_EU_msv',
	    data,
	    Estado_Usuario.gestionDOM.UPD_Success
	);
}