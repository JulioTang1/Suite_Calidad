Datos.funciones = new Object();

/*******************************POPOVER*************************************/
//Captura el evento click sobre la tabla y muestra popoever
Datos.funciones.clickSelected = function(event){
    Datos.eventoSelected = event;
}
/*****************************FIN*POPOVER***********************************/

//AJAX para la tabla
Datos.funciones.genTable1 = function(){
    var replace = {		
		fecha_ini: 	Datos.fechaIniRank,
		fecha_fin: 	Datos.fechaFinRank,
        fincas: 	`${$("#selectFinca").val()[0]}`
	};

    gestionDT.initTable(
    	Datos.urlConexion,
    	"sigatoka", 
    	"#tablaSigatoka", 
    	Datos.comunicaciones.tablaCargada,
    	0,
        replace
    );
}

//AJAX para la tabla
Datos.funciones.genTable2 = function(){
    var replace = {		
		fecha_ini: 	Datos.fechaIniRank,
		fecha_fin: 	Datos.fechaFinRank,
        fincas: 	`${$("#selectFinca").val()[0]}`
	};

    gestionDT.initTable(
    	Datos.urlConexion,
    	"vasculares", 
    	"#tablaVasculares", 
    	Datos.comunicaciones.tablaCargada,
    	1,
        replace
    );
}

//AJAX para la tabla
Datos.funciones.genTable3 = function(){
    var replace = {		
		fecha_ini: 	Datos.fechaIniRank,
		fecha_fin: 	Datos.fechaFinRank,
        fincas: 	`${$("#selectFinca").val()[0]}`
	};

    gestionDT.initTable(
    	Datos.urlConexion,
    	"culturales", 
    	"#tablaCulturales", 
    	Datos.comunicaciones.tablaCargada,
    	2,
        replace
    );
}

//AJAX para la tabla
Datos.funciones.genTable4 = function(){
    var replace = {		
		fecha_ini: 	Datos.fechaIniRank,
		fecha_fin: 	Datos.fechaFinRank,
        fincas: 	`${$("#selectFinca").val()[0]}`
	};

    gestionDT.initTable(
    	Datos.urlConexion,
    	"precipitaciones", 
    	"#tablaPrecipitaciones", 
    	Datos.comunicaciones.tablaCargada,
    	3,
        replace
    );
}

/* SELECTORES ANIDADOS */
/*funcion encargada de apagar las banderas de los demas selectores y activar la de Departamento,
para luego hacer la consulta que pide la informacion del selector*/
Datos.funciones.fillDepartamento = function(){
	if(Datos.filtros.departamento.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		Datos.filtros.departamento.state = 1;
		Datos.filtros.municipio.state = 0;
		Datos.filtros.finca.state = 0;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		Datos.filtros.departamento.uso = 1;
		//Se invoca la consulta
		Datos.funciones.consultaSelectores();
	}
}

/*funcion encargada de apagar las banderas de los demas selectores y activar la de Municipio,
para luego hacer la consulta que pide la informacion del selector*/
Datos.funciones.fillMunicipio = function(){
	if(Datos.filtros.municipio.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		Datos.filtros.departamento.state = 0;
		Datos.filtros.municipio.state = 1;
		Datos.filtros.finca.state = 0;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		Datos.filtros.municipio.uso = 1;
		//Se invoca la consulta
		Datos.funciones.consultaSelectores();
	}
}

/*funcion encargada de apagar las banderas de los demas selectores y activar la de finca,
para luego hacer la consulta que pide la informacion del selector*/
Datos.funciones.fillFinca = function(){
	if(Datos.filtros.finca.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		Datos.filtros.departamento.state = 0;
		Datos.filtros.municipio.state = 0;
		Datos.filtros.finca.state = 1;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		Datos.filtros.finca.uso = 1;
		//Se invoca la consulta
		Datos.funciones.consultaSelectores();
	}
}

