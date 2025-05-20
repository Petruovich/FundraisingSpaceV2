using AutoMapper;
using Fun.Application.Fun.IRepositories;
using Fun.Application.Fun.IServices;
using Fun.Application.IComponentModels;
using Fun.Domain.Fun.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun.Infrastructure.Fun.Services
{
    public class SubscribeService : ISubscribeService
    {
        private readonly ICRUDRepository<Subscribe> _repo;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public SubscribeService(
            ICRUDRepository<Subscribe> repo,
            IUserService userService,
            IMapper mapper)
        {
            _repo = repo;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<Subscribe> CreateAsync(ISubscribeComponentModel model)
        {
            var userId = await _userService.GetMy();
            var exists = (await _repo.ListAsync())
                .Any(s => s.UserId == userId && s.InitiativeId == model.InitiativeId);
            if (exists)
                throw new ArgumentException("You have already subscribed to this initiative");

            var subscribe = _mapper.Map<Subscribe>(model);
            subscribe.UserId = userId;

            return await _repo.Create(subscribe);
        }
    }
}
