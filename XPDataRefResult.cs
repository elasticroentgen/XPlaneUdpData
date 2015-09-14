namespace XPlaneUdpData.Core
{
    public class XPDataRefResult
    {
        private int _dref_en;
        public int dref_en
        {
            get { return _dref_en; }
        }

        private float _dref_flt;
        public float dref_flt
        {
            get { return _dref_flt; }
        }

        public XPDataRefResult(int dref_en, float dref_flt)
        {
            _dref_en = dref_en;
            _dref_flt = dref_flt;
        }
    }
}
