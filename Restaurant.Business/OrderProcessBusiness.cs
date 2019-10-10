using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Restaurant.Business.Filter;
using Restaurant.Business.Interfaces;
using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Business.Paginated;
using Restaurant.Common.Enums;
using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Business
{
    public class OrderProcessBusiness : IOrderProcessBusiness
    {
        private readonly IMapper _mapper;
        private readonly IOrderProcessRepository _menuUnitRepository;
        public OrderProcessBusiness(IMapper mapper,
            IOrderProcessRepository menuUnitRepository)
        {
            _mapper = mapper;
            _menuUnitRepository = menuUnitRepository;
        }
        public async Task<OrderProcess> Add(OrderProcess model)
        {
            var entity = _menuUnitRepository.Add(model);
            await _menuUnitRepository.SaveChangeAsync();

            return model;
        }
        public async Task<bool> Update(OrderProcess model)
        {
            var result = false;
            var record = await _menuUnitRepository.Repo.FirstOrDefaultAsync(c => c.Id == model.Id);

            if (record != null)
            {
                record.Name = model.Name;

                await _menuUnitRepository.SaveChangeAsync();

                result = true;
            }
            return result;
        }
        public async Task<bool> SetActive(int id, int Active)
        {
            var result = false;
            var record = await _menuUnitRepository.Repo.FirstOrDefaultAsync(c => c.Id == id);
            if (record != null)
            {
                record.Status = Active == 1 ? 0 : 1;
                await _menuUnitRepository.SaveChangeAsync();
                result = true;
            }
            return result;
        }
        public Task<bool> Delete(int id)
        {
            throw new System.NotImplementedException();
        }
        public async Task<IPaginatedList<OrderProcess>> GetAll(int pageIndex, int pageSize)
        {

            var result = await _menuUnitRepository.Repo
                          .Where(c => c.Status < (int)EStatus.All)
                          .OrderBy(c => c.Id)
                          .ToPaginatedListAsync(pageIndex, pageSize);
            return result;
        }
        public async Task<OrderProcess> GetById(int id)
        {
            var result = await _menuUnitRepository.Repo.Where(c => c.Id == id)
                .FirstOrDefaultAsync();

            return result;
        }
    }
}
