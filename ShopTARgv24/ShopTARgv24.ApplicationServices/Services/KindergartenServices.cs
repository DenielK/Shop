using Microsoft.EntityFrameworkCore;
using ShopTARgv24.Core.Domain;
using ShopTARgv24.Core.Dto;
using ShopTARgv24.Core.ServiceInterface;
using ShopTARgv24.Data;

namespace ShopTARgv24.ApplicationServices.Services
{
    public class KindergartenServices : IKindergartenServices
    {
        private readonly ShopTARgv24Context _context;
        private readonly IFileServices _fileServices;

        public KindergartenServices(
            ShopTARgv24Context context,
            IFileServices fileServices)
        {
            _context = context;
            _fileServices = fileServices;
        }

        // CREATE
        public async Task<Kindergarten> Create(KindergartenDto dto)
        {
            var kindergarten = new Kindergarten
            {
                Id = Guid.NewGuid(),
                GroupName = dto.GroupName,
                ChildrenCount = dto.ChildrenCount,
                KindergartenName = dto.KindergartenName,
                TeacherName = dto.TeacherName,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            if (dto.Files != null)
            {
                _fileServices.UploadFilesToDatabase(dto, kindergarten);
            }

            await _context.Kindergartens.AddAsync(kindergarten);
            await _context.SaveChangesAsync();

            return kindergarten;
        }

        // DETAILS
        public async Task<Kindergarten> DetailAsync(Guid id)
        {
            var result = await _context.Kindergartens
                .FirstOrDefaultAsync(x => x.Id == id);

            return result;
        }

        // UPDATE
        public async Task<Kindergarten?> Update(KindergartenDto dto)
        {
            if (dto.Id == null)
                return null;

            var domain = await _context.Kindergartens
                .FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (domain == null)
                return null;

            // Обновляем поля
            domain.GroupName = dto.GroupName;
            domain.ChildrenCount = dto.ChildrenCount;
            domain.KindergartenName = dto.KindergartenName;
            domain.TeacherName = dto.TeacherName;
            domain.CreatedAt = dto.CreatedAt;
            domain.UpdatedAt = dto.UpdatedAt ?? DateTime.Now;

            await _context.SaveChangesAsync();

            return domain;
        }

        // DELETE (+ удаление картинок)
        public async Task<Kindergarten?> Delete(Guid id)
        {
            var kindergarten = await _context.Kindergartens
                .FirstOrDefaultAsync(x => x.Id == id);

            if (kindergarten == null)
                return null;

            var images = await _context.KindergartenFileToDatabase
                .Where(x => x.KindergartenId == id)
                .Select(y => new FileToDatabaseDto
                {
                    Id = y.Id,
                    ImageData = y.ImageData,
                    ImageTitle = y.ImageTitle,
                    KindergartenId = y.KindergartenId
                })
                .ToArrayAsync();

            await _fileServices.RemoveImagesFromDatabase(images);

            _context.Kindergartens.Remove(kindergarten);
            await _context.SaveChangesAsync();

            return kindergarten;
        }
    }
}
