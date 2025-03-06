using System.ComponentModel;

namespace Infrastructure.Enums;

public enum JobTitle
{
    [Description("Chief Executive Officer")]
    ChiefExecutiveOfficer,

    [Description("Chief Technician Officer")]
    ChiefTechnicianOfficer,

    Administrator,

    [Description("Frontend Developer")]
    FrontendDeveloper,

    [Description("Fullstack Developer")]
    FullstackDeveloper
}


