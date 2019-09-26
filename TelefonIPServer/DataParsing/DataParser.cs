using System.Linq;
using DataParsing.Containers;

namespace DataParsing
{
    public sealed class DataParser
    {
        private const char DATA_DELIMITER = ';';
        private const int PASSWORD_LENGTH = 64;
        private const int LOGIN_INDEX = 0;

        private string[] ParseData(string data)
        {
            return data.Split(DATA_DELIMITER);
        }

        private string ExtractPassword(string data)
        {
            var passwordCharacters = data.Skip(data.Length - PASSWORD_LENGTH);

            return new string(passwordCharacters.ToArray());
        }

        public LogInCredentials ExtractLogInCredentials(string logInCredentials)
        {
            string[] parsedLogInCredentials = ParseData(logInCredentials);

            return new LogInCredentials(parsedLogInCredentials[LOGIN_INDEX], ExtractPassword(logInCredentials));
        }
    }
}