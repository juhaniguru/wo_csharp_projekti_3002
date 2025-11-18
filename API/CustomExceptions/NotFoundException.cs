
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.CustomExceptions
{
    
    // kun perimme Exception--luokan, voimme heittää tämän custom poikkeuksen servicesta controllerille tarvittaessa
    public class NotFoundException : Exception
    {

        // perus ctor ilman parametrejä
        public NotFoundException() { }

        // ctor, jolle voi antaa tarkemman viestin parametrina
        public NotFoundException(string message)
            : base(message) { }
    }
}