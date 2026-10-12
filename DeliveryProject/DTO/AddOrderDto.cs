using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace DeliveryProject.DTO
{
    public class AddOrderDto
    {

        public Guid Userid { get; set; }

        [Required (ErrorMessage ="პირადი ნომერი სავალდებულოა")]
        [RegularExpression(@"^([0-9]{9}|[0-9]{11})$", ErrorMessage = "საიდენტიფიკაციო/პირადი ნომერი უნდა შედგებოდეს 9 ან 11 ციფრისგან.")]
        public string Pernumber { get; set; } = null!;

        [Required (ErrorMessage ="კლიენტის სახელი სავალდებულოა")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "მომხმარებლის სახელი უნდა შეიცავდეს მინიმუმ2 და მქსიმუმ 100 ასოს")]
        [RegularExpression(@"^[ა-ჰA-Za-z]+([ '""’][ა-ჰA-Za-z]+)*$", ErrorMessage = "სახელის შემადგენლობაში დასაშვებია მხოლოდ ქართული და ლათინური ასოები, ასევე სიმბოლოები ' \" ’")]
        public string Customername { get; set; } = null!;
        

        [Required (ErrorMessage ="კლიენტის ტელეფონის ნომერი სავალდებულოა")]
        [Phone(ErrorMessage = "არავალიდური ტელეფონის ნომერი")]
        [RegularExpression(@"^(\+995\s?)?5\d{2}[\s-]?\d{3}[\s-]?\d{3}$", ErrorMessage = "გთხოვთ, შეკვეტის ცხრილში შეიყვანოთ საქართველოში რეგისტრირებული ტელეფონის ნომერი")]
        public string Customerphone { get; set; } = null!;

        [Required (ErrorMessage ="მიტანის მისამართი სავალდებულოა")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "მიტანის მისამართი უნდა შეიცავდეს მინიმუმ2 და მქსიმუმ 100 ასოს")]
        [RegularExpression(@"^[ა-ჰA-Za-z0-9]+([ ,'""’]+[ა-ჰA-Za-z0-9]+)*$", ErrorMessage = "მიტანის მისამართის შემადგენლობაში დასაშვებია მხოლოდ ქართული და ლათინური ასოები, ციფრები, ასევე სიმბოლოები , ' \" ’")]
        public string Deliveryaddress { get; set; } = null!;
        
        [Required (ErrorMessage ="პროდუქტის სახელი სავალდებულოა")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "პროდუქტის სახელი უნდა შეიცავდეს მინიმუმ2 და მქსიმუმ 100 ასოს")]
        [RegularExpression(@"^[ა-ჰA-Za-z]+([ '""’][ა-ჰA-Za-z]+)*$", ErrorMessage = "პროდუქტის სახელის შემადგენლობაში დასაშვებია მხოლოდ ქართული და ლათინური ასოები, ასევე სიმბოლოები ' \" ’")]
        public string Productname { get; set; } = null!;

        
        public int Quantity { get; set; }

      
        public decimal Unitprice { get; set; }

        public string? Comments { get; set; }

        
        public decimal Productprice { get; set; }

        
        public decimal Deliveryprice { get; set; }

       
        public decimal Fullprice { get; set; }



    }
}
