using System;

namespace Contracts
{
    public class Gemeente
    {
        #region Properties
        public int Id { get; set; }
        public int NaamId { get; set; }
        public string TaalCode { get; set; }
        public string Naam { get; set; }

        /// <summary>
        /// Parent
        /// </summary>
        public int ProvincieId { get; set; }
        #endregion
    }
}
