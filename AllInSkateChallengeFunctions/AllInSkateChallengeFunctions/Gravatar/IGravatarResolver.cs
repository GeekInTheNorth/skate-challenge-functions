namespace AllInSkateChallengeFunctions.Gravatar
{
    public interface IGravatarResolver
    {
        string GetGravatarUrl(string emailAddress);
    }
}