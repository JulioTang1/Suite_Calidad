LimiteIndicadores.funciones = new Object();

//Se trae la informacion de los limites de indicadores
LimiteIndicadores.funciones.info = function(){
    if( $(`#selectEdad`).val() > 0 && $(`#selectInfeccion`).val().length > 0 && $(`#selectTipoFinca`).val().length > 0 ){
        var data = {
            edad: 		`${$(`#selectEdad`).val()}`,
            infeccion: 	`${$(`#selectInfeccion`).val()}`,
			tipoFinca: 	`${$(`#selectTipoFinca`).val()}`
        };
        query.callAjax(LimiteIndicadores.urlConexionVista,
            "info",
            data, 
            LimiteIndicadores.comunicaciones.info);
    }else{
        //Se ocultan los colores y la tabla
        $(".LimiteIndicadoresExterno").css("display", "none");
		$(".form-check").addClass("oculto");
		$(".labelSemaforo").addClass("oculto");
	}
}

//Guarda los nuevos colores, min y max
LimiteIndicadores.funciones.save = function(){
    var data = {
        min:        		parseFloat($(".lim-val").eq(0).html()),
        max:        		parseFloat($(".lim-val").eq(1).html()),
        color1:     		$("#color1").val(),
        color2:     		$("#color2").val(),
        color3:     		$("#color3").val(),
        edad: 				`${$(`#selectEdad`).val()}`,
        infeccion: 			`${$(`#selectInfeccion`).val()}`,
		tipoFinca: 			`${$(`#selectTipoFinca`).val()}`,
		semaforo_activo:	$("#semaforoActivo").prop("checked") == true ? 1 : 0,
		radio:				parseFloat($(".lim-val").eq(2).html())
    };
    query.callAjax(LimiteIndicadores.urlConexionVista,
        "save",
        data, 
        LimiteIndicadores.comunicaciones.save);
}

/* SELECTORES ANIDADOS (edades infecciones) */
/*funcion encargada de apagar las banderas de los demas selectores y activar la de Departamento,
para luego hacer la consulta que pide la informacion del selector*/
LimiteIndicadores.funciones.fillEdad = function(){
	if(LimiteIndicadores.filtrosEI.edad.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		LimiteIndicadores.filtrosEI.edad.state = 1;
		LimiteIndicadores.filtrosEI.indicador.state = 0;
		LimiteIndicadores.filtrosEI.tipoFinca.state = 0;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		LimiteIndicadores.filtrosEI.edad.uso = 1;
		//Se invoca la consulta
		LimiteIndicadores.funciones.consultaSelectoresEI();
	}
}

/*funcion encargada de apagar las banderas de los demas selectores y activar la de Municipio,
para luego hacer la consulta que pide la informacion del selector*/
LimiteIndicadores.funciones.fillInfeccion = function(){
	if(LimiteIndicadores.filtrosEI.indicador.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		LimiteIndicadores.filtrosEI.edad.state = 0;
		LimiteIndicadores.filtrosEI.indicador.state = 1;
		LimiteIndicadores.filtrosEI.tipoFinca.state = 0;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		LimiteIndicadores.filtrosEI.indicador.uso = 1;
		//Se invoca la consulta
		LimiteIndicadores.funciones.consultaSelectoresEI();
	}
}

/*funcion encargada de apagar las banderas de los demas selectores y activar la de Municipio,
para luego hacer la consulta que pide la informacion del selector*/
LimiteIndicadores.funciones.fillTipoFinca = function(){
	if(LimiteIndicadores.filtrosEI.tipoFinca.uso == 0){
		//abre el modal
		gestionModal.alertaBloqueante("CARGANDO...");
		//Con state indico para cual selector es la consulta
		LimiteIndicadores.filtrosEI.edad.state = 0;
		LimiteIndicadores.filtrosEI.indicador.state = 0;
		LimiteIndicadores.filtrosEI.tipoFinca.state = 1;
		//El uso indica si debe cargarse el selector o ya fue cargado = 1
		LimiteIndicadores.filtrosEI.tipoFinca.uso = 1;
		//Se invoca la consulta
		LimiteIndicadores.funciones.consultaSelectoresEI();
	}
}

//Funcion que reactivala recarga de los selectores
LimiteIndicadores.funciones.changeFlagEI = function(event){
	var select = event.data.select;
	/*Se identifica cual fue el selector que cambio y se apagana las banderas de 
	uso de los demas selectores para que se refresquen*/
	if(select == "selectEdad"){
		LimiteIndicadores.filtrosEI.indicador.uso = 0;
		LimiteIndicadores.filtrosEI.tipoFinca.uso = 0;
		//Se añaden los filtros
		if($(`#selectEdad`).val().length > 0){
			LimiteIndicadores.filtrosEI.edad.data = `${$(`#selectEdad`).val()}`;
		}	
		else{
			LimiteIndicadores.filtrosEI.edad.data = '0';
		}
	}
	else if(select == "selectInfeccion"){
		LimiteIndicadores.filtrosEI.edad.uso = 0;
		LimiteIndicadores.filtrosEI.tipoFinca.uso = 0;
		//Se añaden los filtros
		if($("#selectInfeccion").val().length > 0){
			LimiteIndicadores.filtrosEI.indicador.data = `${$(`#selectInfeccion`).val()}`;
		}
		else{
			LimiteIndicadores.filtrosEI.indicador.data = '0';
		}
	}
	else if(select == "selectTipoFinca"){
		LimiteIndicadores.filtrosEI.indicador.uso = 0;
		LimiteIndicadores.filtrosEI.edad.uso = 0;
		//Se añaden los filtros
		if($("#selectTipoFinca").val().length > 0){
			LimiteIndicadores.filtrosEI.tipoFinca.data = `${$(`#selectTipoFinca`).val()}`;
		}
		else{
			LimiteIndicadores.filtrosEI.tipoFinca.data = '0';
		}
	}
}

//Funcion encargada de traer los selectores iniciales
LimiteIndicadores.funciones.consultaSelectoresEI = function(){
    var data = {
    	filter: 	JSON.stringify(LimiteIndicadores.filtrosEI)
    };
    query.callAjax(LimiteIndicadores.urlConexionVista,
        "consulta_selectores_ei",
        data, 
        LimiteIndicadores.comunicaciones.consultaSelectoresEI);
}
/* FIN SELECTORES ANIDADOS (edades infecciones) */