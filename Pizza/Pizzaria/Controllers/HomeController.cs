using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Pizzaria.Services.Pizza;

namespace Pizzaria.Controllers;

public class HomeController : Controller
{
    private readonly IPizzaInterface _pizzaInterface;
    
    public HomeController(IPizzaInterface  pizzaInterface)
    {
        _pizzaInterface = pizzaInterface;
    }
    public async Task<IActionResult> Index(string? pesquisar)
    {
        if (pesquisar == null)
        {
            var pizzas = await _pizzaInterface.ListarPizzas();
            return View(pizzas);
        }
        else
        {
            var pizzas = await _pizzaInterface.ListarPizzas();
            var pizzasFiltradas = pizzas.Where(pizzaBanco => 
                pizzaBanco.Sabor.ToLower().Contains(pesquisar.ToLower())).ToList();
            return View(pizzasFiltradas);
        }
    }
}