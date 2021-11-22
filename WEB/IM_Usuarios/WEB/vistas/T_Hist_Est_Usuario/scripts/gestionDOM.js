
T_Hist_Est_Usuario.gestionDOM = new Object();


T_Hist_Est_Usuario.gestionDOM.dateSelected = function(){

	gestionModal.alertaBloqueante(primario.aplicacion, "Procesando...");

	$("#divTablaHistorico").empty();

	$("#divTablaHistorico").append(`
		<div class="container_DataTable movil">
			<div id="divtabla_0" class="dataTableStyle tab-pane c_shadow rounded">		
				<!--para configurar opciones con los data--> 
				<!-- Por defecto siempre se organiza por la primera columna asc -->
				<table id="tablaT_Hist_Est_Usuario" class="table-striped table-bordered" data-order='[[ 0, "asc" ]]' data-page-length='100'>
				</table> <!-- el id de la tabla si se puede cambiar -->
			</div>
		</div>
	`);

	var data = {
		fecha_ini:	$("#fechas_historico span.fecha_visual")["0"].innerText.substring(0,10),
		fecha_fin:  $("#fechas_historico span.fecha_visual")["0"].innerText.substring(13)
	};

    gestionDT.initTable(
    	T_Hist_Est_Usuario.urlConexion,
    	"T_Hist_Est_Usuario", 
    	"#tablaT_Hist_Est_Usuario", 
		T_Hist_Est_Usuario.gestionDOM.tablaCargada,
		0,
        data
    ); 
}

T_Hist_Est_Usuario.gestionDOM.tablaCargada = function(e, settings, json){

	$("#divTablaHistorico").resize();
	gestionModal.cerrar();

}
