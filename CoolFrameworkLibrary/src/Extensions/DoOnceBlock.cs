using System;
using System.Collections.Generic;

namespace Lokel.CoolFramework.Extensions {
    public class DoOnceBlock {
        private Action _Do;
        private bool _Done;

        public DoOnceBlock(Action Do) {
            _Do = Do;
            _Done = false;
        }

        public DoOnceBlock(Block Do) {
            _Do = () => { Do(); };
            _Done = false;
        }

        public static implicit operator Action(DoOnceBlock dob) {
            return () => { dob.Invoke(); };
        }

        public static implicit operator Block(DoOnceBlock dob) {
            return () => { dob.Invoke(); };
        }

        public void Invoke() {
            bool status;
            lock (this) {
                status = _Done;
                _Done = true;
            }
            if (!status) {
                _Do.Invoke();
            }
        }
    } // DoOnceBlock

} //--namespace --
