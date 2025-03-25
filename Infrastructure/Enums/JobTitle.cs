using System.ComponentModel;


namespace Infrastructure.Enums;

public enum JobTitle
{
    [Description("Unknown")]
    Unknown,

    [Description("Chief Executive Officer")]
    ChiefExecutiveOfficer,

    [Description("Chief Technician Officer")]
    ChiefTechnicianOfficer,

    [Description("Administrator")]
    Administrator,

    [Description("Frontend Developer")]
    FrontendDeveloper,

    [Description("Fullstack Developer")]
    FullstackDeveloper
}


