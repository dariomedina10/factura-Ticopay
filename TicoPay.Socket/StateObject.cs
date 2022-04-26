using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TicoPay.Socket
{
    class StateObject : IDisposable
    {
        // Client  socket.
        public System.Net.Sockets.Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 1000;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool p)
        {
            if (p)
            {
                Array.Clear(buffer, 0, buffer.Length);
                sb.Clear();
            }
        }

        ~StateObject()
        {
            Dispose(false);
        }
    }
}
