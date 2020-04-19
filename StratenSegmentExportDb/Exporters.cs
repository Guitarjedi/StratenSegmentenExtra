using System;
using Microsoft.Data.SqlClient;
using Contracts;
using System.Data;
using System.Collections.Generic;

namespace ExportToDB
{
    public static class Exporters
    {
        static private string _connString = @"Data Source=LAPTOP-DPRRU9CI\SQLEXPRESS1;Initial Catalog=StratenSegmenten;Integrated Security=True";
        //static private string _connString = @"Data Source=LAPTOP-DPRRU9CI\SQLEXPRESS1;Initial Catalog=TestStratenSegmenten;Integrated Security=True";
        #region Privates
        private static int ExportPunt(Punt p)
        {
            string sql = "INSERT INTO Punt(X,Y) " +
                         "VALUES (@X, @Y);" +
                         "SELECT SCOPE_IDENTITY()";
            int id = -1;
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@X", SqlDbType.Float);
                cmd.Parameters.Add("@Y", SqlDbType.Float);
                
                cmd.Parameters["@X"].Value = p.X;
                cmd.Parameters["@Y"].Value = p.Y;
                
               id = Convert.ToInt32(cmd.ExecuteScalar());
            }
            return id;
        }
        private static int ExportKnoop(int knoopId, int puntId)
        {
            string sql = "INSERT INTO Knoop(Id,PuntId) " +
                         "VALUES (@X, @Y);";

            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@X", SqlDbType.Int);
                cmd.Parameters.Add("@Y", SqlDbType.Int);

                cmd.Parameters["@X"].Value = knoopId;
                cmd.Parameters["@Y"].Value = puntId;
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                return knoopId;
            }

        }
        
        private static Dictionary<int, int> _wegGeschrevenKnopenPuntId = new Dictionary<int, int>();

        private static int ExportSegment(Segment s)
        {

            int segmentId = s.Id;
            if (!_wegGeschrevenKnopenPuntId.ContainsKey(s.BeginKnoop.Id))
            {
                int beginKnoopPuntId = ExportPunt(s.BeginKnoop.Punt);
                ExportKnoop(s.BeginKnoop.Id, beginKnoopPuntId);
                ExportTussenTabel(segmentId, beginKnoopPuntId, "SegmentId_PuntId");
                _wegGeschrevenKnopenPuntId.Add(s.BeginKnoop.Id, beginKnoopPuntId);
            }
            else
            {
                ExportTussenTabel(segmentId, _wegGeschrevenKnopenPuntId[s.BeginKnoop.Id], "SegmentId_PuntId");
            }
            if (!_wegGeschrevenKnopenPuntId.ContainsKey(s.EindKnoop.Id))
            {
                int eindKnoopPuntId = ExportPunt(s.EindKnoop.Punt);
                ExportKnoop(s.EindKnoop.Id, eindKnoopPuntId);
                ExportTussenTabel(segmentId, eindKnoopPuntId, "SegmentId_PuntId");
                _wegGeschrevenKnopenPuntId.Add(s.EindKnoop.Id, eindKnoopPuntId);
            }
            else
            {
                ExportTussenTabel(segmentId, _wegGeschrevenKnopenPuntId[s.EindKnoop.Id], "SegmentId_PuntId");
            }
            using (SqlConnection conn = new SqlConnection(_connString))
            {

                string sqlSeg = "INSERT INTO Segment(Id,BeginKnoopId,EindKnoopId)" +
                            "VALUES(@Id,@bkId,@ekId);";
                SqlCommand cmd = new SqlCommand(sqlSeg, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int);
                cmd.Parameters.Add("@bkId", SqlDbType.Int);
                cmd.Parameters.Add("@ekId", SqlDbType.Int);

                cmd.Parameters["@Id"].Value = segmentId;
                cmd.Parameters["@bkId"].Value = s.BeginKnoop.Id;
                cmd.Parameters["@ekId"].Value = s.EindKnoop.Id;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            for (int i = 1; i < s.Vertices.Count - 1; i++)
            {
                int puntId = ExportPunt(s.Vertices[i]);
                ExportTussenTabel(segmentId, puntId, "SegmentId_PuntId");
            }

            return segmentId;
        }
       
        private static HashSet<int> _weggeschrevenSegmenten = new HashSet<int>();
        private static void ExportGraaf(Graaf g)
        {
            int graafId = g.Id;
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                string sql = "INSERT INTO Graaf(Id)" +
                             "VALUES (@Id);";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int);
                cmd.Parameters["@Id"].Value = graafId;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            if (g.Map != null)
            {
                foreach (var segment in g.Map)
                {
                    if (!_weggeschrevenSegmenten.Contains(segment.Id))
                    {
                        ExportSegment(segment);
                        _weggeschrevenSegmenten.Add(segment.Id);
                    }
                    ExportTussenTabel(graafId, segment.Id, "GraafId_SegmentId");

                }

            }

        }
        private static void ExportTussenTabel(int id1, int id2, string tabel)
        {
            string sqlSegPunt = $"INSERT INTO {tabel} " +
                                   "VALUES(@id1,@id2)";
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                SqlCommand cmd = new SqlCommand(sqlSegPunt, conn);
                cmd.Parameters.Add("@id1", SqlDbType.Int);
                cmd.Parameters.Add("@id2", SqlDbType.Int);
                cmd.Parameters["@id1"].Value = id1;
                cmd.Parameters["@id2"].Value = id2;

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
        }
        #endregion

        #region Publics
        public static void ExportStraat(Straat s)
        {

            ExportGraaf(s.Graaf);
            string sql = "INSERT INTO Straat " +
                        "VALUES(@Id, @StraatNaam, @GraafId, @GemeenteId)";
            

            using (SqlConnection conn = new SqlConnection(_connString))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int);
                cmd.Parameters.Add("@StraatNaam", SqlDbType.NVarChar);
                cmd.Parameters.Add("@GraafId", SqlDbType.Int);
                cmd.Parameters.Add("@GemeenteId", SqlDbType.Int);

                cmd.Parameters["@Id"].Value = s.Id;
                cmd.Parameters["@StraatNaam"].Value = s.StraatNaam;
                cmd.Parameters["@GraafId"].Value = s.Graaf.Id;
                cmd.Parameters["@GemeenteId"].Value = s.GemeenteId;

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e.Message} ID:{s.Id} StraatNaam {s.StraatNaam}");
                }
            }
        }
        public static void ExportGemeente(Gemeente g)
        {
            string sql = "INSERT INTO Gemeente " +
                        "VALUES(@Id, @NaamId, @TaalCode, @Naam, @ProvincieId);";
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int);
                cmd.Parameters.Add("@NaamId", SqlDbType.Int);
                cmd.Parameters.Add("@TaalCode", SqlDbType.NVarChar);
                cmd.Parameters.Add("@Naam", SqlDbType.NVarChar);
                cmd.Parameters.Add("@ProvincieId", SqlDbType.Int);

                cmd.Parameters["@Id"].Value = g.Id;
                cmd.Parameters["@NaamId"].Value = g.NaamId;
                cmd.Parameters["@TaalCode"].Value = g.TaalCode;
                cmd.Parameters["@Naam"].Value = g.Naam;
                cmd.Parameters["@ProvincieId"].Value = g.ProvincieId;

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
        public static void ExportProvincie(Provincie p)
        {
            string sql = "INSERT INTO Provincie " +
                        "VALUES(@Id, @TaalCode, @Naam, @RegioId);";
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int);
                cmd.Parameters.Add("@TaalCode", SqlDbType.NVarChar);
                cmd.Parameters.Add("@Naam", SqlDbType.NVarChar);
                cmd.Parameters.Add("@RegioId", SqlDbType.Int);

                cmd.Parameters["@Id"].Value = p.Id;
                cmd.Parameters["@TaalCode"].Value = p.TaalCode;
                cmd.Parameters["@Naam"].Value = p.Naam;
                cmd.Parameters["@RegioId"].Value = p.RegioId;

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
        public static void ExportRegio(Regio r)
        {
            string sql = "INSERT INTO Regio " +
                        "VALUES(@Id, @Naam, @LandId);";
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int);
                cmd.Parameters.Add("@Naam", SqlDbType.NVarChar);
                cmd.Parameters.Add("@LandId", SqlDbType.Int);

                cmd.Parameters["@Id"].Value = r.Id;
                cmd.Parameters["@Naam"].Value = r.Naam;
                cmd.Parameters["@LandId"].Value = r.LandId;

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
        public static void ExportLand(Land l)
        {
            string sql = "INSERT INTO Land " +
                        "VALUES(@Id, @TaalCode, @Naam);";
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int);
                cmd.Parameters.Add("@Naam", SqlDbType.NVarChar);
                cmd.Parameters.Add("@TaalCode", SqlDbType.NVarChar);

                cmd.Parameters["@Id"].Value = l.Id;
                cmd.Parameters["@TaalCode"].Value = l.TaalCode;
                cmd.Parameters["@Naam"].Value = l.Naam;

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
        #endregion
        
    }
}
