using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using LiteDB;
using Newtonsoft.Json;
//using onnaMUD.Utilities;

namespace onnaMUD.BaseClasses
{
    public class Account
    {
        //[Key]
        public Guid Id { get; set; }// guid ID for this account
        //public int AccountID { get; set; }
        public string AccountName { get; set; } = "moo";
        public AccountType AccountType { get; set; }
        public string AllowedServers { get; set; } = "";

        //[BsonIgnore]// NotMapped]
        public List<Guid> Characters { get; set; } = new();//this is so we can look at an account and see all the characters, NotMapped means it's not in the database and we'll have to populate it on client login

    }

    public enum AccountType
    {
        Trial = 0,
        Basic = 1,
        Premium = 2,
        Mod = 3,
        Admin = 4,
        Owner = 5
    }
}
