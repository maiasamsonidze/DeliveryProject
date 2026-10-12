namespace DeliveryProject.DTO
{
    
        public class UserParselDto
        {
            public Guid Id { get; set; }
            public Guid Userid { get; set; }
            public string? UserName { get; set; }

            public Guid Parselid { get; set; }
            public int? ParselCode { get; set; }

            public Guid Statusid { get; set; }
            public string? Status { get; set; }

            public DateTime Createdat { get; set; }
        }
    
}
