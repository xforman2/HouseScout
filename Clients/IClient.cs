namespace HouseScout.Clients;

public interface IClient
{
    Task<object> FetchDataAsync();
}