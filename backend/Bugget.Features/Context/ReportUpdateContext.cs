using Bugget.Entities.BO.ReportBo;
using Bugget.Entities.DbModels.Report;

namespace Bugget.Features.Context;

public record ReportUpdateContext(ReportUpdate Report, ReportDbModel ReportDbModel);