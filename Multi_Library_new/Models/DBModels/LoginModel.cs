using Multi_Library.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Multi_Library_new.Models.DBModels
{
    public class LoginModel
    {
        [Column("id")]
        public int UserId { get; set; }
        [Column("u_type")]
        public int RoleInt { get; set; }
    }
}
