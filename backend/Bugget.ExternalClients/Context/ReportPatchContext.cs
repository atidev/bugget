using Bugget.Entities.DbModels.Report;
using Bugget.Entities.DTO.Report;

namespace Bugget.ExternalClients.Context;

public record ReportPatchContext(string UserId, ReportPatchDto PatchDto, ReportPatchResultDbModel Result);