using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.UnitOfWork;
using UIWeb.Areas.Admin.Models;

namespace UIWeb.Areas.Admin.Controllers
{
  [Area("Admin")] // Controller için area belirteci
  [Authorize(Roles = "Admin, Editor")]
  public class CategoryController : Controller
  {
    private readonly IServiceManager _manager;

    public CategoryController(IServiceManager manager)
    {
      _manager = manager;
    }

    // Views
    public IActionResult Index()
    {
      return View(_manager.CategoryService.GetAllCategories(false));
    }

    public IActionResult Create()
    {
      var model = new CategoryCreateViewModel
      {
        Categories = _manager.CategoryService.GetAllCategories(false)
      };
      return View(model);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CategoryCreateViewModel model)
    {
      if (ModelState.IsValid)
      {
        _manager.CategoryService.AddCategory(model.NewCategory);
      }
      return RedirectToAction("Index");
    }
  }
}