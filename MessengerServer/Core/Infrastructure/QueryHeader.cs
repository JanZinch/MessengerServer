namespace MessengerServer.Core.Infrastructure;

public enum QueryHeader : byte
{
    None = 0,
    Login = 1,
    PostMessage = 2,
    Quit = 10,
}