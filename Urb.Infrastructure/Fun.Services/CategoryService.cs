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
    public class CategoryService: ICategoryService
    {
        private readonly ICRUDRepository<Category> _repository;
        private readonly IMapper _mapper;

        public CategoryService(ICRUDRepository<Category> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Category> CreateAsync(ICategoryComponentModel category)
        {
            var all = await _repository.ListAsync();
            if (all.Any(c => c.CategoryName == category.CategoryName))
                throw new ArgumentException(
                    $"Category with name «{category.CategoryName}» already exist.");
            var newCategory = _mapper.Map<Category>(category);
            var created = await _repository.Create(newCategory);
            return newCategory;
        }
    }
}
