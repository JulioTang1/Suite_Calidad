Roles.funciones = new Object();

Roles.funciones.CrearRol = function(){

    gestionModal.alertaBloqueante(primario.aplicacion, 'Procesando...');

    query.callAjax(
	    Roles.urlConexionVista,
	    'Perfiles',
	    "",
	    Roles.comunicaciones.CrearRol
    );
}


Roles.funciones.RegistroNewRol = function(){

    // Variables para validar la información
    var BanderaError = 0;
    // Icono para cuando se produsca un error para mostrar en el mensaje
    var mensajeError = `<i class="material-icons md-14">error</i>`;
    var mensaje = "Ingrese un nombre";

    // Nombre del nuevo Rol
    var Rol = $("#N_rol").val();
    var Perfil = parseInt($("#N_perfil").val());

    // Se borra los mensajes de error y se quita la clase de error de los inputs
    $(".errorInput").removeClass("errorInput");
    $(".msnError").empty();

    // Validación de Rol
    if((Rol == null) || (Rol == ''))
    {
        BanderaError = 1;

        $("#N_rol").addClass("errorInput");
        $("#N_rol").siblings(".msnError").html(`${mensajeError}<span> ${mensaje}</span>`); 
    }

    if(BanderaError == 0){


        // Parametros que recibe el servicio
        var data = {
            rol:        Rol,
            id_perfil:  Perfil
        };

        gestionModal.alertaBloqueante(primario.aplicacion, 'Procesando...');

        query.callAjax(
            Roles.urlConexionVista,
            'CrearRol',
            data,
            Roles.comunicaciones.RegistroNewRol,
            data
        );
    }
}

/*******************************************************************************************/ 

Roles.funciones.visualizar_RM = function(){

    Roles.gestionDOM.borrarPopover();
	Roles.gestionDOM.TiposMenus();

	$("#T_grupo").off().on("click", {DT: 'ROLES_GRUPO'}, Roles.gestionDOM.table_MR_view);
	$("#T_menu").off().on("click", {DT: 'ROLES_MENU'}, Roles.gestionDOM.table_MR_view);
	$("#T_submenu").off().on("click", {DT: 'ROLES_SUBMENU'}, Roles.gestionDOM.table_MR_view);
}


Roles.funciones.Ed_permisos_menu = function(){

    Roles.gestionDOM.borrarPopover();
	Roles.gestionDOM.TiposMenus();

	$("#T_grupo").off().on("click", {DT: 'ROLES_GRUPO'}, Roles.gestionDOM.table_MR_edit);
	$("#T_menu").off().on("click", {DT: 'ROLES_MENU'}, Roles.gestionDOM.table_MR_edit);
	$("#T_submenu").off().on("click", {DT: 'ROLES_SUBMENU'}, Roles.gestionDOM.table_MR_edit);
}

/*******************************************************************************************/ 
/*******************************************************************************************/ 

Roles.funciones.clickSelected = function(event){
    Roles.eventoSelected = event;
}

/*******************************************************************************************/ 
/*******************************************************************************************/ 
/*******************************************************************************************/ 

Roles.funciones.Editar_Modulo = function(){

    gestionModal.alertaBloqueante(primario.aplicacion, 'Procesando...');
    Roles.gestionDOM.borrarPopover();

    var id_RA = parseInt($(Roles.ultimoElementoConPopoever)[0].attributes["id"].value);
    
    data = {
        id_RA: id_RA
    };

    query.callAjax(
	    Roles.urlConexionVista,
	    'Rol_Modulo',
	    data,
	    Roles.comunicaciones.Editar_Modulo
    );

}

Roles.funciones.upd_RA = function(){

    var id_RA = parseInt($(Roles.ultimoElementoConPopoever)[0].attributes["id"].value);
    var id_NA = parseInt($("#S_NA").val());

    gestionModal.alertaBloqueante(primario.aplicacion, 'Procesando...');
    
    data = {
        id_RA:  id_RA,
        id_NA:  id_NA
    };

    query.callAjax(
	    Roles.urlConexionVista,
	    'upd_RA',
	    data,
	    Roles.gestionDOM.UPD_Success
    ); 
}

Roles.funciones.updateRolAppMsv = function(){

    gestionModal.alertaBloqueante(primario.aplicacion, 'Procesando...');
    Roles.gestionDOM.borrarPopover();

    query.callAjax(
		Roles.urlConexionVista,
	    'Rol_modulo_msv',
	    '',
	    Roles.comunicaciones.updateRolAppMsv
    );
}

Roles.funciones.upd_RA_msv = function(){

    var id_NA = parseInt($("#S_NA").val());

	gestionModal.alertaBloqueante(primario.aplicacion, 'Procesando...');
	
	var data = new FormData();

	var info = Roles.$tabla.ajax.params();
	
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
        Roles.urlConexionVista,
	    "upd_RA_msv",
	    data,
	    Roles.gestionDOM.UPD_Success
	);   
}
/*******************************************************************************************/ 
/*******************************************************************************************/ 
/*******************************************************************************************/ 

