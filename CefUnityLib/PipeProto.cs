using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefUnityLib
{
    public static class PipeProto
    {
        public const byte PACKET_START_MARKER = 2;

        public const byte LENGTH_MARKER_SIZE = 4;

        public const byte OPCODE_FRAME = 1;
        public const byte OPCODE_RESIZE = 2;
        public const byte OPCODE_NAVIGATE = 3;
        public const byte OPCODE_SCRIPT = 4;
        public const byte OPCODE_PING = 5;
        public const byte OPCODE_MOUSE_EVENT = 6;
        public const byte OPCODE_KEY_EVENT = 7;

        public static byte[] BytesToProtoMessage(byte[] input, byte opcode)
        {
            if (input == null)
            {
                input = new byte[0];
            }

            // [START BYTE] + [OPCODE] + [LENGTH MARKER] + [PAYLOAD]
            byte[] output = new byte[1 + 1 + LENGTH_MARKER_SIZE + input.Length];
            var idx = 0;

            // Start marker and opcode
            output[idx++] = PACKET_START_MARKER;
            output[idx++] = opcode;

            // Length marker
            var lengthMarkerBytes = BitConverter.GetBytes((UInt32)input.Length);
            lengthMarkerBytes.CopyTo(output, idx);

            idx += LENGTH_MARKER_SIZE;

            // Payload
            input.CopyTo(output, idx);

            // All done
            return output;
        }
    }
}
