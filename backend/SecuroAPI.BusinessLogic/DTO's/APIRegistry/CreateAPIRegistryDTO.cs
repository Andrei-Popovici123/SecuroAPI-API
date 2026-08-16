using System.ComponentModel.DataAnnotations;

namespace SecuroAPI.BusinessLogic.DTO_s.APIRegistry;

public class CreateAPIRegistryDTO
{
    [Required]
    public string UserID { get; set; } = string.Empty;
    [Required]
    [Url]
    public string TargetURL { get; set; } = string.Empty;
}