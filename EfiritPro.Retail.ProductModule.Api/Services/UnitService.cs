using EfiritPro.Retail.Packages.Errors.Models;
using EfiritPro.Retail.ProductModule.Models;
using EfiritPro.Retail.ProductModule.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EfiritPro.Retail.ProductModule.Api.Services;

public class UnitService
{
    private readonly ProductDbContext _productDbContext;

    public UnitService(ProductDbContext productDbContext)
    {
        _productDbContext = productDbContext;
    }

    public async Task<ServiceAnswer<Unit>> CreateUnit(string name, ushort code)
    {
        var unit = new Unit()
        {
            Name = name,
            Code = code
        };

        await _productDbContext.Units.AddAsync(unit);
        await _productDbContext.SaveChangesAsync();

        return new ServiceAnswer<Unit>()
        {
            Ok = true,
            Answer = unit,
        };
    }

    public async Task<ServiceAnswer<Unit>> GetUnitById(string id)
    {
        if (Guid.TryParse(id, out var guid))
            return await GetUnitById(guid);

        return new ServiceAnswer<Unit>()
        {
            Ok = false,
            Errors = new[]
            {
                new ServiceFieldError()
                {
                    Fields = new[] { "unitId" },
                    Message = "unitId не соответствует формату."
                }
            }
        };
    }

    public async Task<ServiceAnswer<Unit>> GetUnitById(Guid id)
    {
        var unit = await _productDbContext.Units
            .FirstOrDefaultAsync(unit => unit.Id == id);

        return unit is not null
            ? new ServiceAnswer<Unit>()
            {
                Ok = true,
                Answer = unit
            }
            : new ServiceAnswer<Unit>()
            {
                Ok = false,
                Errors = new[]
                {
                    new ServiceFieldError()
                    {
                        Fields = new[] { "unitId" },
                        Message = "Единица измерения не найдена.",
                    },
                }
            };
    }

    public async Task<ServiceAnswer<ICollection<Unit>>> GetUnitList()
    {
        return new ServiceAnswer<ICollection<Unit>>()
        {
            Ok = true, 
            Answer = await _productDbContext.Units.ToArrayAsync()
        };
    }

    public async Task<ServiceAnswer<Unit>> UpdateUnit(string id, string name, ushort code)
    {
        var getUnit = await GetUnitById(id);
        if (!getUnit.Ok || getUnit.Answer is null) return getUnit;
        var unit = getUnit.Answer;

        unit.Name = name;
        unit.Code = code;

        _productDbContext.Units.Update(unit);
        await _productDbContext.SaveChangesAsync();

        return new ServiceAnswer<Unit>()
        {
            Ok = true,
            Answer = unit
        };
    }

    public async Task RemoveUnit(string id)
    {
        if (Guid.TryParse(id, out var guid))
            await RemoveUnit(guid);
    }

    public async Task RemoveUnit(Guid id)
    {
        var unit = (await GetUnitById(id)).Answer;

        if (unit is not null)
        {
            _productDbContext.Units.Remove(unit);
            await _productDbContext.SaveChangesAsync();
        }
    }
}