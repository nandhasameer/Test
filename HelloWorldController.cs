using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebApplication14.Models;

namespace WebApplication14.Controllers
{
    public class HelloWorldController : Controller
    {
        private readonly AppSettings _appSettings;


        public HelloWorldController(IOptions<AppSettings> options)
        {
           
            _appSettings = options.Value;
          
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Welcome()
        {
            DataTable tempTable = new DataTable();
            List<RegisterationInfo> registerationInfo = new List<RegisterationInfo>();
            string CS = _appSettings.ClinicManagementDb;
           
                using (SqlConnection sqlconn = new SqlConnection(CS))
                {
                    try
                    {
                        using (SqlCommand sqlCmd = new SqlCommand("Select * from [WebsiteDb].[dbo].[User]", sqlconn) { CommandType = CommandType.Text })
                        {
                            

                            if (sqlconn.State == ConnectionState.Closed)
                            {
                                sqlconn.Open();
                            }
                             
                            using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                            {
                                using (DataTable dtCashbackDetails = new DataTable())
                                {
                                    da.Fill(dtCashbackDetails);
                                    tempTable = dtCashbackDetails;
                                }
                            }
                           
                        }
                    }
                    finally
                    {

                        if (sqlconn.State == ConnectionState.Open)
                        {
                            sqlconn.Close();
                        }
                    }
                   
                    for (int i = 0; i < tempTable.Rows.Count; i++)
                    {
                       RegisterationInfo student = new RegisterationInfo();
                        student.EmailID = tempTable.Rows[i]["EmialID"].ToString();
                        student.FirstName = tempTable.Rows[i]["FirstName"].ToString();
                        student.LastName = tempTable.Rows[i]["LastName"].ToString();
                        student.Password = tempTable.Rows[i]["Password"].ToString();
                       registerationInfo.Add(student);
                    }

                return View(registerationInfo);
            }
            

            
        }

        [HttpPost]
        public IActionResult Welcome(string FirstName ,string LastName ,string EmailID ,string Password)
        {
            DataTable tempTable = new DataTable();
            List<RegisterationInfo> registerationInfo = new List<RegisterationInfo>();
            string CS = _appSettings.ClinicManagementDb;

            using (SqlConnection sqlconn = new SqlConnection(CS))
            {
                try
                {
                   
                   
                    using (SqlCommand sqlCmd = new SqlCommand("SELECT * FROM [WebsiteDb].[dbo].[User] WHERE FirstName = @FirstName", sqlconn) { CommandType = CommandType.Text })
                    {
                        sqlCmd.Parameters.AddWithValue("@FirstName", FirstName);

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dtCashbackDetails = new DataTable())
                            {
                                da.Fill(dtCashbackDetails);
                                tempTable = dtCashbackDetails;
                            }
                        }

                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }

                for (int i = 0; i < tempTable.Rows.Count; i++)
                {
                    RegisterationInfo student = new RegisterationInfo();
                    student.EmailID = tempTable.Rows[i]["EmialID"].ToString();
                    student.FirstName = tempTable.Rows[i]["FirstName"].ToString();
                    student.LastName = tempTable.Rows[i]["LastName"].ToString();
                    student.Password = tempTable.Rows[i]["Password"].ToString();
                    registerationInfo.Add(student);
                }

                return View(registerationInfo);
            }



        }
    }
}
