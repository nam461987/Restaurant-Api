using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Business.Filter
{
    public static class FilterExtension
    {
        public static IQueryable<T> ToFilterByRole<T>(this IQueryable<T> source, Func<T, int> getRestaurantIdProperty,
            Func<T, int> getBranchIdProperty, int? restaurantId, int? branchId)
        {
            if ((restaurantId == 0 && branchId == 0) || (restaurantId == null && branchId == null))
            {
                return source;
            }
            else if ((restaurantId > 0 && branchId == 0) || (restaurantId > 0 && branchId == null))
            {
                source = source.Where(c => getRestaurantIdProperty(c) == restaurantId);
            }
            else if (restaurantId > 0 && branchId > 0)
            {
                source = source.Where(c => getRestaurantIdProperty(c) == restaurantId && getBranchIdProperty(c) == branchId);
            }
            return source;
        }
    }
}
