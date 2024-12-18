namespace TechChallenge.ContatoPrimary.Domain.Entities
{
    public class RabbitConfig
    {
        public string FilaRabbit { get; set; } = null!;
        public string? VHost { get; set; }

        public static string Servidor => Environment.GetEnvironmentVariable("SRV") ?? "";
        public static string Usuario => Environment.GetEnvironmentVariable("MENSAGERIA_USER") ?? "";
        public static string Senha => Environment.GetEnvironmentVariable("MENSAGERIA_PASSWORD") ?? "";
    }
}