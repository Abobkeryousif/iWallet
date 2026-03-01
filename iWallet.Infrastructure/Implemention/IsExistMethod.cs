using System.Linq.Expressions;

namespace iWallet.Infrastructure.Implemention
{
    public class IsExistMethod<T> : IIsExistMethod<T> where T : class
    {
        private readonly ApplicationDbContext _context;

        public IsExistMethod(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool IsExist(Expression<Func<T, bool>> filter = null)
        {
            return _context.Set<T>().Any(filter);
        }
    }
}
