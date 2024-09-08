using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace TMS.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(Userdata lg)
        {
            try
            {
                string username = lg.LoginName;
                string password = lg.Password;
                using (SqlConnection conn = new SqlConnection("Server=sql.bsite.net\\MSSQL2016;Database=bharatbuys_db;User Id=bharatbuys_db;Password=Ganesh@123.;TrustServerCertificate=True;"))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("select * from chatLogin where username=@username and password=@password", conn))
                    {
                        cmd.Parameters.AddWithValue("username", username);
                        cmd.Parameters.AddWithValue("password", password);
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (await reader.ReadAsync())
                        {
                            HttpContext.Session.SetString("Username", username);
                            reader.Close();
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ViewBag.Message = "incorrect Id or password";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.message = ex.Message;
            }
            return View();

        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Userdata ud)
        {
            try
            {
                string username = ud.LoginName;
                string password = ud.Password;
                if (userexist(username) == true)
                {
                    ViewBag.Message = "User already exist";
                    return View();
                }
                else if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    ViewBag.Message = "FILL ALL DATA";
                    return View();
                }
                else
                {
                    using (SqlConnection conn = new SqlConnection("Server=sql.bsite.net\\MSSQL2016;Database=bharatbuys_db;User Id=bharatbuys_db;Password=Ganesh@123.;TrustServerCertificate=True;"))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("insert into chatLogin values (@username,@password)", conn))
                        {
                            cmd.Parameters.AddWithValue("username", username);
                            cmd.Parameters.AddWithValue("password", password);
                            cmd.ExecuteNonQuery();
                            int rowsAffected = await cmd.ExecuteNonQueryAsync();
                            if (rowsAffected > 0)
                            {
                                HttpContext.Session.SetString("Username", username);
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                ViewBag.Message = "Registration failed";
                                return View();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message + "/n Error occured";
            }
            return View();
        }
        public bool userexist(string username)
        {
            using (SqlConnection conn = new SqlConnection("Server=sql.bsite.net\\MSSQL2016;Database=bharatbuys_db;User Id=bharatbuys_db;Password=Ganesh@123.;TrustServerCertificate=True;"))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("select * from chatLogin where username=@username", conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.ExecuteNonQuery();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (!reader.Read())
                    {
                        return false;
                    }

                }
            }
            return true;
        }
        /**/
        [HttpPost]
        
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Username");
            return RedirectToAction("Index", "Home");
        }

    }
    public class Userdata
    {
        public string LoginName { get; set; }
        public string Password { get; set; }
    }

}
