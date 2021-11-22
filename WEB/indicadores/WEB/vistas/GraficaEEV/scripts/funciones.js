GraficaEEV.funciones = new Object();

/* SELECTORES ANIDADOS */
/*funcion encargada de apagar las banderas de los demas selectores y activar la de Departamento,
para luego hacer la consulta que pide la informacion del selector*/
GraficaEEV.funciones.fillDepartamento = function(){
	if(GraficaEEV.filtros.departamento.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		GraficaEEV.filtros.departamento.state = 1;
		GraficaEEV.filtros.municipio.state = 0;
		GraficaEEV.filtros.finca.state = 0;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		GraficaEEV.filtros.departamento.uso = 1;
		//Se invoca la consulta
		GraficaEEV.funciones.consultaSelectores();
	}
}

/*funcion encargada de apagar las banderas de los demas selectores y activar la de Municipio,
para luego hacer la consulta que pide la informacion del selector*/
GraficaEEV.funciones.fillMunicipio = function(){
	if(GraficaEEV.filtros.municipio.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		GraficaEEV.filtros.departamento.state = 0;
		GraficaEEV.filtros.municipio.state = 1;
		GraficaEEV.filtros.finca.state = 0;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		GraficaEEV.filtros.municipio.uso = 1;
		//Se invoca la consulta
		GraficaEEV.funciones.consultaSelectores();
	}
}

/*funcion encargada de apagar las banderas de los demas selectores y activar la de finca,
para luego hacer la consulta que pide la informacion del selector*/
GraficaEEV.funciones.fillFinca = function(){
	if(GraficaEEV.filtros.finca.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		GraficaEEV.filtros.departamento.state = 0;
		GraficaEEV.filtros.municipio.state = 0;
		GraficaEEV.filtros.finca.state = 1;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		GraficaEEV.filtros.finca.uso = 1;
		//Se invoca la consulta
		GraficaEEV.funciones.consultaSelectores();
	}
}

//Funcion que reactivala recarga de los selectores
GraficaEEV.funciones.changeFlag = function(event){
	var select = event.data.select;
	/*Se identifica cual fue el selector que cambio y se apagana las banderas de 
	uso de los demas selectores para que se refresquen*/
	if(select == "selectDepartamento"){
		GraficaEEV.filtros.municipio.uso = 0;
		GraficaEEV.filtros.finca.uso = 0;
		//Se añaden los filtros
		if($(`#selectDepartamento`).val().length > 0){
			GraficaEEV.filtros.departamento.data = `${$(`#selectDepartamento`).val()}`;
		}	
		else{
			GraficaEEV.filtros.departamento.data = '0';
		}
	}
	else if(select == "selectMunicipio"){
		GraficaEEV.filtros.departamento.uso = 0;
		GraficaEEV.filtros.finca.uso = 0;
		//Se añaden los filtros
		if($("#selectMunicipio").val().length > 0){
			GraficaEEV.filtros.municipio.data = `${$(`#selectMunicipio`).val()}`;
		}
		else{
			GraficaEEV.filtros.municipio.data = '0';
		}
	}
	else if(select == "selectFinca"){
		GraficaEEV.filtros.departamento.uso = 0;
		GraficaEEV.filtros.municipio.uso = 0;
		//Se añaden los filtros
		if($("#selectFinca").val().length > 0){
			GraficaEEV.filtros.finca.data = `${$(`#selectFinca`).val()}`;
		}
		else{
			GraficaEEV.filtros.finca.data = '0';
		}
	}
}

//Funcion encargada de traer los selectores iniciales
GraficaEEV.funciones.consultaSelectores = function(){
    var data = {
    	filter: 	JSON.stringify(GraficaEEV.filtros)
    };
    query.callAjax(GraficaEEV.urlConexionVista,
        "consulta_selectores",
        data, 
        GraficaEEV.comunicaciones.consultaSelectores);
}
/* FIN SELECTORES ANIDADOS */