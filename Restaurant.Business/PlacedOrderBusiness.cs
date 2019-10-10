using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Restaurant.Business.Filter;
using Restaurant.Business.Interfaces;
using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Business.Paginated;
using Restaurant.Common.Arrays;
using Restaurant.Common.Dtos.PlacedOrder;
using Restaurant.Common.Enums;
using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Business
{
    public class PlacedOrderBusiness : IPlacedOrderBusiness
    {
        private readonly IMapper _mapper;
        private readonly IPlacedOrderRepository _placedOrderRepository;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IRestaurantBranchRepository _restaurantBranchRepository;
        private readonly IOrderChannelRepository _orderChannelRepository;
        private readonly IRestaurantTableRepository _restaurantTableRepository;
        private readonly IAdminAccountRepository _adminAccountRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderProcessRepository _orderProcessRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PlacedOrderBusiness(IMapper mapper,
            IPlacedOrderRepository placedOrderRepository,
            IRestaurantRepository restaurantRepository,
            IRestaurantBranchRepository restaurantBranchRepository,
            IOrderChannelRepository orderChannelRepository,
            IRestaurantTableRepository restaurantTableRepository,
            IAdminAccountRepository adminAccountRepository,
            ICustomerRepository customerRepository,
            IOrderProcessRepository orderProcessRepository
            , IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _placedOrderRepository = placedOrderRepository;
            _restaurantRepository = restaurantRepository;
            _restaurantBranchRepository = restaurantBranchRepository;
            _orderChannelRepository = orderChannelRepository;
            _restaurantTableRepository = restaurantTableRepository;
            _adminAccountRepository = adminAccountRepository;
            _customerRepository = customerRepository;
            _orderProcessRepository = orderProcessRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<PlacedOrder> Add(PlacedOrder model)
        {
            DateTime startDateTime = DateTime.Today; //Today at 00:00:00
            DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59
            string dateStr = DateTime.Today.ToString("MMddyyyy");
            string resStr = $"{(model.RestaurantId):D4}";
            string braStr = $"{(model.BranchId):D4}";

            model.Code = string.Empty;

            var totalOrderToday = await _placedOrderRepository.Repo.Where(
                c => c.CreatedDate >= startDateTime && c.CreatedDate <= endDateTime).CountAsync();

            var entity = _placedOrderRepository.Add(model);
            await _unitOfWork.SaveChangesAsync();

            entity.Code = $"OD-{dateStr}{resStr}{braStr}{(totalOrderToday + 1):D5}";
            await _unitOfWork.SaveChangesAsync();
            model.Id = entity.Id;
            model.Code = entity.Code;

            return model;
        }
        public async Task<bool> Update(PlacedOrder model)
        {
            var result = false;
            var record = await _placedOrderRepository.Repo.FirstOrDefaultAsync(c => c.Id == model.Id);

            if (record != null)
            {
                record.OrderTypeId = model.OrderTypeId;
                record.CustomerId = model.CustomerId;
                record.OrderChannelId = model.OrderChannelId;
                record.TableId = model.TableId;
                record.PeopleNum = model.PeopleNum;
                record.CustomerName = model.CustomerName;
                record.CustomerPhone = model.CustomerPhone;
                record.OrderTime = model.OrderTime;
                record.DeliveryTime = model.DeliveryTime;
                record.DeliveryAddress = model.DeliveryAddress;
                record.Tax = model.Tax;
                record.DiscountType = model.DiscountType;
                record.Discount = model.Discount;
                record.Description = model.Description;
                record.UpdatedStaffId = model.UpdatedStaffId;
                record.UpdatedDate = DateTime.Now;

                if (record.DiscountType == (int)EDiscountType.Percent)
                {
                    record.FinalPrice = record.Price + record.Tax - ((record.Price * record.Discount) / 100);
                }
                else if (record.DiscountType == (int)EDiscountType.Money)
                {
                    record.FinalPrice = record.Price + record.Tax - record.Discount;
                }

                await _placedOrderRepository.SaveChangeAsync();

                result = true;
            }
            return result;
        }
        public async Task<bool> SetActive(int id, int Active)
        {
            var result = false;
            var record = await _placedOrderRepository.Repo.FirstOrDefaultAsync(c => c.Id == id);
            if (record != null)
            {
                record.Status = Active == 1 ? 0 : 1;
                await _placedOrderRepository.SaveChangeAsync();
                result = true;
            }
            return result;
        }
        public Task<bool> Delete(int id)
        {
            throw new System.NotImplementedException();
        }
        public Task<IPaginatedList<PlacedOrderDto>> GetAll(int restaurantId, int branchId, int pageIndex, int pageSize)
        {
            var PlacedOrderRepo = _placedOrderRepository.Repo;

            var result = (from placedOrder in PlacedOrderRepo
                          join restaurant in _restaurantRepository.Repo on placedOrder.RestaurantId equals restaurant.Id into rs
                          from restaurant in rs.DefaultIfEmpty()
                          join branch in _restaurantBranchRepository.Repo on placedOrder.BranchId equals branch.Id into brs
                          from branch in brs.DefaultIfEmpty()
                          join orderChannel in _orderChannelRepository.Repo on placedOrder.OrderChannelId equals orderChannel.Id into ocs
                          from orderChannel in ocs.DefaultIfEmpty()
                          join table in _restaurantTableRepository.Repo on placedOrder.TableId equals table.Id into rts
                          from table in rts.DefaultIfEmpty()
                          join createdAccount in _adminAccountRepository.Repo on placedOrder.CreatedStaffId equals createdAccount.Id into caas
                          from createdAccount in caas.DefaultIfEmpty()
                          join updatedAccount in _adminAccountRepository.Repo on placedOrder.UpdatedStaffId equals updatedAccount.Id into uaas
                          from updatedAccount in uaas.DefaultIfEmpty()
                          join customer in _customerRepository.Repo on placedOrder.CustomerId equals customer.Id into cs
                          from customer in cs.DefaultIfEmpty()
                          join process in _orderProcessRepository.Repo on placedOrder.OrderProcessId equals process.Id into ps
                          from process in ps.DefaultIfEmpty()
                          select new PlacedOrderDto
                          {
                              Id = placedOrder.Id,
                              RestaurantId = placedOrder.RestaurantId,
                              RestaurantIdName = restaurant.Name,
                              BranchId = placedOrder.BranchId,
                              BranchIdName = branch.Name,
                              OrderTypeId = placedOrder.OrderTypeId,
                              OrderTypeIdName = AOrderType.OrderType[placedOrder.OrderTypeId.GetValueOrDefault()],
                              CustomerId = placedOrder.CustomerId,
                              CustomerIdName = customer.Name,
                              CustomerIdAddress = customer.Address,
                              CustomerIdEmail = customer.Email,
                              CustomerIdPhone = customer.Phone,
                              OrderChannelId = placedOrder.OrderChannelId,
                              OrderChannelIdName = orderChannel.Name,
                              TableId = placedOrder.TableId,
                              TableIdName = table.Name,
                              Code = placedOrder.Code,
                              OrderProcessId = placedOrder.OrderProcessId,
                              OrderProcessIdName = process.Name,
                              OrderProcessIdColor = process.Color,
                              PeopleNum = placedOrder.PeopleNum.GetValueOrDefault(),
                              CustomerName = placedOrder.CustomerName,
                              CustomerPhone = placedOrder.CustomerPhone,
                              OrderTime = placedOrder.OrderTime,
                              DeliveryTime = placedOrder.DeliveryTime,
                              DeliveryAddress = placedOrder.DeliveryAddress,
                              Tax = placedOrder.Tax,
                              Price = placedOrder.Price,
                              DiscountType = placedOrder.DiscountType,
                              Discount = placedOrder.Discount,
                              FinalPrice = placedOrder.FinalPrice,
                              Description = placedOrder.Description,
                              Status = placedOrder.Status.GetValueOrDefault(),
                              CreatedStaffId = placedOrder.CreatedStaffId,
                              CreatedStaffIdName = createdAccount.UserName,
                              CreatedDate = placedOrder.CreatedDate,
                              UpdatedStaffId = updatedAccount.Id,
                              UpdatedStaffIdName = updatedAccount.UserName,
                              UpdatedDate = placedOrder.UpdatedDate
                          })
                          .ToFilterByRole(f => f.RestaurantId, f => f.BranchId, restaurantId, branchId)
                          .Where(c => c.Status < (int)EStatus.All)
                          .OrderByDescending(c => c.Id)
                          .ToPaginatedListAsync(pageIndex, pageSize);
            return result;
        }
        public async Task<PlacedOrderDto> GetById(int restaurantId, int branchId, int id)
        {
            var result = await _placedOrderRepository.Repo.ToFilterByRole(f => f.RestaurantId,
                f => f.BranchId, restaurantId, branchId).Where(c => c.Id == id)
                .FirstOrDefaultAsync();

            return _mapper.Map<PlacedOrderDto>(result);
        }
        public async Task<bool> UpdatePriceToPlacedOrder(PlacedOrder model)
        {
            var result = false;
            var record = await _placedOrderRepository.Repo.FirstOrDefaultAsync(c => c.Id == model.Id);

            if (record != null)
            {
                record.Price = model.Price;

                await _placedOrderRepository.SaveChangeAsync();

                result = true;
            }
            return result;
        }
        public async Task<bool> UpdateOrderProcess(PlacedOrder model)
        {
            var result = false;
            var record = await _placedOrderRepository.Repo.FirstOrDefaultAsync(c => c.Id == model.Id);

            if (record != null)
            {
                record.OrderProcessId = model.OrderProcessId;

                await _placedOrderRepository.SaveChangeAsync();

                result = true;
            }
            return result;
        }
        public Task<List<PlacedOrderDto>> GetWaitingOrder(int restaurantId, int branchId)
        {
            var PlacedOrderRepo = _placedOrderRepository.Repo;

            var result = (from placedOrder in PlacedOrderRepo
                          join restaurant in _restaurantRepository.Repo on placedOrder.RestaurantId equals restaurant.Id into rs
                          from restaurant in rs.DefaultIfEmpty()
                          join branch in _restaurantBranchRepository.Repo on placedOrder.BranchId equals branch.Id into brs
                          from branch in brs.DefaultIfEmpty()
                          join orderChannel in _orderChannelRepository.Repo on placedOrder.OrderChannelId equals orderChannel.Id into ocs
                          from orderChannel in ocs.DefaultIfEmpty()
                          join table in _restaurantTableRepository.Repo on placedOrder.TableId equals table.Id into rts
                          from table in rts.DefaultIfEmpty()
                          join createdAccount in _adminAccountRepository.Repo on placedOrder.CreatedStaffId equals createdAccount.Id into caas
                          from createdAccount in caas.DefaultIfEmpty()
                          join updatedAccount in _adminAccountRepository.Repo on placedOrder.UpdatedStaffId equals updatedAccount.Id into uaas
                          from updatedAccount in uaas.DefaultIfEmpty()
                          join customer in _customerRepository.Repo on placedOrder.CustomerId equals customer.Id into cs
                          from customer in cs.DefaultIfEmpty()
                          join process in _orderProcessRepository.Repo on placedOrder.OrderProcessId equals process.Id into ps
                          from process in ps.DefaultIfEmpty()
                          select new PlacedOrderDto
                          {
                              Id = placedOrder.Id,
                              RestaurantId = placedOrder.RestaurantId,
                              RestaurantIdName = restaurant.Name,
                              BranchId = placedOrder.BranchId,
                              BranchIdName = branch.Name,
                              OrderTypeId = placedOrder.OrderTypeId,
                              OrderTypeIdName = AOrderType.OrderType[placedOrder.OrderTypeId.GetValueOrDefault()],
                              CustomerId = placedOrder.CustomerId,
                              CustomerIdName = customer.Name,
                              CustomerIdAddress = customer.Address,
                              CustomerIdEmail = customer.Email,
                              CustomerIdPhone = customer.Phone,
                              OrderChannelId = placedOrder.OrderChannelId,
                              OrderChannelIdName = orderChannel.Name,
                              TableId = placedOrder.TableId,
                              TableIdName = table.Name,
                              Code = placedOrder.Code,
                              OrderProcessId = placedOrder.OrderProcessId,
                              OrderProcessIdName = process.Name,
                              OrderProcessIdColor = process.Color,
                              PeopleNum = placedOrder.PeopleNum.GetValueOrDefault(),
                              CustomerName = placedOrder.CustomerName,
                              CustomerPhone = placedOrder.CustomerPhone,
                              OrderTime = placedOrder.OrderTime,
                              DeliveryTime = placedOrder.DeliveryTime,
                              DeliveryAddress = placedOrder.DeliveryAddress,
                              Tax = placedOrder.Tax,
                              Price = placedOrder.Price,
                              DiscountType = placedOrder.DiscountType,
                              Discount = placedOrder.Discount,
                              FinalPrice = placedOrder.FinalPrice,
                              Description = placedOrder.Description,
                              Status = placedOrder.Status.GetValueOrDefault(),
                              CreatedStaffId = placedOrder.CreatedStaffId,
                              CreatedStaffIdName = createdAccount.UserName,
                              CreatedDate = placedOrder.CreatedDate,
                              UpdatedStaffId = updatedAccount.Id,
                              UpdatedStaffIdName = updatedAccount.UserName,
                              UpdatedDate = placedOrder.UpdatedDate
                          })
                          .ToFilterByRole(f => f.RestaurantId, f => f.BranchId, restaurantId, branchId)
                          .Where(c => c.Status < (int)EStatus.All && (c.OrderProcessId == (int)EOrderProcess.WaitingOrder ||
                          c.OrderProcessId == (int)EOrderProcess.PreparingOrder || c.OrderProcessId == (int)EOrderProcess.AddMoreOrder))
                          .OrderByDescending(c => c.OrderTime)
                          .ToListAsync();
            return result;
        }
    }
}
