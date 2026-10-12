using System.ComponentModel.DataAnnotations;

namespace DeliveryProject.DTO
{
    public class AddRoleDto
    {
        [Required (ErrorMessage = "გთხოვთ შეიყვანოთ როლი")]
        public string Rolename { get; set; } = null!;

    }
}
