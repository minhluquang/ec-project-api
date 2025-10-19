using ec_project_api.Dtos.response.pagination;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ec_project_api.Services.discounts
{
    public interface IDiscountService : IBaseService<Discount, int>
    {
     
    }
}