using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPCfSQL.Azure.TimeNormalization.Scatter
{
    public class RandomScatter
    {
        #region Members
        public Random _random;
        #endregion

        #region Properties
        public TimeSpan Step { get; set; }        
        #endregion

        #region Constructors
        public RandomScatter(
            TimeSpan tsStep)
        {
            this.Step = tsStep;
            _random = new Random();
        }
        #endregion

        public List<DateValuePair> Push(DateValuePair dvp)
        {
            List<DateValuePair> lDVP = new List<DateValuePair>(1);
            lDVP.Add((DateValuePair)dvp.Clone());
            lDVP[0].Date.AddMilliseconds(_random.NextDouble() * Step.TotalMilliseconds);
            return lDVP;
        }
    }
}
