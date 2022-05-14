namespace Server.Services.Interfaces;

public interface IAuthenticationService
{
    (bool success, string content) Register(string username, string password);
    (bool success, string content) Login(string username, string password);
}