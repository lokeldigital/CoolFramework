using System;
using System.Collections.Generic;

namespace Lokel.CoolFramework.Extensions {

    using Block = System.Action;

    public static class ObjectExt {
        public static Boolean IfTrue(this System.Boolean condition, Block _do) {
            if (condition)
                _do();
            return condition;
        }
        public static Boolean IfFalse(this System.Boolean condition, Block _do) {
            if (!condition)
                _do();
            return condition;
        }

        public static Boolean IfTrue(this System.Object refVal, Block _do) {
            bool status = refVal != null;
            if (status) _do();
            return status;
        }

        public static Boolean IfFalse(this System.Object refVal, Block _do) {
            bool status = refVal != null;
            if (!status) _do();
            return status;
        }
    }
} //-- namespace --
