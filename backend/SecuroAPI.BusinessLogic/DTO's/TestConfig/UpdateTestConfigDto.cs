using System.ComponentModel.DataAnnotations;

namespace SecuroAPI.BusinessLogic.DTO_s.TestConfig;

public class UpdateTestConfigDto
{
    [Required]
    public List<Guid> EnabledTestIds { get; set; } = new();
}