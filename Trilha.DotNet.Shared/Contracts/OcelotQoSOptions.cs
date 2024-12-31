namespace Trilha.DotNet.Shared.Contracts;

/// <summary>
/// Configuração de Timeout
/// </summary>
/// <param name="ExceptionsAllowedBeforeBreaking">Qtd de Exceções Permitidas depois da erro</param>
/// <param name="DurationOfBreak">Tempo em milisegundos da Duração do Erro</param>
/// <param name="TimeoutValue">Timeout</param>
public record OcelotQoSOptions(int ExceptionsAllowedBeforeBreaking, int DurationOfBreak, int TimeoutValue);