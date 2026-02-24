using System;
using System.Collections.Generic;
using System.Text;

namespace Whizz
{
    public interface IByteSerializable
    {
        byte[] ByteSerialize();
        void ByteDeserialize(byte[] bytes);
    }
}
