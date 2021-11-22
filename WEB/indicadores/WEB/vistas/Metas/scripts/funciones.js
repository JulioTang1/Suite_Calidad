Metas.funciones = new Object();

/*******************************POPOVER*************************************/
//Captura el evento click sobre la tabla y muestra popoever
Metas.funciones.clickSelected = function(event){
    Metas.eventoSelected = event;
}
/*****************************FIN*POPOVER***********************************/

//AJAX para la tabla
Metas.funciones.genTable = function(){
    gestionDT.initTable(
    	Metas.urlConexion,
    	"Metas", 
    	"#tablaMetas", 
    	Metas.comunicaciones.tablaCargada,
    	0
    );
}

//Funcion encargada de realizas una primera validacion de los campos
Metas.funciones.validacionUsuario = function(){
    var error = 0;

    //Se limpian los divs de mensaje invalido
    $("#invalidFeedback_selectEdad").empty();
    $("#invalidFeedback_selectInfeccion").empty();
    $("#invalidFeedback_meta").empty();

    $(".errorInput").removeClass('errorInput');

    var icono = '<i class="material-icons md-14">error</i>';

    //Selector edad
    if( $("#selectEdad").val() == "" || $("#selectEdad").val() == null ){
        var errorMessage = `${icono}<span> Seleccione una edad. </span>`;
        Metas.gestionDOM.mensajeError('#invalidFeedback_selectEdad',errorMessage);
        $("#invalidFeedback_selectEdad").addClass("msnError");
        $('[data-id="selectEdad"]').addClass('errorInput');
        error = 1;
    }

    //Selector indicador
    if( $("#selectInfeccion").val() == "" || $("#selectInfeccion").val() == null ){
        var errorMessage = `${icono}<span> Seleccione un indicador. </span>`;
        Metas.gestionDOM.mensajeError('#invalidFeedback_selectInfeccion',errorMessage);
        $("#invalidFeedback_selectInfeccion").addClass("msnError");
        $('[data-id="selectInfeccion"]').addClass('errorInput');
        error = 1;
    }

    //Meta
    if( $("#meta").val() == "" ){
        var errorMessage = `${icono}<span> Ingrese una meta. </span>`;
        Metas.gestionDOM.mensajeError('#invalidFeedback_meta',errorMessage);
        $("#invalidFeedback_meta").addClass("msnError");
        $('#meta').addClass('errorInput');
        error = 1;
    }

    //Entra solo si las cajas estan llenas
    if(error == 0){
            var data = {
                edad:           $("#selectEdad").val(),
                indicador:      $("#selectInfeccion").val(),
                meta:        $("#meta").val().replaceAll('.', '').replaceAll(',','.')
            };
            query.callAjax(
                Metas.urlConexionVista,
                "insert_meta",
                data,
                Metas.comunicaciones.insertMeta);
    }
}

//Funcion encargada de eliminar una meta
Metas.funciones.deleteMeta = function(){
    var data = {
        id:             Metas.datosFilaTabla[0]["DT_RowId"]
    };
    query.callAjax(
        Metas.urlConexionVista,
        "delete_meta",
        data,
        Metas.comunicaciones.deleteMeta);
}

//Se muestran las validaciones de metas repetidos
Metas.funciones.validateCodes = function(){
    $("#invalidFeedback_meta").empty();

    $(".errorInput").removeClass('errorInput');

    var icono = '<i class="material-icons md-14">error</i>';

    var errorMessage = `${icono}<span> Esta meta ya se encuentra registrada. </span>`;
    Metas.gestionDOM.mensajeError('#invalidFeedback_meta',errorMessage);
    $("#invalidFeedback_meta").addClass("msnError");
    $('#latitud').addClass('errorInput');
}

/* SELECTORES ANIDADOS (edades infecciones) */
/*funcion encargada de apagar las banderas de los demas selectores y activar la de Departamento,
para luego hacer la consulta que pide la informacion del selector*/
Metas.funciones.fillEdad = function(){
	if(Metas.filtrosEI.edad.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		Metas.filtrosEI.edad.state = 1;
		Metas.filtrosEI.indicador.state = 0;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		Metas.filtrosEI.edad.uso = 1;
		//Se invoca la consulta
		Metas.funciones.consultaSelectoresEI();
	}
}

/*funcion encargada de apagar las banderas de los demas selectores y activar la de Municipio,
para luego hacer la consulta que pide la informacion del selector*/
Metas.funciones.fillInfeccion = function(){
	if(Metas.filtrosEI.indicador.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		Metas.filtrosEI.edad.state = 0;
		Metas.filtrosEI.indicador.state = 1;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		Metas.filtrosEI.indicador.uso = 1;
		//Se invoca la consulta
		Metas.funciones.consultaSelectoresEI();
	}
}

//Funcion que reactivala recarga de los selectores
Metas.funciones.changeFlagEI = function(event){
	var select = event.data.select;
	/*Se identifica cual fue el selector que cambio y se apagana las banderas de 
	uso de los demas selectores para que se refresquen*/
	if(select == "selectEdad"){
		Metas.filtrosEI.indicador.uso = 0;
		//Se añaden los filtros
		if($(`#selectEdad`).val().length > 0){
			Metas.filtrosEI.edad.data = `${$(`#selectEdad`).val()}`;
		}	
		else{
			Metas.filtrosEI.edad.data = '0';
		}
	}
	else if(select == "selectInfeccion"){
		Metas.filtrosEI.edad.uso = 0;
		//Se añaden los filtros
		if($("#selectInfeccion").val().length > 0){
			Metas.filtrosEI.indicador.data = `${$(`#selectInfeccion`).val()}`;
		}
		else{
			Metas.filtrosEI.indicador.data = '0';
		}
	}
}

//Funcion encargada de traer los selectores iniciales
Metas.funciones.consultaSelectoresEI = function(){
    var data = {
    	filter: 	JSON.stringify(Metas.filtrosEI)
    };
    query.callAjax(Metas.urlConexionVista,
        "consulta_selectores_ei",
        data, 
        Metas.comunicaciones.consultaSelectoresEI);
}
/* FIN SELECTORES ANIDADOS (edades infecciones) */