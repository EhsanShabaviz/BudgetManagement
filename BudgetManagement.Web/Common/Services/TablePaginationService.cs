using System;
using System.Collections.Generic;
using System.Linq;

namespace BudgetManagement.Web.Common.Services
{
    public static class TablePaginationService
    {
        public static IEnumerable<T> Paginate<T>(IEnumerable<T> source, int currentPage, int pageSize)
        {
            if (source is null) return Enumerable.Empty<T>();
            if (pageSize <= 0) return source;

            var page = Math.Max(1, currentPage);
            var skip = (page - 1) * pageSize;

            return source.Skip(skip).Take(pageSize);
        }
    }
}
