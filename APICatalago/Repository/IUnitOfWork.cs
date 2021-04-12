using System.Threading.Tasks;

namespace APICatalago.Repository
{
    public interface IUnitOfWork
    {
        //uma instancia de cada repositorio que tenho no meu projeto
        IProdutoRepository ProdutoRepository { get; }
        ICategoriaRepository CategoriaRepository { get; }
        Task Commit();
    }
}   
