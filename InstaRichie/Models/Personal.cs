using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;

namespace StartFinance.Models
{
    public class Personal
    {
        [PrimaryKey, AutoIncrement]
        public int PersonalID { get; set; }
        [NotNull]
        public string FirstName { get; set; }
        [NotNull]
        public string LastName { get; set; }
        [NotNull]
        public DateTime DOB { get; set; }
        [Unique, NotNull]
        public string Email { get; set; }
        [Unique, NotNull]
        public string MobileNumber { get; set; }
    }
}
