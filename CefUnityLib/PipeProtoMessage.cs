using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefUnityLib
{
    public class PipeProtoMessage
    {
        public byte Opcode;
        public byte[] Payload;

        public byte[] ToBytes()
        {
            return PipeProto.BytesToProtoMessage(Payload, Opcode);
        }

        public void WriteToClientStream(NamedPipeClientStream stream, bool flush)
        {
            var bytes = ToBytes();

            lock (stream)
            {
                stream.Write(bytes, 0, bytes.Length);

                if (flush)
                {
                    stream.Flush();
                }
            }
        }

        public static PipeProtoMessage ReadFromStream(Stream stream)
        {
            int headerLengthExpected = 1 + 1 + 4;

            byte[] nextHeader = new byte[headerLengthExpected];
            int headerBytesRead = stream.Read(nextHeader, 0, nextHeader.Length);

            if (headerBytesRead == headerLengthExpected)
            {
                byte index = 0;
                byte firstByte = nextHeader[index++];

                if (firstByte == PipeProto.PACKET_START_MARKER)
                {
                    byte packetOpcode = nextHeader[index++];

                    UInt32 payloadLength = BitConverter.ToUInt32(new byte[] { nextHeader[index++], nextHeader[index++],
                                nextHeader[index++], nextHeader[index++] }, 0);

                    var packetParsed = new PipeProtoMessage
                    {
                        Opcode = packetOpcode,
                        Payload = null
                    };

                    if (payloadLength > 0)
                    {
                        packetParsed.Payload = new byte[payloadLength];
                        stream.Read(packetParsed.Payload, 0, (int)payloadLength);
                    }

                    return packetParsed;
                }
            }

            if (headerBytesRead == 0)
            {
                throw new InvalidOperationException("Connection is now closed, cannot read data.");
            }

            return null;
        }
    }
}
