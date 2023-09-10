namespace ApiGenerator;

/// <summary>
/// Low level interface for message communication
/// </summary>
public interface IMessaging
{
  /// <summary>
  /// An observable that emits a json value when a message is received
  /// </summary>
  IObservable<string> MessageReceivedObservable { get; }

  /// <summary>
  /// Post a message (json) that will be sent
  /// </summary>
  /// <param name="message">The (json) message to be sent</param>
  void PostMessage(string message);
}
