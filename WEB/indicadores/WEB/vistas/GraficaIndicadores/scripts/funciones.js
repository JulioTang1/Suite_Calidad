GraficaIndicadores.funciones = new Object();

/* SELECTORES ANIDADOS */
/*funcion encargada de apagar las banderas de los demas selectores y activar la de Departamento,
para luego hacer la consulta que pide la informacion del selector*/
GraficaIndicadores.funciones.fillDepartamento = function(){
	if(GraficaIndicadores.filtros.departamento.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		GraficaIndicadores.filtros.departamento.state = 1;
		GraficaIndicadores.filtros.municipio.state = 0;
		GraficaIndicadores.filtros.finca.state = 0;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		GraficaIndicadores.filtros.departamento.uso = 1;
		//Se invoca la consulta
		GraficaIndicadores.funciones.consultaSelectores();
	}
}

/*funcion encargada de apagar las banderas de los demas selectores y activar la de Municipio,
para luego hacer la consulta que pide la informacion del selector*/
GraficaIndicadores.funciones.fillMunicipio = function(){
	if(GraficaIndicadores.filtros.municipio.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		GraficaIndicadores.filtros.departamento.state = 0;
		GraficaIndicadores.filtros.municipio.state = 1;
		GraficaIndicadores.filtros.finca.state = 0;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		GraficaIndicadores.filtros.municipio.uso = 1;
		//Se invoca la consulta
		GraficaIndicadores.funciones.consultaSelectores();
	}
}

/*funcion encargada de apagar las banderas de los demas selectores y activar la de finca,
para luego hacer la consulta que pide la informacion del selector*/
GraficaIndicadores.funciones.fillFinca = function(){
	if(GraficaIndicadores.filtros.finca.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		GraficaIndicadores.filtros.departamento.state = 0;
		GraficaIndicadores.filtros.municipio.state = 0;
		GraficaIndicadores.filtros.finca.state = 1;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		GraficaIndicadores.filtros.finca.uso = 1;
		//Se invoca la consulta
		GraficaIndicadores.funciones.consultaSelectores();
	}
}

//Funcion que reactivala recarga de los selectores
GraficaIndicadores.funciones.changeFlag = function(event){
	var select = event.data.select;
	/*Se identifica cual fue el selector que cambio y se apagana las banderas de 
	uso de los demas selectores para que se refresquen*/
	if(select == "selectDepartamento"){
		GraficaIndicadores.filtros.municipio.uso = 0;
		GraficaIndicadores.filtros.finca.uso = 0;
		//Se añaden los filtros
		if($(`#selectDepartamento`).val().length > 0){
			GraficaIndicadores.filtros.departamento.data = `${$(`#selectDepartamento`).val()}`;
		}	
		else{
			GraficaIndicadores.filtros.departamento.data = '0';
		}
	}
	else if(select == "selectMunicipio"){
		GraficaIndicadores.filtros.departamento.uso = 0;
		GraficaIndicadores.filtros.finca.uso = 0;
		//Se añaden los filtros
		if($("#selectMunicipio").val().length > 0){
			GraficaIndicadores.filtros.municipio.data = `${$(`#selectMunicipio`).val()}`;
		}
		else{
			GraficaIndicadores.filtros.municipio.data = '0';
		}
	}
	else if(select == "selectFinca"){
		GraficaIndicadores.filtros.departamento.uso = 0;
		GraficaIndicadores.filtros.municipio.uso = 0;
		//Se añaden los filtros
		if($("#selectFinca").val().length > 0){
			GraficaIndicadores.filtros.finca.data = `${$(`#selectFinca`).val()}`;
		}
		else{
			GraficaIndicadores.filtros.finca.data = '0';
		}
	}
}

//Funcion encargada de traer los selectores iniciales
GraficaIndicadores.funciones.consultaSelectores = function(){
    var data = {
    	filter: 	JSON.stringify(GraficaIndicadores.filtros)
    };
    query.callAjax(GraficaIndicadores.urlConexionVista,
        "consulta_selectores",
        data, 
        GraficaIndicadores.comunicaciones.consultaSelectores);
}
/* FIN SELECTORES ANIDADOS */

/* SELECTORES ANIDADOS (edades infecciones) */
/*funcion encargada de apagar las banderas de los demas selectores y activar la de Departamento,
para luego hacer la consulta que pide la informacion del selector*/
GraficaIndicadores.funciones.fillEdad = function(){
	if(GraficaIndicadores.filtrosEI.edad.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		GraficaIndicadores.filtrosEI.edad.state = 1;
		GraficaIndicadores.filtrosEI.indicador.state = 0;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		GraficaIndicadores.filtrosEI.edad.uso = 1;
		//Se invoca la consulta
		GraficaIndicadores.funciones.consultaSelectoresEI();
	}
}

/*funcion encargada de apagar las banderas de los demas selectores y activar la de Municipio,
para luego hacer la consulta que pide la informacion del selector*/
GraficaIndicadores.funciones.fillInfeccion = function(){
	if(GraficaIndicadores.filtrosEI.indicador.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		GraficaIndicadores.filtrosEI.edad.state = 0;
		GraficaIndicadores.filtrosEI.indicador.state = 1;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		GraficaIndicadores.filtrosEI.indicador.uso = 1;
		//Se invoca la consulta
		GraficaIndicadores.funciones.consultaSelectoresEI();
	}
}

//Funcion que reactivala recarga de los selectores
GraficaIndicadores.funciones.changeFlagEI = function(event){
	var select = event.data.select;
	/*Se identifica cual fue el selector que cambio y se apagana las banderas de 
	uso de los demas selectores para que se refresquen*/
	if(select == "selectEdad"){
		GraficaIndicadores.filtrosEI.indicador.uso = 0;
		//Se añaden los filtros
		if($(`#selectEdad`).val().length > 0){
			GraficaIndicadores.filtrosEI.edad.data = `${$(`#selectEdad`).val()}`;
		}	
		else{
			GraficaIndicadores.filtrosEI.edad.data = '0';
		}
	}
	else if(select == "selectInfeccion"){
		GraficaIndicadores.filtrosEI.edad.uso = 0;
		//Se añaden los filtros
		if($("#selectInfeccion").val().length > 0){
			GraficaIndicadores.filtrosEI.indicador.data = `${$(`#selectInfeccion`).val()}`;
		}
		else{
			GraficaIndicadores.filtrosEI.indicador.data = '0';
		}
	}
}

//Funcion encargada de traer los selectores iniciales
GraficaIndicadores.funciones.consultaSelectoresEI = function(){
    var data = {
    	filter: 	JSON.stringify(GraficaIndicadores.filtrosEI)
    };
    query.callAjax(GraficaIndicadores.urlConexionVista,
        "consulta_selectores_ei",
        data, 
        GraficaIndicadores.comunicaciones.consultaSelectoresEI);
}
/* FIN SELECTORES ANIDADOS (edades infecciones) */