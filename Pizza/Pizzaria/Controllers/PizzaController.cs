using Microsoft.AspNetCore.Mvc;
using Pizzaria.Dto;
using Pizzaria.Models;
using Pizzaria.Services.Pizza;

namespace Pizzaria.Controllers
{
    public class PizzaController : Controller
    {
        private readonly IPizzaInterface _pizzaInterface;

        public PizzaController(IPizzaInterface pizzaInterface)
        {
            _pizzaInterface = pizzaInterface;
        }

        public async Task<IActionResult> Index()
        {
            var pizzas = await _pizzaInterface.ListarPizzas();
            return View(pizzas);
        }

        public IActionResult Cadastrar()
        {
            return View();
        }

        public async Task<IActionResult> Detalhes(Guid id)
        {
            var pizza = await  _pizzaInterface.BuscarPizzaPorId(id);
            return View(pizza);
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar(PizzaCriacaoDto pizzaCriacaoDto, IFormFile foto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var pizza = await _pizzaInterface.CriarPizza(pizzaCriacaoDto, foto);
                    return RedirectToAction("Index", "Pizza");
                }
                catch (Exception ex)
                {
                    // Log detalhado do erro
                    Console.WriteLine("Erro ao cadastrar pizza: " + ex.Message);
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine("Exceção interna: " + ex.InnerException.Message);
                    }
                    ModelState.AddModelError(string.Empty, "Erro ao cadastrar pizza: " + ex.Message);
                }
            }
            return View(pizzaCriacaoDto);
        }
        public async Task<IActionResult> Editar(Guid id)
        {
            var pizza = await _pizzaInterface.BuscarPizzaPorId(id);
            return View(pizza);
        }
        [HttpPost]
        public async Task<IActionResult> Editar(PizzaModel pizzaModel, IFormFile? foto)
        {
            if (ModelState.IsValid)
            {
                var pizza = _pizzaInterface.EditarPizza(pizzaModel, foto);
                return RedirectToAction("Index", "Pizza");
            }else
            {
                return View(pizzaModel);
            }

        }

        [HttpPost]
        public async Task<IActionResult> DeletarPizza(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("ID inválido.");
            }

            try
            {
                // Chama o serviço para deletar a pizza
                var pizza = await _pizzaInterface.DeletarPizza(id);
        
                // Redireciona para a página de index após exclusão bem-sucedida
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log do erro
                Console.WriteLine("Erro ao deletar pizza: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Exceção interna: " + ex.InnerException.Message);
                }
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao deletar pizza.");
            }
        }

    }
}