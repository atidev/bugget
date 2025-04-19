using Bugget.Entities.DbModels.Report;
using Bugget.Entities.DTO.Report;

namespace Bugget.Features.Context;

public record ReportPatchContext(string UserId, ReportPatchDto PatchDto, ReportPatchDbModel ReportPatchDbModel);