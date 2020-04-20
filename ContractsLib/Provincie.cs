using System;


namespace Contracts
{
    public class Provincie
    {
        #region Properties
        public int Id { get; set; }
        public string TaalCode { get; set; }
        public string Naam { get; set; }

        public int RegioId { get; set; }
        #endregion
    }
}
