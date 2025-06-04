using Blog.Data.Context;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Panel.Controllers;
public class CustomerController : Controller
{
    private readonly BaseContext _context;
    public CustomerController(BaseContext context) { _context = context; }
    public IActionResult List()
    {
        List<Data.Entity.Customer> customer = _context.Customers.OrderByDescending(x => x.CreatedDateTime).ToList();
        return View(customer);
    }

}
