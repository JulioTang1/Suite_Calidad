En este directorio se encuentran los tipos de datos que envia la libreria DataTable por medio del Ajax, 
los datos simples que no son arreglos de datos se recogen dentro del servicio del Web Api,
Acá se definen los datos que recibira, esto esta ligado al ajax de la libreria

Column -> Recibe 
columns[i][data]: Name_Col
columns[i][name]: 
columns[i][searchable]: true
columns[i][orderable]: true
columns[i][search][value]: 
columns[i][search][regex]: false

Order -> Recibe 
order[i][column]: Index_Col
order[i][dir]: asc

Search -> Recibe
search[value]: 
search[regex]: false

-------------------------------------------------------------------------------------------------------------------

Además la clase dtQueryStrings es la encargada de generar de manera dinámica las diferentes consultas necesarias
para el correcto funcionamiento de datatable