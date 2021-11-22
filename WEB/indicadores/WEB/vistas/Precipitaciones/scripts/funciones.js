Precipitaciones.funciones = new Object();

/* SELECTORES ANIDADOS */
/*funcion encargada de apagar las banderas de los demas selectores y activar la de Departamento,
para luego hacer la consulta que pide la informacion del selector*/
Precipitaciones.funciones.fillDepartamento = function(){
	if(Precipitaciones.filtros.departamento.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		Precipitaciones.filtros.departamento.state = 1;
		Precipitaciones.filtros.municipio.state = 0;
		Precipitaciones.filtros.finca.state = 0;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		Precipitaciones.filtros.departamento.uso = 1;
		//Se invoca la consulta
		Precipitaciones.funciones.consultaSelectores();
	}
}

/*funcion encargada de apagar las banderas de los demas selectores y activar la de Municipio,
para luego hacer la consulta que pide la informacion del selector*/
Precipitaciones.funciones.fillMunicipio = function(){
	if(Precipitaciones.filtros.municipio.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		Precipitaciones.filtros.departamento.state = 0;
		Precipitaciones.filtros.municipio.state = 1;
		Precipitaciones.filtros.finca.state = 0;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		Precipitaciones.filtros.municipio.uso = 1;
		//Se invoca la consulta
		Precipitaciones.funciones.consultaSelectores();
	}
}

/*funcion encargada de apagar las banderas de los demas selectores y activar la de finca,
para luego hacer la consulta que pide la informacion del selector*/
Precipitaciones.funciones.fillFinca = function(){
	if(Precipitaciones.filtros.finca.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		Precipitaciones.filtros.departamento.state = 0;
		Precipitaciones.filtros.municipio.state = 0;
		Precipitaciones.filtros.finca.state = 1;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		Precipitaciones.filtros.finca.uso = 1;
		//Se invoca la consulta
		Precipitaciones.funciones.consultaSelectores();
	}
}

//Funcion que reactivala recarga de los selectores
Precipitaciones.funciones.changeFlag = function(event){
	var select = event.data.select;
	/*Se identifica cual fue el selector que cambio y se apagana las banderas de 
	uso de los demas selectores para que se refresquen*/
	if(select == "selectDepartamento"){
		Precipitaciones.filtros.municipio.uso = 0;
		Precipitaciones.filtros.finca.uso = 0;
		//Se añaden los filtros
		if($(`#selectDepartamento`).val().length > 0){
			Precipitaciones.filtros.departamento.data = `${$(`#selectDepartamento`).val()}`;
		}	
		else{
			Precipitaciones.filtros.departamento.data = '0';
		}
	}
	else if(select == "selectMunicipio"){
		Precipitaciones.filtros.departamento.uso = 0;
		Precipitaciones.filtros.finca.uso = 0;
		//Se añaden los filtros
		if($("#selectMunicipio").val().length > 0){
			Precipitaciones.filtros.municipio.data = `${$(`#selectMunicipio`).val()}`;
		}
		else{
			Precipitaciones.filtros.municipio.data = '0';
		}
	}
	else if(select == "selectFinca"){
		Precipitaciones.filtros.departamento.uso = 0;
		Precipitaciones.filtros.municipio.uso = 0;
		//Se añaden los filtros
		if($("#selectFinca").val().length > 0){
			Precipitaciones.filtros.finca.data = `${$(`#selectFinca`).val()}`;
		}
		else{
			Precipitaciones.filtros.finca.data = '0';
		}
	}
}

//Funcion encargada de traer los selectores iniciales
Precipitaciones.funciones.consultaSelectores = function(){
    var data = {
    	filter: 	JSON.stringify(Precipitaciones.filtros)
    };
    query.callAjax(Precipitaciones.urlConexionVista,
        "consulta_selectores",
        data, 
        Precipitaciones.comunicaciones.consultaSelectores);
}
/* FIN SELECTORES ANIDADOS */

//Se trae las precipitaciones
Precipitaciones.funciones.precipitaciones = function(){
	if($("#selectFinca").val().length > 0){
		var data = {
			fecha: 		Precipitaciones.fechaIniRank,
			id_finca: 	`${$("#selectFinca").val()}`
		};
		query.callAjax(Precipitaciones.urlConexionVista,
			"bring_precipitaciones",
			data, 
			Precipitaciones.comunicaciones.precipitaciones);
	}
}

Precipitaciones.funciones.guardarPrecipitaciones = function(e){
	var resultado = e.data.resultado;
	var array = new Array();

	for(var i = 0; i < resultado.length; i++){
		var registros = new Object();
		registros.fecha = resultado[i].fecha.substring(0,10);
		registros.valor = $(`#${resultado[i].id}`).val().replaceAll('.', '').replaceAll(',','.');
		array[i] = registros;
	}

	var data = {
		data: 			JSON.stringify(array),
		id_finca: 		`${$("#selectFinca").val()}`,
		id_user: 		Login.ID_user
	};
	query.callAjax(Precipitaciones.urlConexionVista,
		"save_precipitaciones",
		data, 
		Precipitaciones.comunicaciones.guardarPrecipitaciones);
}