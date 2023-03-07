namespace Trilha.DotNet.Shared.Contracts;

public interface IAuditable
{
    public Guid UserId { get; set; }
    public DateTime DateTime { get; set; }
    public object Before { get; set; }
    public object After { get; set; }
    public string Navbar { get; set; }
    public bool Reading { get; set; }
    public bool Writing { get; set; }
}
