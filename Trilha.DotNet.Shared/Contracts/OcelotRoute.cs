namespace Trilha.DotNet.Shared.Contracts;

/// <summary>
/// Configuração de Rotas
/// </summary>
/// <param name="UpstreamPathTemplate">URL Parcial da Rota da API de Origem</param>
/// <param name="UpstreamHttpMethod">Post, Put, Patch, Get, Delete</param>
/// <param name="DownstreamPathTemplate">URL Parcial da Rota da API de Destino</param>
/// <param name="DownstreamScheme">https ou http</param>
/// <param name="DownstreamHostAndPorts">Configuração da API de Destino</param>
/// <param name="QoSOptions"></param>
public record OcelotRoute(
    string UpstreamPathTemplate
    , IEnumerable<string> UpstreamHttpMethod
    , string DownstreamPathTemplate
    , string DownstreamScheme
    , IEnumerable<OcelotDownstreamHostAndPort> DownstreamHostAndPorts
    , OcelotQoSOptions QoSOptions);