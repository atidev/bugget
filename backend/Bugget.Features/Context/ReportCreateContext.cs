using Bugget.Entities.BO.ReportBo;
using Bugget.Entities.DbModels.Report;

namespace Bugget.Features.Context;

public record ReportCreateContext(Report Report, ReportObsoleteDbModel ReportDbModel);