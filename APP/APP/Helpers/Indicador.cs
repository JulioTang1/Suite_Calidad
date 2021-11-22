namespace APP.Helpers
{
    public class Indicador
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string valor { get; set; }
        public string fecha { get; set; }
        public int boton { get; set; }
        public int indicador { get; set; }

        public Indicador(int id, string nombre, string valor, string fecha, int boton, int indicador)
        {
            this.id = id;
            this.nombre = nombre;
            this.valor = valor;
            this.fecha = fecha;
            this.boton = boton;
            this.indicador = indicador;
        }
    }
}