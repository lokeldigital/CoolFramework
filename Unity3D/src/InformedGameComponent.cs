using System;

using UnityEngine;

namespace Lokel.CoolFramework.Unity3D {

    /// <summary>
    /// The InformedGameComponent is a MonoBehaviour and also a MessageSubscriber
    /// for use with the middleware in Lokel.CoolFramework.
    /// </summary>
    public class InformedGameComponent : MonoBehaviour, IMessageSubscriber {
        private MessageBus _Bus;

        void Start() {
            _Bus = null;
        }

        public virtual void ReceiveUnhandled(Message msg) {
            throw new NotImplementedException();
        }

        public void SetMessageBus(MessageBus Bus) {
            _Bus = Bus;
            Bus.Register(this);
        }

        public void DeregisterHandlers() {
            _Bus.Deregister(this);
        }

        protected void Send(Message msg) {
            MessageSubscriber.SendVia(msg, _Bus, this);
        }

    }

} // - namespace
