using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Laufevent;

public class CompleteRoundVariables
{
    [Required][DefaultValue("")] 
    public int ID { get; set; }
}