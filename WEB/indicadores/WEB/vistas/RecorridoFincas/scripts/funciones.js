RecorridoFincas.funciones = new Object();

/*******************************POPOVER*************************************/
//Captura el evento click sobre la tabla y muestra popoever
RecorridoFincas.funciones.clickSelected = function(event){
    RecorridoFincas.eventoSelected = event;
}
/*****************************FIN*POPOVER***********************************/

/* SELECTORES ANIDADOS */
/*funcion encargada de apagar las banderas de los demas selectores y activar la de Departamento,
para luego hacer la consulta que pide la informacion del selector*/
RecorridoFincas.funciones.fillDepartamento = function(){
	if(RecorridoFincas.filtros.departamento.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		RecorridoFincas.filtros.departamento.state = 1;
		RecorridoFincas.filtros.municipio.state = 0;
		RecorridoFincas.filtros.finca.state = 0;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		RecorridoFincas.filtros.departamento.uso = 1;
		//Se invoca la consulta
		RecorridoFincas.funciones.consultaSelectores();
	}
}

/*funcion encargada de apagar las banderas de los demas selectores y activar la de Municipio,
para luego hacer la consulta que pide la informacion del selector*/
RecorridoFincas.funciones.fillMunicipio = function(){
	if(RecorridoFincas.filtros.municipio.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		RecorridoFincas.filtros.departamento.state = 0;
		RecorridoFincas.filtros.municipio.state = 1;
		RecorridoFincas.filtros.finca.state = 0;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		RecorridoFincas.filtros.municipio.uso = 1;
		//Se invoca la consulta
		RecorridoFincas.funciones.consultaSelectores();
	}
}

/*funcion encargada de apagar las banderas de los demas selectores y activar la de finca,
para luego hacer la consulta que pide la informacion del selector*/
RecorridoFincas.funciones.fillFinca = function(){
	if(RecorridoFincas.filtros.finca.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		RecorridoFincas.filtros.departamento.state = 0;
		RecorridoFincas.filtros.municipio.state = 0;
		RecorridoFincas.filtros.finca.state = 1;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		RecorridoFincas.filtros.finca.uso = 1;
		//Se invoca la consulta
		RecorridoFincas.funciones.consultaSelectores();
	}
}

//Funcion que reactivala recarga de los selectores
RecorridoFincas.funciones.changeFlag = function(event){
	var select = event.data.select;
	/*Se identifica cual fue el selector que cambio y se apagana las banderas de 
	uso de los demas selectores para que se refresquen*/
	if(select == "selectDepartamento"){
		RecorridoFincas.filtros.municipio.uso = 0;
		RecorridoFincas.filtros.finca.uso = 0;
		//Se añaden los filtros
		if($(`#selectDepartamento`).val().length > 0){
			RecorridoFincas.filtros.departamento.data = `${$(`#selectDepartamento`).val()}`;
		}	
		else{
			RecorridoFincas.filtros.departamento.data = '0';
		}
	}
	else if(select == "selectMunicipio"){
		RecorridoFincas.filtros.departamento.uso = 0;
		RecorridoFincas.filtros.finca.uso = 0;
		//Se añaden los filtros
		if($("#selectMunicipio").val().length > 0){
			RecorridoFincas.filtros.municipio.data = `${$(`#selectMunicipio`).val()}`;
		}
		else{
			RecorridoFincas.filtros.municipio.data = '0';
		}
	}
	else if(select == "selectFinca"){
		RecorridoFincas.filtros.departamento.uso = 0;
		RecorridoFincas.filtros.municipio.uso = 0;
		//Se añaden los filtros
		if($("#selectFinca").val().length > 0){
			RecorridoFincas.filtros.finca.data = `${$(`#selectFinca`).val()}`;
		}
		else{
			RecorridoFincas.filtros.finca.data = '0';
		}
	}
}

