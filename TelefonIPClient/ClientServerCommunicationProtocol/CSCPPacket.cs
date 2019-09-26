using System;
using System.IO;

namespace ClientServerCommunicationProtocol
{
    public sealed class CSCPPacket
    {
        public int Identifier { get; }
        public Command Command { get; }
        public int UserToken { get; }
        public int DataLength { get; }
        public string Data { get; set; }

        public CSCPPacket(int identifier, Command command, int userToken, string data)
        {
            Identifier = identifier;
            Command = command;
            UserToken = userToken;
            DataLength = data.Length;
            Data = data;
        }

        public CSCPPacket(StreamReader streamReader)
        {
            char[] header = new char[8];

            streamReader.ReadBlock(header, 0, 8);

            Identifier = header[0] << 8 | header[1];
            Command = (Command)(header[2] << 8 | header[3]);
            UserToken = header[4] << 8 | header[5];
            DataLength = header[6] << 8 | header[7];

            if (IsDataLengthAPositiveNumber())
            {
                char[] data = new char[DataLength];

                streamReader.ReadBlock(data, 0, DataLength);

                Data = new string(data);
            }
        }

        private bool IsDataLengthAPositiveNumber()
        {
            return DataLength > 0;
        }

        public string Serialize()
        {
            return new string(new char[] {
                Convert.ToChar(Identifier >> 8 & 255), Convert.ToChar(Identifier & 255),
                Convert.ToChar((int)Command >> 8 & 255), Convert.ToChar((int)Command & 255),
                Convert.ToChar(UserToken >> 8 & 255), Convert.ToChar(UserToken & 255),
                Convert.ToChar(DataLength >> 8 & 255), Convert.ToChar(DataLength & 255)
            }) + Data;
        }
    }
}