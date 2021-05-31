using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using InternetAuction.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InternetAuction.Controllers
{
    [Authorize]
    public class LotController : Controller
    {
        private readonly ApplicationContext _db;
        private readonly UserManager<User> _userManager;

        public LotController(ApplicationContext db, UserManager<User> userManager)
        {
            _db = db;
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
            if (image is {Length: > 0})
            {
                byte[] item;
                await using (var fileStream = image.OpenReadStream())
                await using (var memoryStream = new MemoryStream())
                {
                    await fileStream.CopyToAsync(memoryStream);
                    item = memoryStream.ToArray();
                }

                lot.Img = item;
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
            if (image is {Length: > 0})
            {
                byte[] item;
                await using (var fileStream = image.OpenReadStream())
                await using (var memoryStream = new MemoryStream())
                {
                    await fileStream.CopyToAsync(memoryStream);
                    item = memoryStream.ToArray();
                }

                lot.Img = item;
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