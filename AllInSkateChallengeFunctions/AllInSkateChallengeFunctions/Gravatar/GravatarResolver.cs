namespace AllInSkateChallengeFunctions.Gravatar
{
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    public class GravatarResolver : IGravatarResolver
    {
        public string GetGravatarUrl(string emailAddress)
        {
            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                return "https://www.gravatar.com/avatar/00000000000000000000000000000000";
            }

            var cleanEmailAddress = emailAddress?.Trim().ToLower();
            var bytes = Encoding.ASCII.GetBytes(cleanEmailAddress);
            var hash = MD5.Create().ComputeHash(bytes);

            var hexValue = string.Join(string.Empty, hash.Select(x => x.ToString("x2")));

            return $"https://www.gravatar.com/avatar/{hexValue}";
        }
    }
}