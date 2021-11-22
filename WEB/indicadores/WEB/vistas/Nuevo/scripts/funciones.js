Nuevo.funciones = new Object();

/* SELECTORES ANIDADOS */
/*funcion encargada de apagar las banderas de los demas selectores y activar la de Departamento,
para luego hacer la consulta que pide la informacion del selector*/
Nuevo.funciones.fillDepartamento = function(){
	if(Nuevo.filtros.departamento.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		Nuevo.filtros.departamento.state = 1;
		Nuevo.filtros.municipio.state = 0;
		Nuevo.filtros.finca.state = 0;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		Nuevo.filtros.departamento.uso = 1;
		//Se invoca la consulta
		Nuevo.funciones.consultaSelectores();
	}
}

/*funcion encargada de apagar las banderas de los demas selectores y activar la de Municipio,
para luego hacer la consulta que pide la informacion del selector*/
Nuevo.funciones.fillMunicipio = function(){
	if(Nuevo.filtros.municipio.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		Nuevo.filtros.departamento.state = 0;
		Nuevo.filtros.municipio.state = 1;
		Nuevo.filtros.finca.state = 0;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		Nuevo.filtros.municipio.uso = 1;
		//Se invoca la consulta
		Nuevo.funciones.consultaSelectores();
	}
}

/*funcion encargada de apagar las banderas de los demas selectores y activar la de finca,
para luego hacer la consulta que pide la informacion del selector*/
Nuevo.funciones.fillFinca = function(){
	if(Nuevo.filtros.finca.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		Nuevo.filtros.departamento.state = 0;
		Nuevo.filtros.municipio.state = 0;
		Nuevo.filtros.finca.state = 1;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		Nuevo.filtros.finca.uso = 1;
		//Se invoca la consulta
		Nuevo.funciones.consultaSelectores();
	}
}

//Funcion que reactivala recarga de los selectores
Nuevo.funciones.changeFlag = function(event){
	var select = event.data.select;
	/*Se identifica cual fue el selector que cambio y se apagana las banderas de 
	uso de los demas selectores para que se refresquen*/
	if(select == "selectDepartamento"){
		Nuevo.filtros.municipio.uso = 0;
		Nuevo.filtros.finca.uso = 0;
		//Se añaden los filtros
		if($(`#selectDepartamento`).val().length > 0){
			Nuevo.filtros.departamento.data = `${$(`#selectDepartamento`).val()}`;
		}	
		else{
			Nuevo.filtros.departamento.data = '0';
		}
	}
	else if(select == "selectMunicipio"){
		Nuevo.filtros.departamento.uso = 0;
		Nuevo.filtros.finca.uso = 0;
		//Se añaden los filtros
		if($("#selectMunicipio").val().length > 0){
			Nuevo.filtros.municipio.data = `${$(`#selectMunicipio`).val()}`;
		}
		else{
			Nuevo.filtros.municipio.data = '0';
		}
	}
	else if(select == "selectFinca"){
		Nuevo.filtros.departamento.uso = 0;
		Nuevo.filtros.municipio.uso = 0;
		//Se añaden los filtros
		if($("#selectFinca").val().length > 0){
			Nuevo.filtros.finca.data = `${$(`#selectFinca`).val()}`;
		}
		else{
			Nuevo.filtros.finca.data = '0';
		}
	}
}

//Funcion encargada de traer los selectores iniciales
Nuevo.funciones.consultaSelectores = function(){
    var data = {
    	filter: 	JSON.stringify(Nuevo.filtros)
    };
    query.callAjax(Nuevo.urlConexionVista,
        "consulta_selectores",
        data, 
        Nuevo.comunicaciones.consultaSelectores);
}
/* FIN SELECTORES ANIDADOS */

//AJAX para traer las visitas
Nuevo.funciones.bringVisita = function(e, activeEdicion){
	if($("#selectFinca").val().length > 0){
		var data = {
			fecha: 		Nuevo.fechaIniRank,
			id_finca: 	`${$("#selectFinca").val()}`
		};
		query.callAjax(Nuevo.urlConexionVista,
			"select_visita",
			data, 
			Nuevo.comunicaciones.bringVisita,
			activeEdicion);
	}else{
		var vacia = new Array();
		Nuevo.gestionDOM.fillSelectores(vacia,"#selectVisita");
	}
}

