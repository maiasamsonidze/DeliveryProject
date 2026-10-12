using DeliveryProject.Models;

namespace DeliveryProject.DTO
{
    public class UserDto
    {

        public Guid Id { get; set; }

        public string Username { get; set; } = null!;

        public string Email { get; set; } = null!;


        public string Phonenumber { get; set; } = null!;

       // public Guid Roleid { get; set; }

        public DateTime Createdat { get; set; }


        public string Rolename { get; set; }


    }
}
