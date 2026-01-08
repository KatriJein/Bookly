namespace Bookly.Application.Services;

public interface IInitializableSingleton
{
    Task InitializeAsync();
}