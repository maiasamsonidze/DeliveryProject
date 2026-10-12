using System.ComponentModel.DataAnnotations;

namespace DeliveryProject.DTO
{
    public class AddUserDto
    {
        [Required(ErrorMessage = "გთხოვთ შეიყვანოთ მომხმარებლის სახელი")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "მომხმარებლის სახელი უნდა შეიცავდეს მინიმუმ2 და მქსიმუმ 100 ასოს")]
        [RegularExpression(@"^[ა-ჰA-Za-z]+([ '""’][ა-ჰA-Za-z]+)*$", ErrorMessage = "სახელის შემადგენლობაში დასაშვებია მხოლოდ ქართული და ლათინური ასოები, ასევე სიმბოლოები ' \" ’")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "იმეილის შეყვანა სავალდებულოა")]
        [EmailAddress(ErrorMessage = "იმეილის ფორმატი არასწორია, გთხოვთ მიუთითოთ შემდეგი ფორმატით: example@example.com")]
        public string Email { get; set; } = null!;


        [Required(ErrorMessage = "გთხოვთ შეიყვანოთ პაროლი")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "პაროლი უნდა იყოს მინიმუმ {2} და მაქსიმუმ {1} შორის. ")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
       ErrorMessage = "პაროლი უნდა შეიცავდეს მინიმუმ ერთ დიდ ლათინურ ასოს, ერთ პატარა ლათინურ ასოს,  ერთ რიცხვს და ერთ განსაკუთრებულ სიმბოლოს(@,$,!,%,*,?,&)")]
        public string Userpassword { get; set; } = null!;


        [Required(ErrorMessage = "გთხოვთ შეიყვანოთ ტელეფონის ნომერი")]
        [Phone(ErrorMessage = "არავალიდური ტელეფონის ნომერი")]
        [RegularExpression(@"^(\+995\s?)?5\d{2}[\s-]?\d{3}[\s-]?\d{3}$", ErrorMessage = "გთხოვთ, შეიყვანოთ საქართველოში რეგისტრირებული ტელეფონის ნომერი")]

        public string Phonenumber { get; set; } = null!;


        [Required(ErrorMessage = "გთხოვთ შეიყვანოთ როლი")]
        [StringLength(50, ErrorMessage = "როლის სახელი ძალიან გრძელია")]
        [RegularExpression(@"^(DeliveryAgent|Client)$", ErrorMessage = "როლი შეიძლება იყოს მხოლოდ 'Client' ან 'DeliveryAgent'")]

        public string Rolename { get; set; } = null!;


        public DateTime Createdat { get; set; }
    }
}
