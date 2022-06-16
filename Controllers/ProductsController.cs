using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.Data;
using ServerApp.DTO;
using ServerApp.Models;
using Microsoft.AspNetCore.Authorization; // ** [Authorize] kullanabilmek için

namespace ServerApp.Controllers
{
    [Authorize] // token bilgisi gerekli..
    [ApiController]
    [Route("api/[controller]")]    // => localhost:5000/api/Products  
    public class ProductsController:ControllerBase
    {
        /*private static readonly string[] Products = 
        {
            "samsung s6", "samsung s7", "samsung s8"
        };*/

        //private static List<Product> _products;

        private readonly SocialContext _context; // dbcontext
        public ProductsController(SocialContext context)
        {
            _context = context; // depency injection

            /*
            _products = new List<Product>();
            
            _products.Add(new Product() {ProductId = 1, Name="Samsung S6", Price = 3000, IsActive = false });
            _products.Add(new Product() {ProductId = 2, Name="Samsung S7", Price = 4000, IsActive = true });
            _products.Add(new Product() {ProductId = 3, Name="Samsung S8", Price = 5000, IsActive = true });
            _products.Add(new Product() {ProductId = 4, Name="Samsung S9", Price = 6000, IsActive = false });
            _products.Add(new Product() {ProductId = 5, Name="Samsung S10", Price = 7000, IsActive = true });
            */
        }


        [HttpGet] // HttpGet => localhost:5000/api/Products  
        [AllowAnonymous]  // bütün kullacılara açık yani [Authorize] etkisiz kılındı.. token bilgisi gerekmeyecek..
        public async Task<ActionResult> GetProducts() // public List<Product> GetProducts()
        {
            /*
            1.yol
            var products = await _context.
            Products.
            Select(p=> new ProductDTO(){
                ProductId = p.ProductId,
                Name = p.Name,
                Price = p.Price,
                IsActive = p.IsActive
            }).
            ToListAsync(); //await buradaki işlemi bitmeden alttaki kod çalışmaz..
            */

            //2.yol.. kısa olanı => p=> ProductToDTO(p)
            var products = await _context.
            Products.
            Select(p=> ProductToDTO(p)).
            ToListAsync(); //await buradaki işlemi bitmeden alttaki kod çalışmaz..

            return Ok(products);

            // return _context.Products.ToList(); 
            //return _products;
        }

         [HttpGet("{id}")] // HttpGet + id => localhost:5000/api/Products  + /2  => localhost:5000/api/Products/2  
        public async Task<ActionResult> GetProduct(int id) // Product yerine IActionResult.. IActionResult kullanınca NotFound hata vermez ama return p; hata verir hata kaldırmak için ise 
        {
            /*
            if(Products.Length-1<id)
                return "";
            return Products[id];
            */

            /*var p = _products.FirstOrDefault(i => i.ProductId == id);
            if(p == null)
            {
                return NotFound(); // 404 hata bulunamadı kodunu geriye döndürür..
            }
            return Ok(p); // 200 status kodunu oluşturacak olan method..
            */

            /*
            1.yol
             var products = await _context.
             Products.
              Select(p=> new ProductDTO(){
                ProductId = p.ProductId,
                Name = p.Name,
                Price = p.Price,
                IsActive = p.IsActive
            }).
             FirstOrDefaultAsync(i => i.ProductId == id);  // *** FirstOrDefaultAsync
            */

            
             var products = await _context.
             Products.
             FirstOrDefaultAsync(i => i.ProductId == id);  // *** FirstOrDefaultAsync
            
            if(products == null)
            {
                return NotFound(); // 404 hata bulunamadı kodunu geriye döndürür..
            }

            //2.yol.. kısa olanı => ProductToDTO(products)
            return Ok(ProductToDTO(products)); // 200 status kodunu oluşturacak olan method..
        }

        // Method isminin (CreateProduct) ne olduğunun önemi yok. routing işlemi için method ismi kullanılmayacak..
        // routing işlemini yapacak olan => [Route("api/[controller]")] => localhost:5000/api/Products kısmıdır..
        /*
        [HttpPost]
        public IActionResult CreateProduct(Product p)
        {
            _products.Add(p);

            // foreach ile eklenen bilgiyi görmek adına yapılan işlem.. ama Eklenen ürün get sorgusu yapıldığında görülmeyecek
            foreach (var item in _products)
            {
                Console.WriteLine(item.Name); //Terminal ekranında görünecek eklenen...
            }


            return Ok(p); // return Ok(); geriye herhangi bir bilgi dönmez.. Eklenen product bilgisini tekrar client tarafına göndermek için ise eklenen ürün bilgisini  return Ok(p); şeklinde yapılır..

        }
        */

        [HttpPost] // localhost:5000/api/Products (Post)
        public async Task<IActionResult> CreateProduct(Product entity)
        {
            _context.Products.Add(entity);
            await _context.SaveChangesAsync(); // await & SaveChangesAsync 

            // return CreatedAtAction(nameof(GetProduct), new {id=entity.ProductId},entity); => iptal aşağıdakini kullanacağız..

            return CreatedAtAction(nameof(GetProduct), new {id=entity.ProductId},ProductToDTO(entity)); // 201 durum kodu döndürür.. ilk parametre action ismi yani GetProduct'a yönlendirilecek, ikinci paramete route bilgisi (GetProduct(int id)) <=> id=entity.ProductId, üçüncü parametre object
        }

        [HttpPut("{id}")] // metot tipi sorgu localhost:5000/api/Products/2 => buna Get sorgusu yaparsak bu metot çalışır ( ([HttpGet("{id}")])  ) gene aynı url'ye bir güncelleme işlemi yapmak için id bilgisi gönderilmeli url'nin bir parçası olacak güncellenen bilgiyi de veri tabanına gönderiyor olmalıyız.. 
        public async Task<IActionResult> UpdateProduct(int id, Product entity)
        {
            if(id!=entity.ProductId)
            {
                return BadRequest(); // 400 Bad Request hatası..
            }

            var product = await _context.Products.FindAsync(id); 
            // veya var product = await _context.Products.FirstOrDefaultAsync(i => i.ProductId == id); 

            if(product == null)
            {
                return NotFound(); // 404 hata bulunamadı kodunu geriye döndürür..
            }

            product.Name = entity.Name;
            product.Price = entity.Price;
            product.IsActive = entity.IsActive;

            try
            {
                await _context.SaveChangesAsync(); // await & SaveChangesAsync 
            }
            catch(Exception)
            {
                return NotFound(); 
            }

            return NoContent(); // 204 success kodu..
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if(product == null)
            {
                return NotFound(); // 404 hata bulunamadı kodunu geriye döndürür..
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

             return NoContent(); // 204 success kodu..
        }

        // GetProducts, GetProduct(int id) ve CreateProduct(Product entity) de kullanılıcak metot..
        private static ProductDTO ProductToDTO(Product p)
        {
            return new ProductDTO()
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Price = p.Price,
                IsActive = p.IsActive
            };

        }


    }
}