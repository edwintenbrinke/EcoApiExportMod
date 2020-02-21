using LiteDB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Eco.Plugins.EcoApiExportMod
{
    public class data_models
    {
        public int _id { get; set; }
        public int TimeSeconds { get; set; }
        public string ActorId { get; set; }
        public string Username { get; set; }
        public Guid AuthId { get; set; }
        public Guid WorldObjectId { get; set; }
        public double Value { get; set; }
        public string ItemTypeName { get; set; }
        public string SpeciesName { get; set; }
        public string WorldObjectTypeName { get; set; }
    }

    public class DatabaseCollector
    {

        public const string config_file_name = "config.json";
        public const string previous_run_file_name = "previous_run.json";
        public string base_mod_path;

        // get the previous runs data so we don't have to get the records we've handled before
        static List<previous_run_data> getPreviousRunData(string base_path, config config_data)
        {
            string previous_run_file_location = string.Format("{0}{1}", base_path, previous_run_file_name);
            // create previous_run json file if it doesn't exist already
            if (!File.Exists(previous_run_file_location))
            {
                using (StreamWriter w = File.AppendText(previous_run_file_location))
                {
                    w.WriteLine(JsonConvert.SerializeObject(config_data.previous_run_data));
                }
            }

            // read previous run file for the ids
            string json = File.ReadAllText(previous_run_file_location);
            return JsonConvert.DeserializeObject<List<previous_run_data>>(json);
        }

        public void collect(config config_data)
        {
            string base_path = AppDomain.CurrentDomain.BaseDirectory;
            string base_mod_path = string.Format("{0}Mods/EcoApiExportMod/", AppDomain.CurrentDomain.BaseDirectory);

            // get the previous run data
            List<previous_run_data> previous_run_data = getPreviousRunData(base_mod_path, config_data);

            DirectoryInfo backup_directory = new DirectoryInfo(base_path);
            var storage_directories = Directory.GetDirectories(string.Format("{0}/Storage", base_path));
            foreach (string storage in storage_directories)
            {
                if (!storage.Contains("Backup"))
                {
                    continue;
                }

                backup_directory = new DirectoryInfo(string.Format("{0}/Storage/Backup", base_path)).GetDirectories()
                    .OrderByDescending(f => f.LastWriteTime)
                    .First();
            }

            // connect to database
            var db = new LiteDatabase(backup_directory.FullName + "/Game.db");

            // initiate the response array
            Dictionary<string, List<data_models>> api_data = new Dictionary<string, List<data_models>>();
            foreach (var previous_run in previous_run_data)
            {
                // initiate the current database response
                List<data_models> result = new List<data_models>();

                // connect to the collection
                var collection = db.GetCollection<data_models>(previous_run.name);

                // set index on _id
                collection.EnsureIndex(x => x._id);

                // if its lower than the previous_run.id there has been a reset
                // use this id as the new id
                var latest = collection.FindOne(Query.All(Query.Descending));
                if (latest != null && latest._id < previous_run.id)
                {
                    Logger.Debug("an older database has been placed");
                    previous_run.id = latest._id;
                }

                // execute query to get all rows since id
                var query = collection.Find(x => x._id > previous_run.id, limit: config_data.db_query_limit);
                foreach (var entry in query)
                {
                    // add response to the database response array
                    result.Add(entry);
                    previous_run.id = entry._id;
                }


                // add total result to the main data array that will be sent to the api
                api_data.Add(previous_run.name, result);
            }
            // close database connection
            db = null;

            // post the data to the api
            Api.Post("/api/eco/data", config_data, api_data);

            // write the last id gotten from the query to the file
            // so the next time the script runs it doesn't have to get all the current & previous records
            File.WriteAllText(
                string.Format("{0}{1}", base_mod_path, previous_run_file_name),
                JsonConvert.SerializeObject(previous_run_data)
            );
        }
    }
}