Roles.funciones.Editar_Perfil = function(){

    gestionModal.alertaBloqueante(primario.aplicacion, 'Procesando...');
    Roles.gestionDOM.borrarPopover();

    var id_RP = parseInt($(Roles.ultimoElementoConPopoever)[0].attributes["id"].value);
    
    data = {
        id_RP: id_RP
    };

    query.callAjax(
	    Roles.urlConexionVista,
	    'Rol_Perfil',
	    data,
	    Roles.comunicaciones.Editar_Perfil
    );
}

Roles.funciones.upd_RP = function(){

    var id_RP = parseInt($(Roles.ultimoElementoConPopoever)[0].attributes["id"].value);
    var id_perfil = parseInt($("#S_Perfil").val());

    gestionModal.alertaBloqueante(primario.aplicacion, 'Procesando...');
    
    data = {
        id_RP:      id_RP,
        id_perfil:  id_perfil
    };

    query.callAjax(
	    Roles.urlConexionVista,
	    'upd_RP',
	    data,
	    Roles.gestionDOM.UPD_Success
    ); 
}

Roles.funciones.updateRolPerfilMsv = function(){

    gestionModal.alertaBloqueante(primario.aplicacion, 'Procesando...');
    Roles.gestionDOM.borrarPopover();

    query.callAjax(
		Roles.urlConexionVista,
	    'Rol_perfil_msv',
	    '',
	    Roles.comunicaciones.updateRolPerfilMsv
    );
}

Roles.funciones.upd_RP_msv = function(){

    var id_Perfil = parseInt($("#S_Perfil").val());

	gestionModal.alertaBloqueante(primario.aplicacion, 'Procesando...');
	
	var data = new FormData();

	var info = Roles.$tabla.ajax.params();
	
	data.append("id_S", id_Perfil);
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
        Roles.urlConexionVista,
	    "upd_RP_msv",
	    data,
	    Roles.gestionDOM.UPD_Success
	);   
}

/*******************************************************************************************/ 
/*******************************************************************************************/ 
/*******************************************************************************************/ 

Roles.funciones.Editar_Menu = function(ev){

    gestionModal.alertaBloqueante(primario.aplicacion, 'Procesando...');
    Roles.gestionDOM.borrarPopover();

    var DT = ev.data.DT;

    var $pop = $(Roles.ultimoElementoConPopoever)[0];
    var $fila = $($pop).find("td");

    var id_DT = parseInt($pop.attributes["id"].value);
    var L_DT = $fila.length;

    var rol = $fila[0].outerText;
    var cat = $fila[1].outerText;
    var app = $fila[2].outerText;

    var NA = $fila[L_DT - 1].outerText;

    var info = {
        id_DT:  id_DT,
        rol:    rol,
        cat:    cat,
        app:    app,
        NA:     NA,
        DT:     DT
    };

    query.callAjax(
	    Roles.urlConexionVista,
	    "Rol_NA",
	    "",
        Roles.comunicaciones.Editar_Menu,
        info
    );
}

Roles.funciones.upd_RM = function(ev){

    var info = ev.data;
    var id_NA = parseInt($("#S_NA").val());
    var metodo;
    var data;

    gestionModal.alertaBloqueante(primario.aplicacion, 'Procesando...');

    switch(info.DT){
        case "ROLES_GRUPO":
            metodo = "upd_RM_Grupo";
            data = {
                id_grupo:   info.id_DT,
                rol:        info.rol,
                cat:        info.cat,
                app:        info.app,
                id_NA:      id_NA
            };
        break;

        case "ROLES_MENU":
            metodo = "upd_RM_Menu";
            data = {
                id_menu:    info.id_DT,
                rol:        info.rol,
                cat:        info.cat,
                app:        info.app,
                id_NA:      id_NA
            };
        break;

        default:
            metodo = "upd_RM_Submenu";
            data = {
                id_PRM:     info.id_DT,
                id_NA:      id_NA
            };
        break;
    }

    query.callAjax(
	    Roles.urlConexionVista,
	    metodo,
	    data,
	    Roles.gestionDOM.UPD_Success
    ); 
}

Roles.funciones.updateRolMenuMsv = function(ev){

    var DM = ev.data.DM;

    gestionModal.alertaBloqueante(primario.aplicacion, 'Procesando...');
    Roles.gestionDOM.borrarPopover();

    query.callAjax(
		Roles.urlConexionVista,
	    'Rol_NA',
	    '',
        Roles.comunicaciones.updateRolMenuMsv,
        DM
    );
}

Roles.funciones.upd_RM_msv = function(ev){
    
    var DM = ev.data.DM;
    var metodo;

    var id_NA = parseInt($("#S_NA").val());

	gestionModal.alertaBloqueante(primario.aplicacion, 'Procesando...');
	
	var data = new FormData();

	var info = Roles.$tabla.ajax.params();
	
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
    
    if(DM == 1){
        metodo = 'upd_RM_msv_Grupo';
    }
    else if (DM == 2){
        metodo = 'upd_RM_msv_Menu';
    }
    else
    {
        metodo = 'upd_RM_msv_Submenu';
    }

	query.callAjaxDataTablesExcel(
        Roles.urlConexionVista,
	    metodo,
	    data,
	    Roles.gestionDOM.UPD_Success
	);   
}