using System.Security.Cryptography;
using System.Text;

namespace Security
{
    public sealed class HashComputer
    {
        private string ReverseString(string text)
        {
            char[] characters = new char[text.Length];

            int index = 0;

            for (int i = text.Length - 1; i >= 0; i--)
            {
                characters[index] = text[i];
                index++;
            }

            return new string(characters);
        }

        public string ComputeHashUsingSHA512(string text)
        {
            string textWithSalt = "7)#g:" + ReverseString(text) + "H%Ab98$";

            byte[] data = Encoding.UTF8.GetBytes(textWithSalt);
            byte[] hash;

            using (SHA512 sha512Managed = new SHA512Managed())
            {
                hash = sha512Managed.ComputeHash(data);
            }

            return Encoding.UTF8.GetString(hash);
        }
    }
}