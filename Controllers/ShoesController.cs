using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ShoeVault.Data;
using ShoeVault.Models;

namespace ShoeVault.Controllers
{
    [Authorize]
    public class ShoesController : Controller
    {
        private readonly AppDbContext _context;

        public ShoesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Shoes
        public async Task<IActionResult> Index()
        {
            var shoes = await _context.Shoes
                .Include(s => s.Images)
                .ToListAsync();

            return View(shoes);
        }

        // CREATE (GET)
        public IActionResult Create()
        {
            return View();
        }

        // CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Shoe shoe, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                _context.Add(shoe);
                await _context.SaveChangesAsync();

                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid() + "_" + imageFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    var shoeImage = new ShoeImage
                    {
                        ShoeId = shoe.Id,
                        ImagePath = "/uploads/" + uniqueFileName
                    };

                    _context.ShoeImages.Add(shoeImage);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            return View(shoe);
        }

        public async Task<IActionResult> Category(string type)
        {
            if (string.IsNullOrEmpty(type))
                return RedirectToAction("Index");

            var shoes = await _context.Shoes
                .Include(s => s.Images)
                .Where(s => s.Category == type)
                .ToListAsync();

            ViewBag.CategoryTitle = FormatCategoryTitle(type);

            return View("Index", shoes);
        }

        private string FormatCategoryTitle(string type)
        {
            return type switch
            {
                "RoadRunning" => "Road Running",
                "TrailRunning" => "Trail Running",
                "HikingShoes" => "Hiking Shoes",
                "HikingBoots" => "Hiking Boots",
                _ => type
            };
        }



        // GET: /Shoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var shoe = await _context.Shoes
                .Include(s => s.Images)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (shoe == null)
                return NotFound();

            return View(shoe);
        }

        // GET: /Shoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var shoe = await _context.Shoes
                .Include(s => s.Images)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shoe == null)
                return NotFound();

            return View(shoe);
        }

        // POST: /Shoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Shoe shoe, List<IFormFile> imageFiles)
        {
            if (id != shoe.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(shoe);

            var existingShoe = await _context.Shoes
                .Include(s => s.Images)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (existingShoe == null)
                return NotFound();

            // Update allowed fields
            existingShoe.Name = shoe.Name;
            existingShoe.Brand = shoe.Brand;
            existingShoe.Category = shoe.Category;

            await _context.SaveChangesAsync();

            // Handle image uploads
            if (imageFiles != null && imageFiles.Count > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                Directory.CreateDirectory(uploadsFolder);

                foreach (var file in imageFiles)
                {
                    if (file.Length > 0)
                    {
                        var uniqueFileName = Guid.NewGuid() + "_" + file.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var newImage = new ShoeImage
                        {
                            ShoeId = existingShoe.Id,
                            ImagePath = "/uploads/" + uniqueFileName
                        };

                        _context.ShoeImages.Add(newImage);
                    }
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        // GET: /Shoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var shoe = await _context.Shoes
                .Include(s => s.Images)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (shoe == null)
                return NotFound();

            return View(shoe);
        }

        // POST: /Shoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shoe = await _context.Shoes
                .Include(s => s.Images)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shoe != null)
            {
                _context.Shoes.Remove(shoe);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteImage(int imageId)
        {
            var image = await _context.ShoeImages.FindAsync(imageId);

            if (image != null)
            {
                // Delete file from wwwroot/uploads
                var filePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    image.ImagePath.TrimStart('/')
                );

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                _context.ShoeImages.Remove(image);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Edit", new { id = image?.ShoeId });
        }

    }
}
