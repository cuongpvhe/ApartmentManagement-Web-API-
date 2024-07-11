using ApartmentManagement.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
                    ViewData["Message"] = "Đăng ký thành công!";
                    return View("DangNhap"); // Hoặc trang khác bạn muốn chuyển hướng đến
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
