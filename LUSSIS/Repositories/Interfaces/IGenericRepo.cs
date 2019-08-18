using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LUSSIS.Repositories.Interfaces
{
    public interface IGenericRepo<T, ID>
    {
        T FindById(ID id);

        T FindOneBy(Expression<Func<T, bool>> predicate);

        IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate);

        IEnumerable<T> FindAll();

        T Create(T entity);

        void Update(T entity);

        void Delete(T entity);

        void Save();
    }
}