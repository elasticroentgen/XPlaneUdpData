namespace XPlaneUdpData.Core
{
    public class XPDataRef
    {
        public int index;
        public string dref;
        public int interval;

        private XPDataRefResult _result;

        public XPDataRefResult Result
        {
            get { return _result; }
            set
            {
                _result = value;
                OnCallback(_result);
            }
        }

        public delegate void Callback(XPDataRefResult e);
        public event Callback OnUpdate;

        public XPDataRef()
        {
        }

        protected void OnCallback(XPDataRefResult e)
        {
            if (OnUpdate != null)
                OnUpdate(e);
        }
    }
}
