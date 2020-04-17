using Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using ImportFromFiles.Providers;

namespace ExportToDB
{
    public class Export
    {
        private string _taalCode;
        public Export(string taalCode)
        {
            _taalCode = taalCode;
            _regioProvider = new RegioProvider(_taalCode);
            _gemeenteProvider = new GemeenteProvincieProvider(_taalCode);
            _stratenProvider = new StratenProvider();
        }
        private RegioProvider _regioProvider;
        private GemeenteProvincieProvider _gemeenteProvider;
        private StratenProvider _stratenProvider;

        

        public void ExportToDB()
        {
            var land = new Land() { Id = 32, Naam = "Belgie", TaalCode = "nl" };
            Exporters.ExportLand(land);

            var regio = new Regio() { Id = 1, Naam = "Vlaanderen", LandId = 32 };
            Exporters.ExportRegio(regio);

            var provincies = _regioProvider.GetProvinciesByRegioId(regio.Id);
            foreach (var pro in provincies)
            {
                Exporters.ExportProvincie(pro);
                var gemeenten = _gemeenteProvider.GetGemeentenByProvincieId(pro.Id);
                foreach (var gemeente in gemeenten)
                {
                    var straten = _stratenProvider.GetStratenByGemId(gemeente.Id);
                    if (straten != null)
                    {
                        Exporters.ExportGemeente(gemeente);
                        foreach (Straat straat in straten)
                        {
                            Exporters.ExportStraat(straat);
                        }
                    }
                }
            }
        }
    }
}
