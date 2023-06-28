namespace Trilha.DotNet.Shared.Contracts
{
    /// <summary>
    /// URL Base (API de Origem)
    /// </summary>
    /// <param name="BaseUrl"></param>
    public record OcelotGlobalConfiguration(string BaseUrl);

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
        , List<string> UpstreamHttpMethod
        , string DownstreamPathTemplate
        , string DownstreamScheme
        , List<OcelotDownstreamHostAndPort> DownstreamHostAndPorts
        , OcelotQoSOptions QoSOptions);

    /// <summary>
    /// Configuração da API de Destino
    /// </summary>
    /// <param name="Host">URL Base (API de Destino)</param>
    /// <param name="Port">Porta da API de Destino</param>
    public record OcelotDownstreamHostAndPort(string Host, int Port);

    /// <summary>
    /// Configuração de Timeout
    /// </summary>
    /// <param name="ExceptionsAllowedBeforeBreaking">Qtd de Exceções Permitidas depois da erro</param>
    /// <param name="DurationOfBreak">Tempo em milisegundos da Duração do Erro</param>
    /// <param name="TimeoutValue">Timeout</param>
    public record OcelotQoSOptions(int ExceptionsAllowedBeforeBreaking, int DurationOfBreak, int TimeoutValue);

    /// <summary>
    /// Configuração do OCELOT
    /// </summary>
    /// <param name="GlobalConfiguration"></param>
    /// <param name="Routes"></param>
    public record OcelotConfiguration(OcelotGlobalConfiguration GlobalConfiguration, List<OcelotRoute> Routes);
}
