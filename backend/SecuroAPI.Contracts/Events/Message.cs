

using System.Text.Json.Serialization;

namespace SecuroAPI.Contracts.Events;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(TestJobStatusMessage), "status")]
[JsonDerivedType(typeof(TestResultMessage), "result")]
public record RunnerMessage(Guid JobId);
