using System;
using System.Collections.Generic;
using System.Text;

namespace Whizz
{
    public interface IStreamSerializable
    {
        void Serialize(Stream stream);
        void Deserialize(Stream stream)
        {
            using var reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);
            Deserialize(reader);
        }
        void Deserialize(BinaryReader reader);
    }
}
