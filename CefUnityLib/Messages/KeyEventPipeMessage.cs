using CefUnityLib.Helpers;
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
        public const byte TYPE_KEY_CHAR = 2;

        public const byte IDX_EVENT_TYPE = 0;
        public const byte IDX_KEY_CODE = 1;
        public const byte IDX_MODIFIERS = 2;
        
        public KeyEventPipeMessage(byte[] payload)
        {
            Opcode = PipeProto.OPCODE_KEY_EVENT;
            Payload = payload;
        }

        public KeyEventPipeMessage(byte eventType, Keys keys, Keys modifiers = Helpers.Keys.None)
        {
            Opcode = PipeProto.OPCODE_KEY_EVENT;

            Payload = new byte[1 + 4 + 4 + 4]; // [Opcode/1b] + [Int32/KeyCode/4] + [Int32/Modifiers/4]

            KeyEventType = eventType;
            Keys = keys;
            Modifiers = modifiers;
        }

        public KeyEventPipeMessage(byte eventType, int keyCode, Keys modifiers = Helpers.Keys.None)
            : this(eventType, Keys.None, modifiers)
        {
            KeysInt = keyCode;
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

        public Keys Keys
        {
            get
            {
                return (Keys)KeysInt;
            }

            set
            {
                KeysInt = (int)value;
            }
        }

        public int KeysInt
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

        public Keys Modifiers
        {
            get
            {
                return (Keys)BitConverter.ToInt32(Payload.Skip(IDX_MODIFIERS).Take(4).ToArray(), 0);
            }

            set
            {
                BitConverter.GetBytes((int)value).CopyTo(Payload, IDX_MODIFIERS);
            }
        }
    }
}
