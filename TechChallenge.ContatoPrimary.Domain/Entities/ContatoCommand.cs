namespace TechChallenge.ContatoPrimary.Domain.Entities
{
    public class ContatoCommand
    {
        public long? Id { get; set; }
        public string? Nome { get; set; }
        public byte? NrDDD { get; set; }
        public string? NrTelefone { get; set; }
        public string? Email { get; set; }
        public string? Evento { get; set; }
    }
}
