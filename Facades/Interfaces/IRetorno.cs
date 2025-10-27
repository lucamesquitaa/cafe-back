namespace SaudeIA.Facades.Interfaces
{
  public interface IRetorno
  {
    bool Sucesso { get; }
    string? Mensagem { get; }
    string? ExcecaoMensagem { get; }
    object? Data { get; }
  }
  /// <summary>
  /// Versão genérica de IRetorno contendo o tipo dos dados retornados.
  /// </summary>
  public interface IRetorno<T> : IRetorno
  {
    new T? Data { get; }
  }
  /// <summary>
  /// Implementação não genérica de IRetorno.
  /// Use Retorno<T> quando precisar tipar Data.
  /// </summary>
  public class Retorno : IRetorno
  {
    public bool Sucesso { get; private set; }
    public string? Mensagem { get; private set; }
    public string? ExcecaoMensagem { get; private set; }

    public object? Data { get; private set; }

    private Retorno() { }

    public static Retorno Ok(object? data = null, string? mensagem = null)
      => new Retorno { Sucesso = true, Mensagem = mensagem ?? "Operação realizada com sucesso.", Data = data };

    public static Retorno Erro(string mensagem, object? data = null)
      => new Retorno { Sucesso = false, Mensagem = mensagem, Data = data };

    public static Retorno Excecao(Exception ex, string? mensagem = null)
      => new Retorno
      {
        Sucesso = false,
        Mensagem = mensagem ?? "Ocorreu uma exceção interna.",
        ExcecaoMensagem = ex.Message
      };
  }

  /// <summary>
  /// Implementação genérica de IRetorno{T}.
  /// </summary>
  public class Retorno<T> : IRetorno<T>
  {
    public bool Sucesso { get; private set; }
    public string? Mensagem { get; private set; }
    public string? ExcecaoMensagem { get; private set; }

    public T? Data { get; private set; }

    object? IRetorno.Data => Data;

    private Retorno() { }

    public static Retorno<T> Ok(T? data, string? mensagem = null)
      => new Retorno<T> { Sucesso = true, Mensagem = mensagem ?? "Operação realizada com sucesso.", Data = data };

    public static Retorno<T> Erro(string mensagem, T? data = default)
      => new Retorno<T> { Sucesso = false, Mensagem = mensagem,  Data = data };

    public static Retorno<T> Excecao(Exception ex, string? mensagem = null)
      => new Retorno<T>
      {
        Sucesso = false,
        Mensagem = mensagem ?? "Ocorreu uma exceção interna.",
        ExcecaoMensagem = ex.Message
      };
  }
}
