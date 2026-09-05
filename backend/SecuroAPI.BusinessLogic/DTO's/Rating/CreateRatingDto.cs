using System.ComponentModel.DataAnnotations;

namespace SecuroAPI.BusinessLogic.DTO_s.Rating;

public class CreateRatingDto
{
    [Required]
    public int VulnerabilityScore { get; set; }

    [Required]
    [Range(0, 10)] 
    public int NumberOfTests { get; set; }

    [Required]
    [Range(0, 100)]
    public int OverallScore { get; set; }

    [Required]
    public Guid APIID { get; set; }
}