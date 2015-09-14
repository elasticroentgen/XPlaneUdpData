using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace XPlaneUdpData.Core
{
    public class XPDataRefEventArgs : EventArgs
    {
        public List<XPDataRef> DataRefs { get; set; }
    }

    public class XPlaneData
    {
        private class UdpState : Object
        {
            public UdpState(IPEndPoint e, UdpClient c) { this.e = e; this.c = c; }
            public IPEndPoint e;
            public UdpClient c;
        }

        private UdpClient _udpClient;
        private IPEndPoint _localEP;
        private IPEndPoint _remoteEP;
        private IAsyncResult _currentAsyncResult = null;

        private List<XPDataRef> _dataRefs = new List<XPDataRef>();
        public List<XPDataRef> DataRefs
        {
            get { return _dataRefs; }
        }

        public event EventHandler<XPDataRefEventArgs> OnDataRefUpdate = null;

        public XPlaneData()
        { }

        public XPlaneData(string remoteHost, int remotePort = 49000, string localHost = "127.0.0.1", int localPort = 49010)
        {
            _remoteEP = new IPEndPoint(IPAddress.Parse(remoteHost), remotePort);
            _localEP = new IPEndPoint(IPAddress.Parse(localHost), localPort);
            _udpClient = new UdpClient();
            _udpClient.Client.Bind(_localEP);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            if (ar == _currentAsyncResult)
            {
                UdpClient c = (UdpClient)((UdpState)(ar.AsyncState)).c;
                IPEndPoint e = (IPEndPoint)((UdpState)(ar.AsyncState)).e;

                try
                {
                    byte[] buffer = c.EndReceive(ar, ref e);
                    UdpState s = new UdpState(e, c);
                    _currentAsyncResult = _udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), s);

                    XPDataRefStream stream = XPDataRefStream.ReadFromArray(buffer);

                    if (stream != null)
                    {
                        foreach (XPDataRefResult dataref in stream.DataRefs)
                        {
                            _dataRefs[dataref.dref_en - 1].Result = dataref;
                        }

                        DataRefUpdate(this, new XPDataRefEventArgs());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("XPlane not loaded?");
                }
            }
        }

        public int AddDataRef(string dref, int perSecond, XPDataRef.Callback theCallback)
        {
            XPDataRef dataRef = new XPDataRef();
            dataRef.dref = dref;
            dataRef.interval = perSecond;
            dataRef.OnUpdate += theCallback;
            _dataRefs.Add(dataRef);
            dataRef.index = _dataRefs.IndexOf(dataRef) + 1;
            return _dataRefs.IndexOf(dataRef);
        }

        public void StartPolling()
        {
            var s = new UdpState(_localEP, _udpClient);

             _currentAsyncResult = _udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), s);

            foreach (XPDataRef x in _dataRefs)
            {
                Console.WriteLine($"opening up {x.dref} with index {x.index}...");
                byte[] data = GetDref(x.dref, x.interval, x.index);
                _udpClient.Send(data, data.Length, _remoteEP);
            }
        }

        public void StopPolling()
        {
            foreach (XPDataRef x in _dataRefs)
            {
                Console.WriteLine($"closing up {x.dref} with index {x.index}...");
                byte[] data = GetDref(x.dref, 0, x.index);
                _udpClient.Send(data, data.Length, _remoteEP);
            }
        }

        private byte[] GetDref(string dref, int perSecond, int id)
        {
            List<byte> dataStream = new List<byte>();
            string _dref = dref.PadRight(400, '\0');

            dataStream.AddRange(Encoding.ASCII.GetBytes("RREF\0"));
            dataStream.AddRange(BitConverter.GetBytes(perSecond));
            dataStream.AddRange(BitConverter.GetBytes(id));
            dataStream.AddRange(Encoding.ASCII.GetBytes(_dref));

            return dataStream.ToArray();
        }

        private byte[] SetDref(string dref, float value)
        {
            List<byte> dataStream = new List<byte>();
            string _dref = dref.PadRight(500, '\0');

            dataStream.AddRange(Encoding.ASCII.GetBytes("DREF\0"));
            dataStream.AddRange(BitConverter.GetBytes(value));
            dataStream.AddRange(Encoding.ASCII.GetBytes(_dref));

            return dataStream.ToArray();
        }

        protected void DataRefUpdate(object sender, XPDataRefEventArgs e)
        {
            if (OnDataRefUpdate != null)
                OnDataRefUpdate(this, new XPDataRefEventArgs { DataRefs = _dataRefs });
        }
    }
}
