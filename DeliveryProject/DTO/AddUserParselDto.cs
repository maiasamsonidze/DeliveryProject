using System.ComponentModel.DataAnnotations;

namespace DeliveryProject.DTO
{
    public class AddUserParselDto
    {
        [Required (ErrorMessage ="გთხოვთ მიუთითოთ მომხმარებლის ID")]
        public Guid Userid { get; set; }


        [Required (ErrorMessage ="გთხოვთ მიუთითოთ ამანათის ID")]
        public Guid Parselid { get; set; }


        public DateTime Createdat { get; set; }


    }
}
