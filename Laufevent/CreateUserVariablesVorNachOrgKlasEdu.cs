using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Laufevent;

public class CreateUserVariablesVorNachOrgKlasEdu
{
    [Required] [DefaultValue("")] 
    public string Vorname { get; set; }
        
    [Required][DefaultValue("")]   
    public string Nachname { get; set; } 
        
    [Required][DefaultValue("")] 
    public string Organisation { get; set; }
    
    [Required][DefaultValue("")] 
    public string Klasse { get; set; }
    
    [Required][DefaultValue("")] 
    public string Educard { get; set; }
}