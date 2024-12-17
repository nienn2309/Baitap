using Baitap.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace Baitap.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHubContext<ProductHub> _hubContext;

        private const string BaseUrl = "https://simple-product-apis.vercel.app";

        public HomeController(ILogger<HomeController> logger, IHubContext<ProductHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;   
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Product()
        {
            try
            {
                using var client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync($"{BaseUrl}/products?page=1&pageSize=10&orderBy=NAME_ASC");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                var productResponse = JsonSerializer.Deserialize<ProductResponse>(responseBody);

                return View(productResponse.items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch products.");
                return View(new List<Product>());
            }
        }


        public IActionResult CreateProduct()
        {
            return View(new Product()); 
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product model)
        {
            try
            {
                using var client = new HttpClient();
                string json = JsonSerializer.Serialize(model);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync($"{BaseUrl}/products", content);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var createdProduct = JsonSerializer.Deserialize<Product>(responseBody);

                await _hubContext.Clients.All.SendAsync("ReceiveProductCreated", createdProduct);

                return RedirectToAction("Product");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create product.");
                return View(model);
            }
        }


        public async Task<IActionResult> DeleteProduct(string id)
        {
            try
            {
                using var client = new HttpClient();
                HttpResponseMessage response = await client.DeleteAsync($"{BaseUrl}/products/{id}");
                response.EnsureSuccessStatusCode();

                await _hubContext.Clients.All.SendAsync("ReceiveProductDeleted", id);

                return RedirectToAction("Product");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete product.");
                return RedirectToAction("Product");
            }
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