//Funcion encargada de traer los selectores iniciales
RecorridoFincas.funciones.consultaSelectores = function(){
    var data = {
    	filter: 	JSON.stringify(RecorridoFincas.filtros)
    };
    query.callAjax(RecorridoFincas.urlConexionVista,
        "consulta_selectores",
        data, 
        RecorridoFincas.comunicaciones.consultaSelectores);
}
/* FIN SELECTORES ANIDADOS */

/* SELECTORES ANIDADOS (edades infecciones) */
/*funcion encargada de apagar las banderas de los demas selectores y activar la de Departamento,
para luego hacer la consulta que pide la informacion del selector*/
RecorridoFincas.funciones.fillEdad = function(){
	if(RecorridoFincas.filtrosEI.edad.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		RecorridoFincas.filtrosEI.edad.state = 1;
		RecorridoFincas.filtrosEI.indicador.state = 0;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		RecorridoFincas.filtrosEI.edad.uso = 1;
		//Se invoca la consulta
		RecorridoFincas.funciones.consultaSelectoresEI();
	}
}

/*funcion encargada de apagar las banderas de los demas selectores y activar la de Municipio,
para luego hacer la consulta que pide la informacion del selector*/
RecorridoFincas.funciones.fillInfeccion = function(){
	if(RecorridoFincas.filtrosEI.indicador.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		RecorridoFincas.filtrosEI.edad.state = 0;
		RecorridoFincas.filtrosEI.indicador.state = 1;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		RecorridoFincas.filtrosEI.indicador.uso = 1;
		//Se invoca la consulta
		RecorridoFincas.funciones.consultaSelectoresEI();
	}
}

//Funcion que reactivala recarga de los selectores
RecorridoFincas.funciones.changeFlagEI = function(event){
	var select = event.data.select;
	/*Se identifica cual fue el selector que cambio y se apagana las banderas de 
	uso de los demas selectores para que se refresquen*/
	if(select == "selectEdad"){
		RecorridoFincas.filtrosEI.indicador.uso = 0;
		//Se añaden los filtros
		if($(`#selectEdad`).val().length > 0){
			RecorridoFincas.filtrosEI.edad.data = `${$(`#selectEdad`).val()}`;
		}	
		else{
			RecorridoFincas.filtrosEI.edad.data = '0';
		}
	}
	else if(select == "selectInfeccion"){
		RecorridoFincas.filtrosEI.edad.uso = 0;
		//Se añaden los filtros
		if($("#selectInfeccion").val().length > 0){
			RecorridoFincas.filtrosEI.indicador.data = `${$(`#selectInfeccion`).val()}`;
		}
		else{
			RecorridoFincas.filtrosEI.indicador.data = '0';
		}
	}
}

//Funcion encargada de traer los selectores iniciales
RecorridoFincas.funciones.consultaSelectoresEI = function(){
    var data = {
    	filter: 	JSON.stringify(RecorridoFincas.filtrosEI)
    };
    query.callAjax(RecorridoFincas.urlConexionVista,
        "consulta_selectores_ei",
        data, 
        RecorridoFincas.comunicaciones.consultaSelectoresEI);
}
/* FIN SELECTORES ANIDADOS (edades infecciones) */

//AJAX para la tabla
RecorridoFincas.funciones.genTable = function(){
	var replace = {
		id_finca: 	`${$(`#selectFinca`).val()}`,
		fecha_ini: 	RecorridoFincas.fechaIniRank,
		fecha_fin: 	RecorridoFincas.fechaFinRank
	};
    gestionDT.initTable(
    	RecorridoFincas.urlConexion,
    	"RecorridoFincas", 
    	"#tablaRecorridoFincas", 
    	RecorridoFincas.comunicaciones.tablaCargada,
    	0,
		replace
    );
}

