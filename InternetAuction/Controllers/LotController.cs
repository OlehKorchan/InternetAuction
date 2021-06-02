using System;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using ImageMagick;
using InternetAuction.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace InternetAuction.Controllers
{
    [Authorize]
    public class LotController : Controller
    {
        private readonly ApplicationContext _db;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public LotController(ApplicationContext db, UserManager<User> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        public async Task<IActionResult> LotPage(int id)
        {
            var model = await _db.Lots.FirstOrDefaultAsync(i => i.Id == id);
            return View(model);
        }

        public async Task<IActionResult> UserLots()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var model = _db.Lots.Where(i => i.UserId == currentUser.Id).ToList();
            return View(model);
        }

        public async Task<IActionResult> LotsToBuy()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var model = _db.Lots.Where(i => i.FinishDate <= DateTime.Now && i.BuyerId == currentUser.Id).ToList();
            return View(model);
        }

        public async Task<IActionResult> SetLotsToBuy()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            foreach (var lot in _db.Lots.Where(i => i.FinishDate <= DateTime.Now && i.BuyerId == currentUser.Id).ToList())
            {
               if (lot.Sold == false) currentUser.LotsToBuy += lot.Id + ",";
                lot.Sold = true;
            }

            _db.Update(currentUser);
            await _db.SaveChangesAsync();
            return RedirectToAction("LotsToBuy");
        }
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Lot lot, IFormFile image)
        {
            if (image != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                string uniqueFileName = Guid.NewGuid() + "_" + image.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                await image.CopyToAsync(new FileStream(filePath, FileMode.Create));
                lot.Img = uniqueFileName;
            }
            lot.CurrentValue = lot.StartValue;
            _db.Lots.Add(lot);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Edit(int id)
        {
            
            if (id != 0)
            {
                var lot = await _db.Lots.FirstOrDefaultAsync(i => i.Id == id);
                var currentUser = await _userManager.GetUserAsync(User);
                if (lot != null && lot.UserId == currentUser.Id && lot.FinishDate > DateTime.Now) return View(lot);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Lot lot, IFormFile image)
        {
            if (image != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                await image.CopyToAsync(new FileStream(filePath, FileMode.Create));
                lot.Img = filePath;
            }
            _db.Lots.Update(lot);
            await _db.SaveChangesAsync();
            return RedirectToAction("UserLots");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (id != 0)
            {
                Lot lot = await _db.Lots.FirstOrDefaultAsync(i => i.Id == id);
                if (lot != null)
                {
                    _db.Remove(lot);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("UserLots");
                }
            }

            return NotFound();
        }
        [AcceptVerbs("Get", "Post")]
        public IActionResult CheckLotDate(DateTime finishDate)
        {
            return Json(finishDate > DateTime.Now);
        }
        
        public async Task<IActionResult> UpgradeValue(int id, int newBet, string buyerId)
        {
            if (id == 0 || buyerId == null) return NotFound();
            var lot = await _db.Lots.FirstOrDefaultAsync(i => i.Id  == id);
            if (lot == null) return NotFound();
            lot.CurrentValue = newBet;
            lot.BuyerId = buyerId;
            _db.Update(lot);
            await _db.SaveChangesAsync();
            return RedirectToAction("LotPage", new { id });

        }
    }
}