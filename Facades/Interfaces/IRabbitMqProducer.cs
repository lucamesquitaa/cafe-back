namespace Turify.Facades.Interfaces
{
  public interface IRabbitMqProducer
  {
    void SendMessage<MessageEvent>(MessageEvent message);
  }

  public class MessageEvent<T>
  {
    public string EventType { get; set; } = string.Empty;
    public T Data { get; set; }
  }
}
