using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using Microsoft.Azure.Services.AppAuthentication;
using System.Web.Configuration;

namespace SQLApplication.Controllers
{
    public struct Record
    {
        public string Name;
        public int ID;
    }

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Connect(string connectionInput, bool? useIdentity)
        {
            // If user selected the identity checkbox, use managed identity
            bool identity = useIdentity != null ? useIdentity.Value : false;

            using (SqlConnection con = new SqlConnection(connectionInput))
            {
                // If using managed identity, get token from Azure to connect to database
                if (identity)
                {
                    con.AccessToken = (new AzureServiceTokenProvider()).GetAccessTokenAsync("https://database.windows.net/").Result;
                }

                try
                {
                    con.Open();
                }
                catch (SqlException e)
                {
                    // Create list of all errors and send to ViewBag to display to user
                    List<string> messages = new List<string>();
                    for (int i = 0; i < e.Errors.Count; i++)
                    {
                        messages.Add(e.Errors[i].Message);
                    }
                    ViewBag.Error = messages;
                    return View();
                }


                // Get all values from Test table in database
                using (SqlCommand command = new SqlCommand("SELECT * FROM Test", con))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    // Create a new Record and store it into the messages list
                    List<Record> records = new List<Record>();
                    while (reader.Read())
                    {
                        Record r;
                        r.ID = reader.GetInt32(0);
                        r.Name = reader.GetString(1);
                        records.Add(r);
                    }

                    // Set ViewBag.Results to be the list of records we got from the database
                    // Will loop through them in the View page (see Connect.cshtml)
                    ViewBag.Results = records;
                }
            }

            return View();
        }
    }
}