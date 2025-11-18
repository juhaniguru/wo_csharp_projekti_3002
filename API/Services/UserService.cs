using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using API.CustomExceptions;
using API.Data;
using API.Dtos;
using API.Interfaces;
using API.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class UserService(DataContext _repository, ITokenTool _tokenTool) : IUserService
    {
        public async Task<IEnumerable<AppUser>> GetAll()
        {
            var users = await _repository.Users.AsNoTracking().ToListAsync();
            return users;
        }

        public async Task<AppUser> GetById(int id)
        {
            var user = await _repository.Users.FirstAsync(u => u.Id == id);
            return user;

            
        }

        public async Task<AppUser?> GetByUserName(string username)
        {
            
            // Juhani => juhani
            // resumè => resume
            // LATIN_SWEDISH_CI => CaseInsensitive
            //_repository.Users.FirstAsync();

            // SELECT * FROM users WHERE LOWER(users.UserName) = LOWER('Juhani') LIMIT 1;
            return await _repository.Users.FirstOrDefaultAsync(u => u.UserName.ToLower() == username.ToLower());
        }

        public async Task<string> Login(string username, string password)
        {
            var user = await GetByUserName(username);
            if (user == null)
            {
                throw new NotFoundException("user not found");
            }

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedPassword = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            bool passwordsMatch = true;
            for (int i = 0; i < computedPassword.Length; i++)
            {
                // jos salasanat eivät täsmää, palautetaan null
                if (computedPassword[i] != user.HashedPassword[i])
                {
                    // ns. early exitin vuoksi hyökkääjä voi käytännössä laskea suoritusajoista,
                    // kuinka monta bytea on mennyt oikein ja arvata oikean salasanan tavu tavulta
                    //throw new NotFoundException("user not found");
                    passwordsMatch = false;
                }
            }

            // dotnet user-secrets set "TokenKey" "oma salainen salkjdsdffdsdfsdfsdfslkdjsflf23443324lk324j34l2kj32408423lkj234lk"

            if (!passwordsMatch)
            {
                throw new NotFoundException();
            }

            return _tokenTool.CreateToken(user);
            
               
        }

        public async Task<AppUser> Register(string username, string password)
        {
            
            var existingUser = await GetByUserName(username);
            if(existingUser != null)
            {
                
                throw new UserRegistrationException("username must be unique");
            }

            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
                UserName = username,
                Role = "user",
                // hmac.Key on randomi salt, jonka loit automaattisesti
                // kun teit hmac-instanssin
                PasswordSalt = hmac.Key,
                // ComputeHash tekee hashin selkokielisestä salasanasta
                HashedPassword = hmac.ComputeHash(
                    // Encoding.UTF8.GetBytes?
                    // req.Password on string-tyyypiä, mutta ComputeHash
                    // haluaa parametring byte[]-arrayna
                    // GetBytes siis palauttaa merkkijonosta byte[] arrayn.

                    Encoding.UTF8.GetBytes(password)
                )

            };
            // tämä tekee käytännössä insertin Users-tietokantatauluun
            // koska olemme määrittäneet DbSetiksi Usersin, pystymme käyttämään sitä näin
            await _repository.Users.AddAsync(user);
            // insert pitää vahvistaa, jotta rivi oikeasti tallennetaan
            await _repository.SaveChangesAsync();

            return user;
        }
    }
}