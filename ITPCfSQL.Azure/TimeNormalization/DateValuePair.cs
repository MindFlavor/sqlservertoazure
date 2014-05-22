using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPCfSQL.Azure.TimeNormalization
{
    public class DateValuePair
    {
        public DateValuePair()
        {
            OriginalPosition = -1;
        }
        public DateValuePair(long OriginalPosition)
        {
            this.OriginalPosition = OriginalPosition;
        }
        public long OriginalPosition { get; set; }
        public double Value { get; set; }
        public DateTime Date { get; set; }

        public override string ToString()
        {
            return this.GetType().Name + "[Date=" + Date + ", Value=" + Value + "]";
        }
    }
}
