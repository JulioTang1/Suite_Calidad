using System;

namespace APP.Helpers
{
    public class Indicadores
    {
        public int id { get; set; }
        public float valor { get; set; }
        public string valorText { get; set; }

        public String[] paths{ get; set; }

        public Indicadores(int id, float valor)
        {
            this.id = id;
            this.valor = valor;
        }

        public Indicadores(int id, string valorText)
        {
            this.id = id;
            this.valorText = valorText;
        }

        public Indicadores(int id, float valor, String[] paths)
        {
            this.id = id;
            this.valor = valor;
            this.paths = paths;
        }
    }
}