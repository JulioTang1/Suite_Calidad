CoeficientesSeveridad.funciones = new Object();

/*******************************POPOVER*************************************/
//Captura el evento click sobre la tabla y muestra popoever
CoeficientesSeveridad.funciones.clickSelected = function(event){
    CoeficientesSeveridad.eventoSelected = event;
}
/*****************************FIN*POPOVER***********************************/

//AJAX para la tabla
CoeficientesSeveridad.funciones.genTable = function(){
    gestionDT.initTable(
    	CoeficientesSeveridad.urlConexion,
    	"CoeficientesSeveridad", 
    	"#tablaCoeficientesSeveridad", 
    	CoeficientesSeveridad.comunicaciones.tablaCargada,
    	0
    );
}

//Funcion encargada de realizas una primera validacion de los campos
CoeficientesSeveridad.funciones.validacionUsuarioEdit = function(){
    var error = 0;

    //Se limpian los divs de mensaje invalido
    $("#invalidFeedback_dos").empty();
    $("#invalidFeedback_tres").empty();
    $("#invalidFeedback_cuatro").empty();

    $(".errorInput").removeClass('errorInput');

    var icono = '<i class="material-icons md-14">error</i>';

    //Posición de la hoja (II)
    if( $("#dos").val() == "" ){
        var errorMessage = `${icono}<span> Ingrese un valor para posición de la hoja (II). </span>`;
        CoeficientesSeveridad.gestionDOM.mensajeError('#invalidFeedback_dos',errorMessage);
        $("#invalidFeedback_dos").addClass("msnError");
        $('#dos').addClass('errorInput');
        error = 1;
    }

    //Posición de la hoja (III)
    if( $("#tres").val() == "" ){
        var errorMessage = `${icono}<span> Ingrese un valor para posición de la hoja (III). </span>`;
        CoeficientesSeveridad.gestionDOM.mensajeError('#invalidFeedback_tres',errorMessage);
        $("#invalidFeedback_tres").addClass("msnError");
        $('#tres').addClass('errorInput');
        error = 1;
    }

    //Posición de la hoja (IV)
    if( $("#cuatro").val() == "" ){
        var errorMessage = `${icono}<span> Ingrese un valor para posición de la hoja (IV). </span>`;
        CoeficientesSeveridad.gestionDOM.mensajeError('#invalidFeedback_cuatro',errorMessage);
        $("#invalidFeedback_cuatro").addClass("msnError");
        $('#cuatro').addClass('errorInput');
        error = 1;
    }

    //Entra solo si las cajas estan llenas
    if(error == 0){
            var data = {
                id:             CoeficientesSeveridad.datosFilaTabla[0]["DT_RowId"],
                dos:        $("#dos").val().replaceAll('.', '').replaceAll(',','.'),
                tres:       $("#tres").val().replaceAll('.', '').replaceAll(',','.'),
                cuatro:      $("#cuatro").val().replaceAll('.', '').replaceAll(',','.')
            };
            query.callAjax(
                CoeficientesSeveridad.urlConexionVista,
                "edit",
                data,
                CoeficientesSeveridad.comunicaciones.edit);
    }
}