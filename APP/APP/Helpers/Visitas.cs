namespace APP.Helpers
{
    public class Visitas
    {
        public int id { get; set; }

        public string nombre { get; set; }

        public string fecha { get; set; }

        public Visitas(int id, string nombre, string fecha)
        {
            this.id = id;
            this.nombre = nombre;
            this.fecha = fecha;
        }
    }
}