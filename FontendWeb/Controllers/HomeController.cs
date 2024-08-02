using ApartmentManagement.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace FontendWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> Authenticate(Manager manager)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ApiClient");
                var json = JsonSerializer.Serialize(manager);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/Managers/authenticate", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ViewData["Message"] = $"Đăng nhập thất bại! {errorMessage}";
                    return View("DangNhap");
                }
            }
            catch (HttpRequestException)
            {
                ViewData["Message"] = "Đăng nhập thất bại! Lỗi yêu cầu HTTP.";
                return View("DangNhap");
            }
            catch (IOException)
            {
                ViewData["Message"] = "Đăng nhập thất bại! Lỗi IO.";
                return View("DangNhap");
            }
            catch (Exception)
            {
                ViewData["Message"] = "Đăng nhập thất bại! Lỗi không xác định.";
                return View("DangNhap");
            }
        }
        public async Task<IActionResult> Register(Manager manager)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ApiClient");
                var json = JsonSerializer.Serialize(manager);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/Managers/register", content);

                if (response.IsSuccessStatusCode)
                {
                    // Store email in TempData to use in OTP verification
                    TempData["Email"] = manager.Email;
                    return RedirectToAction("VerifyOtp");
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ViewData["Message"] = $"Đăng ký thất bại! {errorMessage}";
                    return View("DangKy");
                }
            }
            catch (HttpRequestException ex)
            {
                ViewData["Message"] = $"Đăng ký thất bại! Lỗi yêu cầu HTTP: {ex.Message}";
                return View("DangKy");
            }
            catch (IOException ex)
            {
                ViewData["Message"] = $"Đăng ký thất bại! Lỗi IO: {ex.Message}";
                return View("DangKy");
            }
            catch (Exception ex)
            {
                ViewData["Message"] = $"Đăng ký thất bại! Lỗi không xác định: {ex.Message}";
                return View("DangKy");
            }
        }
            [HttpGet]
            public IActionResult VerifyOtp()
            {
                return View();
            }

        [HttpPost]
        public async Task<IActionResult> VerifyOtp(int otp)
        {
            try
            {
                // Create an HTTP client to communicate with the API
                var client = _httpClientFactory.CreateClient("ApiClient");

                // Serialize the OTP to JSON
                var json = JsonSerializer.Serialize(new { otp });

                // Prepare the request content
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Send the POST request to the API endpoint for OTP verification
                var response = await client.PostAsync("api/Managers/VerifyOtp", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    // Handle unsuccessful OTP verification
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ViewData["Message"] = $"Xác thực OTP thất bại! {errorMessage}";
                    return View();
                }
            }
            catch (Exception ex)
            {
                // Log the exception for troubleshooting
                ViewData["Message"] = $"Xác thực OTP thất bại! {ex.Message}";
                return View();
            }
        }


        [HttpGet]
            public IActionResult VerifyOtpForgotPassword()
            {
                return View();
            }

            [HttpPost]
            public async Task<IActionResult> VerifyOtpForgotPassword(int otp)
            {
                var client = _httpClientFactory.CreateClient("ApiClient");
                var json = JsonSerializer.Serialize(new { otp });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/Managers/VerifyOTPForgotPassword", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Email"] = TempData["Email"]; // Retrieve email from TempData
                    return RedirectToAction("ResetPassword");
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ViewData["Message"] = $"Xác thực OTP thất bại! {errorMessage}";
                    return View();
                }
            }

            [HttpGet]
            public IActionResult ResetPassword()
            {
                ViewData["Email"] = TempData["Email"]; // Pass email to the view
                return View();
            }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string email, string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                ViewData["Message"] = "Vui lòng điền đầy đủ thông tin.";
                return View();
            }

            if (password != confirmPassword)
            {
                ModelState.AddModelError("confirmPassword", "Mật khẩu xác nhận không khớp.");
                return View();
            }

            try
            {
                var client = _httpClientFactory.CreateClient("ApiClient");

                // Construct JSON payload with 'email', 'password', 'confirmPassword' fields
                var json = JsonSerializer.Serialize(new { email, password, confirmPassword });
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Send POST request to reset password API endpoint
                var response = await client.PostAsync("api/Managers/ResetPassword", content);

                // Handle response
                if (response.IsSuccessStatusCode)
                {
                    ViewBag.Message = "Đặt lại mật khẩu thành công.";
                    return RedirectToAction("DangNhap");
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ViewData["Message"] = $"Yêu cầu đặt lại mật khẩu thất bại! {errorMessage}";
                    return View();
                }
            }
            catch (HttpRequestException ex)
            {
                ViewData["Message"] = $"Đặt lại mật khẩu thất bại! Lỗi yêu cầu HTTP: {ex.Message}";
                return View();
            }
            catch (IOException ex)
            {
                ViewData["Message"] = $"Đặt lại mật khẩu thất bại! Lỗi IO: {ex.Message}";
                return View();
            }
            catch (Exception ex)
            {
                ViewData["Message"] = $"Đặt lại mật khẩu thất bại! Lỗi không xác định: {ex.Message}";
                return View();
            }
        }



        [HttpGet]
            public IActionResult ForgotPassword()
            {
                return View();
            }

            [HttpPost]
            public async Task<IActionResult> ForgotPassword(string email)
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    ViewData["Message"] = "Email không được để trống.";
                    return View();
                }

                var client = _httpClientFactory.CreateClient("ApiClient");
                var json = JsonSerializer.Serialize(new { email }); // Pass email to the JSON payload
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/Managers/ForgotPassword", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Email"] = email; // Store email in TempData for verification
                    return RedirectToAction("VerifyOtpForgotPassword");
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ViewData["Message"] = $"Yêu cầu đặt lại mật khẩu thất bại! {errorMessage}";
                    return View();
                }
            }
            public IActionResult Index()
        {
            return View();
        }

        public IActionResult DangNhap()
        {
            return View();
        }
        public IActionResult DangKy()
        {
            return View();
        }
       
    }
}
