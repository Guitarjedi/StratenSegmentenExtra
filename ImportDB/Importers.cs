using System;
using System.Data.SqlClient;
using Contracts;
using System.Collections.Generic;
using Microsoft.SqlServer;
using System.Linq;

namespace ImportFromDB
{
    public class Importers
    {
        private static string _connString = @"Data Source=LAPTOP-DPRRU9CI\SQLEXPRESS1;Initial Catalog=StratenSegmenten;Integrated Security=True";

        #region QUERIES
        public static Straat GetStraatByStraatNaamEnGemeenteNaam(string straatNaam, string gemeenteNaam)
        {
            string sql = "SELECT s.Id " +
                "FROM Straat s JOIN Gemeente g " +
                "ON(s.GemeenteId = g.Id) " +
                "WHERE s.StraatNaam = @straatNaam AND g.Naam = @gemeenteNaam; ";

            int straatId = -1;
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@straatNaam", straatNaam);
                cmd.Parameters.AddWithValue("@gemeenteNaam", gemeenteNaam);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        straatId = Convert.ToInt32(reader[0]);
                    }
                }
            }
            return GetStraatById(straatId);
        }
        public static Straat GetStraatById(int straatId)
        {
            var straat = new Straat();
            straat.Id = straatId;
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                var sql1 = "SELECT StraatNaam, GraafId, GemeenteId " +
                           "FROM Straat s " +
                           "WHERE s.Id = @StraatId; ";

                SqlCommand cmd1 = new SqlCommand(sql1, conn);
                cmd1.Parameters.AddWithValue("@StraatId", straatId);

                conn.Open();
                using (SqlDataReader reader = cmd1.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        straat.StraatNaam = reader[0].ToString();
                        straat.Graaf.Id = Convert.ToInt32(reader[1]);
                        straat.GemeenteId = Convert.ToInt32(reader[2]);
                    }
                }
                var sql2 = "SELECT s.Id,s.BeginKnoopId, s.EindKnoopId " +
                            "FROM Segment s JOIN GraafId_SegmentId gs " +
                            "ON(s.Id = gs.SegmentId) " +
                            "WHERE gs.GraafId = @GraafId; ";
                SqlCommand cmd2 = new SqlCommand(sql2, conn);
                cmd2.Parameters.AddWithValue("@GraafId", straat.Graaf.Id);

                using (SqlDataReader reader = cmd2.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var segment = new Segment();
                        segment.Id = Convert.ToInt32(reader[0]);
                        segment.BeginKnoop.Id = Convert.ToInt32(reader[1]);
                        segment.EindKnoop.Id = Convert.ToInt32(reader[2]);
                        straat.Graaf.Map.Add(segment);
                    }
                }
                for (int i = 0; i < straat.Graaf.Map.Count; i++)
                {
                    var segment = straat.Graaf.Map[i];
                    segment.BeginKnoop.Punt = GetPuntByKnoopId(segment.BeginKnoop.Id, conn);
                    segment.EindKnoop.Punt = GetPuntByKnoopId(segment.EindKnoop.Id, conn);
                    segment.Vertices = GetAllPointsOfSegment(segment, conn);
                    segment.Vertices = SortVertices(segment);
                }
                straat.Graaf.Map = SortMap(straat.Graaf.Map);
            }
            return straat;

        }
        private static List<Punt> GetAllPointsOfSegment(Segment s, SqlConnection openConnection)
        {
            var sql = "SELECT p.X, p.Y " +
                      "FROM Punt p JOIN SegmentId_PuntId sp " +
                      "ON(p.Id = sp.PuntId) " +
                      "WHERE sp.SegmentId = @segmentId " +
                      "ORDER BY p.Id";
            SqlCommand cmd = new SqlCommand(sql, openConnection);
            cmd.Parameters.AddWithValue("@segmentId", s.Id);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                var punten = new List<Punt>();
                while (reader.Read())
                {
                    var punt = new Punt();
                    punt.X = Convert.ToInt32(reader[0]);
                    punt.Y = Convert.ToInt32(reader[1]);
                    punten.Add(punt);
                }
                return punten;
            }

        }


        public static List<int> GetListStraatIdByGemeenteNaam(string gemeenteNaam)
        {
            List<int> straatIds = new List<int>();
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                var sql = "SELECT s.Id " +
                        "FROM Straat s JOIN Gemeente g " +
                        "ON (s.GemeenteId = g.Id) " +
                        "WHERE s.GemeenteId = " +
                        "(SELECT Id " +
                        "FROM Gemeente " +
                        "WHERE Naam = @GemeenteNaam); ";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@GemeenteNaam", System.Data.SqlDbType.NVarChar);
                cmd.Parameters["@GemeenteNaam"].Value = gemeenteNaam;

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        straatIds.Add(Convert.ToInt32(reader[0]));
                    }
                }
            }
            return straatIds;
        }
        public static List<string> GetListStraatNamenByGemeenteNaam(string gemeenteNaam)
        {
            List<string> straatNamen = new List<string>();
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                var sql = "SELECT s.StraatNaam " +
                        "FROM Straat s JOIN Gemeente g " +
                        "ON (s.GemeenteId = g.Id) " +
                        "WHERE s.GemeenteId = " +
                        "(SELECT Id " +
                        "FROM Gemeente " +
                        "WHERE Naam = @GemeenteNaam); ";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@GemeenteNaam", System.Data.SqlDbType.NVarChar);
                cmd.Parameters["@GemeenteNaam"].Value = gemeenteNaam;

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        straatNamen.Add(reader[0].ToString());
                    }
                }
            }
            straatNamen.Sort();
            return straatNamen;

        }
        public static Tuple<string, string> GetGemeenteNaamEnProvincieNaamByGemeenteId(int gemeenteId)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                var sql = "SELECT g.Naam, p.Naam " +
                    "FROM Gemeente g JOIN Provincie p " +
                    "ON(g.ProvincieId = p.Id) " +
                    "WHERE g.Id = @GemeenteId; ";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@GemeenteId", gemeenteId);
                conn.Open();

                string gemeenteNaam = "";
                string provincieNaam = "";
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        gemeenteNaam = reader[0].ToString();
                        provincieNaam = reader[1].ToString();
                    }
                }
                return new Tuple<string, string>(gemeenteNaam, provincieNaam);
            }

        }
        public static List<Gemeente> GetGemeentesIdNaamByProvincieNaam(string provincieNaam)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                List<Gemeente> gemeenten = new List<Gemeente>();
                var sql = "SELECT Id, Naam " +
                    "FROM Gemeente " +
                    "WHERE ProvincieId = (SELECT Id FROM Provincie WHERE Naam = @ProvincieNaam) " +
                    "ORDER BY Naam; ";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ProvincieNaam", provincieNaam);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var gemeente = new Gemeente();
                        gemeente.Id = Convert.ToInt32(reader[0]);
                        gemeente.Naam = reader[1].ToString();
                        gemeenten.Add(gemeente);
                    }
                }
                return gemeenten;
            }

        }

        public static List<Straat> GetAdjacentStraten(Straat s)
        {
            var ids = GetAdjacentStraatIdsInGemeente(s);
            var straten = new List<Straat>();
            foreach(var id in ids)
            {

                straten.Add(GetStraatById(id));
            }
            return straten;
        }
        private static HashSet<int> GetAdjacentStraatIdsInGemeente(Straat s)
        {
            HashSet<int> adjStratenIds = new HashSet<int>();
            List<int> knopen = GetAllKnoopIdsFromStraatId(s.Id);
            string gemeenteNaam = GetGemeenteNaamEnProvincieNaamByGemeenteId(s.GemeenteId).Item1;
            List<int> straatIds = GetListStraatIdByGemeenteNaam(gemeenteNaam);
            foreach (var straatId in straatIds)
            {
                if (straatId != s.Id && !adjStratenIds.Contains(straatId))
                {
                    foreach (var knoopid in GetAllKnoopIdsFromStraatId(straatId))
                    {
                        if (knopen.Contains(knoopid))
                        {
                            adjStratenIds.Add(straatId);
                        }
                    }
                }
            }
            return adjStratenIds;
        }
        public static List<int> GetAllKnoopIdsFromStraatId(int straatId)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                List<int> knopen = new List<int>();
                var sql = "(SELECT s.BeginKnoopId " +
                    "FROM Segment s JOIN GraafId_SegmentId gs " +
                    "ON(s.Id = gs.SegmentId) " +
                    "WHERE gs.GraafId = (SELECT GraafId " +
                    "FROM Straat " +
                    "WHERE Id = @straatId)) " +
                    "UNION " +
                    "(SELECT s.EindKnoopId " +
                    "FROM Segment s JOIN GraafId_SegmentId gs " +
                    "ON(s.Id = gs.SegmentId) " +
                    "WHERE gs.GraafId = (SELECT GraafId " +
                    "FROM Straat " +
                    "WHERE Id = @straatId));";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@straatId", straatId);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        knopen.Add(Convert.ToInt32(reader[0]));
                    }
                }
                return knopen;
            }

        }
        private static Punt GetPuntByKnoopId(int knoopid, SqlConnection openConnection)
        {
            var sql3 = "SELECT p.X,p.Y " +
                        "FROM punt p JOIN Knoop k " +
                        "ON(k.PuntId = p.Id) " +
                        "WHERE k.Id = @KnoopId;";
            SqlCommand cmd3 = new SqlCommand(sql3, openConnection);
            cmd3.Parameters.AddWithValue("@KnoopId", knoopid);

            using (SqlDataReader reader = cmd3.ExecuteReader())
            {
                var punt = new Punt();
                if (reader.Read())
                {
                    punt.X = Convert.ToInt32(reader[0]);
                    punt.Y = Convert.ToInt32(reader[1]);
                }
                return punt;
            }

        }
        
        #endregion
        #region TOOLS
        private static List<Punt> SortVertices(Segment s)
        {
            if (s.Vertices.Count != 0)
            {
                LinkedList<Punt> orderedList = new LinkedList<Punt>(s.Vertices);
                orderedList.Remove(s.BeginKnoop.Punt);
                orderedList.AddFirst(s.BeginKnoop.Punt);
                orderedList.Remove(s.EindKnoop.Punt);
                orderedList.AddLast(s.EindKnoop.Punt);
                return orderedList.ToList<Punt>();
            }
            else
                return s.Vertices;

        }
        private static List<Segment> SortMap(List<Segment> segmenten)
        {
            if (segmenten.Count != 0)
            {
                var linkedList = new LinkedList<Segment>();
                var dummyList = new List<Segment>(segmenten);
                int i = -1;
                linkedList.AddLast(dummyList[0]);
                dummyList.RemoveAt(0);
                int attempts = 0;
                while (dummyList.Count > 0)
                {
                    i++;
                    if (linkedList.First.Value.BeginKnoop.Id == dummyList[i].BeginKnoop.Id)
                    { linkedList.AddFirst(ReverseSegment(dummyList[i])); dummyList.RemoveAt(i);attempts = 0; i = -1; continue; }

                    if (linkedList.First.Value.BeginKnoop.Id == dummyList[i].EindKnoop.Id)
                    { linkedList.AddFirst(dummyList[i]); dummyList.RemoveAt(0); attempts = 0; i = -1; continue; }

                    if (linkedList.Last.Value.EindKnoop.Id == dummyList[i].BeginKnoop.Id)
                    { linkedList.AddLast(dummyList[i]); dummyList.RemoveAt(i); attempts = 0; i = -1; continue; }

                    if (linkedList.Last.Value.EindKnoop.Id == dummyList[i].EindKnoop.Id)
                    { linkedList.AddLast(ReverseSegment(dummyList[i])); dummyList.RemoveAt(i); attempts = 0;i = -1; continue; }

                    if( i == dummyList.Count - 1 && attempts == 0)
                    {
                        linkedList.AddLast(dummyList[i]);
                        dummyList.RemoveAt(i);
                        attempts++;
                        i = -1;
                        continue;
                    }
                    if (i == dummyList.Count - 1 && attempts == 1)
                    {
                        var segment = linkedList.ElementAt(linkedList.Count - 1);
                        linkedList.RemoveLast();
                        linkedList.AddLast(ReverseSegment(segment));
                        attempts++;
                        i = -1;
                        continue;
                    }
                    if(i == dummyList.Count - 1 && attempts >= 2)
                    {
                        attempts = 0;
                        i = -1;
                        continue;
                    }
                    
                    
                }
                return linkedList.ToList<Segment>();
            }
            else
                return segmenten;
        }
        private static Segment ReverseSegment (Segment s)
        {
            var segment = new Segment();
            segment.BeginKnoop = s.EindKnoop;
            segment.EindKnoop = s.BeginKnoop;
            segment.Id = s.Id;

            for(int i = s.Vertices.Count - 1; i >= 0; i --)
            {
                segment.Vertices.Add(s.Vertices[i]);
            }
            return segment;
        }
        

    }
    #endregion
}

