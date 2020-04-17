
using System;

namespace Contracts
{
    public class Straat
    {
        public Straat(int straatId, string straatNaam, int gemeenteId, Graaf graaf)
        {
            Id = straatId;
            StraatNaam = straatNaam;
            GemeenteId = gemeenteId;
            Graaf = graaf;
        }
        public Straat() { Graaf = new Graaf(); }
        #region Properties
        public int Id { get; set; }
        public string StraatNaam { get; set; }

        public int GemeenteId { get; set; }
        public Graaf Graaf { get; set; }

        #endregion


    }
}
