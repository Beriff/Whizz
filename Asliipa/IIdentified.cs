using System;
using System.Collections.Generic;
using System.Text;

namespace Whizz
{
    public interface IIdentified
    {
        public ushort Id { get; protected set; }
    }
}
