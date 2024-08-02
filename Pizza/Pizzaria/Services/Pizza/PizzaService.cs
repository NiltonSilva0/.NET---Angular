using Microsoft.EntityFrameworkCore;
using Pizzaria.Data;
using Pizzaria.Dto;
using Pizzaria.Models;

namespace Pizzaria.Services.Pizza
{
    public class PizzaService : IPizzaInterface
    {
        private readonly AppDbContext _context;
        private readonly string _webHostEnvironment;

        public PizzaService(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment.WebRootPath;
        }

        public string GeraCaminhoArquivo(IFormFile foto)
        {
            if (foto == null || foto.Length == 0)
            {
                throw new ArgumentException("O arquivo de imagem não foi enviado ou está vazio.");
            }

            var codigoUnico = Guid.NewGuid().ToString();
            var nomeCaminhoImagem = Path.GetFileNameWithoutExtension(foto.FileName).Replace(" ", "").ToLower() + codigoUnico + Path.GetExtension(foto.FileName);

            var caminhoParaSalvarImagens = Path.Combine(_webHostEnvironment, "imagem");
            if (!Directory.Exists(caminhoParaSalvarImagens))
            {
                Directory.CreateDirectory(caminhoParaSalvarImagens);
            }

            var caminhoCompleto = Path.Combine(caminhoParaSalvarImagens, nomeCaminhoImagem);

            using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
            {
                foto.CopyToAsync(stream).Wait();
            }

            return nomeCaminhoImagem;
        }

        public async Task<PizzaModel> CriarPizza(PizzaCriacaoDto pizzaCriacaoDto, IFormFile foto)
        {
            try
            {
                var nomeCaminhoImagem = GeraCaminhoArquivo(foto);
                Console.WriteLine("Caminho da imagem gerado: " + nomeCaminhoImagem);

                var pizza = new PizzaModel
                {
                    Sabor = pizzaCriacaoDto.Sabor,
                    Descricao = pizzaCriacaoDto.Descricao,
                    Preco = pizzaCriacaoDto.Preco,
                    Capa = nomeCaminhoImagem
                };

                _context.Add(pizza);
                await _context.SaveChangesAsync();

                return pizza;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao criar pizza: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Exceção interna: " + ex.InnerException.Message);
                }
                throw new Exception("Erro ao criar pizza: " + ex.Message, ex);
            }
        }

        public async Task<List<PizzaModel>> ListarPizzas()
        {
            try
            {
               return  await _context.Pizzas.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao listar pizzas: " + ex.Message, ex);
            }
        }

        public async Task<PizzaModel> BuscarPizzaPorId(Guid id)
        {
            try
            {
                return await _context.Pizzas.FirstOrDefaultAsync(pizzaBanco => pizzaBanco.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar pizza: " + ex.Message, ex);
            }
        }

        public async Task<PizzaModel> EditarPizza(PizzaModel pizza, IFormFile? foto)
        {
            try
            {
                var pizzaBanco = await _context.Pizzas.AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == pizza.Id);
        
                if (pizzaBanco == null)
                {
                    throw new Exception("Pizza não encontrada.");
                }

                if (foto != null)
                {
                    var caminhoCapaExiste = Path.Combine(_webHostEnvironment, "imagem", pizzaBanco.Capa);
            
                    if (System.IO.File.Exists(caminhoCapaExiste))
                    {
                        System.IO.File.Delete(caminhoCapaExiste);
                    }
            
                    var nomeCaminhoImagem = GeraCaminhoArquivo(foto);
                    pizza.Capa = nomeCaminhoImagem;
                }
                else
                {
                    pizza.Capa = pizzaBanco.Capa; // Mantém a capa antiga se nenhuma nova for enviada
                }
        
                // Atualiza os campos
                _context.Update(pizza);
                await _context.SaveChangesAsync();
        
                return pizza;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao editar pizza: " + ex.Message, ex);
            }
        }

        public async Task<PizzaModel> DeletarPizza(Guid id)
        {
            try
            {
                var pizza = await _context.Pizzas.FirstOrDefaultAsync(pizzaBanco => pizzaBanco.Id == id);
                if (pizza == null)
                {
                    throw new Exception("Pizza não encontrada.");
                }
                else
                {
                    _context.Remove(pizza);
                    await _context.SaveChangesAsync();
                    return pizza;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao deletar pizza: " + ex.Message, ex);
            }
            
        }
    }
}
