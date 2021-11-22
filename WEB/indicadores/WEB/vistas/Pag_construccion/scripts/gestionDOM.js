Pag_construccion.gestionDOM = new Object();

//Se construye el btn para descargar la app
Pag_construccion.gestionDOM.downloadAppBtn = function(){
    $("#contenedorPag_construccion").html(`
        <button id="app" type="button" class="btn btn-info">Descargar Aplicación</button>
    `);

    //Se crea el evento para descargar la app
	$("#app").off("click").on("click", Pag_construccion.gestionDOM.downloadApp);
}

//Se crea el evento para descargar la app
Pag_construccion.gestionDOM.downloadApp = function(){
    //se crea una etiqueta a
    var downloadLink = document.createElement("a");
    //en el atributo href se le asigna la url de descarga
    downloadLink.href = "/apiServices/APK/banasan.fitosanitario.apk";
    //se le da el link de descarga //en esta linea se cambian los datos de descarga
    downloadLink.download = "banasan.fitosanitario.apk";
    //se añade al documento html la etiqueta a
    document.body.appendChild(downloadLink);
    // se clickea esa etiqueta
    downloadLink.click();
    //se remueve el link de la etiqueta
    document.body.removeChild(downloadLink);
}