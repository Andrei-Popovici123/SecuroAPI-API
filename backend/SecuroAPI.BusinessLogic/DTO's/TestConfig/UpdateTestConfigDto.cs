using System.ComponentModel.DataAnnotations;

namespace SecuroAPI.BusinessLogic.DTO_s.TestConfig;

public class UpdateTestConfigDto
{
    [Required]
    [MinLength(1), MaxLength(10)]
    public List<Guid> EnabledTestIds { get; set; } = new();
}