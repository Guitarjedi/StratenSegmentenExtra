using Microsoft.Data.SqlClient;
using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImportFromDB
{
    class Program
    {
        static void Main(string[] args)
        {


            StartConsoleApp();


        }
        static void StartConsoleApp()
        {
            int keuze;
            bool isNumber = false;
            do
            {
                do
                {
                    Console.WriteLine("Wat wenst u doen? Druk een van de volgende cijfers");
                    Console.WriteLine("Om te stoppen duw 0\n");
                    Console.WriteLine("   1. Lijst van straatIDs van een gemeente");
                    Console.WriteLine("   2. Straatinfo opvragen van een straatID");
                    Console.WriteLine("   3. Straatinfo opvragen van een gemeente en naam");
                    Console.WriteLine("   4. Lijst van straatnamen van een gemeente");
                    Console.WriteLine("   5. Lijst van alle gemeenten, hun straten en hun lengte in een provincie");
                    Console.WriteLine("   6. Alle straten die een bepaalde straat kruisen");
                    Console.WriteLine();
                    Console.Write("   Uw keuze: ");
                    isNumber = int.TryParse(Console.ReadLine(), out keuze);
                    Console.Clear();
                }
                while (keuze < 0 || keuze >= 7 || !isNumber);

                switch (keuze)
                {
                    case 1:
                        {
                            Console.Write("Geef een gemeente: ");
                            var gemeenteNaam = Console.ReadLine();
                            Console.Clear();
                            Print.PrintLijstStraatIds(gemeenteNaam);
                            break;
                        }
                    case 2:
                        {

                            int straatId;
                            bool isOk;
                            do
                            {
                                Console.Write("Geef een straatId: ");
                                isOk = int.TryParse(Console.ReadLine(), out straatId);
                                Console.Clear();
                            } while (!isOk);
                            Print.PrintStraat(straatId);
                            break;
                        }
                    case 3:
                        {
                            Console.Write("Geef een gemeente: ");
                            var gemeenteNaam = Console.ReadLine();
                            Console.Write("Geef een straatnaam: ");
                            var straatNaam = Console.ReadLine();
                            Console.Clear();
                            Print.PrintStraat(straatNaam, gemeenteNaam);
                            break;
                        }
                    case 4:
                        {
                            Console.Write("Geef een gemeente: ");
                            var gemeenteNaam = Console.ReadLine();
                            Console.Clear();
                            Print.PrintLijstStraatNamen(gemeenteNaam);
                            break;
                        }
                    case 5:
                        {
                            Console.Write("Geef een provincie: ");
                            var provincieNaam = Console.ReadLine();
                            Console.Clear();
                            Print.PrintProvincieInfo(provincieNaam);
                            break;
                        }
                    case 6:
                        {
                            int straatId;
                            bool isOk;
                            do
                            {
                                Console.Write("Geef een straatId: ");
                                isOk = int.TryParse(Console.ReadLine(), out straatId);
                                Console.Clear();
                            } while (!isOk);
                            Print.PrintKruisendeStraten(straatId);
                            break;
                        }


                }
                Console.ReadLine();
                Console.Clear();
            } while (keuze != 0);
        }
        
    }
}
