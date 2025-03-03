﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace onnaMUD.BaseClasses
{
    public class Thing
    {
        //[BsonIgnore, JsonIgnore]
        //public ThingType ThingType { get; set; }
        public Guid Id { get; set; }// guid ID for this object
        public string Name { get; set; } = "NewThing";//character first name (that we match against) or 'ivory-hilted jambiya' (that we don't match against, for items/objects)

    }

    public enum ThingType
    {
        Item = 0,//all other objects/gear/equipment/coins/gems/misc other stuff that can be picked up and carried around
        Character = 1,//all player/npc characters
        Mob = 2,//all other generic mobs that there can be more than 1 of (monsters, guards, etc)
        RoomObject = 3//doors, bridges, portals, gates, etc. all perm/semi-perm objects in a room that can't (usually) be picked up and carried

    }
}
