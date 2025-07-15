using System.Diagnostics;
using _7_FormsApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace _7_FormsApp.Controllers
{
    public class HomeController : Controller
    {

        public HomeController(ILogger<HomeController> logger)
        {
        }

        [HttpGet]
        public IActionResult Index(string searchString,string category)
        {

            var products = Repository.Products;

            if (!String.IsNullOrEmpty(searchString))
            {
                ViewBag.searchString = searchString;
                products = products.Where(p => p.Name!.ToLower().Contains(searchString)).ToList();
            }

            if (!String.IsNullOrEmpty(category) && category != "0")
            {
                products = products.Where(p => p.CategoryId == int.Parse(category)).ToList();
            }


            //ViewBag.categories = new SelectList(Repository.Categories, "CategoryId", "Name",category);

            var model = new ProductViewModel
            {
                Products = products,
                Categories = Repository.Categories,
                SelectedCategory = category
            };


            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
            return View();
        }

        [HttpPost]
        public async  Task<IActionResult> Create(/*[Bind("Name","Price")] */ Product model,IFormFile imageFile)   // bind ile sadece almak istediklerimizi alýrýz 
        {
            var extension = "";
            if (imageFile != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" }; // allowedExtensions ile sadece bu uzantýlarý kabul ediyoruz
                 extension = Path.GetExtension(imageFile.FileName); // imageFile ýn uzantýsýný alýyoruz
                if (!allowedExtensions.Contains(extension))  // format kontrolü yapýyoruz, eðer allowedExtensions içinde yoksa hata mesajý veriyoruz
                {
                    ModelState.AddModelError("", "Sadece .jpg, .jpeg ve .png uzantýlý dosyalar yükleyebilirsiniz!"); // ModelState e hata ekliyoruz
                }
            }
           

            if (ModelState.IsValid)  // hiçbir required þartýna takýlmazsa if bloðuna giriyor
            {
                if(imageFile != null)
                {
                     var randomFileName = string.Format($"{Guid.NewGuid().ToString()}{extension}"); // Guid.NewGuid() ile rastgele bir isim oluþturuyoruz ve uzantýyý ekliyoruz
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", randomFileName); // path i wwwroot/img klasörüne kaydediyoruz
                    using (var stream = new FileStream(path,FileMode.Create)) // FileStream ile dosyayý oluþturuyoruz
                {
                    await imageFile.CopyToAsync(stream); // imageFile ý stream e kopyalýyoruz
                }
                    model.Image = randomFileName; // modelin image alanýna rastgele ismi atýyoruz
                    model.ProductId = Repository.Products.Count + 1; // ProductId yi repository deki ürün sayýsýndan 1 fazla yapýyoruz
                    Repository.CreateProduct(model);
                    return RedirectToAction("Index");  // redirect to action ile index e yönlendirip orayý çalýþtýrmasýný saðladýk 
                }
                
            }
                ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
                return View(model);  // eðer model geçerli deðilse yani required þartlarýna takýlýrsa tekrar ayný sayfayý gösteriyoruz
           
            
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var entity = Repository.Products.FirstOrDefault(p => p.ProductId == id);
            if(entity == null)
            {
                return NotFound();
            }
            ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
            return View(entity);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id , Product model , IFormFile? imageFile)
        {
            if(id != model.ProductId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    var extension = Path.GetExtension(imageFile.FileName); // imageFile ýn uzantýsýný alýyoruz
                    var randomFileName = string.Format($"{Guid.NewGuid().ToString()}{extension}"); // Guid.NewGuid() ile rastgele bir isim oluþturuyoruz ve uzantýyý ekliyoruz
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", randomFileName); // path i wwwroot/img klasörüne kaydediyoruz

                    using (var stream = new FileStream(path, FileMode.Create)) // FileStream ile dosyayý oluþturuyoruz
                    {
                        await imageFile.CopyToAsync(stream); // imageFile ý stream e kopyalýyoruz
                    }
                    model.Image = randomFileName;
                }
                Repository.EditProduct(model);
                return RedirectToAction("Index");
            }
            ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
            return View(model);

        }


        public IActionResult Delete(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var entity = Repository.Products.FirstOrDefault(p => p.ProductId == id);
            if(entity == null)
            {
                return NotFound();
            }

            Repository.DeleteProduct(entity);
            return RedirectToAction("Index");

        }


    }
}
