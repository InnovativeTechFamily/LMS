namespace LMS.API.Helpers
{
    public class TokenGenerator
    {
        /// <summary>
        /// Generate a random activation code
        /// </summary>
        public static string GenerateActivationCode()
        {
            return Random.Shared.Next(1000, 10000).ToString();
        }

        /// <summary>
        /// Generate a random token string
        /// </summary>
        public static string GenerateRandomToken(int length = 32)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Range(0, length)
                .Select(_ => chars[random.Next(chars.Length)])
                .ToArray());
        }
    }
}
