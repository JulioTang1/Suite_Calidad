Permisos_Recurso.funciones = new Object();

Permisos_Recurso.funciones.crearPerfil = function(){

    gestionModal.alertaBloqueante(primario.aplicacion, 'Procesando...');

    query.callAjax(
	    Permisos_Recurso.urlConexionVista,
	    'Perfil_recurso',
	    "",
	    Permisos_Recurso.comunicaciones.crearPerfil
    );
}

Permisos_Recurso.funciones.checked = function(){
    var estado = $("#Apply_to_all").is(":checked");
    
    if(estado){

        $("#divForm .selectpicker").prop('disabled',true).selectpicker('refresh');
        $($("#divForm .selectpicker")[0]).prop('disabled',false).selectpicker('refresh');

        $($("#divForm .selectpicker")[0]).off("change", Permisos_Recurso.gestionDOM.selectores_perfil).on("change", Permisos_Recurso.gestionDOM.selectores_perfil);

        Permisos_Recurso.gestionDOM.selectores_perfil();
    }
    else
    {
        $("#divForm .selectpicker").prop('disabled',false).selectpicker('refresh');
        $($("#divForm .selectpicker")[0]).off("change", Permisos_Recurso.gestionDOM.selectores_perfil);
    }
}

Permisos_Recurso.funciones.RegistroNewPerfil = function(){

    // Variables para validar la información
    var BanderaError = 0;
    // Icono para cuando se produsca un error para mostrar en el mensaje
    var mensajeError = `<i class="material-icons md-14">error</i>`;
    var mensaje = "Ingrese un nombre";

    // Arreglos para enviar al servicio de registro -------------------------
    var recursos = new Array();
    var nivel_acceso = new Array();

    // Nombre del nuevo Perfil
    var Perfil = $("#N_perfil").val();

    // Se borra los mensajes de error y se quita la clase de error de los inputs
    $(".errorInput").removeClass("errorInput");
    $(".msnError").empty();

    // Validación de Perfil
    if((Perfil == null) || (Perfil == ''))
    {
        BanderaError = 1;

        $("#N_perfil").addClass("errorInput");
        $("#N_perfil").siblings(".msnError").html(`${mensajeError}<span> ${mensaje}</span>`); 
    }

    if(BanderaError == 0){

        var $contenedor = $(".container_Select");
        var $select;

        for (var x = 0; x < $contenedor.length; x++){
            
            $select = $contenedor.eq(x).find('select');
            recursos[x] = $select.data("id_recurso");
            nivel_acceso[x] = parseInt($select.val());
        }

        // Parametros que recibe el servicio
        var data = {
            perfil:         Perfil,
            recursos:       JSON.stringify(recursos),
            nivel_acceso:   JSON.stringify(nivel_acceso)
        };

        gestionModal.alertaBloqueante(primario.aplicacion, 'Procesando...');

        query.callAjax(
            Permisos_Recurso.urlConexionVista,
            'CrearPerfil',
            data,
            Permisos_Recurso.comunicaciones.RegistroNewPerfil,
            data
        );
    }
}

/*******************************************************************************************/ 

Permisos_Recurso.funciones.visualizar_RM = function(){

    Permisos_Recurso.gestionDOM.borrarPopover();
	Permisos_Recurso.gestionDOM.TiposMenus();

	$("#T_grupo").off().on("click", {DT: 'Permisos_Recurso_GRUPO'}, Permisos_Recurso.gestionDOM.table_MR_view);
	$("#T_menu").off().on("click", {DT: 'Permisos_Recurso_MENU'}, Permisos_Recurso.gestionDOM.table_MR_view);
	$("#T_submenu").off().on("click", {DT: 'Permisos_Recurso_SUBMENU'}, Permisos_Recurso.gestionDOM.table_MR_view);
}


Permisos_Recurso.funciones.Ed_permisos_menu = function(){

    Permisos_Recurso.gestionDOM.borrarPopover();
	Permisos_Recurso.gestionDOM.TiposMenus();

	$("#T_grupo").off().on("click", {DT: 'Permisos_Recurso_GRUPO'}, Permisos_Recurso.gestionDOM.table_MR_edit);
	$("#T_menu").off().on("click", {DT: 'Permisos_Recurso_MENU'}, Permisos_Recurso.gestionDOM.table_MR_edit);
	$("#T_submenu").off().on("click", {DT: 'Permisos_Recurso_SUBMENU'}, Permisos_Recurso.gestionDOM.table_MR_edit);
}

/*******************************************************************************************/ 
/*******************************************************************************************/ 

Permisos_Recurso.funciones.clickSelected = function(event){
    Permisos_Recurso.eventoSelected = event;
}

/*******************************************************************************************/ 
/*******************************************************************************************/ 
/*******************************************************************************************/ 

Permisos_Recurso.funciones.Editar_Perfil = function(){

    gestionModal.alertaBloqueante(primario.aplicacion, 'Procesando...');
    Permisos_Recurso.gestionDOM.borrarPopover();

    var id_PPR = parseInt($(Permisos_Recurso.ultimoElementoConPopoever)[0].attributes["id"].value);
    
    data = {
        id_PPR: id_PPR
    };

    query.callAjax(
	    Permisos_Recurso.urlConexionVista,
	    'Perfil_NA',
	    data,
	    Permisos_Recurso.comunicaciones.Editar_Perfil
    );
}

Permisos_Recurso.funciones.upd_P = function(){

    var id_PPR = parseInt($(Permisos_Recurso.ultimoElementoConPopoever)[0].attributes["id"].value);
    var id_NA = parseInt($("#S_NA").val());

    gestionModal.alertaBloqueante(primario.aplicacion, 'Procesando...');
    
    data = {
        id_PPR: id_PPR,
        id_NA:  id_NA
    };

    query.callAjax(
	    Permisos_Recurso.urlConexionVista,
	    'upd_P',
	    data,
	    Permisos_Recurso.gestionDOM.UPD_Success
    ); 
}

Permisos_Recurso.funciones.updatePerfilMsv = function(){

    gestionModal.alertaBloqueante(primario.aplicacion, 'Procesando...');
    Permisos_Recurso.gestionDOM.borrarPopover();

    query.callAjax(
		Permisos_Recurso.urlConexionVista,
	    'Perfil_msv',
	    '',
	    Permisos_Recurso.comunicaciones.updatePerfilMsv
    );
}

Permisos_Recurso.funciones.upd_P_msv = function(){

    var id_NA = parseInt($("#S_NA").val());

	gestionModal.alertaBloqueante(primario.aplicacion, 'Procesando...');
	
	var data = new FormData();

	var info = Permisos_Recurso.$tabla.ajax.params();
	
	data.append("id_S", id_NA);
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
        Permisos_Recurso.urlConexionVista,
	    "upd_P_msv",
	    data,
	    Permisos_Recurso.gestionDOM.UPD_Success
	);   
}

/*******************************************************************************************/ 
/*******************************************************************************************/ 
/*******************************************************************************************/ 
