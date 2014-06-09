using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPCfSQL.Azure.TimeNormalization.Scatter
{
    public class EvenlyDistribute
    {
        #region Members
        private DateTime dtNextStep = DateTime.MinValue;
        private List<DateValuePair> lDVP = null;
        #endregion

        #region Properties
        public TimeSpan Step { get; set; }
        #endregion

        #region Constructors
        public EvenlyDistribute(
            TimeSpan tsStep)
        {
            this.Step = tsStep;
        }
        #endregion

        public void Start(DateValuePair Start)
        {
            this.dtNextStep = Start.Date.Add(Step);
            lDVP = new List<DateValuePair>();
        }

        public List<DateValuePair> Push(DateValuePair dvp)
        {
            if (dvp.Date >= dtNextStep)
            {
                // output collected
                double dMSEven = Step.TotalMilliseconds / ((double)lDVP.Count);
                for (int i = 0; i < lDVP.Count; i++)
                {
                    lDVP[i].Date = lDVP[i].Date.AddMilliseconds(dMSEven*i);
                }

                List<DateValuePair> lDVPOutput = lDVP;

                // reset accumulator
                lDVP = new List<DateValuePair>();
                lDVP.Add(dvp);

                // Find next step containing this dvp... this *should* be optimized
                while (dvp.Date >= (dtNextStep.Add(Step)))
                {
                    dtNextStep = dtNextStep.Add(Step);
                }

                return lDVPOutput;
            }
            else
            {
                // add
                lDVP.Add(dvp);
                // return empty list
                return new List<DateValuePair>();
            }
        }
    }
}
