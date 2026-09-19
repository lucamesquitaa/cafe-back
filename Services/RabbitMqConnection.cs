using RabbitMQ.Client;

namespace Cafeteria.Services
{
  /// <summary>
  /// Singleton responsável por manter uma única conexão AMQP com o RabbitMQ
  /// durante todo o ciclo de vida da aplicação, expondo canais (IModel) sob demanda.
  /// Registre com AddSingleton&lt;RabbitMqConnection&gt; para reaproveitar a mesma instância em toda a app.
  /// </summary>
  public sealed class RabbitMqConnection : IDisposable
  {
    private readonly IConnectionFactory _connectionFactory;
    private readonly object _syncRoot = new();
    private IConnection? _connection;
    private bool _disposed;

    public RabbitMqConnection(IConfiguration configuration)
    {
      var host = configuration["RabbitMq:Host"] ?? Environment.GetEnvironmentVariable("RabbitMq__Host") ?? "localhost";
      var port = int.TryParse(configuration["RabbitMq:Port"] ?? Environment.GetEnvironmentVariable("RabbitMq__Port"), out var parsedPort)
        ? parsedPort
        : 5672;
      var username = configuration["RabbitMq:Username"] ?? Environment.GetEnvironmentVariable("RabbitMq__Username") ?? "guest";
      var password = configuration["RabbitMq:Password"] ?? Environment.GetEnvironmentVariable("RabbitMq__Password") ?? "guest";
      var virtualHost = configuration["RabbitMq:VirtualHost"] ?? "/";

      _connectionFactory = new ConnectionFactory
      {
        HostName = host,
        Port = port,
        UserName = username,
        Password = password,
        VirtualHost = virtualHost,
        AutomaticRecoveryEnabled = true,
        NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
        TopologyRecoveryEnabled = true
      };
    }

    private IConnection Connection
    {
      get
      {
        if (_connection is { IsOpen: true })
          return _connection;

        lock (_syncRoot)
        {
          if (_disposed)
            throw new ObjectDisposedException(nameof(RabbitMqConnection));

          if (_connection is not { IsOpen: true })
            _connection = _connectionFactory.CreateConnection();

          return _connection;
        }
      }
    }

    /// <summary>
    /// Cria um novo canal (IModel) na conexão compartilhada. Canais não são thread-safe:
    /// cada thread/worker deve criar e descartar o seu próprio canal.
    /// </summary>
    public IModel CreateChannel() => Connection.CreateModel();

    public void Dispose()
    {
      lock (_syncRoot)
      {
        if (_disposed)
          return;

        _disposed = true;
        if (_connection is not null)
        {
          if (_connection.IsOpen)
            _connection.Close();
          _connection.Dispose();
        }
      }
    }
  }
}
