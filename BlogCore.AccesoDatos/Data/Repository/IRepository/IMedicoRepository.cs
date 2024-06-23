using BlogCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BlogCore.AccesoDatos.Data.Repository.IRepository
{
    public interface IMedicoRepository : IRepository<Medico>
    {
        void Update(Medico medico);
        IEnumerable<SelectListItem> GetListaEspecialidades();
        IQueryable<Medico> AsQueryable();
        Medico Get(int id, string includeProperties = null); 
        Medico GetFirstOrDefault(Expression<Func<Medico, bool>> filter = null, string includeProperties = null);
    }
}
