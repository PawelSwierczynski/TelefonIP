using System.ComponentModel.DataAnnotations;

namespace TelefonIPClient
{
    public sealed class CredentialsValidator
    {
        private const int MINIMAL_LOGIN_LENGTH = 4;
        private const int MAXIMAL_LOGIN_LENGHT = 20;
        private const int MINIMAL_PASSWORD_LENGTH = 6;
        private const int MAXIMAL_PASSWORD_LENGHT = 36;
        private const int MINIMAL_EMAIL_LENGTH = 3;
        private const int MAXIMAL_EMAIL_LENGHT = 50;

        public bool ValidateLogin(string login)
        {
            return login.Length >= MINIMAL_LOGIN_LENGTH && login.Length <= MAXIMAL_LOGIN_LENGHT;
        }

        public bool ValidatePassword(string password)
        {
            return password.Length >= MINIMAL_PASSWORD_LENGTH && password.Length <= MAXIMAL_PASSWORD_LENGHT;
        }

        public bool ValidateEmail(string email)
        {
            if (email.Length >= MINIMAL_EMAIL_LENGTH && email.Length <= MAXIMAL_EMAIL_LENGHT)
            {
                return new EmailAddressAttribute().IsValid(email);
            }
            return false;
        }
    }
}