//Se trae los datos de recorridos si no es nuevo
Nuevo.funciones.datos = function(){
	if ( $("#selectVisita").val() != "" && $("#selectVisita").val() != null ){
		$("#main").addClass("spinnerPosition");
		$("#main").html('<div class="spinner-border text-primary"></div>');
		
		var data = {
			fecha: 		Nuevo.fechaIniRank,
			id_visita: 	`${$("#selectVisita").val()}`,
			id_finca: 	`${$("#selectFinca").val()}`
		};
		query.callAjax(Nuevo.urlConexionVista,
			"bring_visita",
			data, 
			Nuevo.comunicaciones.datos);
	}else{
		$("#main").empty();
	}
}

Nuevo.funciones.guardar = function(){
	//abre el modal
	gestionModal.alertaBloqueante("CARGANDO...");

	var obj = new Object();

	//Datos iniciales de la visita
	obj.id_visita = `${$("#selectVisita").val()}`;
	obj.id_finca = `${$("#selectFinca").val()}`;
	obj.fecha = Nuevo.fechaIniRank;
	obj.id_user = Login.ID_user;
	obj.precipitacion = $("#precipitacion").val().replaceAll('.', '');
	obj.temperatura_minima = $("#temperaturaMinima").val().replaceAll('.', '');
	obj.temperatura = $("#temperatura").val().replaceAll('.', '');
	obj.temperatura_maxima = $("#temperaturaMaxima").val().replaceAll('.', '');
	obj.humedad  = $("#humedad").val().replaceAll('.', '');

	obj.pp = new Array();
	obj.viisem = new Array();
	obj.xsem = new Array();
	obj.pfsem = new Array();
	for(var i = 1; i <= 15; i++){
		//Proxima a parir
		var pp = new Object();
		pp.ID = $(`#pxp .row-${i}`).data("id");
		pp.ORDEN = i;
		pp.TH = $(`#pxp_th_${i}`).val().replaceAll('.', '');
		pp.YLI = $(`#pxp_yli_${i}`).val().replaceAll('.', '');
		pp.YLS = $(`#pxp_yls_${i}`).val().replaceAll('.', '');
		pp.CF = $(`#pxp_cf_${i}`).data("url");
		pp.Lote = $(`#pxp_lote_${i}`).val().replaceAll('.', '');
		obj.pp[i-1] = pp;

		//7 semanas
		var viisem = new Object();
		viisem.ID = $(`#viisem .row-${i}`).data("id");
		viisem.ORDEN = i;
		viisem.YLS = $(`#viisem_yls_${i}`).val().replaceAll('.', '');
		viisem.CF = $(`#viisem_cf_${i}`).data("url");
		viisem.HF = $(`#viisem_hf_${i}`).val().replaceAll('.', '');
		viisem.Lote = $(`#viisem_lote_${i}`).val().replaceAll('.', '');
		obj.viisem[i-1] = viisem;

		//10 semanas
		var xsem = new Object();
		xsem.ID = $(`#xsem .row-${i}`).data("id");
		xsem.ORDEN = i;
		xsem.YLS = $(`#xsem_yls_${i}`).val().replaceAll('.', '');
		xsem.CF = $(`#xsem_cf_${i}`).data("url");
		xsem.HF = $(`#xsem_hf_${i}`).val().replaceAll('.', '');
		xsem.Lote = $(`#xsem_lote_${i}`).val().replaceAll('.', '');
		obj.xsem[i-1] = xsem;

		//parcela fija
		pfsem = new Object();
		pfsem.ID = $(`#pfsem .row-${i}`).data("id");
		pfsem.ORDEN = i;
		pfsem.TH = $(`#pfsem_th_${i}`).val().replaceAll('.', '');
		pfsem.EFA = $(`#pfsem_efa_${i}`).val().replaceAll('.', '');
		pfsem.CF = $(`#pfsem_cf_${i}`).data("url");
		pfsem.H2 = $(`#pfsem_h2_${i}`).val();
		pfsem.H3 = $(`#pfsem_h3_${i}`).val();
		pfsem.H4 = $(`#pfsem_h4_${i}`).val();
		pfsem.Lote = $(`#pfsem_lote_${i}`).val().replaceAll('.', '');
		obj.pfsem[i-1] = pfsem;
	}
	var data = {
		json: 			JSON.stringify(obj).replaceAll('\"\"', 'null')
	};
	var activeEdicion = obj.id_visita == 'undefined' ? undefined : 1;
	query.callAjax(Nuevo.urlConexionVista,
		"insert_visita",
		data, 
		Nuevo.comunicaciones.guardar,
		activeEdicion);
}