using EfiritPro.Retail.Packages.Errors.Models;
using EfiritPro.Retail.ProductModule.Models;
using EfiritPro.Retail.ProductModule.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EfiritPro.Retail.ProductModule.Api.Services;

public class MarkingTypeService
{
    private readonly ProductDbContext _productDbContext;

    public MarkingTypeService(ProductDbContext productDbContext)
    {
        _productDbContext = productDbContext;
    }

    public async Task<ServiceAnswer<MarkingType>> CreateMarkingType(string name)
    {
        var markingType = new MarkingType()
        {
            Name = name
        };

        await _productDbContext.MarkingTypes.AddAsync(markingType);
        await _productDbContext.SaveChangesAsync();

        return new ServiceAnswer<MarkingType>()
        {
            Ok = true,
            Answer = markingType,
        };
    }

    public async Task<ServiceAnswer<MarkingType>> GetMarkingTypeById(string id)
    {
        if (Guid.TryParse(id, out var guid))
            return await GetMarkingTypeById(guid);

        return new ServiceAnswer<MarkingType>()
        {
            Ok = false,
            Errors = new[]
            {
                new ServiceFieldError()
                {
                    Fields = new[] { "markingTypeId" },
                    Message = "markingTypeId не соответствует формату."
                }
            }
        };
    }

    public async Task<ServiceAnswer<MarkingType>> GetMarkingTypeById(Guid id)
    {
        var markingType = await _productDbContext.MarkingTypes.FirstOrDefaultAsync(mt => mt.Id == id);

        return markingType is not null
            ? new ServiceAnswer<MarkingType>()
            {
                Ok = true,
                Answer = markingType
            }
            : new ServiceAnswer<MarkingType>()
            {
                Ok = false,
                Errors = new[]
                {
                    new ServiceFieldError()
                    {
                        Fields = new[] { "markingTypeId" },
                        Message = "Тип маркировки не найден."
                    }
                }
            };
    }

    public async Task<ServiceAnswer<ICollection<MarkingType>>> GetMarkingTypeList()
    {
        return new ServiceAnswer<ICollection<MarkingType>>()
        {
            Ok = true,
            Answer = await _productDbContext.MarkingTypes.ToArrayAsync(),
        };
    }

    public async Task<ServiceAnswer<MarkingType>> UpdateMarkingType(string id, string name)
    {
        if (Guid.TryParse(id, out var guid))
            return await UpdateMarkingType(guid, name);

        return new ServiceAnswer<MarkingType>()
        {
            Ok = false,
            Errors = new[]
            {
                new ServiceFieldError()
                {
                    Fields = new[] { "markingTypeId" },
                    Message = "markingTypeId не соответствует формату."
                }
            }
        };
    }

    public async Task<ServiceAnswer<MarkingType>> UpdateMarkingType(Guid id, string name)
    {
        var markingType = await GetMarkingTypeById(id);
        if (!markingType.Ok || markingType.Answer is null) return markingType;

        markingType.Answer.Name = name;
        _productDbContext.MarkingTypes.Update(markingType.Answer);
        await _productDbContext.SaveChangesAsync();

        return markingType;
    }

    public async Task RemoveMarkingType(string id)
    {
        if (Guid.TryParse(id, out var guid))
            await RemoveMarkingType(guid);
    }

    public async Task RemoveMarkingType(Guid id)
    {
        var markingType = await GetMarkingTypeById(id);
        if (!markingType.Ok || markingType.Answer is null) return;

        _productDbContext.MarkingTypes.Remove(markingType.Answer);
        await _productDbContext.SaveChangesAsync();
    }
}