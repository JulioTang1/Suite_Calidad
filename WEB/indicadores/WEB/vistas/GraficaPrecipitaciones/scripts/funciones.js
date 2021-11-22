GraficaPrecipitaciones.funciones = new Object();

/* SELECTORES ANIDADOS */
/*funcion encargada de apagar las banderas de los demas selectores y activar la de Departamento,
para luego hacer la consulta que pide la informacion del selector*/
GraficaPrecipitaciones.funciones.fillDepartamento = function(){
	if(GraficaPrecipitaciones.filtros.departamento.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		GraficaPrecipitaciones.filtros.departamento.state = 1;
		GraficaPrecipitaciones.filtros.municipio.state = 0;
		GraficaPrecipitaciones.filtros.finca.state = 0;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		GraficaPrecipitaciones.filtros.departamento.uso = 1;
		//Se invoca la consulta
		GraficaPrecipitaciones.funciones.consultaSelectores();
	}
}

/*funcion encargada de apagar las banderas de los demas selectores y activar la de Municipio,
para luego hacer la consulta que pide la informacion del selector*/
GraficaPrecipitaciones.funciones.fillMunicipio = function(){
	if(GraficaPrecipitaciones.filtros.municipio.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		GraficaPrecipitaciones.filtros.departamento.state = 0;
		GraficaPrecipitaciones.filtros.municipio.state = 1;
		GraficaPrecipitaciones.filtros.finca.state = 0;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		GraficaPrecipitaciones.filtros.municipio.uso = 1;
		//Se invoca la consulta
		GraficaPrecipitaciones.funciones.consultaSelectores();
	}
}

/*funcion encargada de apagar las banderas de los demas selectores y activar la de finca,
para luego hacer la consulta que pide la informacion del selector*/
GraficaPrecipitaciones.funciones.fillFinca = function(){
	if(GraficaPrecipitaciones.filtros.finca.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		GraficaPrecipitaciones.filtros.departamento.state = 0;
		GraficaPrecipitaciones.filtros.municipio.state = 0;
		GraficaPrecipitaciones.filtros.finca.state = 1;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		GraficaPrecipitaciones.filtros.finca.uso = 1;
		//Se invoca la consulta
		GraficaPrecipitaciones.funciones.consultaSelectores();
	}
}

//Funcion que reactivala recarga de los selectores
GraficaPrecipitaciones.funciones.changeFlag = function(event){
	var select = event.data.select;
	/*Se identifica cual fue el selector que cambio y se apagana las banderas de 
	uso de los demas selectores para que se refresquen*/
	if(select == "selectDepartamento"){
		GraficaPrecipitaciones.filtros.municipio.uso = 0;
		GraficaPrecipitaciones.filtros.finca.uso = 0;
		//Se añaden los filtros
		if($(`#selectDepartamento`).val().length > 0){
			GraficaPrecipitaciones.filtros.departamento.data = `${$(`#selectDepartamento`).val()}`;
		}	
		else{
			GraficaPrecipitaciones.filtros.departamento.data = '0';
		}
	}
	else if(select == "selectMunicipio"){
		GraficaPrecipitaciones.filtros.departamento.uso = 0;
		GraficaPrecipitaciones.filtros.finca.uso = 0;
		//Se añaden los filtros
		if($("#selectMunicipio").val().length > 0){
			GraficaPrecipitaciones.filtros.municipio.data = `${$(`#selectMunicipio`).val()}`;
		}
		else{
			GraficaPrecipitaciones.filtros.municipio.data = '0';
		}
	}
	else if(select == "selectFinca"){
		GraficaPrecipitaciones.filtros.departamento.uso = 0;
		GraficaPrecipitaciones.filtros.municipio.uso = 0;
		//Se añaden los filtros
		if($("#selectFinca").val().length > 0){
			GraficaPrecipitaciones.filtros.finca.data = `${$(`#selectFinca`).val()}`;
		}
		else{
			GraficaPrecipitaciones.filtros.finca.data = '0';
		}
	}
}

//Funcion encargada de traer los selectores iniciales
GraficaPrecipitaciones.funciones.consultaSelectores = function(){
    var data = {
    	filter: 	JSON.stringify(GraficaPrecipitaciones.filtros)
    };
    query.callAjax(GraficaPrecipitaciones.urlConexionVista,
        "consulta_selectores",
        data, 
        GraficaPrecipitaciones.comunicaciones.consultaSelectores);
}
/* FIN SELECTORES ANIDADOS */