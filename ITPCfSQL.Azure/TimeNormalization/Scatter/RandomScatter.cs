using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPCfSQL.Azure.TimeNormalization.Scatter
{
    public class RandomScatter
    {
        #region Properties
        public TimeSpan Step { get; set; }

        public Random _random;        
        #endregion

        #region Constructors
        public RandomScatter(
            TimeSpan tsStep)
        {
            this.Step = tsStep;
            rand
        }
        #endregion

        public DateValuePair Scatter(DateValuePair dvp)
        {

            DateValuePair dvpOutput = (DateValuePair)dvp.Clone();


        }
    }
}
