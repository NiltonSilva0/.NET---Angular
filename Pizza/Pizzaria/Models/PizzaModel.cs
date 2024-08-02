namespace Pizzaria.Models;

public class PizzaModel
{
    public Guid Id { get; set; }
    public string Sabor { get; set; } = string.Empty;
    public string Capa { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Preco { get; set; }
}