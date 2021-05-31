using Microsoft.AspNetCore.Identity;

namespace InternetAuction.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string LotsToBuy { get; set; } = "";
    }
}