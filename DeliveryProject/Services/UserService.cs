using AutoMapper;
using DeliveryProject.DTO;
using DeliveryProject.Exceptions;
using DeliveryProject.Models;
using DeliveryProject.PasswordHash;
using DeliveryProject.Services.Iservices;
using DeliveryProject.Tokens;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DeliveryProject.Services
{
    public class UserService:IUserService
    {
        private readonly DeliveryContext deliveryContext;
        private readonly IMapper mapper;
        private readonly RefreshTokenGenerator refreshTokenGenerator;
        private readonly IConfiguration config;

        public UserService(DeliveryContext deliveryContext, RefreshTokenGenerator refreshTokenGenerator, IConfiguration config)
        {
            this.deliveryContext = deliveryContext;
            this.mapper = mapper;
            this.refreshTokenGenerator = refreshTokenGenerator;
            this.config = config;
        }

        public async Task<List<UserDto>> GetAllUsers()
        {

            var result = await deliveryContext.Users.Include(x => x.Role).Select(x => new UserDto
          {
              Id = x.Id,
              Username = x.Username,
              Email = x.Email,
              Phonenumber = x.Phonenumber,
              Createdat = x.Createdat,
              Rolename = x.Role.Rolename
          }).ToListAsync();

            if (!result.Any())
            {
                throw new UserException("მომხმარებელი არ მოიძებნა");
            }
            else
            {
                return result;
            }
        }


        public async Task<string> DeleteOwnAccount(Guid id, LoginUserDto loginUserDto)
        {
            var user = await deliveryContext.Users.FirstOrDefaultAsync(x => x.Id == id);


            if (user == null)
            {
                throw new UserException("მომხმარებელი ვერ მოიძებნა");
            }

            if (user.Email != loginUserDto.Email)
            {
                throw new EmailException("Email არასწორია");
            }

            bool isValid = PasswordHashClass.VerifyPassword(loginUserDto.Password, user.Userpassword);



            if (!isValid)
            {
                throw new PasswordException("პაროლი არასწორია");
            }
            deliveryContext.Users.Remove(user);
            await deliveryContext.SaveChangesAsync();
            return "მომხმარებელი წარმატებით წაიშალა";
        }


       

        public async Task<string> UpdateOwnPassword(Guid id, ChangePasswordDto changePasswordDto)
        {
            var user = await deliveryContext.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user==null)
            {
                throw new UserException("მომხმარებელი ვერ მოიძებნა");
            }


            bool isValid = PasswordHashClass.VerifyPassword(changePasswordDto.OldPassword, user.Userpassword);


            if (!isValid)
            {
                throw new OldPasswordException("ძველი პაროლი არასწორია");
            }

            else if (changePasswordDto.OldPassword == changePasswordDto.NewPassword)
            {
                throw new SamePasswordException("ახალი პაროლი უნდა იყოს განსხვავებული ძველი პაროლისგან");

            }

            user.Userpassword = PasswordHashClass.HashPassword(changePasswordDto.NewPassword);

            await deliveryContext.SaveChangesAsync();




            return "პაროლი წარმატებით შეიცვალა";

        }

        public async Task<BothTokenDto> UserLogin(string email, string password)
        {
            //string hash = BCrypt.Net.BCrypt.HashPassword("Admin123!");
            //Console.WriteLine(hash);


            var result = await deliveryContext.Users.Include(x => x.Role).FirstOrDefaultAsync(x => x.Email == email);


            if (result == null)
            {
                throw new EmailException("მომხმარებელი ასეთი იმეილით არ არსებობს");
            }


            bool isValid = PasswordHashClass.VerifyPassword(password, result.Userpassword);

            if (!isValid)
            {
                throw new PasswordException("პაროლი არასწორია");
            }

            var claims = new List<Claim>
            {
               new Claim(ClaimTypes.NameIdentifier, result.Id.ToString()),
               new Claim(ClaimTypes.Email, result.Email),
               //new Claim(ClaimTypes.Role, result.Userrole.Role.Rolename)
            };

            if (result.Role != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, result.Role.Rolename));

            }

            var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(config["JwtSettings:Key"])
                );

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: config["JwtSettings:Issuer"],
                audience: config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: creds
            );

            string accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            string refreshTokenValue = refreshTokenGenerator.GenerateRefreshToken();

            var refreshToken = new Refreshtoken
            {
                Id = Guid.NewGuid(),
                Userid = result.Id,
                Token = refreshTokenValue,
                Expiresat = DateTime.Now.AddHours(1),
                Createdat = DateTime.Now
            };

            deliveryContext.Refreshtokens.Add(refreshToken);
            await deliveryContext.SaveChangesAsync();

            return new BothTokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue
            };
        }


        public async Task<BothTokenDto> ModifyToken(RefreshTokenRequest refreshTokenRequest)
        {
            var storedToken = await deliveryContext.Refreshtokens.FirstOrDefaultAsync(x => x.Token == refreshTokenRequest.RefreshToken);

            if (storedToken == null)
            {
                throw new TokenException("ტოკენი არავალიდურია");
            }

            if (storedToken.Revokedat != null || storedToken.Expiresat <= DateTime.Now)
            {
                if (storedToken.Revokedat != null)
                {
                    await RevokeAllActiveTokensAsync(storedToken.Userid);
                }

                throw new RefreshTokenException("ეს რეფრეშ ტოკენი არ არის ვალიდური");
            }

            var user = await deliveryContext.Users.Include(x => x.Role).FirstOrDefaultAsync(x => x.Id == storedToken.Userid);



            if (user == null)
            {
                throw new UserException("მომხმარებელი არ არსებობს");
            }

            storedToken.Revokedat = DateTime.Now;


            var claims = new List<Claim>
            {
                  new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                   new Claim(ClaimTypes.Email, user.Email)
            };


            if (user.Role != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, user.Role.Rolename));



            }


            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(config["JwtSettings:Key"])
            );


            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );


            var token = new JwtSecurityToken(
                issuer: config["JwtSettings:Issuer"],
                audience: config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: creds
            );


            string accessToken = new JwtSecurityTokenHandler()
                .WriteToken(token);



            string refreshTokenValue = refreshTokenGenerator.GenerateRefreshToken();


            var newRefreshToken = new Refreshtoken
            {
                Id = Guid.NewGuid(),
                Userid = user.Id,
                Token = refreshTokenValue,
                Expiresat = DateTime.Now.AddHours(1),
                Createdat = DateTime.Now
            };


            deliveryContext.Refreshtokens.Add(newRefreshToken);

            await deliveryContext.SaveChangesAsync();


            return new BothTokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue
            };


        }



        public async Task RevokeAllActiveTokensAsync(Guid userId)
        {
            {
                var activeTokens = await deliveryContext.Refreshtokens.Where(x => x.Userid == userId && x.Revokedat == null).ToListAsync();


                foreach (var token in activeTokens)
                {
                    token.Revokedat = DateTime.Now;
                }

                await deliveryContext.SaveChangesAsync();
            }
        }


        public async  Task<string> UserRegistration(AddUserDto addUserDto)
        {

            var SameUser = await deliveryContext.Users.FirstOrDefaultAsync(x => x.Email == addUserDto.Email);


            if (SameUser != null)
            {
                throw new UserException("ასეთი იმეილით მომხმარებელი უკვე არსებობს");
            }


            var isrole= await deliveryContext.Roles.FirstOrDefaultAsync(y=> y.Rolename == addUserDto.Rolename);

            if (isrole == null)
            {
                throw new RoleException("მითითებული როლი არ არსებობს.");
            }


            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = addUserDto.Username,
                Email = addUserDto.Email,
                Userpassword = PasswordHashClass.HashPassword(addUserDto.Userpassword),
                Phonenumber = addUserDto.Phonenumber,

                Roleid =isrole.Id,

                Createdat = DateTime.Now
            };

       
       

            var result = await deliveryContext.Users.AddAsync(user);

            await deliveryContext.SaveChangesAsync();
            return "მომხმარებელი წარმატებით დაემატა";
        }



        public async Task<List<Role>> GetAllRoles()
        {
            var result  = await deliveryContext.Roles.ToListAsync();    

            if (!result.Any())
            {
                throw new RoleException("როლი ვერ მოიძებნა");
            }
            else
            {
                return result;
            }

        }


        public async Task<string> AddRoleByAdmin( AddRoleDto addRoleDto)
        {
          
            var samerole = await deliveryContext.Roles.FirstOrDefaultAsync(x => x.Rolename == addRoleDto.Rolename);

            if (samerole == null)
            {
                throw new RoleException("მითითებული როლი არ არსებობს");
            }

            Role role = mapper.Map<Role>(addRoleDto);
            role.Id = Guid.NewGuid();

             var result = await deliveryContext.Roles.AddAsync(role);

            await deliveryContext.SaveChangesAsync();

            return "როლი წარმატებით დაემატა მომხმარებელს";
        }

    





    }
}
