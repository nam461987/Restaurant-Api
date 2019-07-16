using AutoMapper;
using Restaurant.Common.Dtos.AdminAccount;
using Restaurant.Common.Dtos.Ingredient;
using Restaurant.Common.Dtos.Menu;
using Restaurant.Common.Dtos.MenuCategory;
using Restaurant.Common.Dtos.MenuPrice;
using Restaurant.Common.Dtos.MenuSize;
using Restaurant.Common.Dtos.MenuUnit;
using Restaurant.Common.Dtos.OrderChannel;
using Restaurant.Common.Dtos.OrderChannelTime;
using Restaurant.Common.Dtos.Restaurant;
using Restaurant.Common.Dtos.RestaurantBranch;
using Restaurant.Entities.Models;

namespace Restaurant.API.AutoMapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<AdminAccount, AccountDto>();
            CreateMap<AccountDto, AdminAccount>();

            CreateMap<RestaurantBranch, BranchDto>();
            CreateMap<BranchDto, RestaurantBranch>();

            CreateMap<Ingredient, IngredientDto>();
            CreateMap<IngredientDto, Ingredient>();

            CreateMap<MenuCategory, MenuCategoryDto>();
            CreateMap<MenuCategoryDto, MenuCategory>();

            CreateMap<MenuUnit, MenuUnitDto>();
            CreateMap<MenuUnitDto, MenuUnit>();

            CreateMap<OrderChannel, OrderChannelDto>();
            CreateMap<OrderChannelDto, OrderChannel>();

            CreateMap<OrderChannelTime, OrderChannelTimeDto>();
            CreateMap<OrderChannelTimeDto, OrderChannelTime>();
            
            CreateMap<MenuSize, MenuSizeDto>();
            CreateMap<MenuSizeDto, MenuSize>();

            CreateMap<MenuPrice, MenuPriceDto>();
            CreateMap<MenuPriceDto, MenuPrice>();

            CreateMap<Menu, MenuDto>();
            CreateMap<MenuDto, Menu>();

            CreateMap<Entities.Models.Restaurant, RestaurantDto>();
            CreateMap<RestaurantDto, Entities.Models.Restaurant>();
        }
    }
}
