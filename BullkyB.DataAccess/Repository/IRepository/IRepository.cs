using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BullkeyBookPro.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T:class
    {
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null,string?includeProperity=null);
        T GetFirstOrDefualt(Expression<Func<T, bool>>filter, string? includeProperity = null,bool tracked=true);
        void Add(T identity);
        void remove(T identity);
        void removerange(IEnumerable<T> identity);


    }
}
