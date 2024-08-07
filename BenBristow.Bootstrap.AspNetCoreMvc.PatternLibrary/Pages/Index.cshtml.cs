using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BenBristow.Bootstrap.AspNetCoreMvc.PatternLibrary.Pages;

public class IndexModel : PageModel
{
    [BindProperty]
    public string Name { get; init; }

    [BindProperty]
    [Required(ErrorMessage = "Please select a favourite color")]
    [Display(Name = "Favourite color")]
    public string FavouriteColor { get; init; }

    [BindProperty]
    [EmailAddress]
    public string Email { get; init; }

    [BindProperty]
    public string Description { get; init; }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
            return Page();

        return RedirectToPage();
    }
}