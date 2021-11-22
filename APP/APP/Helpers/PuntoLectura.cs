namespace APP.Helpers
{
    public class PuntoLectura
    {
        public int id { get; set; }
        public string punto { get; set; }
        public string fecha { get; set; }

        public PuntoLectura(int id, string punto, string fecha)
        {
            this.id = id;
            this.punto = punto;
            this.fecha = fecha;
        }
    }
}