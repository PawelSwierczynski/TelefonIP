using System;
using System.Collections.Generic;

namespace TelefonIPServer.Accounts
{
    public sealed class TokenGenerator
    {
        private const int TOKEN_MINIMAL_VALUE = 0;
        private const int TOKEN_MAXIMAL_VALUE = 65356;
        private readonly Random random;

        public TokenGenerator(Random random)
        {
            this.random = random;
        }

        public int RandomizeToken(List<int> tokensInUse)
        {
            int token;

            for (; ; )
            {
                token = random.Next(TOKEN_MINIMAL_VALUE, TOKEN_MAXIMAL_VALUE);

                if (!tokensInUse.Contains(token))
                {
                    break;
                }
            }

            return token;
        }
    }
}