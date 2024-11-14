using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Laufevent
{
    public class CreateUservariables_Vorname_Nachname
    {
        [Required] [DefaultValue("")] 
        public string Vorname { get; set; }
        
        [Required][DefaultValue("")]   
         public string Nachname { get; set; } 
        
        [Required][DefaultValue("")] 
         public string Organisation { get; set; }
        

    }
}

