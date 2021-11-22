namespace APP.Helpers
{
    public class Selectores
    {
        public int id_server { get; set; }
        public string nombre { get; set; }
        public int activo { get; set; }
        
        public Selectores(int id_server, string nombre)
        {
            this.id_server = id_server;
            this.nombre = nombre;
        }

        public Selectores(int id_server, string nombre, int activo)
        {
            this.id_server = id_server;
            this.nombre = nombre;
            this.activo = activo;
        }
    }
}