//Funcion que reactivala recarga de los selectores
Datos.funciones.changeFlag = function(event){
	var select = event.data.select;
	/*Se identifica cual fue el selector que cambio y se apagana las banderas de 
	uso de los demas selectores para que se refresquen*/
	if(select == "selectDepartamento"){
		Datos.filtros.municipio.uso = 0;
		Datos.filtros.finca.uso = 0;
		//Se añaden los filtros
		if($(`#selectDepartamento`).val().length > 0){
			Datos.filtros.departamento.data = `${$(`#selectDepartamento`).val()}`;
		}	
		else{
			Datos.filtros.departamento.data = '0';
		}
	}
	else if(select == "selectMunicipio"){
		Datos.filtros.departamento.uso = 0;
		Datos.filtros.finca.uso = 0;
		//Se añaden los filtros
		if($("#selectMunicipio").val().length > 0){
			Datos.filtros.municipio.data = `${$(`#selectMunicipio`).val()}`;
		}
		else{
			Datos.filtros.municipio.data = '0';
		}
	}
	else if(select == "selectFinca"){
		Datos.filtros.departamento.uso = 0;
		Datos.filtros.municipio.uso = 0;
		//Se añaden los filtros
		if($("#selectFinca").val().length > 0){
			Datos.filtros.finca.data = `${$(`#selectFinca`).val()}`;
		}
		else{
			Datos.filtros.finca.data = '0';
		}
	}
}

//Funcion encargada de traer los selectores iniciales
Datos.funciones.consultaSelectores = function(){
    var data = {
    	filter: 	JSON.stringify(Datos.filtros)
    };
    query.callAjax(Datos.urlConexionVista,
        "consulta_selectores",
        data, 
        Datos.comunicaciones.consultaSelectores);
}
/* FIN SELECTORES ANIDADOS */

//Funcion encargada de realizas una primera validacion de los campos y actualizar la informacion de sigatoka para P10 y P7
Datos.funciones.saveSigatokaP10P7 = function(e){
	var error = 0;

    //Se limpian los divs de mensaje invalido
	$("#invalidFeedback_YLS").empty();
	$("#invalidFeedback_CF").empty();
	$("#invalidFeedback_HF").empty();
	$("#invalidFeedback_Lote").empty();

    $(".errorInput").removeClass('errorInput');

    var icono = '<i class="material-icons md-14">error</i>';

    //YLS
    if( $("#YLS").val() == "" ){
        var errorMessage = `${icono}<span> Ingrese un valor para YLS. </span>`;
        Datos.gestionDOM.mensajeError('#invalidFeedback_YLS',errorMessage);
        $("#invalidFeedback_YLS").addClass("msnError");
        $('#YLS').addClass('errorInput');
        error = 1;
    }

    //CF
    if( $("#CF").val() == "" ){
        var errorMessage = `${icono}<span> Ingrese un valor para CF. </span>`;
        Datos.gestionDOM.mensajeError('#invalidFeedback_CF',errorMessage);
        $("#invalidFeedback_CF").addClass("msnError");
        $('#CF').addClass('errorInput');
        error = 1;
    }

    //HF
    if( $("#HF").val() == "" ){
        var errorMessage = `${icono}<span> Ingrese un valor para HF. </span>`;
        Datos.gestionDOM.mensajeError('#invalidFeedback_HF',errorMessage);
        $("#invalidFeedback_HF").addClass("msnError");
        $('#HF').addClass('errorInput');
        error = 1;
    }

	//Entra solo si las cajas estan llenas
	if(error == 0){
		$('#editar').off();
		gestionModal.alertaBloqueante(primario.aplicacion, 'Procesando...');
		var data = {
			id_planta: 		Datos.datosFilaTabla[0]["DT_RowId"],//D_planta_punto_captura
			YLS:	        $("#YLS").val().replaceAll('.', '').replaceAll(',','.'),
			CF: 	        $("#CF").val(),
			HF: 		    $("#HF").val().replaceAll('.', '').replaceAll(',','.'),
			Lote:			$("#Lote").val()
		};
		query.callAjax(
			Datos.urlConexionVista,
			"save_sigatoka_p10_p7",
			data,
			Datos.comunicaciones.save,
			e.data.index);
    }
}

