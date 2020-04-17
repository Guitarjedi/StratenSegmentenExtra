using System;

namespace Contracts
{
    public class Land
    {
        #region Properties
        //Bij gebrek aan info heb ik properties hier als default gezet 
        public int Id { get; set; } = 32;
        public string Naam { get; set; } = "Belgie";
        public string TaalCode { get; set; } = "nl";
        #endregion
    }
}
