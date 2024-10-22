using System.Security.Cryptography;
using System.Text;

namespace NGO.Common.Helpers
{
    public static  class Cryptography
    {
        public static string ComputeSHA256Hash(string plainText, string salt, string pepper = "")
        {
            using (var sha256 = SHA256.Create())
            {
                Encoding encoding = Encoding.UTF8;
                Byte[] sha256Hash = sha256.ComputeHash(encoding.GetBytes(plainText + salt + pepper));

                return Convert.ToBase64String(sha256Hash);
            }
        }

        public static string GetRandomString(int stringLength)
        {
            const string stringSource = "@abcdefghijklmnopqrstuvwxyz@ABCDEFGHIJKLMNOPQRSTUVWXYZ@1234567890";

            StringBuilder passwordBuilder = new StringBuilder();
            Random random = new Random();

            while (0 < stringLength--)
            {
                passwordBuilder.Append(stringSource[random.Next(stringSource.Length)]);
            }

            return passwordBuilder.ToString();
        }
    }
}