//Funcion encargada de realizas una primera validacion de los campos y actualizar la informacion de sigatoka para PxP
Datos.funciones.saveSigatokaPxP = function(e){
	var error = 0;

    //Se limpian los divs de mensaje invalido
	$("#invalidFeedback_TH").empty();
	$("#invalidFeedback_YLI").empty();
	$("#invalidFeedback_YLS").empty();
	$("#invalidFeedback_CF").empty();
	$("#invalidFeedback_Lote").empty();

    $(".errorInput").removeClass('errorInput');

    var icono = '<i class="material-icons md-14">error</i>';

    //TH
    if( $("#TH").val() == "" ){
        var errorMessage = `${icono}<span> Ingrese un valor para TH. </span>`;
        Datos.gestionDOM.mensajeError('#invalidFeedback_TH',errorMessage);
        $("#invalidFeedback_TH").addClass("msnError");
        $('#TH').addClass('errorInput');
        error = 1;
    }

    //YLI
    if( $("#YLI").val() == "" ){
        var errorMessage = `${icono}<span> Ingrese un valor para YLI. </span>`;
        Datos.gestionDOM.mensajeError('#invalidFeedback_YLI',errorMessage);
        $("#invalidFeedback_YLI").addClass("msnError");
        $('#YLI').addClass('errorInput');
        error = 1;
    }

    //YLS
    if( $("#YLS").val() == "" ){
        var errorMessage = `${icono}<span> Ingrese un valor para YLS. </span>`;
        Datos.gestionDOM.mensajeError('#invalidFeedback_YLS',errorMessage);
        $("#invalidFeedback_YLS").addClass("msnError");
        $('#YLS').addClass('errorInput');
        error = 1;
    }

    //CF
    if( $("#CF").val() == "" ){
        var errorMessage = `${icono}<span> Ingrese un valor para CF. </span>`;
        Datos.gestionDOM.mensajeError('#invalidFeedback_CF',errorMessage);
        $("#invalidFeedback_CF").addClass("msnError");
        $('#CF').addClass('errorInput');
        error = 1;
    }

	//Entra solo si las cajas estan llenas
	if(error == 0){
		$('#editar').off();
		gestionModal.alertaBloqueante(primario.aplicacion, 'Procesando...');
		var data = {
			id_planta: 		Datos.datosFilaTabla[0]["DT_RowId"],//D_planta_punto_captura
			TH: 		    $("#TH").val().replaceAll('.', '').replaceAll(',','.'),
			YLI: 		    $("#YLI").val().replaceAll('.', '').replaceAll(',','.'),
			YLS:	        $("#YLS").val().replaceAll('.', '').replaceAll(',','.'),
			CF: 	        $("#CF").val(),
			Lote:			$("#Lote").val()
		};
		query.callAjax(
			Datos.urlConexionVista,
			"save_sigatoka_pxp",
			data,
			Datos.comunicaciones.save,
			e.data.index);
    }
}

