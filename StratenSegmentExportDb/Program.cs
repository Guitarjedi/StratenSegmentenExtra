using System;
using Microsoft.Data.SqlClient;
using ImportFromFiles;
using System.Collections.Generic;
using System.Threading;
using Contracts;

namespace ExportToDB
{
    class Program
    {
        static void Main(string[] args)
        {
            var export = new Export("nl");
            export.ExportToDB();
        }
        
        

    }
}
