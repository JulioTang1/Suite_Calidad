
Topbar.funciones = new Object();

// funcion que dependiendo la techa se presione en el input del search bar hace una
// determinada acción
Topbar.funciones.searchBarEvent = function(event){
	switch(event.which){
		//Escape dento del input del search bar
		case 27:
			// se limpia la lista 
			// y se quita el focus al input del searchBar
			Topbar.gestionDOM.SearchBarClean();
			$(".search-input").blur();
			break;
		// Enter dento del input del search bar
		case 13:
			// se hace la busqueda con el contenido del searchbar, se limpia la lista
			// y se quita el focus al input del searchBar
			Topbar.funciones.SearchAndClean();
			break;
		// Flecha arriba dento del input del search bar
		case 38:
			// se desplaza hacia arriba en la lista al input del searchBar
			if($("#autocomplete-result li").length > 0){
				Topbar.gestionDOM.ListTop();
			}
			break;
		// Flecha abajo dento del input del search bar
		case 40:
			// se desplaza hacia abajo en la lista al input del searchBar
			if($("#autocomplete-result li").length > 0){
				Topbar.gestionDOM.ListDown();
			}
			break;
		// Las demás teclas dento del input del search bar
		default:
			// se toma el valor del input, si es vacío se limpia la lista y si tiene un valor
			// se hace la consulta en base de datos
			var search_data = $("#Topbar-wrapper .search-box .search-input").val();
			if(search_data == '')
			{
				$("#autocomplete-result").empty();
				Topbar.searchBarList = -1;
			}
			else
			{
				data = {
					busqueda: search_data,
					Rol: Login.Rol
				};
				// callAjax modificado para hacer dos consultas para la barra de busqueda
				query.callAjaxSearchBar(Topbar.urlConexion,"autocomplete",data,Topbar.comunicaciones.searchBarResult);
			}
	}
}

// esta funcion cuando se selecciona un elemento del menu de autocompletar
// poner el valor en el input y busca la vista
Topbar.funciones.selectListSearchBar = function(ev){

	// se extrae el valor del id y el url de la vista de la lista del searchbar y se ingresa al input
	// en datas
	var id 		= $(ev.currentTarget.lastElementChild)[0].attributes["data-id"].nodeValue;
	var value 	= ev.currentTarget.lastElementChild.innerText;
	// valor del string que se selecciono del menu de autocompletar
	$("#Topbar-wrapper .search-input").val(value);
	$("#Topbar-wrapper .search-input").data('id',id);
	Topbar.funciones.SearchAndClean();
}

// se hace la busqueda y se limpia la lista de menus
Topbar.funciones.SearchAndClean = function(){
	Topbar.gestionDOM.busqueda();
	Topbar.gestionDOM.SearchBarClean();
}

//función que carga la foto en el input y carga la imagen nueva para visualizar
Topbar.funciones.URLandPhoto = function(){
	Topbar.funciones.readURL();
	Topbar.gestionDOM.loadPhoto();
}


// Funcion para visualizar la nueva imagen cargada cuando se edita la foto de perfil
Topbar.funciones.readURL = function(e) {

	input = $("#inputFile")[0];

    if (input.files && input.files[0]) {
        var reader = new FileReader();
        
        reader.onload = function (e) {
            $('#profile-img-tag').attr('src', e.target.result);
        }
        reader.readAsDataURL(input.files[0]);
    }
}


