using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityAssetLib
{
    class UnknownTypeException : Exception
    {
        public UnknownTypeException() { }

        public UnknownTypeException(string message) : base(message) { }
    }
}
