using Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using GenericParsing;


namespace ImportFromFiles.Providers
{
    public class GemeenteProvincieProvider
    {
        private Dictionary<int, Gemeente> _gemeentes = new Dictionary<int, Gemeente>();
        private Dictionary<int, Provincie> _provincies = new Dictionary<int, Provincie>();
        private Dictionary<int, List<Gemeente>> _proIdGemeenten = new Dictionary<int, List<Gemeente>>();
        private string _taalCode;

        public GemeenteProvincieProvider(string taalCode)
        {
            _taalCode = taalCode;
            Read();
        }

        private void Read()
        {

            GenericParser parser = new GenericParser(Config.Path + "/" + Config.Gemeentenamen)
            {
                ColumnDelimiter = ';',
                MaxBufferSize = 4096,
                FirstRowHasHeader = true
            };
            while (parser.Read())
            {

                int gemeenteNaamId = Convert.ToInt32(parser[0].Trim());
                int gemeenteId = Convert.ToInt32(parser[1].Trim());
                string taalCode = parser[2].Trim();
                string gemeenteNaam = parser[3].Trim();
                if (taalCode != _taalCode) continue;
                var nieuweGem = new Gemeente() { Id = gemeenteId, NaamId = gemeenteNaamId, TaalCode = taalCode, Naam = gemeenteNaam };
                _gemeentes.Add(gemeenteId, nieuweGem);

            }
            parser = new GenericParser(Config.Path + "/" + Config.ProvincieInfo)
            {
                ColumnDelimiter = ';',
                MaxBufferSize = 4096,
                FirstRowHasHeader = true
            };
            while (parser.Read())
            {
                int gemeenteId = Convert.ToInt32(parser[0].Trim());
                int provincieId = Convert.ToInt32(parser[1].Trim());
                string taalCode = parser[2].Trim();
                string provincieNaam = parser[3].Trim();
                if (taalCode != _taalCode) continue;
                if (_gemeentes.ContainsKey(gemeenteId))
                {
                    _gemeentes[gemeenteId].ProvincieId = provincieId;
                    if (!_proIdGemeenten.ContainsKey(provincieId))
                    {
                        _proIdGemeenten.Add(provincieId, new List<Gemeente> { GetGemeenteById(gemeenteId) });
                    }
                    else
                    {
                        _proIdGemeenten[provincieId].Add(GetGemeenteById(gemeenteId));
                    }
                }
                if(!_provincies.ContainsKey(provincieId))
                _provincies.Add(provincieId, new Provincie { Id = provincieId, Naam = provincieNaam, TaalCode = _taalCode });
            }
        }
        private Gemeente GetGemeenteById(int gemeenteId)
        {
            if (_gemeentes.ContainsKey(gemeenteId)) return _gemeentes[gemeenteId];
            else throw new Exception("No data for gemeenteId " + gemeenteId);
        }
        public Provincie GetProvincieWithoutRegioIdById(int provincieId)
        {
            if (_provincies.ContainsKey(provincieId)) return _provincies[provincieId];
            else throw new Exception("No data for provincieId " + provincieId);
        }
        public List<Gemeente> GetGemeentenByProvincieId(int provincieId)
        {
            if (_proIdGemeenten.ContainsKey(provincieId)) return _proIdGemeenten[provincieId];
            else throw new Exception("No data for provincieId " + provincieId);
        }



    }
}
