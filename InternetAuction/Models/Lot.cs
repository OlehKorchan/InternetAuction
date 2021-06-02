using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InternetAuction.Models
{
    [Authorize]
    public class Lot
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Укажите название лота!")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина строки должна быть от 3 до 50 символов")]
        public string Name { get; set; }
        [StringLength(1000, MinimumLength = 3, ErrorMessage = "Длина строки должна быть от 3 до 1000 символов")]
        public string Desc { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public bool Sold { get; set; }
        public string UserId { get; set; }
        public string BuyerId { get; set; }
        [Required(ErrorMessage = "Укажите стартовую цену лота!")]
        public int StartValue { get; set; }
        public int CurrentValue { get; set; }
        [Required(ErrorMessage = "Укажите минимальное повышение ставки!")]
        public int MinUpgradeBet { get; set; }
        public string Img { get; set; }
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "Укажите дату окончания торгов!")]
        [Remote(action: "CheckLotDate", controller: "Lot", ErrorMessage ="Дата должна быть больше текущей")]
        public DateTime FinishDate { get; set; }
    }
}