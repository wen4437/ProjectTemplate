using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace System.My.CommonUtil
{
    public class ModelBase
    {
        public Expression<Func<T, object>>[] Select<T>(params Expression<Func<T, object>>[] select)
        {
            return select;
        }

        public Expression<Func<T, object>> Where<T>(Expression<Func<T, object>> where)
        {
            return where;
        }

        public Expression<Func<T, object>>[] WhereAnd<T>(params Expression<Func<T, object>>[] where)
        {
            return where;
        }

        public Expression<Func<T, object>>[] WhereOr<T>(params Expression<Func<T, object>>[] where)
        {
            return where;
        }
    }
}
