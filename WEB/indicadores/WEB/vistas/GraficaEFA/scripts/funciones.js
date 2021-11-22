GraficaEFA.funciones = new Object();

/* SELECTORES ANIDADOS */
/*funcion encargada de apagar las banderas de los demas selectores y activar la de Departamento,
para luego hacer la consulta que pide la informacion del selector*/
GraficaEFA.funciones.fillDepartamento = function(){
	if(GraficaEFA.filtros.departamento.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		GraficaEFA.filtros.departamento.state = 1;
		GraficaEFA.filtros.municipio.state = 0;
		GraficaEFA.filtros.finca.state = 0;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		GraficaEFA.filtros.departamento.uso = 1;
		//Se invoca la consulta
		GraficaEFA.funciones.consultaSelectores();
	}
}

/*funcion encargada de apagar las banderas de los demas selectores y activar la de Municipio,
para luego hacer la consulta que pide la informacion del selector*/
GraficaEFA.funciones.fillMunicipio = function(){
	if(GraficaEFA.filtros.municipio.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		GraficaEFA.filtros.departamento.state = 0;
		GraficaEFA.filtros.municipio.state = 1;
		GraficaEFA.filtros.finca.state = 0;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		GraficaEFA.filtros.municipio.uso = 1;
		//Se invoca la consulta
		GraficaEFA.funciones.consultaSelectores();
	}
}

/*funcion encargada de apagar las banderas de los demas selectores y activar la de finca,
para luego hacer la consulta que pide la informacion del selector*/
GraficaEFA.funciones.fillFinca = function(){
	if(GraficaEFA.filtros.finca.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		GraficaEFA.filtros.departamento.state = 0;
		GraficaEFA.filtros.municipio.state = 0;
		GraficaEFA.filtros.finca.state = 1;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		GraficaEFA.filtros.finca.uso = 1;
		//Se invoca la consulta
		GraficaEFA.funciones.consultaSelectores();
	}
}

//Funcion que reactivala recarga de los selectores
GraficaEFA.funciones.changeFlag = function(event){
	var select = event.data.select;
	/*Se identifica cual fue el selector que cambio y se apagana las banderas de 
	uso de los demas selectores para que se refresquen*/
	if(select == "selectDepartamento"){
		GraficaEFA.filtros.municipio.uso = 0;
		GraficaEFA.filtros.finca.uso = 0;
		//Se añaden los filtros
		if($(`#selectDepartamento`).val().length > 0){
			GraficaEFA.filtros.departamento.data = `${$(`#selectDepartamento`).val()}`;
		}	
		else{
			GraficaEFA.filtros.departamento.data = '0';
		}
	}
	else if(select == "selectMunicipio"){
		GraficaEFA.filtros.departamento.uso = 0;
		GraficaEFA.filtros.finca.uso = 0;
		//Se añaden los filtros
		if($("#selectMunicipio").val().length > 0){
			GraficaEFA.filtros.municipio.data = `${$(`#selectMunicipio`).val()}`;
		}
		else{
			GraficaEFA.filtros.municipio.data = '0';
		}
	}
	else if(select == "selectFinca"){
		GraficaEFA.filtros.departamento.uso = 0;
		GraficaEFA.filtros.municipio.uso = 0;
		//Se añaden los filtros
		if($("#selectFinca").val().length > 0){
			GraficaEFA.filtros.finca.data = `${$(`#selectFinca`).val()}`;
		}
		else{
			GraficaEFA.filtros.finca.data = '0';
		}
	}
}

//Funcion encargada de traer los selectores iniciales
GraficaEFA.funciones.consultaSelectores = function(){
    var data = {
    	filter: 	JSON.stringify(GraficaEFA.filtros)
    };
    query.callAjax(GraficaEFA.urlConexionVista,
        "consulta_selectores",
        data, 
        GraficaEFA.comunicaciones.consultaSelectores);
}
/* FIN SELECTORES ANIDADOS */