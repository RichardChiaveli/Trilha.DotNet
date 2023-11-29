namespace Trilha.DotNet.Shared.Contracts;

/// <summary>
/// Configuração da API de Destino
/// </summary>
/// <param name="Host">URL Base (API de Destino)</param>
/// <param name="Port">Porta da API de Destino</param>
public record OcelotDownstreamHostAndPort(string Host, int Port);