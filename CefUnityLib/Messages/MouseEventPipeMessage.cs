using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefUnityLib.Messages
{
    public class MouseEventPipeMessage : PipeProtoMessage
    {
        public const byte TYPE_MOVE = 0;
        public const byte TYPE_MOUSE_DOWN = 1;
        public const byte TYPE_MOUSE_UP = 2;

        public const byte IDX_EVENT_TYPE = 0;
        public const byte IDX_COORD_X = 1;
        public const byte IDX_COORD_Y = 5;

        public MouseEventPipeMessage(byte[] payload)
        {
            Opcode = PipeProto.OPCODE_MOUSE_EVENT;
            Payload = payload;
        }

        public MouseEventPipeMessage(byte eventType = TYPE_MOVE, Int32 coordX = 0, Int32 coordY = 0)
        {
            Opcode = PipeProto.OPCODE_MOUSE_EVENT;

            Payload = new byte[1 + 4 + 4]; // [Opcode/1b] + [Int32/CoordX/4] + [Int32/CoordY/4]

            MouseEventType = eventType;
            CoordX = coordX;
            CoordY = coordY;
        }

        public byte MouseEventType
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

        public Int32 CoordX
        {
            get
            {
                return BitConverter.ToInt32(Payload.Skip(IDX_COORD_X).Take(4).ToArray(), 0);
            }

            set
            {
                BitConverter.GetBytes(value).CopyTo(Payload, IDX_COORD_X);
            }
        }

        public Int32 CoordY
        {
            get
            {
                return BitConverter.ToInt32(Payload.Skip(IDX_COORD_Y).Take(4).ToArray(), 0);
            }

            set
            {
                BitConverter.GetBytes(value).CopyTo(Payload, IDX_COORD_Y);
            }
        }
    }
}
