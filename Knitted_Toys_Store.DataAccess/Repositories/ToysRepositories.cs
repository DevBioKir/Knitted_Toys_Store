using AutoMapper;
using Knitted_Toys_Store.DataAccess.Entities;
using Knitted_Toys_Store.Domain.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Knitted_Toys_Store.DataAccess.Repositories
{
    public class ToysRepositories : IToysRepositories
    {
        private readonly Knitted_Toys_StoreDBContext _context;
        private readonly IMapper _mapper;

        public ToysRepositories(Knitted_Toys_StoreDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<Toy>> GetAllToysAsync() //Получение всех игрушек //Возвращает Toy из типа Domain
        {
            var entitiesToy = await _context.Toys //получение Entties Toy
                .AsNoTracking() //не отслеживать изменения
                .ToListAsync();

            var toys = entitiesToy.Select(t => Toy.Load(
                t.Id, t.Name, t.Description, t.Size, t.Price, t.ImageUrl
                )).ToList();

            return toys;
        }

        public async Task<Guid> CreateToyAsync(Toy toy) //Создать игрушку
        {
            var entitiesToy = new ToyEntity //создание экземпляра игрушки
            {
                Name = toy.Name,
                Description = toy.Description,
                Size = toy.Size,
                Price = toy.Price,
                ImageUrl = toy.ImageUrl
            };
            await _context.Toys.AddAsync(entitiesToy);//создание игрушки и передача в Toy Entities
            await _context.SaveChangesAsync(); //сохраняем изменение

            return entitiesToy.Id; //получаем id игрушки, чтобы удостоверится что была создана игрушка в БД
        }

        public async Task<Guid> UpdateAsync(Guid id, string name, string description, string size, decimal price, string imageUrl)
        {
            var toys = await _context.Toys.FirstOrDefaultAsync(t => t.Id == id); //поиск игрушки по Id
            if (toys == null) throw new KeyNotFoundException($"Toy with ID {id} not found"); //если такой игрушки нет, выкинуть исключение

            await _context.Toys
                .Where(t => t.Id == id) //найдем запись по id и изменим через SetProperty данные игрушки
                .ExecuteUpdateAsync(s => s
                    .SetProperty(t => t.Name, t => name)
                    .SetProperty(t => t.Description, t => description)
                    .SetProperty(t => t.Size, t => size)
                    .SetProperty(t => t.Price, t => price)
                    .SetProperty(t => t.ImageUrl, t => imageUrl));

            return id; //выведем как результат успеха операции
        }

        public async Task<Guid> DeleteAsync(Guid id)
        {
            var toy = await _context.Toys
                .Where(t => t.Id == id)
                .ExecuteDeleteAsync();

            if (toy == null)
            {
                throw new ArgumentException("Toy not found.");
            }
            await _context.SaveChangesAsync();
            return id;
        }

        public async Task<Toy?> GetToyByIdAsync(Guid id)
        {
            var entityToy = await _context.Toys.FindAsync(id);

            return _mapper.Map<Toy>(entityToy);
        }
    }
}
