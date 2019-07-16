using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Restaurant.Business.Interfaces;
using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Business.Paginated;
using Restaurant.Common.Constants;
using Restaurant.Common.Enums;
using Restaurant.Common.Filters;
using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Restaurant.Business
{
    public class CustomerBusiness : ICustomerBusiness
    {
        private readonly IMapper _mapper;
        private readonly ICustomerRepository _customerRepository;
        public CustomerBusiness(IMapper mapper,
            ICustomerRepository customerRepository)
        {
            _mapper = mapper;
            _customerRepository = customerRepository;
        }
        public async Task<Customer> Add(Customer model)
        {
            var entity = _customerRepository.Add(model);
            await _customerRepository.SaveChangeAsync();

            return model;
        }
        public async Task<bool> Update(Customer model)
        {
            _customerRepository.Update(model);
            var recordUpdated = await _customerRepository.SaveChangeAsync();
            return recordUpdated > 0;
        }
        public async Task<bool> SetActive(int id, int Active)
        {
            var result = false;
            var record = await _customerRepository.Repo.FirstOrDefaultAsync(c => c.Id == id);
            if (record != null)
            {
                record.Status = Active;
                await _customerRepository.SaveChangeAsync();
                result = true;
            }
            return result;
        }
        public Task<bool> Delete(int id)
        {
            //_customerRepository.Delete(id);
            //var recordUpdated = await _customerRepository.SaveChangeAsync();
            //return recordUpdated > 0;
            throw new System.NotImplementedException();
        }        
        public Task<IPaginatedList<Customer>> GetAll(int pageIndex = Constant.PAGE_INDEX_DEFAULT, int pageSize = Constant.PAGE_SIZE_DEFAULT)
        {
            var result = GetAll(null, pageIndex, pageSize);
            return result;
        }
        public Task<IPaginatedList<Customer>> GetAllByRestaurant(int restaurantId, int branchId, CustomerFilter filter)
        {
            Expression<Func<Customer, bool>> expression = null;
            if (!string.IsNullOrEmpty(filter.Name))
            {
                expression = c => (c.Name).Contains(filter.Name) && c.RestaurantId == restaurantId;
            }
            else if (!string.IsNullOrEmpty(filter.Phone))
            {
                expression = c => c.Phone.Contains(filter.Phone) && c.RestaurantId == restaurantId;
            }
            else
            {
                expression = c => c.RestaurantId == restaurantId;
            }
            return GetAll(expression, filter.PageIndex, filter.PageSize);
        }
        public Task<IPaginatedList<Customer>> GetAll(Expression<Func<Customer, bool>> expression, int pageIndex, int pageSize)
        {
            var resultRepo = _customerRepository.Repo;

            if (expression != null)
            {
                resultRepo = resultRepo.Where(expression);
            }

            var result = resultRepo.Where(c => c.Status < (int)EStatus.All)
                .OrderByDescending(c => c.Id)
                .ToPaginatedListAsync(pageIndex, pageSize);
            return result;
        }
        public async Task<Customer> GetById(int id)
        {
            var result = await _customerRepository.Repo.Where(c => c.Id == id)
                .FirstOrDefaultAsync();
            return result;
        }
    }
}
