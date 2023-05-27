using System.Security.Cryptography;

namespace server
{
    public class KeyManager
    {
        private const string SecretKeyEnvironmentVariable = "__SECRET_KEY";

        public string GetSecretKey()
        {
            string secretKey = Environment.GetEnvironmentVariable(SecretKeyEnvironmentVariable);
            if (string.IsNullOrEmpty(secretKey))
            {
                secretKey = GenerateAndStoreSecretKey();
            }
            return secretKey;
        }

        private string GenerateAndStoreSecretKey()
        {
            byte[] key = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(key);
            }
            string secretKey = Convert.ToBase64String(key);

            Environment.SetEnvironmentVariable(SecretKeyEnvironmentVariable, secretKey);

            return secretKey;
        }

        // todo self rotate every some time
        public void RotateSecretKey()
        {
            byte[] key = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(key);
            }
            string newSecretKey = Convert.ToBase64String(key);

            Environment.SetEnvironmentVariable(SecretKeyEnvironmentVariable, newSecretKey);
        }
    }
}
