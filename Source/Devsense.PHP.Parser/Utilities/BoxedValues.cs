using System;
using System.Collections.Generic;
using System.Text;

namespace Devsense.PHP.Utilities
{
    public static class BoxedValues
    {
        public static readonly object s_true = (object)true;

        public static readonly object s_false = (object)false;

        public static object Get(bool @bool) => @bool ? s_true : s_false;
    }
}
