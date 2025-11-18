
namespace API.CustomExceptions
{
    
    // kun perimme Exception--luokan, voimme heittää tämän custom poikkeuksen servicesta controllerille tarvittaessa
    public class UserRegistrationException : Exception
    {

        // perus ctor ilman parametrejä
        public UserRegistrationException() { }

        // ctor, jolle voi antaa tarkemman viestin parametrina
        public UserRegistrationException(string message)
            : base(message) { }
    }
}