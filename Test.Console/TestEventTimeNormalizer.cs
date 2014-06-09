using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITPCfSQL.Azure.TimeNormalization;

namespace Test.Console
{
    public class TestEventTimeNormalizer
    {
        public static void Do()
        {
            List<DateValuePair> lDVPs = new List<DateValuePair>();
            lDVPs.AddRange(new DateValuePair[]
            {
                new DateValuePair() { Value = 100.0D, Date = DateTime.Parse("2014-01-01 17:00:00") },
                new DateValuePair() { Value = 50.0D, Date = DateTime.Parse("2014-01-01 17:00:01") },
                new DateValuePair() { Value = 0.0D, Date = DateTime.Parse("2014-01-01 17:00:02") },
                new DateValuePair() { Value = 180.0D, Date = DateTime.Parse("2014-01-01 17:00:02.5") }
            });

            StreamNormalizer sn = new StreamNormalizer(new TimeSpan(0, 0, 1));

            foreach (DateValuePair dvp in lDVPs)
            {
                List<DateValuePair> lOutput = sn.Push(dvp);

                for (int i = 0; i < lOutput.Count; i++)
                {
                    System.Console.WriteLine("{0:S} - {1:N2}", lOutput[i].Date.ToString(), lOutput[i].Value);
                }
            }

            DateValuePair dvpLast = sn.Finish();
            if (dvpLast != null)
                System.Console.WriteLine("*Last {0:S} - {1:N2}", dvpLast.Date.ToString(), dvpLast.Value);
        }
    }
}