//Funcion encargada de realizas una primera validacion de los campos y actualizar la informacion de sigatoka para Fija
Datos.funciones.saveSigatokaFija = function(e){
	var error = 0;
    //Se limpian los divs de mensaje invalido
	$("#invalidFeedback_TH").empty();
	$("#invalidFeedback_EFA").empty();
	$("#invalidFeedback_CF").empty();
	$("#invalidFeedback_H2").empty();
	$("#invalidFeedback_Lote").empty();

    $(".errorInput").removeClass('errorInput');

    var icono = '<i class="material-icons md-14">error</i>';

    //TH
    if( $("#TH").val() == "" ){
        var errorMessage = `${icono}<span> Ingrese un valor para TH. </span>`;
        Datos.gestionDOM.mensajeError('#invalidFeedback_TH',errorMessage);
        $("#invalidFeedback_TH").addClass("msnError");
        $('#TH').addClass('errorInput');
        error = 1;
    }

    //EFA
    if( $("#EFA").val() == "" ){
        var errorMessage = `${icono}<span> Ingrese un valor para EFA. </span>`;
        Datos.gestionDOM.mensajeError('#invalidFeedback_EFA',errorMessage);
        $("#invalidFeedback_EFA").addClass("msnError");
        $('#EFA').addClass('errorInput');
        error = 1;
    }

    //CF
    if( $("#CF").val() == "" ){
        var errorMessage = `${icono}<span> Ingrese un valor para CF. </span>`;
        Datos.gestionDOM.mensajeError('#invalidFeedback_CF',errorMessage);
        $("#invalidFeedback_CF").addClass("msnError");
        $('#CF').addClass('errorInput');
        error = 1;
    }

    //H2
    if( $("#H2").val() == "" || $("#H2").val() == null ){
        var errorMessage = `${icono}<span> Ingrese un valor para H2. </span>`;
        Fincas.gestionDOM.mensajeError('#invalidFeedback_H2',errorMessage);
        $("#invalidFeedback_H2").addClass("msnError");
        $('[data-id="H2"]').addClass('errorInput');
        error = 1;
    }

    //H3
    if( $("#H3").val() == "" || $("#H3").val() == null ){
        var errorMessage = `${icono}<span> Ingrese un valor para H3. </span>`;
        Fincas.gestionDOM.mensajeError('#invalidFeedback_H3',errorMessage);
        $("#invalidFeedback_H3").addClass("msnError");
        $('[data-id="H3"]').addClass('errorInput');
        error = 1;
    }

    //H4
    if( $("#H4").val() == "" || $("#H4").val() == null ){
        var errorMessage = `${icono}<span> Ingrese un valor para H4. </span>`;
        Fincas.gestionDOM.mensajeError('#invalidFeedback_H4',errorMessage);
        $("#invalidFeedback_H4").addClass("msnError");
        $('[data-id="H4"]').addClass('errorInput');
        error = 1;
    }

	//Entra solo si las cajas estan llenas
	if(error == 0){
		$('#editar').off();
		gestionModal.alertaBloqueante(primario.aplicacion, 'Procesando...');
		var data = {
			id_planta: 		Datos.datosFilaTabla[0]["DT_RowId"],//D_planta_punto_captura
			TH: 		    $("#TH").val().replaceAll('.', '').replaceAll(',','.'),
			EFA: 		    $("#EFA").val().replaceAll('.', '').replaceAll(',','.'),
			CF:	        	$("#CF").val(),
			H2: 	        $("#H2").val(),
			H3: 	        $("#H3").val(),
			H4: 	        $("#H4").val(),
			Lote:			$("#Lote").val()
		};
		query.callAjax(
			Datos.urlConexionVista,
			"save_sigatoka_fija",
			data,
			Datos.comunicaciones.save,
			e.data.index);
    }
}

//Funcion encargada de realizas una primera validacion de los campos y actualizar la informacion de E.Vasculares
Datos.funciones.saveVasculares = function(e){
	var error = 0;
    //Se limpian los divs de mensaje invalido
	$("#invalidFeedback_Fusarium").empty();
	$("#invalidFeedback_Moko").empty();
	$("#invalidFeedback_Erwinia").empty();

    $(".errorInput").removeClass('errorInput');

    var icono = '<i class="material-icons md-14">error</i>';

    //Fusarium
    if( $("#Fusarium").val() == "" || $("#Fusarium").val() == null ){
        var errorMessage = `${icono}<span> Ingrese un valor para Fusarium. </span>`;
        Fincas.gestionDOM.mensajeError('#invalidFeedback_Fusarium',errorMessage);
        $("#invalidFeedback_Fusarium").addClass("msnError");
        $('[data-id="Fusarium"]').addClass('errorInput');
        error = 1;
    }

    //Moko
    if( $("#Moko").val() == "" || $("#Moko").val() == null ){
        var errorMessage = `${icono}<span> Ingrese un valor para Moko. </span>`;
        Fincas.gestionDOM.mensajeError('#invalidFeedback_Moko',errorMessage);
        $("#invalidFeedback_Moko").addClass("msnError");
        $('[data-id="Moko"]').addClass('errorInput');
        error = 1;
    }

    //Erwinia
    if( $("#Erwinia").val() == "" || $("#Erwinia").val() == null ){
        var errorMessage = `${icono}<span> Ingrese un valor para Erwinia. </span>`;
        Fincas.gestionDOM.mensajeError('#invalidFeedback_Erwinia',errorMessage);
        $("#invalidFeedback_Erwinia").addClass("msnError");
        $('[data-id="Erwinia"]').addClass('errorInput');
        error = 1;
    }

	//Entra solo si las cajas estan llenas
	if(error == 0){
		$('#editar').off();
		gestionModal.alertaBloqueante(primario.aplicacion, 'Procesando...');
		var data = {
			id_planta: 		Datos.datosFilaTabla[0]["DT_RowId"],//D_planta_punto_captura
			Fusarium: 	        $("#Fusarium").val(),
			Moko: 	        	$("#Moko").val(),
			Erwinia: 	        $("#Erwinia").val()
		};
		query.callAjax(
			Datos.urlConexionVista,
			"save_sigatoka_vasculares",
			data,
			Datos.comunicaciones.save,
			e.data.index);
    }
}

