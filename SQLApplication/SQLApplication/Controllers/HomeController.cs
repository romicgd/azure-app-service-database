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
            bool identity = useIdentity != null ? useIdentity.Value : false;
            if(identity)
            {
                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = connectionInput;
                    con.AccessToken = (new AzureServiceTokenProvider()).GetAccessTokenAsync("https://database.windows.net").Result;
                    try
                    {
                        con.Open();
                    }
                    catch (SqlException e)
                    {
                        List<string> messages = new List<string>();
                        for (int i = 0; i < e.Errors.Count; i++)
                        {
                            messages.Add(e.Errors[i].Message);
                        }
                        ViewBag.Error = messages;
                        return View();
                    }


                    using (SqlCommand command = new SqlCommand("SELECT * FROM Test", con))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<Record> messages = new List<Record>();
                        while (reader.Read())
                        {
                            Record r;
                            r.ID = reader.GetInt32(0);
                            r.Name = reader.GetString(1);
                            messages.Add(r);
                        }
                        ViewBag.Results = messages;
                    }
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(connectionInput))
                {
                    try
                    {
                        con.Open();
                    }
                    catch (SqlException e)
                    {
                        List<string> messages = new List<string>();
                        for (int i = 0; i < e.Errors.Count; i++)
                        {
                            messages.Add(e.Errors[i].Message);
                        }
                        ViewBag.Error = messages;
                        return View();
                    }


                    using (SqlCommand command = new SqlCommand("SELECT * FROM Test", con))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<string> messages = new List<string>();
                        while (reader.Read())
                        {
                            messages.Add(reader.GetString(1));
                        }
                        ViewBag.Results = messages;
                    }
                }
            }
           

            return View();
        }
    }
}