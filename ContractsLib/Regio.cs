using System;

namespace Contracts
{
    public class Regio
    {
        #region Properties
        //Bij gebrek aan info heb ik properties hier als default gezet 
        public int Id { get; set; } = 1;
        public string Naam { get; set; } = "Vlaanderen";
        public int LandId { get; set; } = 32;
        #endregion
    }
}
