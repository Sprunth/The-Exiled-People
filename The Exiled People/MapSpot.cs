using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;
using The_Exiled_People.TileContents;

namespace The_Exiled_People
{
    class MapSpot
    {
        private Floor _floor;
        private Person _person;

        public MapSpot(FloorType ft, PersonProfession pp)
        {
            _floor = new Floor(ft);
            _person = new Person(pp);
        }

        public FloorType FloorType { get { return _floor.Type; } }
        public PersonProfession PersonProfession { get { return _person.Type; } }

        public override string ToString()
        {
            return _floor.Type.ToString();
        }
    }
}
