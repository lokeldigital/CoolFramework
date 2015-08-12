using System;

using UnityEngine;

namespace Lokel.CoolFramework.Unity3D {

    /// <summary>
    /// The InformedGameComponent is a MonoBehaviour and also a MessageSubscriber
    /// for use with the middleware in Lokel.CoolFramework.
    /// </summary>
    public abstract class InformedGameComponent : MonoBehaviour, IMessageSubscriber {
        private MessageBus _Bus;

        /// <summary>
        /// Called by Unity - should be explicitly called by subclasses
        /// to initialise the message subsciber code.
        /// </summary>
        void Start() {
            _Bus = null;
            StartComponent();
        }

        /// <summary>
        /// Write the equivalent to the MonoBehaviour Start() logic here.
        /// </summary>
        protected abstract void StartComponent();

        /// <summary>
        /// Called by the message bus, during Send() method time,
        /// whenever messages did not find their way to any subscribers.
        /// </summary>
        /// <param name="msg"></param>
        public virtual void ReceiveUnhandled(Message msg) {
        }

        /// <summary>
        /// Called to inject the MessageBus as a dependency.
        /// </summary>
        /// <param name="Bus">The one MessageBus instance used by communicating instances.</param>
        public void SetMessageBus(MessageBus Bus) {
            _Bus = Bus;
            Bus.Register(this);
        }

        /// <summary>
        /// Used for graceful destruction - deregistration is important to ensure
        /// any references to this object are let go so related heap memory can
        /// be garbage collected.
        /// </summary>
        public void DeregisterHandlers() {
            _Bus.Deregister(this);
        }

        /// <summary>
        /// Send a message to a subscriber according to the properties set in the
        /// message.
        /// </summary>
        /// <param name="msg">Message to send to one or more subscribers.</param>
        protected void Send(Message msg) {
            MessageSubscriber.SendVia(msg, _Bus, this);
        }

    }

} // - namespace
