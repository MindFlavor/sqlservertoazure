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
                new DateValuePair() { Value = 100.0D, Date = DateTime.Parse("01-01-01 15:30:00.00") },
                new DateValuePair() { Value = 50.0D, Date = DateTime.Parse("01-01-01 15:30:00.30") },
                new DateValuePair() { Value = 10.0D, Date = DateTime.Parse("01-01-01 15:30:02.50") },
                new DateValuePair() { Value = 10.0D, Date = DateTime.Parse("01-01-01 15:30:15.00") }
            });

            StreamNormalizer sn = new StreamNormalizer(new TimeSpan(0, 0, 1));
            
            foreach(DateValuePair dvp in lDVPs)
            {
                List<DateValuePair> lOutput = sn.Push(dvp);

                for(int i=0; i<lOutput.Count; i++)
                {
                    System.Console.WriteLine("{0:S} - {1:N2}", lOutput[i].Date.ToString(), lOutput[i].Value);
                }
            }
        }
    }
}
