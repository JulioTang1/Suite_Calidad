GraficaPorcentaje.funciones = new Object();

/* SELECTORES ANIDADOS */
/*funcion encargada de apagar las banderas de los demas selectores y activar la de Departamento,
para luego hacer la consulta que pide la informacion del selector*/
GraficaPorcentaje.funciones.fillDepartamento = function(){
	if(GraficaPorcentaje.filtros.departamento.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		GraficaPorcentaje.filtros.departamento.state = 1;
		GraficaPorcentaje.filtros.municipio.state = 0;
		GraficaPorcentaje.filtros.finca.state = 0;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		GraficaPorcentaje.filtros.departamento.uso = 1;
		//Se invoca la consulta
		GraficaPorcentaje.funciones.consultaSelectores();
	}
}

/*funcion encargada de apagar las banderas de los demas selectores y activar la de Municipio,
para luego hacer la consulta que pide la informacion del selector*/
GraficaPorcentaje.funciones.fillMunicipio = function(){
	if(GraficaPorcentaje.filtros.municipio.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		GraficaPorcentaje.filtros.departamento.state = 0;
		GraficaPorcentaje.filtros.municipio.state = 1;
		GraficaPorcentaje.filtros.finca.state = 0;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		GraficaPorcentaje.filtros.municipio.uso = 1;
		//Se invoca la consulta
		GraficaPorcentaje.funciones.consultaSelectores();
	}
}

/*funcion encargada de apagar las banderas de los demas selectores y activar la de finca,
para luego hacer la consulta que pide la informacion del selector*/
GraficaPorcentaje.funciones.fillFinca = function(){
	if(GraficaPorcentaje.filtros.finca.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		GraficaPorcentaje.filtros.departamento.state = 0;
		GraficaPorcentaje.filtros.municipio.state = 0;
		GraficaPorcentaje.filtros.finca.state = 1;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		GraficaPorcentaje.filtros.finca.uso = 1;
		//Se invoca la consulta
		GraficaPorcentaje.funciones.consultaSelectores();
	}
}

//Funcion que reactivala recarga de los selectores
GraficaPorcentaje.funciones.changeFlag = function(event){
	var select = event.data.select;
	/*Se identifica cual fue el selector que cambio y se apagana las banderas de 
	uso de los demas selectores para que se refresquen*/
	if(select == "selectDepartamento"){
		GraficaPorcentaje.filtros.municipio.uso = 0;
		GraficaPorcentaje.filtros.finca.uso = 0;
		//Se añaden los filtros
		if($(`#selectDepartamento`).val().length > 0){
			GraficaPorcentaje.filtros.departamento.data = `${$(`#selectDepartamento`).val()}`;
		}	
		else{
			GraficaPorcentaje.filtros.departamento.data = '0';
		}
	}
	else if(select == "selectMunicipio"){
		GraficaPorcentaje.filtros.departamento.uso = 0;
		GraficaPorcentaje.filtros.finca.uso = 0;
		//Se añaden los filtros
		if($("#selectMunicipio").val().length > 0){
			GraficaPorcentaje.filtros.municipio.data = `${$(`#selectMunicipio`).val()}`;
		}
		else{
			GraficaPorcentaje.filtros.municipio.data = '0';
		}
	}
	else if(select == "selectFinca"){
		GraficaPorcentaje.filtros.departamento.uso = 0;
		GraficaPorcentaje.filtros.municipio.uso = 0;
		//Se añaden los filtros
		if($("#selectFinca").val().length > 0){
			GraficaPorcentaje.filtros.finca.data = `${$(`#selectFinca`).val()}`;
		}
		else{
			GraficaPorcentaje.filtros.finca.data = '0';
		}
	}
}

//Funcion encargada de traer los selectores iniciales
GraficaPorcentaje.funciones.consultaSelectores = function(){
    var data = {
    	filter: 	JSON.stringify(GraficaPorcentaje.filtros)
    };
    query.callAjax(GraficaPorcentaje.urlConexionVista,
        "consulta_selectores",
        data, 
        GraficaPorcentaje.comunicaciones.consultaSelectores);
}
/* FIN SELECTORES ANIDADOS */

//Trae la informacion del selector grupos
GraficaPorcentaje.funciones.bringGrupos = function(){
    query.callAjax(GraficaPorcentaje.urlConexionVista,
        "metas",
        0, 
        GraficaPorcentaje.comunicaciones.bringGrupos);
}