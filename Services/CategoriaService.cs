using StockLine_API.DTOs;
using StockLine_API.Models; 


namespace StockLine_API.Services
{
    public class CategoriaService
    {
        private readonly StockLineContext _context;

        public CategoriaService(StockLineContext context)
        {
            _context = context;
        }

        public List<CategoriaDTO> GetAll()
        {
            return _context.Categorias
                .Select(c => new CategoriaDTO
                {
                    CategoriaID = c.CategoriaID,
                    Nombre = c.Nombre,
                    Activo = c.Activo
                })
                .ToList();
        }

        public CategoriaDTO? Get(int id)
        {
            var c = _context.Categorias.Find(id);
            if (c == null) return null;
            return new CategoriaDTO { CategoriaID = c.CategoriaID, Nombre = c.Nombre, Activo = c.Activo };
        }

        public Categoria GetCategoriaModel(int id)
        {
            return _context.Categorias.Find(id);
        }

        public void Create(CategoriaDTO dto)
        {
            var categoria = new Categoria { Nombre = dto.Nombre }; 
            _context.Categorias.Add(categoria);
            _context.SaveChanges();
        }

        public void Update(CategoriaDTO dto)
        {
            var categoria = _context.Categorias.Find(dto.CategoriaID);
            if (categoria == null) return;
            categoria.Nombre = dto.Nombre;
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var categoria = _context.Categorias.Find(id);
            if (categoria == null) return;
            _context.Categorias.Remove(categoria);
            _context.SaveChanges();
        }
    }
}
