using LUSSIS.Models;
using LUSSIS.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace LUSSIS.Repositories
{
    public class GenericRepo<T, ID> : IGenericRepo<T, ID> where T : class
    {
        private LUSSISContext context = new LUSSISContext();
        public LUSSISContext Context { get { return this.context; } }

        public T Create(T entity)
        {
            context = new LUSSISContext();
            T newEntity = context.Set<T>().Add(entity);
            Save();
            return newEntity;
        }

        public void Delete(T entity)
        {
            context.Set<T>().Remove(entity);
            Save();
        }

        public IEnumerable<T> FindAll()
        {
            context = new LUSSISContext();
            if (typeof(T) == typeof(CartDetail))
            {
                context.CartDetail_releaseCartData();//Note:release cart detail which are more than 1hour existed by last item of an employee
                context = new LUSSISContext();
            }
            return context.Set<T>().ToList<T>();
        }

        public T FindById(ID id)
        {
            context = new LUSSISContext();
            if (typeof(T) == typeof(CartDetail))
            {
                context.CartDetail_releaseCartData();//Note:release cart detail which are more than 1hour existed by last item of an employee
                context = new LUSSISContext();
            }

            return context.Set<T>().Find(id);

        }

        public IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            if (typeof(T) == typeof(CartDetail))
            {
                context.CartDetail_releaseCartData();//Note:release cart detail which are more than 1hour existed by last item of an employee
                context = new LUSSISContext();
            }
            context = new LUSSISContext();
            return context.Set<T>().Where(predicate).ToList();
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Update(T entity)
        {
            //is this line necessary?
            try
            {
                context.Set<T>().Attach(entity);
                context.Entry(entity).State = EntityState.Modified;
                Save();
            }
            catch
            {

            }


        }

        public T FindOneBy(Expression<Func<T, bool>> predicate)
        {
            if (typeof(T) == typeof(CartDetail))
            {
                context.CartDetail_releaseCartData();//Note:release cart detail which are more than 1hour existed by last item of an employee
                context = new LUSSISContext();
            }
            context = new LUSSISContext();
            return context.Set<T>().Where(predicate).SingleOrDefault();

        }
    }
}