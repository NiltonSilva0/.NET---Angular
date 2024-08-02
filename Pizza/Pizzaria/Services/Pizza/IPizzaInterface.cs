using Pizzaria.Dto;
using Pizzaria.Models;

namespace Pizzaria.Services.Pizza;

public interface IPizzaInterface //A interface tem todos os métodos que a controler pode utilizar
{
    Task<PizzaModel> CriarPizza(PizzaCriacaoDto pizzaCriacaoDto, IFormFile foto);
    Task<List<PizzaModel>> ListarPizzas();
    Task<PizzaModel> BuscarPizzaPorId(Guid id);
    Task<PizzaModel> EditarPizza(PizzaModel pizza, IFormFile? foto);
    Task<PizzaModel> DeletarPizza(Guid id);
}