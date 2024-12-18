namespace TechChallenge.ContatoPrimary.Domain.Entities
{
    public class ApiSettings
    {
        public static string Url => Environment.GetEnvironmentVariable("API_URL") ?? "";
        public string ResourceContato { get; set; } = string.Empty;
        public string ResourceDDD { get; set; } = string.Empty;
    }
}
