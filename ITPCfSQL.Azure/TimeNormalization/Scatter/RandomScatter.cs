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
        #endregion
   

        #region Constructors
        public RandomScatter(
            TimeSpan tsStep)
        {
            this.Step = tsStep;
        }
        #endregion
    }
}
