namespace FunctionAppConsumoAPI.Models
{
    public class ResultadoContador
    {
        public int ValorAtual { get; set; }
        public string MachineName { get; set; }
        public string Local { get; set; }
        public string Sistema { get; set; }
        public string Variavel { get; set; }
        public string TargetFramework { get; set; }
    }
}