// funcion para actualizar la foto de perfil
Topbar.funciones.updatePhoto = function(){

	var BanderaError = 0;
	// icono para cuando se produsca un error
	var mensajeError = `<i class="material-icons md-14">error</i>`;

	//imagen en el formulario en el modal
	var foto = $("#fileLoad input").val();

	// se borra los mensajes de error y se quita la clase de error de los inputs
	$(".errorInput").removeClass("errorInput");
	$(".msnError").empty();

	// valida que los inputs contengan información, si no contienen información
	// se le agrega una clase al input y se muestra un mensaje de error
	if((foto == null) || (foto == ''))
	{
		BanderaError=1;

		$(`#fileLoad`).addClass("errorInput");
		$(`#fileLoad`).next().html(`${mensajeError}<span>Subir una foto</span>`);
	}

	if(BanderaError == 0){

		gestionModal.alertaBloqueante(primario.aplicacion, "Procesando...");

    	// se extrae la extensión de la imagen
       	var nameImg = $("#fileLoad input").val().split(".");
       	var ext = nameImg[nameImg.length - 1];

       	// se organizan la imagen y la información para el registro
		var formElement = $("#fileLoad")[0];

		var formData = new FormData(formElement);
		// se agrega la información para enviarla junto a la imagen
		formData.append("user",Login.User);
		formData.append("ext",ext);

		query.callAjaxUser(Topbar.urlConexion,"edit_Photo",formData,Topbar.gestionDOM.RegCorrecto);	
	}
}

// menu que manda la consulta para saber que menus estan en favoritos
Topbar.funciones.menuFavoritos = function(){
	query.callAjax(Topbar.urlConexion,"menuFavoritos",{id_user: Login.ID_user},Topbar.comunicaciones.menuFavoritos);
}

// consulta para saber si la vista actual es favorita o no para el icono de la estrella
Topbar.funciones.favoriteView = function(data){

	query.callAjax(Topbar.urlConexion,"favoriteView",data,Topbar.comunicaciones.favoriteView);
}

// funcion que ejecuta la funcion de redibujar el menu de favoritos y la estrella
Topbar.funciones.toggleFavorite = function(){
	// se toma el id del menu que se esta observando en pantalla y el id del usuario
	// para quitar de favoritos
	var data = {
			id_user: Login.ID_user,
			id_menu: Sidebar.menuIDActual
	};

	// si Topbar.viewFav se encuentra en 1 signifa que esta en favoritos y se desea quitar de esta lista
	if(Topbar.viewFav){
		query.callAjax(Topbar.urlConexion,"DeleteFavorite",data,Topbar.funciones.quitarFavorito);
	}
	// si Topbar.viewFav se encuentra en 0 signifa que no esta en favoritos y se desea agregar a la lista
	else
	{	
		query.callAjax(Topbar.urlConexion,"AddFavorite",data,Topbar.funciones.agregarFavorito);
		
	}
}

// funcion que ejecuta la funcion de redibujar el menu de favoritos y la estrella
Topbar.funciones.quitarFavorito = function(){
	Topbar.funciones.menuFavoritos();
	Topbar.gestionDOM.ViewNotFav();
	// comparador que cambia el overflow-y cuando son menos de 3 favoritos, 
	// quitandolo para que el tooltip se vea bien
	Topbar.gestionDOM.overflowFav();
}

// funcion que ejecuta la funcion de redibujar el menu de favoritos y la estrella
Topbar.funciones.agregarFavorito = function(){
	Topbar.funciones.menuFavoritos();
	Topbar.gestionDOM.ViewFav();
	// comparador que cambia el overflow-y cuando son menos de 3 favoritos, 
	// quitandolo para que el tooltip se vea bien
	Topbar.gestionDOM.overflowFav();
}

Topbar.funciones.ordenMenuFav = function(){
	id_menus = new Array();
	var tam = $("#grid div").length;
	for (var x=0; x < tam; x++)
	{
		id_menus[x]=$("#grid div")[x].id;
		id_menus[x]=id_menus[x].replace("menuFav_","");
		id_menus[x]=parseInt(id_menus[x]);
	}
	var data = {
		id_menus: JSON.stringify(id_menus),
		id_user: Login.ID_user
	}
	query.callAjax(Topbar.urlConexion,"UpdateOrdenFav",data,Topbar.funciones.menuFavActualizado);
}

// funcion de retorno que notifica que el menu de favoritos actualizo su orden correctamente 
Topbar.funciones.menuFavActualizado = function(){
	console.log("Menú Favoritos Actualizado");
}