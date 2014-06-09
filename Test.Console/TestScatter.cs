using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITPCfSQL.Azure.TimeNormalization.Scatter;
using ITPCfSQL.Azure.TimeNormalization;

namespace Test.Console
{
    public class TestScatter
    {
        static List<DateValuePair> lDVP;

        static TestScatter()
        {
            lDVP = new List<DateValuePair>();
            
            lDVP.Add(new DateValuePair() { Date = DateTime.Parse("2014-01-01 10:00:00"), Value = 200 });
            lDVP.Add(new DateValuePair() { Date = DateTime.Parse("2014-01-01 10:00:00"), Value = 150 });
            lDVP.Add(new DateValuePair() { Date = DateTime.Parse("2014-01-01 10:00:00"), Value = 150 });
            lDVP.Add(new DateValuePair() { Date = DateTime.Parse("2014-01-01 10:00:00"), Value = 200 });
            lDVP.Add(new DateValuePair() { Date = DateTime.Parse("2014-01-01 10:00:00"), Value = 200 });
            lDVP.Add(new DateValuePair() { Date = DateTime.Parse("2014-01-01 10:00:00"), Value = 170 });

            lDVP.Add(new DateValuePair() { Date = DateTime.Parse("2014-01-01 10:00:01"), Value = 170 });
            lDVP.Add(new DateValuePair() { Date = DateTime.Parse("2014-01-01 10:00:01"), Value = 170 });
            lDVP.Add(new DateValuePair() { Date = DateTime.Parse("2014-01-01 10:00:01"), Value = 40 });
            lDVP.Add(new DateValuePair() { Date = DateTime.Parse("2014-01-01 10:00:01"), Value = 1470 });
            lDVP.Add(new DateValuePair() { Date = DateTime.Parse("2014-01-01 10:00:01"), Value = 370 });
            lDVP.Add(new DateValuePair() { Date = DateTime.Parse("2014-01-01 10:00:01"), Value = 1370 });
            lDVP.Add(new DateValuePair() { Date = DateTime.Parse("2014-01-01 10:00:01"), Value = 60 });
            lDVP.Add(new DateValuePair() { Date = DateTime.Parse("2014-01-01 10:00:01"), Value = 170 });
            lDVP.Add(new DateValuePair() { Date = DateTime.Parse("2014-01-01 10:00:01"), Value = 60 });

            lDVP.Add(new DateValuePair() { Date = DateTime.Parse("2014-01-01 10:00:02"), Value = 30 });
            lDVP.Add(new DateValuePair() { Date = DateTime.Parse("2014-01-01 10:00:02"), Value = 3000 });
        }

        public static void DoRandom()
        {
            RandomScatter rs = new RandomScatter(new TimeSpan(0,0,1));

            foreach(DateValuePair dvp in lDVP)
            {
                List<DateValuePair> lDVPOutput = rs.Push(dvp);
                foreach(DateValuePair dvpOutput in lDVPOutput)
                {
                    System.Console.WriteLine("{0:S} - {1:S}", dvpOutput.Date.ToString("hh:mm:ss.ffff"), dvpOutput.Value.ToString());
                }
            }

        }

        public static void DoEven()
        {
            EvenlyDistribute ed = new EvenlyDistribute(new TimeSpan(0, 0, 1));
            ed.Start(lDVP[0]);

            foreach (DateValuePair dvp in lDVP)
            {
                List<DateValuePair> lDVPOutput = ed.Push(dvp);
                foreach (DateValuePair dvpOutput in lDVPOutput)
                {
                    System.Console.WriteLine("{0:S} - {1:S}", dvpOutput.Date.ToString("hh:mm:ss.ffff"), dvpOutput.Value.ToString());
                }
            }
        }
    }
}
