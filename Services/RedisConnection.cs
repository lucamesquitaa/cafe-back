using StackExchange.Redis;

namespace Cafeteria.Services
{
  /// <summary>
  /// Singleton responsável por manter uma única conexão multiplexada com o Redis
  /// durante todo o ciclo de vida da aplicação, expondo o IDatabase sob demanda.
  /// Registre com AddSingleton&lt;RedisConnection&gt; para reaproveitar a mesma instância em toda a app.
  /// </summary>
  public sealed class RedisConnection : IDisposable
  {
    private readonly Lazy<ConnectionMultiplexer> _connection;
    private bool _disposed;

    public RedisConnection(IConfiguration configuration)
    {
      var configurationString = configuration["Redis:Configuration"]
        ?? Environment.GetEnvironmentVariable("REDIS_HOST")
        ?? "localhost:6379";

      _connection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(configurationString));
    }

    private ConnectionMultiplexer Connection
    {
      get
      {
        if (_disposed)
          throw new ObjectDisposedException(nameof(RedisConnection));

        return _connection.Value;
      }
    }

    /// <summary>
    /// Obtém o IDatabase (db padrão) da conexão compartilhada.
    /// </summary>
    public IDatabase GetDatabase(int db = -1) => Connection.GetDatabase(db);

    /// <summary>
    /// Obtém o ISubscriber para operações de pub/sub na conexão compartilhada.
    /// </summary>
    public ISubscriber GetSubscriber() => Connection.GetSubscriber();

    public void Dispose()
    {
      if (_disposed)
        return;

      _disposed = true;
      if (_connection.IsValueCreated)
        _connection.Value.Dispose();
    }
  }
}
