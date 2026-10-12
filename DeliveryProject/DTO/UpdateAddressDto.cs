using System.ComponentModel.DataAnnotations;

namespace DeliveryProject.DTO
{
    public class UpdateAddressDto
    {

        [Required (ErrorMessage = "გთხოვთ შეიყვანოთ არსებული მომხმარებლის პირადი ნომერი.")]
        [RegularExpression(@"^([0-9]{9}|[0-9]{11})$", ErrorMessage = "საიდენტიფიკაციო/პირადი ნომერი უნდა შედგებოდეს 9 ან 11 ციფრისგან.")]
        public string Pernumber { get; set; } = null!;


        [Required (ErrorMessage = "გთხოვთ შეიყვანოთ ახალი მიწოდების მისამართი.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "მიტანის მისამართი უნდა შეიცავდეს მინიმუმ2 და მქსიმუმ 100 ასოს")]
        [RegularExpression(@"^[ა-ჰA-Za-z0-9]+([ ,'""’]+[ა-ჰA-Za-z0-9]+)*$", ErrorMessage = "მიტანის მისამართის შემადგენლობაში დასაშვებია მხოლოდ ქართული და ლათინური ასოები, ციფრები, ასევე სიმბოლოები , ' \" ’")]
        public string Deliveryaddress { get; set; } = null!;

    }
}
