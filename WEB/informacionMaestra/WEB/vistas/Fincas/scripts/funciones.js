Fincas.funciones = new Object();

/*******************************POPOVER*************************************/
//Captura el evento click sobre la tabla y muestra popoever
Fincas.funciones.clickSelected = function(event){
    Fincas.eventoSelected = event;
}
/*****************************FIN*POPOVER***********************************/

//AJAX para la tabla
Fincas.funciones.genTable = function(){
    gestionDT.initTable(
    	Fincas.urlConexion,
    	"Fincas", 
    	"#tablaFincas", 
    	Fincas.comunicaciones.tablaCargada,
    	0
    );
}

//AJAX para traer la informacion de selectores del formulario
Fincas.funciones.bringSelects = function(){
    query.callAjax(
        Fincas.urlConexionVista,
        "select_form",
        0,
        Fincas.comunicaciones.bringSelects);
}

//AJAX para traer los municipios de un departamento
Fincas.funciones.bringMunicipios = function(){
    var data = {
        id:             $("#selectDepartamento").val() == "" ? 0 : $("#selectDepartamento").val()
    };
    query.callAjax(
        Fincas.urlConexionVista,
        "bring_municipios",
        data,
        Fincas.comunicaciones.bringMunicipios);
}

//Funcion encargada de realizas una primera validacion de los campos
Fincas.funciones.validacionUsuario = function(){
    var error = 0;

    //Se limpian los divs de mensaje invalido
    $("#invalidFeedback_codigo").empty();
    $("#invalidFeedback_nombre").empty();
    $("#invalidFeedback_selectDepartamento").empty();
    $("#invalidFeedback_selectMunicipio").empty();
    $("#invalidFeedback_selectSector").empty();
    $("#invalidFeedback_selectGrupo").empty();
    $("#invalidFeedback_selectEstado").empty();
    $("#invalidFeedback_latitud").empty();
    $("#invalidFeedback_longitud").empty();
    $("#invalidFeedback_hectareas").empty();
    $("#invalidFeedback_selectOrganica").empty();

    $(".errorInput").removeClass('errorInput');

    var icono = '<i class="material-icons md-14">error</i>';
    
    //Codigo
    if( $("#codigo").val() == "" || $("#codigo").val().length < 4){
        var errorMessage = `${icono}<span> Ingrese un código de cuatro dígitos. </span>`;
        Fincas.gestionDOM.mensajeError('#invalidFeedback_codigo',errorMessage);
        $("#invalidFeedback_codigo").addClass("msnError");
        $('#codigo').addClass('errorInput');
        error = 1;
    }

    //nombre
    if( $("#nombre").val() == "" ){
        var errorMessage = `${icono}<span> Ingrese un nombre. </span>`;
        Fincas.gestionDOM.mensajeError('#invalidFeedback_nombre',errorMessage);
        $("#invalidFeedback_nombre").addClass("msnError");
        $('#nombre').addClass('errorInput');
        error = 1;
    }

    //Selector departamento
    if( $("#selectDepartamento").val() == "" || $("#selectDepartamento").val() == null ){
        var errorMessage = `${icono}<span> Seleccione un departamento. </span>`;
        Fincas.gestionDOM.mensajeError('#invalidFeedback_selectDepartamento',errorMessage);
        $("#invalidFeedback_selectDepartamento").addClass("msnError");
        $('[data-id="selectDepartamento"]').addClass('errorInput');
        error = 1;
    }

    //Selector municipio
    if( $("#selectMunicipio").val() == "" || $("#selectMunicipio").val() == null ){
        var errorMessage = `${icono}<span> Seleccione un municipio. </span>`;
        Fincas.gestionDOM.mensajeError('#invalidFeedback_selectMunicipio',errorMessage);
        $("#invalidFeedback_selectMunicipio").addClass("msnError");
        $('[data-id="selectMunicipio"]').addClass('errorInput');
        error = 1;
    }

    //Selector sector
    if( $("#selectSector").val() == "" || $("#selectSector").val() == null ){
        var errorMessage = `${icono}<span> Seleccione un sector. </span>`;
        Fincas.gestionDOM.mensajeError('#invalidFeedback_selectSector',errorMessage);
        $("#invalidFeedback_selectSector").addClass("msnError");
        $('[data-id="selectSector"]').addClass('errorInput');
        error = 1;
    }

    //Selector grupo
    if( $("#selectGrupo").val() == "" || $("#selectGrupo").val() == null ){
        var errorMessage = `${icono}<span> Seleccione un grupo. </span>`;
        Fincas.gestionDOM.mensajeError('#invalidFeedback_selectGrupo',errorMessage);
        $("#invalidFeedback_selectGrupo").addClass("msnError");
        $('[data-id="selectGrupo"]').addClass('errorInput');
        error = 1;
    }

    //Selector estado
    if( $("#selectEstado").val() == "" || $("#selectEstado").val() == null ){
        var errorMessage = `${icono}<span> Seleccione un estado. </span>`;
        Fincas.gestionDOM.mensajeError('#invalidFeedback_selectEstado',errorMessage);
        $("#invalidFeedback_selectEstado").addClass("msnError");
        $('[data-id="selectEstado"]').addClass('errorInput');
        error = 1;
    }

    //Latitud
    if( $("#latitud").val() == "" ){
        var errorMessage = `${icono}<span> Ingrese un latitud. </span>`;
        Fincas.gestionDOM.mensajeError('#invalidFeedback_latitud',errorMessage);
        $("#invalidFeedback_latitud").addClass("msnError");
        $('#latitud').addClass('errorInput');
        error = 1;
    }

    //Longitud
    if( $("#longitud").val() == "" ){
        var errorMessage = `${icono}<span> Ingrese un longitud. </span>`;
        Fincas.gestionDOM.mensajeError('#invalidFeedback_longitud',errorMessage);
        $("#invalidFeedback_longitud").addClass("msnError");
        $('#longitud').addClass('errorInput');
        error = 1;
    }

    //Hectareas
    if( $("#hectareas").val() == "" ){
        var errorMessage = `${icono}<span> Ingrese un valor de hectáreas. </span>`;
        Fincas.gestionDOM.mensajeError('#invalidFeedback_hectareas',errorMessage);
        $("#invalidFeedback_hectareas").addClass("msnError");
        $('#hectareas').addClass('errorInput');
        error = 1;
    }

    //Latitud
    if( $("#latitud").val() == "0" || $("#latitud").val() == "-0" ){
        var errorMessage = `${icono}<span> La latitud no puede ser 0. </span>`;
        Fincas.gestionDOM.mensajeError('#invalidFeedback_latitud',errorMessage);
        $("#invalidFeedback_latitud").addClass("msnError");
        $('#latitud').addClass('errorInput');
        error = 1;
    }

    //Longitud
    if( $("#longitud").val() == "0" || $("#longitud").val() == "-0" ){
        var errorMessage = `${icono}<span> La longitud no puede ser 0. </span>`;
        Fincas.gestionDOM.mensajeError('#invalidFeedback_longitud',errorMessage);
        $("#invalidFeedback_longitud").addClass("msnError");
        $('#longitud').addClass('errorInput');
        error = 1;
    }

    //Hectareas
    if( $("#hectareas").val() == "0" || $("#hectareas").val() == "-0" ){
        var errorMessage = `${icono}<span> El valor de hectáreas no puede ser 0. </span>`;
        Fincas.gestionDOM.mensajeError('#invalidFeedback_hectareas',errorMessage);
        $("#invalidFeedback_hectareas").addClass("msnError");
        $('#hectareas').addClass('errorInput');
        error = 1;
    }

    //Selector organica
    if( $("#selectOrganica").val() == "" || $("#selectOrganica").val() == null ){
        var errorMessage = `${icono}<span> Indique si la finca es orgánica. </span>`;
        Fincas.gestionDOM.mensajeError('#invalidFeedback_selectOrganica',errorMessage);
        $("#invalidFeedback_selectOrganica").addClass("msnError");
        $('[data-id="selectOrganica"]').addClass('errorInput');
        error = 1;
    }

    //Entra solo si las cajas estan llenas
    if(error == 0){
            var data = {
                codigo:         $("#codigo").val(),
                nombre:         $("#nombre").val(),
                municipio:      $("#selectMunicipio").val(),
                sector:         $("#selectSector").val(),
                grupo:          $("#selectGrupo").val(),
                estado:         $("#selectEstado").val(),
                latitud:        $("#latitud").val().replaceAll('.', '').replaceAll(',','.'),
                longitud:       $("#longitud").val().replaceAll('.', '').replaceAll(',','.'),
                hectareas:      $("#hectareas").val().replaceAll('.', '').replaceAll(',','.'),
                organica:       $("#selectOrganica").val()
            };
            query.callAjax(
                Fincas.urlConexionVista,
                "insert_finca",
                data,
                Fincas.comunicaciones.insertFinca);
    }
}

