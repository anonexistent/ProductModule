using EfiritPro.Retail.Packages.Errors.Models;
using EfiritPro.Retail.ProductModule.Models;
using EfiritPro.Retail.ProductModule.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EfiritPro.Retail.ProductModule.Api.Services;

public class VATService
{
    private readonly ProductDbContext _productDbContext;

    public VATService(ProductDbContext productDbContext)
    {
        _productDbContext = productDbContext;
    }
    
    public async Task<ServiceAnswer<VAT>> CreateVAT(string name, ushort percent)
    {
        var vat = new VAT()
        {
            Name = name,
            Percent = percent
        };

        await _productDbContext.VATs.AddAsync(vat);
        await _productDbContext.SaveChangesAsync();

        return new ServiceAnswer<VAT>()
        {
            Ok = true,
            Answer = vat,
        };
    }

    public async Task<ServiceAnswer<VAT>> GetVATById(string id)
    {
        if (Guid.TryParse(id, out var guid))
            return await GetVATById(guid);
        return new ServiceAnswer<VAT>()
        {
            Ok = false,
            Errors = new[]
            {
                new ServiceFieldError()
                {
                    Fields = new[] { "vatId" },
                    Message = "vatId не соответствует формату."
                }
            }
        };
    }

    public async Task<ServiceAnswer<VAT>> GetVATById(Guid id)
    {
        var vat = await _productDbContext.VATs
            .FirstOrDefaultAsync(vat => vat.Id == id);

        return vat is not null
            ? new ServiceAnswer<VAT>()
            {
                Ok = true,
                Answer = vat
            }
            : new ServiceAnswer<VAT>()
            {
                Ok = false,
                Errors = new[]
                {
                    new ServiceFieldError()
                    {
                        Fields = new[] { "vatId" },
                        Message = "НДС не найден."
                    }
                }
            };
    }

    public async Task<ServiceAnswer<ICollection<VAT>>> GetVATList()
    {
        return new ServiceAnswer<ICollection<VAT>>()
        {
            Ok = true,
            Answer = await _productDbContext.VATs.ToArrayAsync()
        };
    }

    public async Task<ServiceAnswer<VAT>> UpdateVAT(string id, string name, ushort percent)
    {
        if (Guid.TryParse(id, out var guid))
            return await UpdateVAT(guid, name, percent);
        return new ServiceAnswer<VAT>()
        {
            Ok = false,
            Errors = new[]
            {
                new ServiceFieldError()
                {
                    Fields = new[] { "vatId" },
                    Message = "vatId не соответствует формату."
                }
            }
        };
    }

    private async Task<ServiceAnswer<VAT>> UpdateVAT(Guid id, string name, ushort percent)
    {
        var getVat = await GetVATById(id);
        if (!getVat.Ok || getVat.Answer is null) return getVat;

        var vat = getVat.Answer;
        
        vat.Name = name;
        vat.Percent = percent;
        _productDbContext.VATs.Update(vat);
        await _productDbContext.SaveChangesAsync();

        return new ServiceAnswer<VAT>()
        {
            Ok = true,
            Answer = vat,
        };
    }

    public async Task RemoveVAT(string id)
    {
        if (Guid.TryParse(id, out var guid))
            await RemoveVAT(guid);
    }

    public async Task RemoveVAT(Guid id)
    {
        var vat = (await GetVATById(id)).Answer;

        if (vat is not null)
        {
            _productDbContext.VATs.Remove(vat);
            await _productDbContext.SaveChangesAsync();
        }
    }
}