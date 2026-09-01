using System.ComponentModel.DataAnnotations;
using SecuroAPI.Common.Enums;

namespace SecuroAPI.BusinessLogic.DTO_s.APIRegistry;

public class UpdateAPIRegistryDTO
{
    [Required]
    [Url]
    public string TargetURL { get; set; } = string.Empty;
    public string AuthType { get; set; } = string.Empty;
}