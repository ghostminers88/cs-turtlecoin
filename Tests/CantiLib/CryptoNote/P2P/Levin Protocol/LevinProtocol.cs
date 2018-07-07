﻿using Canti.Data;
using System;

namespace Canti.CryptoNote.P2P
{
    internal partial class LevinProtocol
    {
        // Server connection
        private Server Connection;

        // Entry point
        internal LevinProtocol(Server Connection)
        {
            // Set connection
            this.Connection = Connection;

            // Bind event handlers
            this.Connection.OnDataReceived += OnDataReceived;
            this.Connection.OnPeerConnected += OnPeerConnected;
        }

        // Data received
        private void OnDataReceived(object sender, EventArgs e)
        {
            // Decode command

            // Process command

            // Send response if requested
        }

        // Peer connected
        private void OnPeerConnected(object sender, EventArgs e)
        {
            // Send ping

            // Await pong

            // Send handhshake
        }

        // Notifies a peer with a command, no response expected
        internal void Notify(PeerConnection Peer, int CommandCode, byte[] Data)
        {
            // Form message header
            bucket_head2 head = new bucket_head2
            {
                m_signature =         LEVIN_SIGNATURE,
                m_have_to_return_data =  false,
                m_cb =        (ulong)Data.Length,
                m_command =       (uint)CommandCode,
                m_protocol_version =   LEVIN_PROTOCOL_VER_1,
                m_flags =             LEVIN_PACKET_REQUEST
            };

            // Send header packet
            if (Connection.SendMessage(Peer, Encoding.ObjectToByteArray(head)))
            {
                // Send body packet
                Connection.SendMessage(Peer, Data);
            }
        }

        // Notifies all peers with a command, no response expected
        internal void NotifyAll(int CommandCode, byte[] Data)
        {
            // Form message header
            bucket_head2 head = new bucket_head2
            {
                m_signature = LEVIN_SIGNATURE,
                m_have_to_return_data = false,
                m_cb = (ulong)Data.Length,
                m_command = (uint)CommandCode,
                m_protocol_version = LEVIN_PROTOCOL_VER_1,
                m_flags = LEVIN_PACKET_REQUEST
            };

            // Send header packet
            Connection.Broadcast(Encoding.ObjectToByteArray(head));
            
            // Send body packet
            Connection.Broadcast(Data);
        }

        // Encodes a command and returns the raw bytes
        internal byte[] Encode(ICommandRequestBase Data)
        {
            // Return the serialized byte array
            return Data.Serialize();
        }

        // Decodes a command to a specified type
        internal T Decode<T>(byte[] Data)
        {
            // Verify type
            if (!typeof(ICommandResponseBase<T>).IsAssignableFrom(typeof(T)))
            {
                Connection.OnError?.Invoke("Failed to decode command response - incorrect type: " + typeof(T).FullName + ", expected type ICommandResponseBase<T>", EventArgs.Empty);
                return default(T);
            }

            // Create a default object of the deserialized type
            ICommandResponseBase<T> O = default(T) as ICommandResponseBase<T>;

            // Return the deserialized object
            return O.Deserialize(Data);
        }
    }
}
