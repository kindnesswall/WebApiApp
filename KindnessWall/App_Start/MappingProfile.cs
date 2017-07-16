using AutoMapper;
using KindnessWall.Dto.Category;
using KindnessWall.Dto.Gift;
using KindnessWall.Dto.User;
using KindnessWall.Helper;
using KindnessWall.Models;
using System;
using KindnessWall.Dto.Bookmark;
using KindnessWall.Dto.Request;

namespace KindnessWall.App_Start
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DateTime, string>().ConvertUsing(new DateTimeToPersianDateTimeConverter());

            CreateMap<Gift, string>().ConvertUsing(x => x.Title);
            CreateMap<GiftImage, string>().ConvertUsing(x => x.ImageUrl);
            CreateMap<Gift, GiftPublicItemDto>();
            CreateMap<Gift, GiftPrivateItemDto>();
            CreateMap<Gift, GiftPublicListItemDto>();
            CreateMap<Gift, GiftPrivateListItemDto>();
            CreateMap<Gift, GiftRequestedListItemDto>();
            CreateMap<GiftAddDto, Gift>();
            CreateMap<GiftEditDto, Gift>();



            CreateMap<Category, string>().ConvertUsing(x => x?.Title ?? "");
            CreateMap<Category, CategoryItemDto>();
            CreateMap<CategoryAddDto, Category>();
            CreateMap<CategoryEditDto, Category>();

            /*CreateMap<Location, string>().ConvertUsing(x => x?.Title ?? "");
            CreateMap<Location, LocationItemDto>();
            CreateMap<LocationAddDto, Location>();
            CreateMap<LocationEditDto, Location>();*/

            CreateMap<ApplicationUser, string>().ConvertUsing(x => x?.Mobile);
            CreateMap<ApplicationUser, UserPublicItemDto>();
            CreateMap<ApplicationUser, UserPrivateItemDto>();

            CreateMap<Request, RequestItemDto>();
            CreateMap<RequestItemDto, Request>();
            CreateMap<RequestAddDto, Request>();

            CreateMap<Bookmark, BookmarkItemDto>();

        }
    }
}