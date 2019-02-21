using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;

namespace SQLApplication.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Connect(string connectionInput)
        {
            var connectionString = connectionInput;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                }
                catch(SqlException e)
                {
                    List<string> messages = new List<string>();
                    for(int i = 0; i < e.Errors.Count; i++)
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

            return View();
        }
    }
}