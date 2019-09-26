using System.Security.Cryptography;
using System.Text;

namespace Security
{
    public sealed class HashComputer
    {
        public string ComputeHashUsingSHA512(string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            byte[] hash;

            using (SHA512 sha512Managed = new SHA512Managed())
            {
                hash = sha512Managed.ComputeHash(data);
            }

            return Encoding.UTF8.GetString(hash);
        }
    }
}