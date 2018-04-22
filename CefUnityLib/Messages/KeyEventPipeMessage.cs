using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefUnityLib.Messages
{
    public class KeyEventPipeMessage : PipeProtoMessage
    {
        public const byte TYPE_KEY_DOWN = 0;
        public const byte TYPE_KEY_UP = 1;

        public const byte IDX_EVENT_TYPE = 0;
        public const byte IDX_KEY_CODE = 1;
        
        public KeyEventPipeMessage(byte[] payload)
        {
            Opcode = PipeProto.OPCODE_KEY_EVENT;
            Payload = payload;
        }

        public KeyEventPipeMessage(byte eventType, Int32 keyCode)
        {
            Opcode = PipeProto.OPCODE_KEY_EVENT;

            Payload = new byte[1 + 4 + 4]; // [Opcode/1b] + [Int32/KeyCode/4]

            KeyEventType = eventType;
            KeyCode = keyCode;
        }

        public byte KeyEventType
        {
            get
            {
                return Payload[0];
            }

            set
            {
                Payload[IDX_EVENT_TYPE] = value;
            }
        }

        public Int32 KeyCode
        {
            get
            {
                return BitConverter.ToInt32(Payload.Skip(IDX_KEY_CODE).Take(4).ToArray(), 0);
            }

            set
            {
                BitConverter.GetBytes(value).CopyTo(Payload, IDX_KEY_CODE);
            }
        }
    }
}
