using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lokel.CoolFramework.Extensions;

namespace Lokel.CoolFramework {
    using RefBlock = Action;

    public struct Reference {
        public static Reference NotNull(object o) {
            return new Reference(o);
        }

        private bool _NotNull;

        private Reference(object o) {
            _NotNull = o != null;
        }

        public Reference AndNotNull(object o) {
            _NotNull = _NotNull && (o != null);
            return this;
        }

        public Reference IfTrue(RefBlock Do) {
            _NotNull.IfTrue( Do );
            return this;
        }

        public Reference IfFalse(RefBlock Do) {
            _NotNull.IfFalse(Do);
            return this;
        }
    }

} //-namespace
