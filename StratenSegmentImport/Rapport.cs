using System;
using System.Collections.Generic;
using System.Text;
using ImportFromFiles.Providers;
using System.IO;
using Contracts;
using System.Linq;

namespace ImportFromFiles
{
    class Rapport
    {
        private static RegioProvider _regioProvider = new RegioProvider("nl");
        private static GemeenteProvincieProvider _gemeenteProvider = new GemeenteProvincieProvider("nl");
        private static StratenProvider _stratenProvider = new StratenProvider();

        public static void Rapporteer(string path)
        {
            var sb = new StringBuilder();

            var provincies = _regioProvider.GetProvinciesByRegioId(1);
            sb.Append("Aantal straten per provincie: \n");
            foreach (var provincie in provincies)
            {
                int aantalStraten = 0;
                var gemeenten = _gemeenteProvider.GetGemeentenByProvincieId(provincie.Id);
                foreach (var gemeente in gemeenten)
                {
                    var straten = _stratenProvider.GetStratenByGemId(gemeente.Id);
                    if (straten != null)
                        aantalStraten += straten.Count;
                }
                sb.Append("  -" + provincie.Naam + ": " + aantalStraten + "\n");

            }
            sb.Append("\n");
            foreach (var provincie in provincies)
            {
                sb.Append($"StraatInfo {provincie.Naam}: \n");
                foreach (var gemeente in _gemeenteProvider.GetGemeentenByProvincieId(provincie.Id).OrderBy(g => g.Naam))
                {
                    sb.Append(PrintGemeenteInfo(gemeente));
                }
            }
            Console.WriteLine(sb.ToString());
            using var streamWriter = new StreamWriter(path);
            streamWriter.Write(sb);

        }
        private static StringBuilder PrintGemeenteInfo(Gemeente g)
        {
            var sb = new StringBuilder();
            double totalLength = 0;
            double lengteKorsteStraat = double.MaxValue;
            int indexKorste = 0;
            double lengteLangsteStraat = 0;
            int indexLangste = 0;
            var straten = _stratenProvider.GetStratenByGemId(g.Id);
            if (straten == null)
            {
                sb.Append("     †  " + g.Naam + " werd gefuseerd in 2019  †\n\n");
                return sb;
            }
            for (int i = 0; i < straten.Count; i++)
            {
                var length = Calculator.getLenthOfStraat(straten[i]);
                totalLength += length;
                if (length > lengteLangsteStraat)
                {
                    indexLangste = i;
                    lengteLangsteStraat = length;
                }
                if (length < lengteKorsteStraat)
                {
                    indexKorste = i;
                    lengteKorsteStraat = length;
                }
            }
            Straat korsteStraat = straten[indexKorste];
            Straat langsteStraat = straten[indexLangste];
            sb.Append("     -" + g.Naam + ": " + straten.Count + " straten ||  " + totalLength + "\n");
            sb.Append(GetSpaces(g.Naam.Length) + "       +" + korsteStraat.Id + ", " + korsteStraat.StraatNaam + ", " + lengteKorsteStraat + "\n");
            sb.Append(GetSpaces(g.Naam.Length) + "       +" + langsteStraat.Id + ", " + langsteStraat.StraatNaam + ", " + lengteLangsteStraat + "\n\n");
            return sb;
        }
        
        private static string GetSpaces(int n)
        {
            string s = "";
            for (int i = 0; i < n; i++)
            {
                s += " ";
            }
            return s;
        }
    }
}
