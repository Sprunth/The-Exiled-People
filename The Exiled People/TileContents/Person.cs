using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Exiled_People.TileContents
{
    class Person
    {
        public PersonProfession Type { get; set; }

        public Person(PersonProfession pp)
        {
            Type = pp;
        }
    }

    public enum PersonProfession { None, Baby, Kid, CoffeeMan, Guard}
}
