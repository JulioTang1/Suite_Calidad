namespace APP.Helpers
{
    public class ControlesBioseguridad
    {
        public int id { get; set; }
        public string pregunta { get; set; }
        public int respuesta { get; set; }
        public int camara { get; set; }
        public int aspecto { get; set; }

        public ControlesBioseguridad(int id, string pregunta, int respuesta, int camara, int aspecto)
        {
            this.id = id;
            this.pregunta = pregunta;
            this.respuesta = respuesta;
            this.camara = camara;
            this.aspecto = aspecto;
        }
    }
}