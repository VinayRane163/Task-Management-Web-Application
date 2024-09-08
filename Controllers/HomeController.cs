using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Diagnostics;

namespace TMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public string connectionstring = "Server=sql.bsite.net\\MSSQL2016;Database=bharatbuys_db;User Id=bharatbuys_db;Password=Ganesh@123.;TrustServerCertificate=True;";
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        /**/
        [HttpGet]
        public IActionResult Index()
        {
            List<taskdata> tasks = new List<taskdata>();

            if (HttpContext.Session.GetString("Username") == null)  
            {
                return RedirectToAction("Login","Account");
            }
            else
            {
                string user = HttpContext.Session.GetString("Username");
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionstring))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("select * from Tasks where username=@username", conn))
                        {
                            cmd.Parameters.AddWithValue("@username", user);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while(reader.Read())
                                {
                                    tasks.Add(new taskdata
                                    { 
                                        taskid = reader["task_no"].ToString(), 
                                        taskperson = reader["username"].ToString(),
                                        taskname = reader["task_name"].ToString(),
                                        taskdate = reader["task_date"].ToString(),
                                        taskdetails = reader["task_details"].ToString()
                                    });
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                }

            }
       
            return View(tasks);
        }
        /**/
        [HttpGet]
        public IActionResult Edit(string task,string User,edit_task ed)
        {
           
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionstring))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("select *  from Tasks where username=@username and task_no=@task_no", conn))
                    {
                        cmd.Parameters.AddWithValue("@username", User);
                        cmd.Parameters.AddWithValue("@task_no", task);
                        cmd.ExecuteNonQuery();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ed.taskid = reader["task_no"].ToString();
                                ed.taskperson = User;
                                ed.taskname = reader["task_name"].ToString();
                                ed.taskdate = reader["task_date"].ToString();
                                ed.taskdetails = reader["task_details"].ToString();                                
                            }
                        }

                    }
                }
            }
            catch
            {
                ViewBag.Message = "error";
            }
            return View(ed);
        }

        [HttpPost]
        public IActionResult Edit(edit_task ed)
        {
            DateTime currentDate = DateTime.Now.Date;
            DateTime selecteddate = Convert.ToDateTime(ed.taskdate);

            if (string.IsNullOrEmpty(ed.taskid) || string.IsNullOrEmpty(ed.taskperson) || string.IsNullOrEmpty(ed.taskname) || string.IsNullOrEmpty(ed.taskdate) || string.IsNullOrEmpty(ed.taskdetails))
            {
                ViewBag.Message = "fill all data";
                return View(ed);
            }
            else if (selecteddate < currentDate)
            {
                ViewBag.Message = "date in incorrect format";
                return View(ed);
            }
            else
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionstring))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("UPDATE Tasks SET task_name = @task_name, task_date = @task_date, task_details = @task_details " +
                            "WHERE username = @username AND task_no = @task_no", conn))
                        {
                            cmd.Parameters.AddWithValue("@username", ed.taskperson);
                            cmd.Parameters.AddWithValue("@task_no", ed.taskid);
                            cmd.Parameters.AddWithValue("@task_name", ed.taskname);
                            cmd.Parameters.AddWithValue("@task_date", ed.taskdate);
                            cmd.Parameters.AddWithValue("@task_details", ed.taskdetails);

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch
                {
                    ViewBag.Message = "Error updating task.";
                    return View(ed); // Return the view with the model to show error messages
                }
            }

            return RedirectToAction("Index");
        }


        /**/
        [HttpPost]
        public IActionResult delete(string task, string User)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionstring))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("delete  from Tasks where username=@username and task_no=@task_no", conn))
                    {
                        cmd.Parameters.AddWithValue("@username", User);
                        cmd.Parameters.AddWithValue("@task_no", task);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                ViewBag.Message = "error";
            }
            return RedirectToAction("Index","Home");
        }

        [HttpPost]
        public IActionResult done(string task, string User)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionstring))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("delete  from Tasks where username=@username and task_no=@task_no", conn))
                    {
                        cmd.Parameters.AddWithValue("@username", User);
                        cmd.Parameters.AddWithValue("@task_no", task);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                ViewBag.Message = "error";
            }
            return RedirectToAction("Index", "Home");
        }
        /**/

        [HttpGet]
        public IActionResult TaskAdd()
        {
            return View();
        }
        [HttpPost]
        public IActionResult TaskAdd(addtaskdata atd)
        {
            string tname = atd.taskname;
            string tdate = atd.taskdate;
            string tdetails = atd.taskdetails;

            DateTime currentDate = DateTime.Now.Date;
            DateTime selecteddate = Convert.ToDateTime(tdate);
            

            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else if(string.IsNullOrWhiteSpace(tname)||string.IsNullOrWhiteSpace(tdate)||string.IsNullOrWhiteSpace(tdetails))
            {
                ViewBag.Message = "fill all data ";
                return View();
            }
            else if(selecteddate < currentDate)
            {
                ViewBag.Message = "date in incorrect format";
                return View();
            }
            else 
            {
                string uname = HttpContext.Session.GetString("Username");
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionstring))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("insert into Tasks  (username,task_name,task_date,task_details) values (@username,@taskname,@taskdate,@taskdetails)", conn))
                        {
                            cmd.Parameters.AddWithValue("@username",uname);
                            cmd.Parameters.AddWithValue("@taskname", tname);
                            cmd.Parameters.AddWithValue("@taskdate", tdate);
                            cmd.Parameters.AddWithValue("@taskdetails", tdetails);
                            cmd.ExecuteNonQuery();

                           
                                return RedirectToAction("Index","Home");
                            
                        }
                    }
                }
                catch(Exception ex) {
                    ViewBag.Message = ex.Message;
                }

            }
            return View();
        }
        /**/
        public IActionResult Privacy()
        {
            return View();
        }

            
    }
    public class addtaskdata
    {
        public string taskname { get; set; }

        public string taskdate { get; set; }

        public string taskdetails { get; set; }
    }

    public class taskdata
    {
        public string taskid { get; set; }
        public string taskperson { get; set; }
        public string taskname { get; set; }
        public string taskdate { get; set; }
        public string taskdetails { get; set; }
    }

    public class edit_task
    {
        public string taskid { get; set; }
        public string taskperson { get; set; }
        public string taskname { get; set; }
        public string taskdate { get; set; }
        public string taskdetails { get; set; }
    }
}
