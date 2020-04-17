using Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using GenericParsing;
using System.IO;

namespace ImportFromFiles.Providers
{
    public class RegioProvider
    {
        private string _taalCode;
        private Dictionary<int, List<Provincie>> _regioIdListProvincies = new Dictionary<int, List<Provincie>>();
        public RegioProvider(string taalCode)
        {
            _taalCode = taalCode;
        }

        private void Read()
        {
            int _regioIdCounter = 0;
            var provider = new GemeenteProvincieProvider(_taalCode);

            StreamReader reader = new StreamReader(Config.Path + "/" + Config.ProvincieIds);
            string s;

            while ((s = reader.ReadLine()) != null)
            {
                ++_regioIdCounter;
                var idAsStringArray = s.Split(',');

                List<Provincie> provincies = new List<Provincie>();
                for (int i = 0; i < idAsStringArray.Length; i++)
                {
                    provincies.Add(provider.GetProvincieWithoutRegioIdById(Convert.ToInt32(idAsStringArray[i])));
                    provincies[i].RegioId = _regioIdCounter;

                }
                _regioIdListProvincies.Add(_regioIdCounter, provincies);
            }
        }
        public List<Provincie> GetProvinciesByRegioId(int id)
        {
            Read();
            if (_regioIdListProvincies.ContainsKey(id))
                return _regioIdListProvincies[id];
            else
                throw new Exception($"No Data found for RegioId = {id}");
               
        }
    }
}
