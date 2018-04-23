using System;
using System.Linq;

namespace CefUnityLib.Messages
{
    public class MouseWheelEventPipeMessage : PipeProtoMessage
    {
        public const byte IDX_COORD_X = 0;
        public const byte IDX_COORD_Y = 4;
        public const byte IDX_DELTA = 8;

        public MouseWheelEventPipeMessage(byte[] payload)
        {
            Opcode = PipeProto.OPCODE_MOUSE_WHEEL_EVENT;
            Payload = payload;
        }

        public MouseWheelEventPipeMessage(int x, int y, int delta)
        {
            Opcode = PipeProto.OPCODE_MOUSE_WHEEL_EVENT;

            Payload = new byte[4 + 4 + 4]; // [Int32/CoordX/4] + [Int32/CoordY/4] + [Int32/Delta/4]
            
            CoordX = x;
            CoordY = y;
            Delta = delta;
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

        public Int32 Delta
        {
            get
            {
                return BitConverter.ToInt32(Payload.Skip(IDX_DELTA).Take(4).ToArray(), 0);
            }

            set
            {
                BitConverter.GetBytes(value).CopyTo(Payload, IDX_DELTA);
            }
        }
    }
}
