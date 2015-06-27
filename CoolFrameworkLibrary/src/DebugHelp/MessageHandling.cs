using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lokel.CoolFramework;
using Lokel.CoolFramework.Extensions;

namespace Lokel.CoolFramework.DebugHelp {

    public static class MessageHandling {
        public static void DebugSend(
            MessageBus msgBus,
            Message message,
            StringBuilder textExplanation
        ) {
            int count_PtToPt = 0,
                count_Observer = 0,
                count_RoleEvent = 0;

            Action<HandlerInfo, Message> Msg_PtToPt = (HandlerInfo handler, Message msg) => {
                textExplanation.AppendFormat(
                    "Point-to-Point Basis: Message match  # {0}\r\n"
                    + "Message processed by handler: {1}\r\n",
                    count_PtToPt,
                    handler
                );
                count_PtToPt++;
            };
            Action<SubscriberInfo, HandlerInfo, string> Almost_PointToPoint = (SubscriberInfo sub, HandlerInfo h, string s) => 
            {
                textExplanation.AppendFormat("Point to Point near match: {0}\r\n",s);
            };

            Action<HandlerInfo, Message> Msg_Observer = (HandlerInfo handler, Message msg) => {
                textExplanation.AppendFormat(
                    "Role and Observer Basis: Message match  # {0}\r\n"
                    +"Message processed by handler: {1}\r\n",
                    count_Observer,
                    handler
                );
                count_Observer++;
            };
            Action<SubscriberInfo, HandlerInfo, string> Almost_Observer = (SubscriberInfo sub, HandlerInfo h, string s) =>
            {
                textExplanation.AppendFormat("Observer near match: {0}\r\n",s);
            };

            Action<HandlerInfo, Message> Msg_RoleEvent = (HandlerInfo handler, Message msg) => {
                textExplanation.AppendFormat(
                    "Role and Event Code Basis: Message match  # {0}\r\n"
                    + "Message processed by handler: {1}\r\n",
                    count_RoleEvent,
                    handler
                );
                count_RoleEvent++;
            };
            Action<SubscriberInfo, HandlerInfo, string> Almost_RoleEvent = (SubscriberInfo sub, HandlerInfo h, string s) =>
            {
                textExplanation.AppendFormat("Role and Event Code near match: {0}\r\n",s);
            };

            Action Msg_Unhandled = () =>
            {
                textExplanation.Append("No matches found.\r\n");
            };

            textExplanation.AppendFormat(
                "Performing match testing for message on the message bus:\r\n"
                + "Message : {0}\r\n", message
            );
            msgBus.MatchHandlers(message,
                Msg_PtToPt, Almost_PointToPoint,
                Msg_Observer, Almost_Observer,
                Msg_RoleEvent, Almost_RoleEvent,
                Msg_Unhandled
            );

            textExplanation.Append(
                "-----\r\nMatch Ended."
            );
        }
    }

} //--namespace --
