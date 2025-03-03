using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace onnaMUD.BaseClasses
{
    //this is the base for ALL characters, player AND NPC ?
    public class Character : Thing
    {
        //[Key]
//        public Guid Id { get; set; }//guid ID for this character
        public Guid AccountID { get; set; }//account id for the player who has this character, .Empty for npc?
        //public int CharacterID { get; set; }
//        public string CharName { get; set; } = "";
        public string Server { get; set; }//which server this character is made on
        public Guid CurrentRoom { get; set; }
        public Races Race { get; set; }
        public Gender Gender { get; set; }
    }

    public enum Races
    {
        Human = 0,
        Elf = 1,
        Dwarf = 2,
        Halfling = 3
    }

    public enum Gender
    {
        Male = 0,
        Female = 1,
        Other = 3
    }
}
