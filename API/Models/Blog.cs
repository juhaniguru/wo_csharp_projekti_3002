

namespace API.Models
{
    public class Blog
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }

        // tämä property toimittaa foreign keyn virkaa
        public int AppUserId { get; set; } 
    
        // tämä on ns. navigation property. Koska se viittaa toiseen modeliin, EF Core osaa rakentaa relaation taulujen välille
        // kun myöhemmin luomme uutta blogipostausta, käytämme AppUserId:tä, emme Owneria (koska Owner-columnia ei oikeasti ole tietokannassa)
        public AppUser? Owner { get; set; } = null;

        public ICollection<Tag> Tags { get; set; } = new List<Tag>();

    }
}