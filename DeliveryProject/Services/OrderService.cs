using ClosedXML.Excel;
using DeliveryProject.DTO;
using DeliveryProject.Exceptions;
using DeliveryProject.Models;
using DeliveryProject.Services.Iservices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DeliveryProject.Services
{
    public class OrderService: IOrdersService
    {

        private readonly DeliveryContext OrderDelivery;
        private readonly IHubContext<OrderNotificationHub> hubContext;

        public OrderService(DeliveryContext OrderDelivery, IHubContext<OrderNotificationHub> hubContext)
        {
            this.OrderDelivery = OrderDelivery;
            this.hubContext = hubContext;
        }

       

        public async Task<string> AddParselCodeByAdmin(AddParselDto addParselDto)
        {
           
            var order = await OrderDelivery.Orders.FirstOrDefaultAsync(x => x.Id == addParselDto.Orderid);

            if (order == null)
            {
                throw new Exception("შეკვეთა ვერ მოიძებნა.");
            }

          
            var alreadyExists = await OrderDelivery.Parsels.AnyAsync(x => x.Orderid == addParselDto.Orderid);

            if (alreadyExists)
            {
                throw new Exception("ამ შეკვეთას უკვე აქვს ამანათის კოდი.");
            }

            
            var maxCode = await OrderDelivery.Parsels.MaxAsync(x => (int?)x.Parselcode) ?? 0;

            var newCode = maxCode + 1;

            var parsel = new Parsel
            {
                Orderid = addParselDto.Orderid,
                Parselcode = newCode
            };

            await OrderDelivery.Parsels.AddAsync(parsel);
            await OrderDelivery.SaveChangesAsync();

            return $"ამანათის კოდი წარმატებით განისაზღვრა: {newCode}";


        }

        public async Task<string> DeleteOrderByAdmin(Guid orderId)
        {
            var result = await OrderDelivery.Orders.FirstOrDefaultAsync(o => o.Id == orderId);

            if (result== null)
            {
                throw new Exception("შეკვეთა ვერ მოიძებნა.");
            }

                OrderDelivery.Orders.Remove(result);
                await OrderDelivery.SaveChangesAsync();
                return "შეკვეთა წარმატებით წაიშალა.";
            

        }



        public async Task<string> DeleteParsel(Guid parselId)
        {
            var parsel = await OrderDelivery.Parsels.FirstOrDefaultAsync(p => p.Id == parselId);

            if (parsel == null)
            {
                throw new Exception("ამანათი ვერ მოიძებნა.");
            }

            OrderDelivery.Parsels.Remove(parsel);
            await OrderDelivery.SaveChangesAsync();
            return "ამანათი წარმატებით წაიშალა.";
        }


        public async Task<List<OrderDto>> GetAllOrders()
        {
            var result = await OrderDelivery.Orders.Include(x => x.User).Select(x => new OrderDto
            {
                Id = x.Id,
                Pernumber = x.Pernumber,
                Customername = x.Customername,
                Customerphone = x.Customerphone,
                Deliveryaddress = x.Deliveryaddress,
                Productname = x.Productname,
                Quantity = x.Quantity,
                Unitprice = x.Unitprice,
                Comments = x.Comments,
                Productprice = x.Productprice,
                Deliveryprice = x.Deliveryprice,
                Fullprice = x.Fullprice,
                Username = x.User.Username


            }).ToListAsync();  ;


            if (!result.Any())
            {
                throw new Exception("შეკვეთები ვერ მოიძებნა.");
            }
            else
            {
               return result;
            }

        }



        public async Task<List<Parsel>> GetAllParsel()
        {
            var result = await OrderDelivery.Parsels.Include(x => x.Order).ToListAsync();

            var parselDtos = result.Select(p => new Parsel
            {
                Id = p.Id,
                Orderid = p.Orderid,
                Parselcode = p.Parselcode,
                Order = new Order
                {
                    Id = p.Order.Id,
                    Pernumber = p.Order.Pernumber,
                    Customername = p.Order.Customername,
                    Customerphone = p.Order.Customerphone,
                    Deliveryaddress = p.Order.Deliveryaddress,
                    Productname = p.Order.Productname,
                    Quantity = p.Order.Quantity,
                    Unitprice = p.Order.Unitprice,
                    Comments = p.Order.Comments,
                    Productprice = p.Order.Productprice,
                    Deliveryprice = p.Order.Deliveryprice,
                    Fullprice = p.Order.Fullprice
                }
            }).ToList();

            if (!parselDtos.Any())
            {
                throw new Exception("ამანათები ვერ მოიძებნა.");
            }
            else
            {
                return result;
            }
        }



        public async Task<List<OwnOrdersDto>> GetOwnOrders(Guid userId)
        {

            var orders = await OrderDelivery.Userparsels.
                Where(up => up.Parsel.Order.User.Id==userId).Select(up => new OwnOrdersDto
         {
             parselcode = up.Parsel.Parselcode,
             Pernumber = up.Parsel.Order.Pernumber,
             Customername = up.Parsel.Order.Customername,
             Customerphone = up.Parsel.Order.Customerphone,
             Deliveryaddress = up.Parsel.Order.Deliveryaddress,
             Productname = up.Parsel.Order.Productname,
             Quantity = up.Parsel.Order.Quantity,
             Unitprice = up.Parsel.Order.Unitprice,
             Comments = up.Parsel.Order.Comments,
             Productprice = up.Parsel.Order.Productprice,
             Deliveryprice = up.Parsel.Order.Deliveryprice,
             Fullprice = up.Parsel.Order.Fullprice
         }).ToListAsync();

            //Include(p => p.Parsel).ThenInclude(o=>o.Order).ThenInclude(o=>o.User)


            if (!orders.Any())
            {
                throw new Exception("შეკვეთები ვერ მოიძებნა.");
            }
            else
            {
                return orders;
            }
        }



        public async Task<string> UpdateParselCodeByAdmin(Guid parselId, int newCode)
        {
            var parsel = await OrderDelivery.Parsels.FirstOrDefaultAsync(p => p.Id == parselId);

            if (parsel == null)
            {
                throw new Exception("ამანათი ვერ მოიძებნა.");
            }

            parsel.Parselcode = newCode;
            await OrderDelivery.SaveChangesAsync();
            return "ამანათის კოდი წარმატებით განახლდა.";
        }


        private async Task<(double? Lat, double? Lon)> GeocodeAddress(string deliveryAddress)
        {
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "DeliveryProject/1.0");

                var url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(deliveryAddress)}&format=json&limit=1";
                var response = await client.GetStringAsync(url);

                var results = JsonSerializer.Deserialize<List<NominatimResult>>(response);

                if (results != null && results.Any())
                {
                    double lat = double.Parse(results[0].Lat, CultureInfo.InvariantCulture);
                    double lon = double.Parse(results[0].Lon, CultureInfo.InvariantCulture);
                    return (lat, lon);
                }

                return (null, null);
            }
            catch
            {
                return (null, null); 
            }
        }

        private class NominatimResult
        {
            [JsonPropertyName("lat")]
            public string Lat { get; set; } = null!;

            [JsonPropertyName("lon")]
            public string Lon { get; set; } = null!;
        }

        public async Task<UploadOrderResult> UploadOrders(IFormFile? file, Guid userId)
        {
            if (file is null || file.Length == 0)
            {
                return new UploadOrderResult
                {
                    Success = false,
                    Errors = new List<string> { "გთხოვთ, აირჩიოთ ფაილი." }
                };
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (extension != ".xlsx")
            {
                return new UploadOrderResult
                {
                    Success = false,
                    Errors = new List<string>
            {
                "ფაილის ფორმატი არასწორია. შესაძლებელია მხოლოდ Excel (.xlsx) ფაილის ატვირთვა."
            }
                };
            }

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;

            XLWorkbook workbook;
            try
            {
                workbook = new XLWorkbook(stream);
            }
            catch (Exception)
            {
                return new UploadOrderResult
                {
                    Success = false,
                    Errors = new List<string> { "ფაილი დაზიანებულია ან არ არის სწორი Excel (.xlsx) დოკუმენტი." }
                };
            }

            using var workbookScope = workbook;


            if (workbook.Worksheets.Count == 0)
            {
                return new UploadOrderResult
                {
                    Success = false,
                    Errors = new List<string> { "ფაილში sheet ვერ მოიძებნა." }
                };
            }

            var worksheet = workbook.Worksheet(1);
            var rows = worksheet.RowsUsed().Skip(1);

            var orders = new List<Order>(); 
            var errors = new List<string>();

            int rowNumber = 2;

            foreach (var row in rows)
            {

                if (!int.TryParse(row.Cell(6).GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int quantity))
                {
                    errors.Add($"სტრიქონი {rowNumber}: პროდუქტის რაოდენობა უნდა იყოს მთელი რიცხვი.");
                    rowNumber++;
                    continue;
                }

                if (!decimal.TryParse(row.Cell(7).GetString(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal unitPrice))
                {
                    errors.Add($"სტრიქონი {rowNumber}: ერთეულის ფასი უნდა იყოს 1-ზე მეტი");
                    rowNumber++;
                    continue;
                }


                if (!decimal.TryParse(row.Cell(9).GetString(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal productPrice))
                {
                    errors.Add($"სტრიქონი {rowNumber}: პროდუქტის ფასი უნდა იყოს 1-ზე მეტი");
                    rowNumber++;
                    continue;
                }

                if (!decimal.TryParse(row.Cell(10).GetString(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal deliveryPrice))
                {
                    errors.Add($"სტრიქონი {rowNumber}: მიტანი ფასი უნდა იყოს 1-ზე მეტი");
                    rowNumber++;
                    continue;
                }

                if (!decimal.TryParse(row.Cell(11).GetString(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal fullPrice))
                {
                    errors.Add($"სტრიქონი {rowNumber}: სრული ფასი უნდა იყოს 1-ზე მეტი.");
                    rowNumber++;
                    continue;
                }

                var dto = new AddOrderDto
                {
                    Pernumber = row.Cell(1).GetString(),
                    Customername = row.Cell(2).GetString(),
                    Customerphone = row.Cell(3).GetString(),
                    Deliveryaddress = row.Cell(4).GetString(),
                    Productname = row.Cell(5).GetString(),
                    Quantity = quantity,
                    Unitprice = unitPrice,
                    Comments = row.Cell(8).GetString(),
                    Productprice = productPrice,
                    Deliveryprice = deliveryPrice,
                    Fullprice = fullPrice,
                    Userid = userId
                };

                var validationResults = new List<ValidationResult>();

                bool isValid = Validator.TryValidateObject(
                    dto, new ValidationContext(dto),
                    validationResults,
                    true);

                if (!isValid)
                {
                    foreach (var validationResult in validationResults)
                    {
                        errors.Add($"სტრიქონი {rowNumber}: {validationResult.ErrorMessage}");
                    }

                    rowNumber++;
                    continue;
                }

                decimal calculatedFullPrice = dto.Productprice + dto.Deliveryprice;

                if (dto.Fullprice != calculatedFullPrice)
                {
                    errors.Add($"სტრიქონი {rowNumber}: ჯამური ფასი არასწორია.");
                    rowNumber++;
                    continue;
                }

                var (lat, lon) = await GeocodeAddress(dto.Deliveryaddress);  
                await Task.Delay(1000);

                if (lat == null || lon == null)
                {
                    errors.Add($"სტრიქონი {rowNumber}: მისამართი ვერ მოიძებნა რუკაზე — გადაამოწმეთ მისამართის სისწორე.");
                    rowNumber++;
                    continue;
                }

                orders.Add(new Order
                {
                    Pernumber = dto.Pernumber,
                    Customername = dto.Customername,
                    Customerphone = dto.Customerphone,
                    Deliveryaddress = dto.Deliveryaddress,
                    Productname = dto.Productname,
                    Quantity = dto.Quantity,
                    Unitprice = dto.Unitprice,
                    Comments = dto.Comments,
                    Productprice = dto.Productprice,
                    Deliveryprice = dto.Deliveryprice,
                    Fullprice = dto.Fullprice,
                    Userid = dto.Userid,
                    Latitude = lat,       
                    Longitude = lon
                });
              

                rowNumber++;
            }

            if (errors.Any())
            {
                return new UploadOrderResult
                {
                    Success = false,
                    Errors = errors
                };
            }

            await OrderDelivery.Orders.AddRangeAsync(orders);
            await OrderDelivery.SaveChangesAsync();

            return new UploadOrderResult
            {
                Success = true
            };
        }

        public async Task<string> UpdateOrderAddress(UpdateAddressDto updateAddressDto, Guid userId)
        {
            var order = await OrderDelivery.Orders.FirstOrDefaultAsync(x => x.Pernumber == updateAddressDto.Pernumber);

            if (order == null)
            {
                throw new UserException("მომხმარებელი ასეთი პირადი ნომრით არ არსებობს");
            }

            if (order.Userid != userId)
            {
                throw new TokenException("არ გაქვთ უფლება ამ შეკვეთის მისამართის შეცვლაზე.");
            }


            var (lat, lon) = await GeocodeAddress(updateAddressDto.Deliveryaddress);

            if (lat == null || lon == null)
            {
                throw new AddressException("მისამართი ვერ მოიძებნა რუკაზე — გადაამოწმეთ მისამართის სისწორე.");
            }

            order.Deliveryaddress = updateAddressDto.Deliveryaddress;
            order.Latitude = lat;
            order.Longitude = lon;

            await OrderDelivery.SaveChangesAsync();

            var deliveryAgentUserId = await OrderDelivery.Userparsels.Where(y => y.Parsel.Orderid == order.Id).Select(y => y.Userid).FirstOrDefaultAsync();

            if (deliveryAgentUserId != Guid.Empty)
            {
                await hubContext.Clients.Group(deliveryAgentUserId.ToString()).SendAsync("AddressUpdated", new
                    {
                        OrderId = order.Id,
                        NewAddress = order.Deliveryaddress,
                        Latitude = order.Latitude,
                        Longitude = order.Longitude
                    });
            }

            return "მისამართი წარმატებით განახლდა.";


        }

















    }
}
