Consolidado.funciones = new Object();

//AJAX para la tabla
Consolidado.funciones.genTable1 = function(){
    var replace = {		
		fecha_ini: 	Consolidado.fechaIniRank,
		fecha_fin: 	Consolidado.fechaFinRank,
        fincas: 	`${$("#selectFinca").val()}`
	};

    gestionDT.initTable(
    	Consolidado.urlConexion,
    	"parametros", 
    	"#tablaParametros", 
    	Consolidado.comunicaciones.tablaCargada,
    	0,
        replace
    );
}

//AJAX para la tabla
Consolidado.funciones.genTable2 = function(){
    var replace = {		
		fecha_ini: 	Consolidado.fechaIniRank,
		fecha_fin: 	Consolidado.fechaFinRank,
        fincas: 	`${$("#selectFinca").val()}`
	};

    gestionDT.initTable(
    	Consolidado.urlConexion,
    	"promedio", 
    	"#tablaPromedio", 
    	Consolidado.comunicaciones.tablaCargada,
    	1,
        replace
    );
}

/* SELECTORES ANIDADOS */
/*funcion encargada de apagar las banderas de los demas selectores y activar la de Departamento,
para luego hacer la consulta que pide la informacion del selector*/
Consolidado.funciones.fillDepartamento = function(){
	if(Consolidado.filtros.departamento.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		Consolidado.filtros.departamento.state = 1;
		Consolidado.filtros.municipio.state = 0;
		Consolidado.filtros.finca.state = 0;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		Consolidado.filtros.departamento.uso = 1;
		//Se invoca la consulta
		Consolidado.funciones.consultaSelectores();
	}
}

/*funcion encargada de apagar las banderas de los demas selectores y activar la de Municipio,
para luego hacer la consulta que pide la informacion del selector*/
Consolidado.funciones.fillMunicipio = function(){
	if(Consolidado.filtros.municipio.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		Consolidado.filtros.departamento.state = 0;
		Consolidado.filtros.municipio.state = 1;
		Consolidado.filtros.finca.state = 0;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		Consolidado.filtros.municipio.uso = 1;
		//Se invoca la consulta
		Consolidado.funciones.consultaSelectores();
	}
}

/*funcion encargada de apagar las banderas de los demas selectores y activar la de finca,
para luego hacer la consulta que pide la informacion del selector*/
Consolidado.funciones.fillFinca = function(){
	if(Consolidado.filtros.finca.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		Consolidado.filtros.departamento.state = 0;
		Consolidado.filtros.municipio.state = 0;
		Consolidado.filtros.finca.state = 1;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		Consolidado.filtros.finca.uso = 1;
		//Se invoca la consulta
		Consolidado.funciones.consultaSelectores();
	}
}

//Funcion que reactivala recarga de los selectores
Consolidado.funciones.changeFlag = function(event){
	var select = event.data.select;
	/*Se identifica cual fue el selector que cambio y se apagana las banderas de 
	uso de los demas selectores para que se refresquen*/
	if(select == "selectDepartamento"){
		Consolidado.filtros.municipio.uso = 0;
		Consolidado.filtros.finca.uso = 0;
		//Se añaden los filtros
		if($(`#selectDepartamento`).val().length > 0){
			Consolidado.filtros.departamento.data = `${$(`#selectDepartamento`).val()}`;
		}	
		else{
			Consolidado.filtros.departamento.data = '0';
		}
	}
	else if(select == "selectMunicipio"){
		Consolidado.filtros.departamento.uso = 0;
		Consolidado.filtros.finca.uso = 0;
		//Se añaden los filtros
		if($("#selectMunicipio").val().length > 0){
			Consolidado.filtros.municipio.data = `${$(`#selectMunicipio`).val()}`;
		}
		else{
			Consolidado.filtros.municipio.data = '0';
		}
	}
	else if(select == "selectFinca"){
		Consolidado.filtros.departamento.uso = 0;
		Consolidado.filtros.municipio.uso = 0;
		//Se añaden los filtros
		if($("#selectFinca").val().length > 0){
			Consolidado.filtros.finca.data = `${$(`#selectFinca`).val()}`;
		}
		else{
			Consolidado.filtros.finca.data = '0';
		}
	}
}

//Funcion encargada de traer los selectores iniciales
Consolidado.funciones.consultaSelectores = function(){
    var data = {
    	filter: 	JSON.stringify(Consolidado.filtros)
    };
    query.callAjax(Consolidado.urlConexionVista,
        "consulta_selectores",
        data, 
        Consolidado.comunicaciones.consultaSelectores);
}
/* FIN SELECTORES ANIDADOS */