namespace Trilha.DotNet.Shared.Contracts;

public interface IFormFileJson<TModel>
{
    public TModel Model { get; set; }
    public IFormFileCollection Upload { get; set; }
}
