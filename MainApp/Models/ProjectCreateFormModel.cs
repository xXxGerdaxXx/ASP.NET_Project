using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MainApp.Models;

public class ProjectCreateFormModel
{
    [Required(ErrorMessage = "Project Name is required.")]
    [Display(Name = "Project Name", Prompt = "Enter project name")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "You must select a client.")]
    [Display(Name = "Client", Prompt = "Select a client")]
    public int? ClientId { get; set; }

    public SelectList ClientList { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());

    [Display(Name = "Description", Prompt = "Type something")]
    public string Description { get; set; } = null!;

    [Required(ErrorMessage = "Start Date is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "Start Date")]
    public DateTime? StartDate { get; set; }

    [Required(ErrorMessage = "End Date is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "End Date")]
    public DateTime? EndDate { get; set; }

    public List<SelectListItem> TeamMemberList { get; set; } = [];
    [Required(ErrorMessage = "Please assign at least one member to the project.")]
    [Display(Name = "Members")]
    public List<int> SelectedTeamMemberIds { get; set; } = [];

    [Required(ErrorMessage = "Budget is required.")]
    [Range(0, double.MaxValue, ErrorMessage = "Budget must be a positive number.")]
    [Display(Name = "Budget", Prompt = "Enter project budget")]
    public decimal? Budget { get; set; }

    [Required(ErrorMessage = "You must select a status.")]
    [Display(Name = "Status")]
    public int? StatusId { get; set; }

    public SelectList StatusList { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());

    public string? AvatarUrl { get; set; } 

    [Display(Name = "Project Image")]
    public IFormFile? ProjectImage { get; set; }
}