//AJAX para traer la informacion de selectores del formulario
Fincas.funciones.bringSelectsEdit = function(){
	//Se borra el popover
	Fincas.gestionDOM.borrarPopover();
    query.callAjax(
        Fincas.urlConexionVista,
        "select_form",
        0,
        Fincas.comunicaciones.bringSelectsEdit);
}

//Funcion encargada de realizas una primera validacion de los campos
Fincas.funciones.validacionUsuarioEdit = function(){
    var error = 0;

    //Se limpian los divs de mensaje invalido
    $("#invalidFeedback_codigo").empty();
    $("#invalidFeedback_nombre").empty();
    $("#invalidFeedback_selectDepartamento").empty();
    $("#invalidFeedback_selectMunicipio").empty();
    $("#invalidFeedback_selectSector").empty();
    $("#invalidFeedback_selectGrupo").empty();
    $("#invalidFeedback_selectEstado").empty();
    $("#invalidFeedback_latitud").empty();
    $("#invalidFeedback_longitud").empty();
    $("#invalidFeedback_hectareas").empty();
    $("#invalidFeedback_selectOrganica").empty();

    $(".errorInput").removeClass('errorInput');

    var icono = '<i class="material-icons md-14">error</i>';

    //Codigo
    if( $("#codigo").val() == "" || $("#codigo").val().length < 4){
        var errorMessage = `${icono}<span> Ingrese un código de cuatro dígitos. </span>`;
        Fincas.gestionDOM.mensajeError('#invalidFeedback_codigo',errorMessage);
        $("#invalidFeedback_codigo").addClass("msnError");
        $('#codigo').addClass('errorInput');
        error = 1;
    }

    //nombre
    if( $("#nombre").val() == "" ){
        var errorMessage = `${icono}<span> Ingrese un nombre. </span>`;
        Fincas.gestionDOM.mensajeError('#invalidFeedback_nombre',errorMessage);
        $("#invalidFeedback_nombre").addClass("msnError");
        $('#nombre').addClass('errorInput');
        error = 1;
    }

    //Selector departamento
    if( $("#selectDepartamento").val() == "" || $("#selectDepartamento").val() == null ){
        var errorMessage = `${icono}<span> Seleccione un departamento. </span>`;
        Fincas.gestionDOM.mensajeError('#invalidFeedback_selectDepartamento',errorMessage);
        $("#invalidFeedback_selectDepartamento").addClass("msnError");
        $('[data-id="selectDepartamento"]').addClass('errorInput');
        error = 1;
    }

    //Selector municipio
    if( $("#selectMunicipio").val() == "" || $("#selectMunicipio").val() == null ){
        var errorMessage = `${icono}<span> Seleccione un municipio. </span>`;
        Fincas.gestionDOM.mensajeError('#invalidFeedback_selectMunicipio',errorMessage);
        $("#invalidFeedback_selectMunicipio").addClass("msnError");
        $('[data-id="selectMunicipio"]').addClass('errorInput');
        error = 1;
    }

    //Selector sector
    if( $("#selectSector").val() == "" || $("#selectSector").val() == null ){
        var errorMessage = `${icono}<span> Seleccione un sector. </span>`;
        Fincas.gestionDOM.mensajeError('#invalidFeedback_selectSector',errorMessage);
        $("#invalidFeedback_selectSector").addClass("msnError");
        $('[data-id="selectSector"]').addClass('errorInput');
        error = 1;
    }

    //Selector grupo
    if( $("#selectGrupo").val() == "" || $("#selectGrupo").val() == null ){
        var errorMessage = `${icono}<span> Seleccione un grupo. </span>`;
        Fincas.gestionDOM.mensajeError('#invalidFeedback_selectGrupo',errorMessage);
        $("#invalidFeedback_selectGrupo").addClass("msnError");
        $('[data-id="selectGrupo"]').addClass('errorInput');
        error = 1;
    }

    //Selector estado
    if( $("#selectEstado").val() == "" || $("#selectEstado").val() == null ){
        var errorMessage = `${icono}<span> Seleccione un estado. </span>`;
        Fincas.gestionDOM.mensajeError('#invalidFeedback_selectEstado',errorMessage);
        $("#invalidFeedback_selectEstado").addClass("msnError");
        $('[data-id="selectEstado"]').addClass('errorInput');
        error = 1;
    }

    //Latitud
    if( $("#latitud").val() == "" ){
        var errorMessage = `${icono}<span> Ingrese un latitud. </span>`;
        Fincas.gestionDOM.mensajeError('#invalidFeedback_latitud',errorMessage);
        $("#invalidFeedback_latitud").addClass("msnError");
        $('#latitud').addClass('errorInput');
        error = 1;
    }

    //Longitud
    if( $("#longitud").val() == "" ){
        var errorMessage = `${icono}<span> Ingrese un longitud. </span>`;
        Fincas.gestionDOM.mensajeError('#invalidFeedback_longitud',errorMessage);
        $("#invalidFeedback_longitud").addClass("msnError");
        $('#longitud').addClass('errorInput');
        error = 1;
    }

    //Hectareas
    if( $("#hectareas").val() == "" ){
        var errorMessage = `${icono}<span> Ingrese un valor de hectáreas. </span>`;
        Fincas.gestionDOM.mensajeError('#invalidFeedback_hectareas',errorMessage);
        $("#invalidFeedback_hectareas").addClass("msnError");
        $('#hectareas').addClass('errorInput');
        error = 1;
    }

    //Latitud
    if( $("#latitud").val() == "0" || $("#latitud").val() == "-0" ){
        var errorMessage = `${icono}<span> La latitud no puede ser 0. </span>`;
        Fincas.gestionDOM.mensajeError('#invalidFeedback_latitud',errorMessage);
        $("#invalidFeedback_latitud").addClass("msnError");
        $('#latitud').addClass('errorInput');
        error = 1;
    }

    //Longitud
    if( $("#longitud").val() == "0" || $("#longitud").val() == "-0" ){
        var errorMessage = `${icono}<span> La longitud no puede ser 0. </span>`;
        Fincas.gestionDOM.mensajeError('#invalidFeedback_longitud',errorMessage);
        $("#invalidFeedback_longitud").addClass("msnError");
        $('#longitud').addClass('errorInput');
        error = 1;
    }

    //Hectareas
    if( $("#hectareas").val() == "0" || $("#hectareas").val() == "-0" ){
        var errorMessage = `${icono}<span> El valor de hectáreas no puede ser 0. </span>`;
        Fincas.gestionDOM.mensajeError('#invalidFeedback_hectareas',errorMessage);
        $("#invalidFeedback_hectareas").addClass("msnError");
        $('#hectareas').addClass('errorInput');
        error = 1;
    }

    //Selector organica
    if( $("#selectOrganica").val() == "" || $("#selectOrganica").val() == null ){
        var errorMessage = `${icono}<span> Indique si la finca es orgánica. </span>`;
        Fincas.gestionDOM.mensajeError('#invalidFeedback_selectOrganica',errorMessage);
        $("#invalidFeedback_selectOrganica").addClass("msnError");
        $('[data-id="selectOrganica"]').addClass('errorInput');
        error = 1;
    }

    //Entra solo si las cajas estan llenas
    if(error == 0){
            var data = {
                id:             Fincas.datosFilaTabla[0]["DT_RowId"],
                codigo:         $("#codigo").val(),
                nombre:         $("#nombre").val(),
                municipio:      $("#selectMunicipio").val(),
                sector:         $("#selectSector").val(),
                grupo:          $("#selectGrupo").val(),
                estado:         $("#selectEstado").val(),
                latitud:        $("#latitud").val().replaceAll('.', '').replaceAll(',','.'),
                longitud:       $("#longitud").val().replaceAll('.', '').replaceAll(',','.'),
                hectareas:      $("#hectareas").val().replaceAll('.', '').replaceAll(',','.'),
                organica:       $("#selectOrganica").val()
            };
            query.callAjax(
                Fincas.urlConexionVista,
                "edit_finca",
                data,
                Fincas.comunicaciones.editFinca);
    }
}

//Se muestran las validaciones de cogigos o nombres repetidos
Fincas.funciones.validateCodes = function(resultado){
    $("#invalidFeedback_codigo").empty();
    $("#invalidFeedback_nombre").empty();

    $(".errorInput").removeClass('errorInput');

    var icono = '<i class="material-icons md-14">error</i>';

    if(resultado.CODIGO > 0 ){
        var errorMessage = `${icono}<span> Este código ya se encuentra registrado. </span>`;
        Fincas.gestionDOM.mensajeError('#invalidFeedback_codigo',errorMessage);
        $("#invalidFeedback_codigo").addClass("msnError");
        $('#codigo').addClass('errorInput');
    }

    if(resultado.NOMBRE > 0 ){
        var errorMessage = `${icono}<span> Este nombre ya se encuentra registrado. </span>`;
        Fincas.gestionDOM.mensajeError('#invalidFeedback_nombre',errorMessage);
        $("#invalidFeedback_nombre").addClass("msnError");
        $('#nombre').addClass('errorInput');
    }
}