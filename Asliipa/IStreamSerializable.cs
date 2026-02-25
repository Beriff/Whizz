using System;
using System.Collections.Generic;
using System.Text;

namespace Whizz
{
    public interface IStreamSerializable
    {
        void Serialize(Stream stream);
        void Deserialize(Stream stream);
    }
}
