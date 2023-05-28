using System;
using Tozawa.Language.Client.Models.Enum;

namespace Tozawa.Language.Client.Models.FormModels
{
    public class GetXliffDistributionFilesLogsQuery
    {
        public DateTime StartDate { get; set; } = DateTime.Today.AddDays(-7);
        public DateTime EndDate { get; set; } = DateTime.Today;
        public XliffFileState XliffFileState { get; set; } = XliffFileState.Export;
    }
}
