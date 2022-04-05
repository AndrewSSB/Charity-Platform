using AutoMapper;
using ProiectSoft.DAL.Entities;
using ProiectSoft.DAL.Models.CasesModels;
using ProiectSoft.DAL.Models.DonationModels;
using ProiectSoft.DAL.Models.LocationModels;
using ProiectSoft.DAL.Models.OrganisationModels;
using ProiectSoft.DAL.Models.RefugeeModels;
using ProiectSoft.DAL.Models.ShelterModels;
using ProiectSoft.DAL.Models.UserModels;
using ProiectSoft.DAL.Models.VolunteerModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.DAL.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            CreateMap<Cases, CasesGetModel>().ReverseMap();
            CreateMap<Cases, CasesPostModel>().ReverseMap();
            CreateMap<Cases, CasesPutModel>().ReverseMap();

            CreateMap<Donation, DonationGetModel>().ReverseMap();
            CreateMap<Donation, DonationPostModel>().ReverseMap();
            //CreateMap<Donation, DonationPutModel>().ReverseMap();

            CreateMap<Location, LocationGetModel>().ReverseMap();
            CreateMap<Location, LocationPostModel>().ReverseMap();
            CreateMap<Location, LocationPutModel>().ReverseMap();

            CreateMap<Organisation, OrganisationGetModel>().ReverseMap();
            CreateMap<Organisation, OrganisationPostModel>().ReverseMap();
            //CreateMap<Organisation, OrganisationGetModel>().ReverseMap();

            CreateMap<Refugee, RefugeeGetModel>().ReverseMap();
            CreateMap<Refugee, RefugeePostModel>().ReverseMap();
            //CreateMap<Refugee, RefugeePutModel>().ReverseMap();

            CreateMap<Shelter, ShelterGetModel>().ReverseMap();
            CreateMap<Shelter, ShelterPostModel>().ReverseMap();

            CreateMap<User, UserGetModel>().ReverseMap();
            CreateMap<User, UserPutModel>().ReverseMap();

            CreateMap<Volunteer, VolunteerGetModel>().ReverseMap();
            CreateMap<Volunteer, VolunteerPostModel>().ReverseMap();
        } 
    }
}
