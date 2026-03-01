using System.Linq.Expressions;

namespace iWallet.Application.Interface
{
    public interface IIsExistMethod<T> where T : class
    {
        bool IsExist(Expression<Func<T,bool>> filter = null);
    }
}
