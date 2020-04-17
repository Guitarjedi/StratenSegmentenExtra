using Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using GenericParsing;
using System.Diagnostics;

namespace ImportFromFiles.Providers
{
    public class StratenProvider
    {
        public StratenProvider()
        {
            Read();
        }
        private WRDataProvider _wrDataProvider = new Providers.WRDataProvider();
        private Dictionary<int, string> _idStraatnaam = new Dictionary<int, string>();
        private Dictionary<int, List<Straat>> _gemIdStraten = new Dictionary<int, List<Straat>>();
        private void Read()
        {

            GenericParser reader = new GenericParser(Config.Path + "/" + Config.Straatnamen)
            {
                ColumnDelimiter = ';',
                MaxBufferSize = 49600,
                FirstRowHasHeader = true
            };
            while (reader.Read())
            {
                int id = Convert.ToInt32(reader[0].Trim());
                string straatNaam = reader[1].Trim();
                if (_idStraatnaam.ContainsKey(id)) continue;
                _idStraatnaam.Add(id, straatNaam);
            }
            reader = new GenericParser(Config.Path + "/" + Config.StraatIdGemeenteId)
            {
                ColumnDelimiter = ';',
                MaxBufferSize = 49600,
                FirstRowHasHeader = true
            };
            int graafIdCounter = 0;
            while (reader.Read())
            {
                int gemeenteId = Convert.ToInt32(reader[1].Trim());
                int straatNaamId = Convert.ToInt32(reader[0].Trim());

                Graaf graaf = new Graaf();
                graaf.Id = ++graafIdCounter;

                var map = _wrDataProvider.GetSegmentListByStraatId(straatNaamId);
                if (map == null) continue;
                graaf.Map = map;
                
                string straatnaam = _idStraatnaam.ContainsKey(straatNaamId) ? _idStraatnaam[straatNaamId] : null;
                if (straatnaam != null)
                {
                    Straat s = new Straat(straatNaamId, straatnaam, gemeenteId, graaf);


                    if (_gemIdStraten.ContainsKey(gemeenteId))
                        _gemIdStraten[gemeenteId].Add(s);
                    else
                        _gemIdStraten.Add(gemeenteId, new List<Straat> { s });
                }
            }
        }
        public List<Straat> GetStratenByGemId(int gemeenteId)
        {

            if (_gemIdStraten.ContainsKey(gemeenteId))
                return _gemIdStraten[gemeenteId];
            else
                return null;

        }
    }
}
