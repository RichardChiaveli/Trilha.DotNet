namespace Trilha.DotNet.Shared.Contracts;

/// <summary>
/// Configuração do OCELOT
/// </summary>
/// <param name="GlobalConfiguration"></param>
/// <param name="Routes"></param>
public record OcelotContract(OcelotGlobalConfiguration GlobalConfiguration, IEnumerable<OcelotRoute> Routes);