using Microsoft.AspNetCore.Mvc;

public class ProviderController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}