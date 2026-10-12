using DeliveryProject.DTO;
using DeliveryProject.Exceptions;
using DeliveryProject.Models;
using DeliveryProject.Services.Iservices;
using Microsoft.EntityFrameworkCore;

namespace DeliveryProject.Services
{
    public class UserParselService : IUserParselService
    {
        private readonly DeliveryContext deliveryContext;


        public UserParselService(DeliveryContext deliveryContext)
        {
            this.deliveryContext = deliveryContext;
        }

        public async Task<string> AddUserParsel(AddUserParselDto addUserParselDto)
        {

            var user= await deliveryContext.Users.FirstOrDefaultAsync(u => u.Id == addUserParselDto.Userid);

            if (user == null)
            {
                throw new UserException("DeliveryAgent ვერ მოიძებნა");
            }

            var parsel = await deliveryContext.Parsels.FirstOrDefaultAsync(p => p.Id == addUserParselDto.Parselid);

            if (parsel == null)
            {
                throw new OrderException("ამანათი ვერ მოიძებნა");
            }


            var sameDeliveryAgent = await deliveryContext.Userparsels.FirstOrDefaultAsync(up => up.Userid == addUserParselDto.Userid && up.Parselid == addUserParselDto.Parselid);
            if (sameDeliveryAgent != null)
            {
                throw new OrderException("ეს ამანათი უკვე მინიჭებული აქვს ამ კურიერს.");
            }

            var defaultStatusId = await deliveryContext.Parselstatuses.Where(s => s.Statusname == "მისატანი").Select(s => s.Id).FirstOrDefaultAsync();

            var userParsel = new Userparsel
            {
                Userid = addUserParselDto.Userid,
                Parselid = addUserParselDto.Parselid,
                Statusid = defaultStatusId
            };

            await deliveryContext.Userparsels.AddAsync(userParsel);
            await deliveryContext.SaveChangesAsync();

            return "ამანათი წარმატებით მიენიჭა კურიერს.";
        }



        public async Task<string> DeleteUserParsel(Guid userId, Guid parselId)
        {
            var user = await deliveryContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new UserException("მომხმარებელი ვერ მოიძებნა");
            }

            var userParsel = await deliveryContext.Userparsels
                .FirstOrDefaultAsync(up => up.Userid == userId && up.Parselid == parselId);

            if (userParsel == null)
            {
                throw new OrderException("ამანათი ვერ მოიძებნა.");
            }

            deliveryContext.Userparsels.Remove(userParsel);
            await deliveryContext.SaveChangesAsync();
            return "ამანათი წარმატებით წაიშალა.";
        }



        public async Task<List<UserParselDto>> GetAllUserParsel()
        {
            var userparsel= await deliveryContext.Userparsels.Select(up => new UserParselDto
             {
               Id = up.Id,
                Userid = up.Userid,
                Parselid = up.Parselid,
                UserName = up.User.Username,
                ParselCode = up.Parsel.Parselcode,
                Statusid = up.Statusid,
                Status = up.Status.Statusname,
                Createdat = up.Createdat

            }).ToListAsync();

            if (userparsel == null)
            {
                throw new Exception("სია ცარიელია");
            }

            return userparsel;

            
        }

        public async Task<List<ParselMapDto>> GetOwnParsel(Guid userId)
        {
           var parsel = await deliveryContext.Userparsels.Where(u => u.Userid == userId).Select(x => new ParselMapDto
           {
               OrderId = x.Parsel.Orderid,
               Latitude = x.Parsel.Order.Latitude,
               Longitude = x.Parsel.Order.Longitude,
               Address = x.Parsel.Order.Deliveryaddress,
               status = x.Status.Statusname
           }).ToListAsync();

            if (!parsel.Any())
            {
                throw new OrderException("ამანათი ვერ მოიძებნა");
            }

            return parsel;
        }



        public async Task<string> UpdateParselStatus(int parselCode, string newStatusName, Guid userId, bool isAdmin)
        {
            var parsel = await deliveryContext.Parsels
                .FirstOrDefaultAsync(p => p.Parselcode == parselCode);

            if (parsel == null)
                throw new OrderException("ამანათი ვერ მოიძებნა");

            var userParsel = await deliveryContext.Userparsels
                .FirstOrDefaultAsync(up => up.Parselid == parsel.Id && (isAdmin || up.Userid == userId));

            if (userParsel == null)
                throw new OrderException("ამანათი ვერ მოიძებნა ან არ გეკუთვნით");

            var status = await deliveryContext.Parselstatuses
                .FirstOrDefaultAsync(s => s.Statusname == newStatusName);

            if (status == null)
                throw new OrderException("მითითებული სტატუსი ვერ მოიძებნა");

            userParsel.Statusid = status.Id;
            await deliveryContext.SaveChangesAsync();

            return "სტატუსი წარმატებით განახლდა.";
        }

        public async Task<string> UpdateUserParsel(Guid newUserId, Guid parselId)
        {
            var user = await deliveryContext.Users.FirstOrDefaultAsync(x => x.Id == newUserId);

            if (user == null)
            {
                throw new UserException("DeliveryAgent ვერ მოიძებნა");
            }

            var parsel = await deliveryContext.Userparsels.FirstOrDefaultAsync(y => y.Parselid == parselId);

            if (parsel == null)
            {
                throw new OrderException("ეს ამანათი არავისზეა მინიჭებული.");
            }

            if (parsel.Userid == newUserId)
            {
                throw new OrderException("ეს ამანათი უკვე მინიჭებული აქვს ამ კურიერს.");
            }

            parsel.Userid = newUserId;
            await deliveryContext.SaveChangesAsync();

            return "ამანათი წარმატებით გადაეცა ახალ კურიერს.";


        }


        public async Task<string> GetOrderPhoneForParselAsync(Guid parselId, Guid userId, bool isAdmin)
        {
            var parsel = await deliveryContext.Parsels
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.Id == parselId);

            if (parsel == null)
                throw new OrderException("ამანათი ვერ მოიძებნა");

            if (!isAdmin)
            {
                var isAssigned = await deliveryContext.Userparsels
                    .AnyAsync(up => up.Parselid == parselId && up.Userid == userId);

                if (!isAssigned)
                    throw new UserException("თქვენ არ ხართ ამ ამანათზე მიმაგრებული კურიერი");
            }

            return parsel.Order.Customerphone;
        }

        public async Task<List<string>> GetPhoneNumbersForDeliverableParselsAsync(Guid userId)
        {
            var phones = await deliveryContext.Userparsels
                .Where(up => up.Userid == userId && up.Status.Statusname == "მისატანი")
                .Select(up => up.Parsel.Order.Customerphone)
                .Distinct()
                .ToListAsync();

            if (!phones.Any())
                throw new OrderException("თქვენ არ გაქვთ მისატანი ამანათები");

            return phones;
        }



        public async Task<string> GetOwnedParselPhoneAsync(int parselCode, Guid userId)
        {
            var parsel = await deliveryContext.Parsels
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.Parselcode == parselCode);

            if (parsel == null)
                throw new OrderException("ამანათი ვერ მოიძებნა");

            var isOwner = await deliveryContext.Userparsels
                .AnyAsync(up => up.Parselid == parsel.Id && up.Userid == userId);

            if (!isOwner)
                throw new UserException("ეს ამანათი არ გეკუთვნით");

            return parsel.Order.Customerphone;
        }






    }
}
