using System.ComponentModel.DataAnnotations;

namespace DeliveryProject.DTO
{
    public class LoginUserDto
    {

            [Required(ErrorMessage = "იმეილის შეყვანა სავალდებულოა")]
            [EmailAddress(ErrorMessage = "იმეილის ფორმატი არასწორია, გთხოვთ მიუთითოთ შემდეგი ფორმატით: example@example.com")]

            public string Email { get; set; }

            [Required(ErrorMessage = "გთხოვთ შეიყვანოთ პაროლი")]
            [StringLength(100, MinimumLength = 8, ErrorMessage = "პაროლი უნდა იყოს მინიმუმ {2} და მაქსიმუმ {1} შორის. ")]
            [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
           ErrorMessage = "პაროლი უნდა შეიცავდეს მინიმუმ ერთ დიდ ლათინურ ასოს, ერთ პატარა ლათინურ ასოს,  ერთ რიცხვს და ერთ განსაკუთრებულ სიმბოლოს(@,$,!,%,*,?,&)")]
            public string Password { get; set; }


    }
    
}

