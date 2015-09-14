using System;
using System.Collections.Generic;
using System.Text;

namespace XPlaneUdpData.Core
{
    class XPDataRefStream
    {
        private string _cmd;
        private List<XPDataRefResult> _dataRefs = new List<XPDataRefResult>();

        public IEnumerable<XPDataRefResult> DataRefs
        {
            get { return _dataRefs; }
        }

        public XPDataRefStream(string cmd, IEnumerable<XPDataRefResult> datarefs)
        {
            _cmd = cmd;
            _dataRefs.AddRange(datarefs);
        }

        public static XPDataRefStream ReadFromArray(byte[] from)
        {
            int dataref_count = (from.Length - 5) / 8;
            int dataref_pos = 0;
            List<XPDataRefResult> datarefs = new List<XPDataRefResult>();
            string dref_cmd = Encoding.UTF8.GetString(from, 0, 4);

            int dref_en = BitConverter.ToInt32(from, 5 + dataref_pos);
            while (dref_en > 0)
            {
                float dref_flt = BitConverter.ToSingle(from, 9 + dataref_pos);
                datarefs.Add(new XPDataRefResult(dref_en, dref_flt));
                dataref_pos = (dataref_pos + 8);
                if (from.Length - (5 + dataref_pos) < 8) break;
                dref_en = BitConverter.ToInt32(from, 5 + dataref_pos);
            }

            return new XPDataRefStream(dref_cmd, datarefs);
        }
    }
}
