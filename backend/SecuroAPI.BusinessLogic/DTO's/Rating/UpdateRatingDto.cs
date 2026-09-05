using System.ComponentModel.DataAnnotations;

namespace SecuroAPI.BusinessLogic.DTO_s.Rating;

public class UpdateRatingDto
{
    [Required]
    public int VulnerabilityScore { get; set; }

    [Required]
    [Range(0, 10)] 
    public int NumberOfTests { get; set; }

    [Required]
    [Range(0, 100)]
    public int OverallScore { get; set; }
    
}