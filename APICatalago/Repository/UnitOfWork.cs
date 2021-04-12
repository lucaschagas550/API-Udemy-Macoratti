using APICatalago.Context;
using System.Threading.Tasks;

namespace APICatalago.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ProdutoRepository _produtoRepo;
        private CategoriaRepository _categoriaRepo;
        public AppDbContext _context;

        public UnitOfWork(AppDbContext contexto)
        {
            _context = contexto;
        }

        public IProdutoRepository ProdutoRepository
        {
            get
            {
                return _produtoRepo = _produtoRepo ?? new ProdutoRepository(_context); // verica se a instancia de repositorio eh null, se for null passa a intancia do contexto
                // se não passa a instancia que ja existe no repositorio
            }
        }

        public ICategoriaRepository CategoriaRepository
        {
            get
            {
                return _categoriaRepo = _categoriaRepo ?? new CategoriaRepository(_context);
            }
        }

        public async Task Commit()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose() // libera os recursos do contexto
        {
            _context.Dispose();
        }
    }
}
