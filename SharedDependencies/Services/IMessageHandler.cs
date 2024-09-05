namespace SharedDependencies.Services;

public interface IMessageHandler
{
    Task HandleMessageAsync();
}
