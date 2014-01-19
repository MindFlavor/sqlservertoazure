using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITPCfSQL.Azure.Streaming;

namespace Test.Console
{
    class TestStreaming
    {
        public static void StringSplit()
        {
            string strContent = "!(140) ,!025 and !140,!253,!253 senza 352,!52Y,!CC 1300,!CC 1400,!CC 1600,!CC 900,!CNG,(!CC 1300)AND(!CC 1400),(!CC 1300)AND(!CC 1400)AND(!CC 1600),(!CC 900)AND(!CC 1300),(!CC 900)AND(!CC 1300)AND(!CC 1400),(!CC 900)AND(!CC 1600),(025) and (140),(025-)AND(140-)AND(52Y),(025)AND(52Y),(082-),(140),(150) Knee Bag,(508),(52Y) and (!140),(6DC),(Benz) and (Benz/Metan),(Benz) OR (Benz/Metan),(CC 1300 OR CC1600),(CC 1300 OR CC1600) !OPT253,(CC 1300 OR CC1600) and OPT253,(CC900 OR CC 1300 OR CC1600),025,1.4TJet,352 PC633,400 or 59E (Open roof),407,420,421,431,432,433,435,439,4JF,4WQ,55E,56J,59E,5A6,5DE or CC1900,5EQ,5ER,5EV,5FB,68P,6DM,6QB,all,Bombola metano,CC 0.9 OR 1.4Jet,CC 1.6 !025 !140,CC 1.6 OR 1.4Jet ,CC 1300,CC 1300 !407,CC 1300 !MTA,CC 1300 407,CC 1300 MTA,CC 1300 OR CC 1600,CC 1400,CC 1400 330 530 OPT_!140_!025,CC 1400 330 OPT_!025,CC 1400 330 OPT_!140,CC 1400 530 OPT_!025,CC 1400 530 OPT_!140,CC 1400 TUTTI_I_MODELLI,CC 1600,CC 1600 OPT 253,CC 900,CC 900 OPT_!025,CC 900 OPT_!025_!140,CC 900 OPT_!140,CC 900 OPT_025,CC 900 OPT_140,CC1.4 !352 ,CC1400 OPT025 OPT 140,Cerchi in Lega,CNG,Diesel,ECU_ETM,ECU_LTM,Ext Color 268,G_DX,G_SX,GUIDA51,MOD 330 and (CC1.3 or CC1.6),MOD 330 and Color 268,MOD 352 !(PC633) !(OPT222),MOD 352 (PC633) (OPT222),MOD 352 CC1.4,MOD 352 CC1.4 (C633),MOD 352 CC1.4 (C635),MOD 530 CC 1300 !407,MOD 530 CC 1300 OPT.407,MOD 530 CC1.4,MOD_351 with CAMERA,MODELLO 351 OPT087,MODELLO OMOLOGATIVO 330,No Body 104,No colore 268,OPT253 senza 352,P253,P365,P52Y,P52Y senza 352,P5DE,Ruote in Lamiera (404 OR 406),RUOTE_OPT_404,RUOTE_OPT_406,RUOTE_OPT_433,RUOTE_OPT_435,RUOTE_OPT_439,RUOTE_OPT_4WQ,RUOTE_OPT_4XE,RUOTE_OPT_55E,RUOTE_OPT_56J,RUOTE_OPT_5A6,RUOTE_OPT_5EQ,RUOTE_OPT_5FB,RUOTE_OPT_68P,RUOTE_OPT_6U4,RUOTE_OPT_6WR,RUOTE_OPT_6XN,RUOTE_OPT_6Y8,SeqMatCall-Test-201489,test_Marco_MaterialCall,Tetto Appribile (OPT400),TETTO_APERTO,TUTTI I TIPI - MODELLI 330 351 352 !OPT087,TUTTI I TIPI - MODELLI 330 e 351,TUTTI I TIPI - MODELLI 330 e 352 Passo Corto,TUTTI I TIPI - MODELLI 530 !OPT087,TUTTI I TIPI - MODELLO 330,TUTTI I TIPI - MODELLO 351,TUTTI I TIPI - MODELLO 352,TUTTI I TIPI - MODELLO 530";
            StringSplit sp = new StringSplit(strContent, ",");

            int iNum = 0;
            foreach (string str in sp)
            {
                System.Console.WriteLine(str);
                iNum++;
            }

            System.Diagnostics.Debug.Assert(iNum == 143);
        }

        public static void LineStreamerFromHTTPS()
        {
            System.Net.WebRequest request = System.Net.WebRequest.Create("https://www.dati.lombardia.it/api/views/q563-n2qm/rows.csv?accessType=DOWNLOAD");
            request.Method = "GET";

            LineStreamer lineStreamer = new LineStreamer(request.GetResponse().GetResponseStream());
            foreach (string str in lineStreamer)
            {
                System.Console.WriteLine(str);
            }
        }

        public static void XMLPlainLevelStreamerFromHTTPS()
        {
            System.Net.WebRequest request = System.Net.WebRequest.Create("https://www.dati.lombardia.it/api/views/q563-n2qm/rows.xml?accessType=DOWNLOAD");
            request.Method = "GET";
            using (System.IO.Stream s = request.GetResponse().GetResponseStream())
            {
                _XMLPlainLevelStreamer(s);
            }
        }

        public static void XMLPlainLevelStreamerFromFile()
        {
            using (System.IO.FileStream fs = new System.IO.FileStream(
                ".\\testdata\\rowsos.xml",
                System.IO.FileMode.Open,
                System.IO.FileAccess.Read,
                System.IO.FileShare.Read))
            {
                _XMLPlainLevelStreamer(fs);
            }
        }

        private static void _XMLPlainLevelStreamer(System.IO.Stream s)
        {
            using (XMLPlainLevelStreamer xmlPlainLevelStreamer = new XMLPlainLevelStreamer(s, 2))
            {
                int iCnt = 0;
                foreach (Dictionary<string, string> dResult in xmlPlainLevelStreamer)
                {
                    System.Console.WriteLine(
                        "{0:D4}) {1:N0} keys",
                        iCnt++, dResult.Keys.Count);
                    foreach (KeyValuePair<string, string> kvp in dResult)
                    {
                        System.Console.WriteLine(
                            "\t{0:S}: {1:S}",
                            kvp.Key, kvp.Value);
                    }
                }
            }
        }
    }
}
