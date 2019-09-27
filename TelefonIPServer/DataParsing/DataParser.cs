using System.Linq;
using DataParsing.Containers;

namespace DataParsing
{
    public sealed class DataParser
    {
        private const char DATA_DELIMITER = ';';
        private const int PASSWORD_LENGTH = 64;
        private const int LOGIN_INDEX = 0;
        private const int EMAIL_INDEX = 1;

        private string[] ParseData(string data)
        {
            return data.Split(DATA_DELIMITER);
        }

        private string ExtractPasswordFromLogInCredentials(string data)
        {
            return data.Substring(data.IndexOf(";") + 1);
        }

        private string ExtractPasswordFromRegisterCredentials(string data)
        {
            string emailWithPassword = data.Substring(data.IndexOf(";") + 1);
            return emailWithPassword.Substring(emailWithPassword.IndexOf(";") + 1);
        }

        public LogInCredentials ExtractLogInCredentials(string logInCredentials)
        {
            string[] parsedLogInCredentials = ParseData(logInCredentials);

            return new LogInCredentials(parsedLogInCredentials[LOGIN_INDEX], ExtractPasswordFromLogInCredentials(logInCredentials));
        }

        public RegisterCredentials ExtractRegisterCredentials(string registerCredentials)
        {
            string[] parsedRegisterCredentials = ParseData(registerCredentials);

            return new RegisterCredentials(parsedRegisterCredentials[LOGIN_INDEX], ExtractPasswordFromRegisterCredentials(registerCredentials), parsedRegisterCredentials[EMAIL_INDEX]);
        }
    }
}