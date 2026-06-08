namespace SecuroAPI.DataAccess.Entities;

public class Rating
{
    public Guid RatingId { get; set; }
    
    public int VulnerabilityScore { get; set; }
    
    public int NumberOfTests { get; set; }
    
    public int OverallScore { get; set; }
    
    public Guid RegistryId { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    
    public virtual APIRegistry Registry { get; set; } = null!;
    
    public ICollection<ScoreReport> ScoreReports { get; set; } = new HashSet<ScoreReport>();

}