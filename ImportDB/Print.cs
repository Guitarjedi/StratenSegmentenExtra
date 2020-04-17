using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImportFromDB
{
    public static class Print
    {

        #region PrinterMethods
        public static void PrintKruisendeStraten(int straatId)
        {
            var s = Importers.GetStraatById(straatId);
            var gemeenteNaam = Importers.GetGemeenteNaamEnProvincieNaamByGemeenteId(s.GemeenteId).Item1;
            var message = "Straten die kruisen met " + straatId + ", " + s.StraatNaam + " " + gemeenteNaam;
            GetHeader(message);
            var straten = Importers.GetAdjacentStraten(s);
            for (int i = 0; i < straten.Count; i++)
            {
                Console.WriteLine(GetXchars(55, ' ') + straten[i].StraatNaam);

            }
            if (straten.Count == 0)
            {
                Console.WriteLine(GetXchars(45, ' ') + "Geen info over deze straat gevonden");
            }
            GetFooter(message);


        }
        public static void PrintLijstStraatNamen(string gemeenteNaam)
        {

            var message = "Straten in " + gemeenteNaam;
            GetHeader(message);

            var straatNamen = Importers.GetListStraatNamenByGemeenteNaam(gemeenteNaam);
            string lijntje = GetXchars(20, ' ');
            for (int i = 0; i < straatNamen.Count; i++)
            {
                lijntje += GetXchars(2, ' ') + straatNamen[i];

                if (lijntje.Length >= 90)
                { Console.WriteLine(lijntje); lijntje = GetXchars(20, ' '); }

            }
            if (straatNamen.Count == 0)
            {
                Console.WriteLine(GetXchars(45, ' ') + "Geen info over deze gemeente gevonden");
            }

            Console.WriteLine("\n\n" + GetXchars(50, ' ') + GetXchars(message.Length, '='));

        }
        public static void PrintLijstStraatIds(string gemeenteNaam)
        {
            var message = "StraadIds in " + gemeenteNaam;
            GetHeader(message);
            var straatIds = Importers.GetListStraatIdByGemeenteNaam(gemeenteNaam);
            for (int i = 0; i < straatIds.Count; i++)
            {
                if (i % 13 == 0)
                    Console.Write("\n" + GetXchars(15, ' ') + straatIds[i]);
                else
                    Console.Write(GetXchars(2, ' ') + straatIds[i]);
            }
            if (straatIds.Count == 0)
            {
                Console.WriteLine(GetXchars(45, ' ') + "Geen info over deze gemeente gevonden");
            }
            GetFooter(message);
        }
        public static void PrintStraat(string straatNaam, string gemeenteNaam)
        {
            var message = $"Info van {straatNaam} in {gemeenteNaam}";
            GetHeader(message);
            var s = Importers.GetStraatByStraatNaamEnGemeenteNaam(straatNaam, gemeenteNaam);
            if (s.Graaf.Map.Count == 0)
                Console.WriteLine(GetXchars(45, ' ') + $"Geen info gevonden voor de {straatNaam} in {gemeenteNaam}");
            else
                PrintStraat(s);
            GetFooter(message);
        }
        public static void PrintStraat(int straatId)
        {
            var message = $"Info van {straatId}";
            GetHeader(message);
            var s = Importers.GetStraatById(straatId);
            if (s.Graaf.Map.Count == 0)
                Console.WriteLine(GetXchars(45, ' ') + "Geen info gevonden voor dit straatId");
            else
                PrintStraat(s);
            GetFooter(message);
        }
        public static void PrintProvincieInfo(string provincieNaam)
        {
            var message = "Straten in " + provincieNaam;
            GetHeader(message);

            var gemeenten = Importers.GetGemeentesIdNaamByProvincieNaam(provincieNaam);
            if (gemeenten.Count != 0)
            {
                foreach (var g in gemeenten)
                {
                    var tempList = new List<Tuple<string, double>>();
                    var stratenIds = Importers.GetListStraatIdByGemeenteNaam(g.Naam);
                    var gemeenteInfo = $"{g.Naam}, {stratenIds.Count}";
                    Console.WriteLine("\n\n" + GetXchars(55, ' ') + gemeenteInfo);
                    Console.WriteLine(GetXchars(55, ' ') + GetXchars(gemeenteInfo.Length, '='));
                    foreach (var straatId in stratenIds)
                    {
                        var straat = Importers.GetStraatById(straatId);
                        var lengte = Calculator.getLenthOfStraat(straat);
                        tempList.Add(new Tuple<string, double>(straat.StraatNaam, lengte));

                    }
                    var orderedStaat = tempList.OrderBy(t => t.Item2);
                    foreach (var tuple in orderedStaat)
                    {
                        Console.WriteLine(GetXchars(45, ' ') + $"-{tuple.Item1}, {tuple.Item2}");
                    }
                }
            }
            else
            {
                Console.WriteLine(GetXchars(45, ' ') + "Geen info gevonden voor " + provincieNaam);
            }

            GetFooter(message);
        }
        private static void PrintStraat(Straat s)
        {
            var info = Importers.GetGemeenteNaamEnProvincieNaamByGemeenteId(s.GemeenteId);
            Console.WriteLine(s.Id + ", " + s.StraatNaam + ", " + info.Item1 + ", " + info.Item2);
            Console.WriteLine("Graaf: " + s.Graaf.Id);
            Console.WriteLine("Aantal knopen : " + Importers.GetAllKnoopIdsFromStraatId(s.Id).Count);
            Console.WriteLine("Aantal wegsegmenten : " + s.Graaf.Map.Count);

            foreach (var segment in s.Graaf.Map)
            {
                Console.WriteLine($"Knoop[{segment.BeginKnoop.Id}, [{segment.BeginKnoop.Punt.X}, {segment.BeginKnoop.Punt.Y}]]");
                Console.WriteLine($"     [segment : {segment.Id}, begin:{segment.BeginKnoop.Id}, eind:{segment.EindKnoop.Id}]");
                foreach (var punt in segment.Vertices)
                {
                    Console.WriteLine($"              ({punt.X}, {punt.Y})");
                }
                Console.WriteLine($"Knoop[{segment.EindKnoop.Id}, [{segment.EindKnoop.Punt.X}, {segment.EindKnoop.Punt.Y}]]");
            }
        }
        #endregion

        #region PrinterTools
        private static void GetHeader(string message)
        {
            Console.WriteLine(GetXchars(50, ' ') + message);
            Console.WriteLine(GetXchars(50, ' ') + GetXchars(message.Length, '='));
            Console.WriteLine();
        }
        private static void GetFooter(string message)
        {
            Console.WriteLine("\n\n" + GetXchars(50, ' ') + GetXchars(message.Length, '='));
        }
        private static string GetXchars(int n, char c)
        {
            string s = "";
            for (int i = 0; i < n; i++)
            {
                s += c;
            }
            return s;
        }
        #endregion
    }
}
