using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gRPC_Server;
using System.IO;
using Newtonsoft.Json;
using System.Data.Entity;
using Grpc.Core;
using ErikEJ.SqlCe;
namespace gRPC_Server
{
    class Program
    {
        
        static void Main(string[] args)
        {
            Server server = new Server
            {
                Services = { Students.BindService(new Students_Profiles()) },
                Ports = { new ServerPort("localhost", Convert.ToInt32(Properties.Settings.Default.Port), ServerCredentials.Insecure) }

            };
            server.Start();

            Console.WriteLine("gRPC server listening on port " + Properties.Settings.Default.Port);
            Console.WriteLine("Press any key to stop the server...");
            string path = AppDomain.CurrentDomain.BaseDirectory;
            if (!File.Exists(path + @"\\gRPC_Server.DbprofilDbcontext.sdf"))
            {
                Console.WriteLine(path);
                using (DbprofilDbcontext db = new DbprofilDbcontext())
                {
                    db.profiles.ToList();
                }
                Task PerformInsertAsync = PerformAsync();
            }
            
            

         
           
            
            Console.ReadKey();
            server.ShutdownAsync().Wait();
        }
        static async Task PerformAsync()
        {
            List<DbProfile> parsedData;
            await Task.Run(() =>
            {
                parsedData = ParseJsonResponce(Properties.Settings.Default.Sample);
                
                Database.SetInitializer(new DropCreateDatabaseAlways<DbprofilDbcontext>());

                using (var context = new DbprofilDbcontext())
                {
                    context.Database.Initialize(true);

                    //radProgressBar1.Text = "Working";
                    using (SqlCeBulkCopy bcp = new SqlCeBulkCopy(context.Database.Connection.ConnectionString))
                    {
                        bcp.DestinationTableName = "DbProfiles";
                        // sw.Restart();
                        bcp.WriteToServer(parsedData);
                        // parsedData = null;
                        //sw.Stop();

                        //Console.WriteLine(
                        //    "Saved {0} entities using SqlCeBulkCopy in {1}", students.Count, sw.Elapsed.ToString());
                    }
                }
            });

            }
        private static List<DbProfile> ParseJsonResponce(string json)
        {

            List<DbProfile> pr = new List<DbProfile>();
            pr = JsonConvert.DeserializeObject<List<DbProfile>>(json);
           
            return pr;
        }
    }
}
