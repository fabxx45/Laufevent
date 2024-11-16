using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Laufevent
{
    public class CreateUserVariablesVornameNachname
    {
        [Required] [DefaultValue("")] 
        public string Vorname { get; set; }
        
        [Required][DefaultValue("")]   
         public string Nachname { get; set; } 
        
        [Required][DefaultValue("")] 
         public string Organisation { get; set; }
        

    }
}

