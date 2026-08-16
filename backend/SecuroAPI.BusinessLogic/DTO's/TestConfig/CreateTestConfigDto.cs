using System.ComponentModel.DataAnnotations;

namespace SecuroAPI.BusinessLogic.DTO_s.TestConfig;

public class CreateTestConfigDto
{
    [Required]
    public Guid APIID { get; set; }

    [Required]
    public List<Guid> EnabledTestIds { get; set; } = new();
}