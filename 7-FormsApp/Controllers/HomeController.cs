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
        public async  Task<IActionResult> Create(/*[Bind("Name","Price")] */ Product model,IFormFile imageFile)   // bind ile sadece almak istediklerimizi al�r�z 
        {
            var extension = "";
            if (imageFile != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" }; // allowedExtensions ile sadece bu uzant�lar� kabul ediyoruz
                 extension = Path.GetExtension(imageFile.FileName); // imageFile �n uzant�s�n� al�yoruz
                if (!allowedExtensions.Contains(extension))  // format kontrol� yap�yoruz, e�er allowedExtensions i�inde yoksa hata mesaj� veriyoruz
                {
                    ModelState.AddModelError("", "Sadece .jpg, .jpeg ve .png uzant�l� dosyalar y�kleyebilirsiniz!"); // ModelState e hata ekliyoruz
                }
            }
           

            if (ModelState.IsValid)  // hi�bir required �art�na tak�lmazsa if blo�una giriyor
            {
                if(imageFile != null)
                {
                     var randomFileName = string.Format($"{Guid.NewGuid().ToString()}{extension}"); // Guid.NewGuid() ile rastgele bir isim olu�turuyoruz ve uzant�y� ekliyoruz
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", randomFileName); // path i wwwroot/img klas�r�ne kaydediyoruz
                    using (var stream = new FileStream(path,FileMode.Create)) // FileStream ile dosyay� olu�turuyoruz
                {
                    await imageFile.CopyToAsync(stream); // imageFile � stream e kopyal�yoruz
                }
                    model.Image = randomFileName; // modelin image alan�na rastgele ismi at�yoruz
                    model.ProductId = Repository.Products.Count + 1; // ProductId yi repository deki �r�n say�s�ndan 1 fazla yap�yoruz
                    Repository.CreateProduct(model);
                    return RedirectToAction("Index");  // redirect to action ile index e y�nlendirip oray� �al��t�rmas�n� sa�lad�k 
                }
                
            }
                ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
                return View(model);  // e�er model ge�erli de�ilse yani required �artlar�na tak�l�rsa tekrar ayn� sayfay� g�steriyoruz
           
            
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
                    var extension = Path.GetExtension(imageFile.FileName); // imageFile �n uzant�s�n� al�yoruz
                    var randomFileName = string.Format($"{Guid.NewGuid().ToString()}{extension}"); // Guid.NewGuid() ile rastgele bir isim olu�turuyoruz ve uzant�y� ekliyoruz
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", randomFileName); // path i wwwroot/img klas�r�ne kaydediyoruz

                    using (var stream = new FileStream(path, FileMode.Create)) // FileStream ile dosyay� olu�turuyoruz
                    {
                        await imageFile.CopyToAsync(stream); // imageFile � stream e kopyal�yoruz
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
