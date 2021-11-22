Bioseguridad.funciones = new Object();

/*******************************POPOVER*************************************/
//Captura el evento click sobre la tabla y muestra popoever
Bioseguridad.funciones.clickSelected = function(event){
    Bioseguridad.eventoSelected = event;
}
/*****************************FIN*POPOVER***********************************/

//AJAX para la tabla
Bioseguridad.funciones.genTable = function(){
	var replace = {
		id_finca: 	`${$("#selectFinca").val()}`,
		fecha_ini: 	Bioseguridad.fechaIniRank,
		fecha_fin: 	Bioseguridad.fechaFinRank
	};
    gestionDT.initTable(
    	Bioseguridad.urlConexion,
    	"Bioseguridad", 
    	"#tablaBioseguridad", 
    	Bioseguridad.comunicaciones.tablaCargada,
    	0,
		replace
    );
}

/* SELECTORES ANIDADOS */
/*funcion encargada de apagar las banderas de los demas selectores y activar la de Departamento,
para luego hacer la consulta que pide la informacion del selector*/
Bioseguridad.funciones.fillDepartamento = function(){
	if(Bioseguridad.filtros.departamento.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		Bioseguridad.filtros.departamento.state = 1;
		Bioseguridad.filtros.municipio.state = 0;
		Bioseguridad.filtros.finca.state = 0;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		Bioseguridad.filtros.departamento.uso = 1;
		//Se invoca la consulta
		Bioseguridad.funciones.consultaSelectores();
	}
}

/*funcion encargada de apagar las banderas de los demas selectores y activar la de Municipio,
para luego hacer la consulta que pide la informacion del selector*/
Bioseguridad.funciones.fillMunicipio = function(){
	if(Bioseguridad.filtros.municipio.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		Bioseguridad.filtros.departamento.state = 0;
		Bioseguridad.filtros.municipio.state = 1;
		Bioseguridad.filtros.finca.state = 0;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		Bioseguridad.filtros.municipio.uso = 1;
		//Se invoca la consulta
		Bioseguridad.funciones.consultaSelectores();
	}
}

/*funcion encargada de apagar las banderas de los demas selectores y activar la de finca,
para luego hacer la consulta que pide la informacion del selector*/
Bioseguridad.funciones.fillFinca = function(){
	if(Bioseguridad.filtros.finca.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		Bioseguridad.filtros.departamento.state = 0;
		Bioseguridad.filtros.municipio.state = 0;
		Bioseguridad.filtros.finca.state = 1;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		Bioseguridad.filtros.finca.uso = 1;
		//Se invoca la consulta
		Bioseguridad.funciones.consultaSelectores();
	}
}

//Funcion que reactivala recarga de los selectores
Bioseguridad.funciones.changeFlag = function(event){
	var select = event.data.select;
	/*Se identifica cual fue el selector que cambio y se apagana las banderas de 
	uso de los demas selectores para que se refresquen*/
	if(select == "selectDepartamento"){
		Bioseguridad.filtros.municipio.uso = 0;
		Bioseguridad.filtros.finca.uso = 0;
		//Se añaden los filtros
		if($(`#selectDepartamento`).val().length > 0){
			Bioseguridad.filtros.departamento.data = `${$(`#selectDepartamento`).val()}`;
		}	
		else{
			Bioseguridad.filtros.departamento.data = '0';
		}
	}
	else if(select == "selectMunicipio"){
		Bioseguridad.filtros.departamento.uso = 0;
		Bioseguridad.filtros.finca.uso = 0;
		//Se añaden los filtros
		if($("#selectMunicipio").val().length > 0){
			Bioseguridad.filtros.municipio.data = `${$(`#selectMunicipio`).val()}`;
		}
		else{
			Bioseguridad.filtros.municipio.data = '0';
		}
	}
	else if(select == "selectFinca"){
		Bioseguridad.filtros.departamento.uso = 0;
		Bioseguridad.filtros.municipio.uso = 0;
		//Se añaden los filtros
		if($("#selectFinca").val().length > 0){
			Bioseguridad.filtros.finca.data = `${$(`#selectFinca`).val()}`;
		}
		else{
			Bioseguridad.filtros.finca.data = '0';
		}
	}
}

//Funcion encargada de traer los selectores iniciales
Bioseguridad.funciones.consultaSelectores = function(){
    var data = {
    	filter: 	JSON.stringify(Bioseguridad.filtros)
    };
    query.callAjax(Bioseguridad.urlConexionVista,
        "consulta_selectores",
        data, 
        Bioseguridad.comunicaciones.consultaSelectores);
}
/* FIN SELECTORES ANIDADOS */

//Se consulta la informacion de la visita
Bioseguridad.funciones.visita = function(){
	$("#main").html(`<div class="spinner-border text-warning"></div>`);
	$("#main").css("display", "block");
	$("#tableBioseguridad").css("display", "none");
	$("#divSelectVista").attr("style", "display: none!important");
	//Cerrar popover
	Bioseguridad.gestionDOM.popoverOff();
	$("#tableBioseguridad").css("display", "none");
	$("#divSelectVista").attr("style", "display: none!important");
	var data = {
		id_visita: 	Bioseguridad.datosFilaTabla[0].DT_RowId
	};
	query.callAjax(Bioseguridad.urlConexionVista,
		"visita",
		data, 
		Bioseguridad.comunicaciones.visita);
}

//AJAX para eliminar una visita
Bioseguridad.funciones.delete = function(){
	gestionModal.alertaBloqueante("CARGANDO...");
	var data = {
		id_visita: 	Bioseguridad.datosFilaTabla[0].DT_RowId
	};
	query.callAjax(Bioseguridad.urlConexionVista,
		"delete_visita",
		data, 
		Bioseguridad.comunicaciones.delete);
}

//Se traen las urls de las fotos
Bioseguridad.funciones.camara = function(){
	var data = {
		id_indicador: 	$(this).parents("tr").data("id")
	};
	query.callAjax(Bioseguridad.urlConexionVista,
		"fotos",
		data, 
		Bioseguridad.comunicaciones.camara);
}