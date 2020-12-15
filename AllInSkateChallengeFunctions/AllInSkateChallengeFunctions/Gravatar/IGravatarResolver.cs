namespace AzureFunctionTest.Gravatar
{
    public interface IGravatarResolver
    {
        string GetGravatarUrl(string emailAddress);
    }
}