//Funcion encargada de realizas una primera validacion de los campos y actualizar la informacion de E.Vasculares
Datos.funciones.saveCulturales = function(e){
	var error = 0;
    //Se limpian los divs de mensaje invalido
	$("#invalidFeedback_NF").empty();
	$("#invalidFeedback_Moko").empty();
	$("#invalidFeedback_Erwinia").empty();

    $(".errorInput").removeClass('errorInput');

    var icono = '<i class="material-icons md-14">error</i>';

    //NF
    if( $("#NF").val() == "" ){
        var errorMessage = `${icono}<span> Ingrese un valor para NF. </span>`;
        Datos.gestionDOM.mensajeError('#invalidFeedback_NF',errorMessage);
        $("#invalidFeedback_NF").addClass("msnError");
        $('#NF').addClass('errorInput');
        error = 1;
    }

    //FIT
    if( $("#FIT").val() == "" || $("#FIT").val() == null ){
        var errorMessage = `${icono}<span> Ingrese un valor para FIT. </span>`;
        Fincas.gestionDOM.mensajeError('#invalidFeedback_FIT',errorMessage);
        $("#invalidFeedback_FIT").addClass("msnError");
        $('[data-id="FIT"]').addClass('errorInput');
        error = 1;
    }

    //RTI
    if( $("#RTI").val() == "" || $("#RTI").val() == null ){
        var errorMessage = `${icono}<span> Ingrese un valor para RTI. </span>`;
        Fincas.gestionDOM.mensajeError('#invalidFeedback_RTI',errorMessage);
        $("#invalidFeedback_RTI").addClass("msnError");
        $('[data-id="RTI"]').addClass('errorInput');
        error = 1;
    }

	//Entra solo si las cajas estan llenas
	if(error == 0){
		$('#editar').off();
		gestionModal.alertaBloqueante(primario.aplicacion, 'Procesando...');
		var data = {
			id_planta: 			Datos.datosFilaTabla[0]["DT_RowId"],//D_planta_punto_captura
			NF: 		        $("#NF").val().replaceAll('.', '').replaceAll(',','.'),
			FIT:  	        	$("#FIT").val(),
			RTI: 		        $("#RTI").val(),
			CFIT:				$("#CFIT").val(),
			CRTI:				$("#CRTI").val()
		};
		query.callAjax(
			Datos.urlConexionVista,
			"save_sigatoka_culturales",
			data,
			Datos.comunicaciones.save,
			e.data.index);
    }
}

//Funcion encargada de realizas una primera validacion de los campos y actualizar la informacion de precipitaciones
Datos.funciones.savePrecipitaciones = function(e){
	var error = 0;
    //Se limpian los divs de mensaje invalido
	$("#invalidFeedback_precipitacion").empty();

    $(".errorInput").removeClass('errorInput');

    var icono = '<i class="material-icons md-14">error</i>';

    //NF
    if( $("#NF").val() == "" ){
        var errorMessage = `${icono}<span> Ingrese un valor para precipitación. </span>`;
        Datos.gestionDOM.mensajeError('#invalidFeedback_precipitacion',errorMessage);
        $("#invalidFeedback_precipitacion").addClass("msnError");
        $('#precipitacion').addClass('errorInput');
        error = 1;
    }

	//Entra solo si las cajas estan llenas
	if(error == 0){
		$(".modal-dialog-2").removeClass("modal-dialog-2");
		$('#editar').off();
		gestionModal.alertaBloqueante(primario.aplicacion, 'Procesando...');
		var data = {
			id_precipitacion: 	Datos.datosFilaTabla[0]["DT_RowId"],//D_planta_punto_captura
			precipitacion: 		$("#precipitacion").val().replaceAll('.', '').replaceAll(',','.')
		};
		query.callAjax(
			Datos.urlConexionVista,
			"save_precipitaciones",
			data,
			Datos.comunicaciones.save,
			e.data.index);
    }
}