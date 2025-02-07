namespace CSharpApp.Core.Exceptions;

public class ServerErrorException(string message) : Exception(message)
{
}