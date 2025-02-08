namespace CSharpApp.Core.Interfaces;

public interface ITokenCacheService
{
	public Token? Get(string id);
	public void Set(string id, Token token);
}
