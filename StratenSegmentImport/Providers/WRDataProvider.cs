using System;
using System.Collections.Generic;
using System.Text;
using GenericParsing;
using System.Diagnostics;
using Contracts;

namespace ImportFromFiles.Providers
{
    public class WRDataProvider    {
        public WRDataProvider()
        {
            Read();
        }
        private Dictionary<int, List<Segment>> _straatIdSegmenten = new Dictionary<int, List<Segment>>();
        private void Read()
        {
            Console.WriteLine("Reading the data...");
            
            GenericParser reader = new GenericParser(Config.Path + "/" + Config.Data)
            {
                ColumnDelimiter = ';',
                MaxBufferSize = 40960,
                FirstRowHasHeader = true
            };
            while (reader.Read())
            {
                int linksStraatId = Convert.ToInt32(reader[6].Trim());
                int rechtsStraatId = Convert.ToInt32(reader[7].Trim());
                if ((linksStraatId != -9) && (rechtsStraatId != -9))
                {
                    int wegsegmentId = Convert.ToInt32(reader[0].Trim());

                    string geo = reader[1];
                    int startIndex = geo.IndexOf("(") + 1;
                    int endIndex = geo.IndexOf(")");
                    string enkelInts = geo.Substring(startIndex, endIndex - startIndex);
                    string[] puntjes = enkelInts.Split(',');
                    List<Punt> punten = new List<Punt>();
                    foreach (string puntje in puntjes)
                    {
                        string tss = puntje.Replace('.', ',');
                        string[] puntData = tss.Trim(' ').Split(' ');

                        double x = Convert.ToDouble(puntData[0].Trim());
                        double y = Convert.ToDouble(puntData[1].Trim());
                        punten.Add(new Punt(x, y));
                    }
                    int beginKnoopId = Convert.ToInt32(reader[4].Trim());
                    Knoop beginKnoop = new Knoop(beginKnoopId, punten[0]);
                    int eindKnoopId = Convert.ToInt32(reader[5].Trim());
                    Knoop eindKnoop = new Knoop(eindKnoopId, punten[punten.Count - 1]);
                    Segment nieuwSegment = new Segment(wegsegmentId, beginKnoop, eindKnoop, punten);
                    
                    
                    if (_straatIdSegmenten.ContainsKey(rechtsStraatId))
                    {
                        if (!_straatIdSegmenten[rechtsStraatId].Contains(nieuwSegment))
                        {
                            _straatIdSegmenten[rechtsStraatId].Add(nieuwSegment);
                        }
                    }
                    else
                        _straatIdSegmenten.Add(rechtsStraatId, new List<Segment> { nieuwSegment });

                    if (_straatIdSegmenten.ContainsKey(linksStraatId))
                    {
                        if (!_straatIdSegmenten[linksStraatId].Contains(nieuwSegment))
                        {
                            _straatIdSegmenten[linksStraatId].Add(nieuwSegment);
                        }
                    }
                    else
                        _straatIdSegmenten.Add(linksStraatId, new List<Segment> { nieuwSegment });


                }
            }
        }
        public List<Segment> GetSegmentListByStraatId(int straatId)
        {
            if (_straatIdSegmenten.ContainsKey(straatId))
                return _straatIdSegmenten[straatId];
            else
                return null;
                throw new Exception("StraatId not present in data");
        }

        

    }
}
