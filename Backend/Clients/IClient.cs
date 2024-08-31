namespace Backend.Clients;

public interface IClient
{
    Task<object> FetchDataAsync();
}