//Se consulta la informacion de la visita
RecorridoFincas.funciones.visita = function(){
	//Se escribe informacion de la visita seleccionada
	RecorridoFincas.gestionDOM.infoVisita();

	$("#map").html(`<div class="spinner-border text-warning"></div>`);

	//Se pide informacion de tablas cada que haya un cambio de visita para mostrar datos
	if(RecorridoFincas.idVisita != RecorridoFincas.datosFilaTabla[0].DT_RowId){
		RecorridoFincas.idVisita = RecorridoFincas.datosFilaTabla[0].DT_RowId;
		$("#tablas").html(`<div class="spinner-border text-warning"></div>`);	
		RecorridoFincas.funciones.genDatos();
	}
	
	//Aparece el navbar
	$("#nav-wrapper-balance").css("display", "flex");
	$(".tab-content").css("display", "block");
	$("#navWrapper").css("display", "block");

	//Cerrar popover
	RecorridoFincas.gestionDOM.popoverOff();
	$("#tableRecorridoFincas").css("display", "none");

	$("#selectEdad").parent().css("display", "block");
	$("#selectInfeccion").parent().css("display", "block");
	$("#volver").css("display", "block");

	$("#calendarPlanning").css("display", "none");
	$("#selectDepartamento").parent().css("display", "none");
	$("#selectMunicipio").parent().css("display", "none");
	$("#selectFinca").parent().css("display", "none");
	$("#cargar").css("display", "none");

	$("#map").html(`<div class="spinner-border text-warning"></div>`);
	var data = {
		id_visita: 	RecorridoFincas.datosFilaTabla[0].DT_RowId,
		edad: 		`${$(`#selectEdad`).val().length == 0 ? "0" : $(`#selectEdad`).val()}`,
		infeccion: 	`${$(`#selectInfeccion`).val().length == 0 ? "0" : $(`#selectInfeccion`).val()}`
	};
	//incremeto de bandera que guarda los mapas pedidos
	RecorridoFincas.mapRequest += 1;
	query.callAjax(RecorridoFincas.urlConexionVista,
		"visita",
		data, 
		RecorridoFincas.comunicaciones.visita,
		RecorridoFincas.mapRequest);	
}

//AJAX para los datos
RecorridoFincas.funciones.genDatos = function(){
	var data = {
		id_visita: 	RecorridoFincas.datosFilaTabla[0].DT_RowId
	};
	
	query.callAjax(RecorridoFincas.urlConexionVista,
		"tabla_visita",
		data, 
		RecorridoFincas.comunicaciones.genDatos);
}

//AJAX para eliminar una visita
RecorridoFincas.funciones.delete = function(){
	gestionModal.alertaBloqueante("CARGANDO...");
	var data = {
		id_visita: 	RecorridoFincas.datosFilaTabla[0].DT_RowId
	};
	query.callAjax(RecorridoFincas.urlConexionVista,
		"delete_visita",
		data, 
		RecorridoFincas.comunicaciones.delete);
}

//Se traen las urls de las fotos
RecorridoFincas.funciones.camara = function(){
	//Llama servicio si son indicadores NF o FIT
	if($(this).parent().data("indicador") == 12 || $(this).parent().data("indicador") == 13){
		var data = {
			id_planta: 	    $(this).parents("tr").data("id"),
			id_indicador: 	$(this).parent().data("indicador")
		};
		query.callAjax(RecorridoFincas.urlConexionVista,
			"fotos",
			data, 
			RecorridoFincas.comunicaciones.camara);
	}
	//Muestra fotos predeterminadas para indicadores CF
	else if($(this).parent().data("indicador") == 1){
		RecorridoFincas.gestionDOM.camaraCF([{url : "vistas/RecorridoFincas/imagenes/CF_ideal.jpeg"}]);
	}
	else if($(this).parent().data("indicador") == 2){
		RecorridoFincas.gestionDOM.camaraCF([{url : "vistas/RecorridoFincas/imagenes/CF_Sintomas_Arrepollamiento.jpeg"}]);
	}
	else if($(this).parent().data("indicador") == 3){
		RecorridoFincas.gestionDOM.camaraCF([{url : "vistas/RecorridoFincas/imagenes/CF_Sintomas_Arrepollamiento_2.jpeg"}]);
	}
	else if($(this).parent().data("indicador") == 4){
		RecorridoFincas.gestionDOM.camaraCF([{url : "vistas/RecorridoFincas/imagenes/CF_Sintomas_Arrepollamiento_3.jpeg"}]);
	}
	else{}
}