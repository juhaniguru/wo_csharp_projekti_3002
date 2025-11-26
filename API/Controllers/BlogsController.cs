using System.IdentityModel.Tokens.Jwt;
using API.CustomExceptions;
using API.Dtos;
using API.Interfaces;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogsController(IBlogService _blogService, IMapper _mapper) : ControllerBase
    {
        [HttpDelete("{id}")]
        public async Task<ActionResult> RemoveBlog(int id)
        {
            try
            {
                await _blogService.Remove(id);
                return NoContent();

            } catch(NotFoundException e)
            {
                return NotFound(new
                {
                    Title = "error removing blog",
                    Detail = e.Message    

                });
            } catch(Exception e)
            {
                return Problem(title: "error removig blog", detail: e.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<BlogDto>> UpdateBlog(int id, UpdateBlogReq requestData)
        {
            try
            {
                var blog = await _blogService.Edit(id, requestData);
                return Ok(
                    _mapper.Map<BlogDto>(blog)
                );
            } catch(NotFoundException e)
            {
                return NotFound(e.Message);
            } catch(Exception e)
            {
                return Problem(title: "error updating blog", detail: e.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BlogDto>> GetById(int id)
        {
            try
            {
                var blog = await _blogService.GetById(id);
                return Ok(
                    _mapper.Map<BlogDto>(blog)
                );
            } catch(NotFoundException e)
            {
                return NotFound(e.Message);
            } catch(Exception e)
            {
                return Problem(title: "error fetching blog", detail: e.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<BlogDto>> CreateBlog(CreateBlogReq requestData)
        {
            try
            {

                var loggedInUser = HttpContext.Items["loggedInUser"] as AppUser;

                var blog = await _blogService.Create(requestData, loggedInUser!.Id);
                
                // return Ok(blog) aiheutti ongelman oppitunnin lopussa
                // action ei valita väärästä tietotyypistä returnissa (CreateBlogin pitäisi palauttaa BlogDto, mutta blog on tietotyyppiä Blog)
                // koska Blog -> BlogDto -tietotyyppimuutos onnistuu implisiittisesti (eli .net tekee sen itse 'konepellin alla')
                // Kun pääsemme OK()-kutsuun, JSONserializer yrittää muuttaa palautettavan objektin datat jsoniksi ja tästä alkaa kehä
                // Blog-modelissa on Ownern, joka on tyyppiä AppUser ja AppUser-modelissa on Blogs, joka on lista Blog-modeleita

                

                // ongelmaa ei ole, kun käytetään automapperia ja muutetaan Blog -> BlogDto-tyypiksi
                // Miksi automapperin käyttäminen (Blog->BlogDto) ei aiheuta ongelmaa?
                // Koska BlogDto:ssa on Owner, joka on UserDto-tyyppiä, mutta UserDto-luokassa ei ole propertya Blogs List<BlogDto>

                // virheellinen koodi kiertää siis kehää: 

                // Blog -> Owner (AppUser) -> Blogs (List<Blog>)..ja kehä alkaa alusta

                // mutta _mapper.Map<BlogDto>(blog) ei kierrä kehää, vaan

                // BlogDto -> UserDto -> Loppu

                // #################### MIKSI TÄMÄ TAPAHTUU NYT, KUN MIDDLEWARE ON KÄYTÖSSÄ? ################################## //

                // Koska käytämme GetById-metodia middlewaressa, joka palauttaa AppUser-instanssin ja tärkeinpänä, koska EFCoressa on ns. Tracking päällä oletuksena
                // Tracking tarkoittaa sitä, että EFCore yhdessä JSONSerializerin kanssa yrittää populoida relatiivisiä objekteja automaattisesti (kuten yo. esimerkissä on käyty läpi)
                // EFCore on tässä avainroolissa, koska Tracking on automaattisesti päällä ja aiheuttaa autom. populoinnin, mikä aiehuttaa ikiluupin JSONSerializerissa

                /*
                
                UserServicen GetById näyttää tältä:

                public async Task<AppUser> GetById(int id)
                {
                    var user = await _repository.Users.FirstAsync(u => u.Id == id);
                    return user;

                    
                }

                // Toinen vaihtoehto korjata ongelma olisi ottaa Tracking pois päältä yo servicen metodissa näin:
                // Huom. AsNoTracking()-lisätty 

                public async Task<AppUser> GetById(int id)
                {
                    var user = await _repository.Users.AsNoTracking().FirstAsync(u => u.Id == id);
                    return user;

                    
                }
                
                */

                // AsNoTracking() ottaa trackingin pois päältä, jolloin JSONSerializer ei yritä populoida automaattisesti relatiivista dataa, ja ketju katkeaa

                // OPETUS: MUISTA PALAUTTAA DTO-OBJEKTEJA CONTROLLERIN ACTIONEISTA



                //return Ok(blog);
                return _mapper.Map<BlogDto>(blog);
            } catch(Exception e)
            {
                return Problem(title: "Error creating blog", detail: e.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlogDto>>> GetAll()
        {
            try
            {
                var blogs = await _blogService.GetAll();
                return Ok(
                    _mapper.Map<IEnumerable<BlogDto>>(blogs)
                );  
            } catch(Exception e)
            {
                return Problem(title: "Error fetching blogs", detail: e.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}