using EfiritPro.Retail.ProductModule.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EfiritPro.Retail.ProductModule.Api.Services;

public class ProductGroupTreeService
{
    private readonly ProductDbContext _db;
    private const int GroupDeepLimit = 10;

    public ProductGroupTreeService(ProductDbContext db)
    {
        _db = db;
    }

    public async Task<IDictionary<Guid, Guid?>> GetDictTree(Guid ownerId, Guid organizationId) =>
        (await _db.ProductGroups
            .Where(pg => pg.OwnerId == ownerId && pg.OrganizationId == organizationId)
            .ToArrayAsync())
        .ToDictionary(pg => pg.Id, pgm => pgm.ParentGroupId);

    public IDictionary<Guid, ICollection<Guid>> GetReversedDictTree(IDictionary<Guid, Guid?> dictTree)
    {
        var reversedDictTree = new Dictionary<Guid, ICollection<Guid>>();

        foreach (var (childId, parentId) in dictTree)
        {
            if (!parentId.HasValue) continue;
            var group = reversedDictTree.TryGetValue(parentId.Value, out var innerGroup)
                ? innerGroup
                : new List<Guid>();
            group.Add(childId);
            reversedDictTree[parentId.Value] = group;
        }

        return reversedDictTree;
    }

    public async Task<bool> CheckGroupLimitations(Guid childGroupId, Guid parentGroupId, Guid ownerId, Guid organizationId)
    {
        if (childGroupId == parentGroupId) return false;

        var groupMemberships = await GetDictTree(ownerId, organizationId);

        if (groupMemberships.TryGetValue(childGroupId, out var childGroupParentId) &&
            childGroupParentId == parentGroupId) return true;

        var (deep, cycle) = GetGroupDeep(parentGroupId, childGroupId, groupMemberships);
        return !cycle && deep <= GroupDeepLimit;
    }
    
    private (int deep, bool cycle) GetGroupDeep(Guid rootGroupId, Guid childGroupId, IDictionary<Guid, Guid?> groupMemberships)
    {
        var deep = 1;
        var cycle = false;

        while (groupMemberships.TryGetValue(rootGroupId, out var newRoot) && newRoot.HasValue)
        {
            rootGroupId = newRoot.Value;
            deep++;
            if (rootGroupId == childGroupId) cycle = true;
        }

        var reversedGroupMembership = GetReversedDictTree(groupMemberships);

        if (cycle) return (deep, cycle);

        var childrenToApprove = new Queue<(Guid newChild, int deep)>();
        childrenToApprove.Enqueue((childGroupId, deep + 1));
        
        while (childrenToApprove.TryDequeue(out var child))
        {
            if (child.deep > deep) deep = child.deep;
            if (!reversedGroupMembership.TryGetValue(child.newChild, out var newChildren)) continue;
            
            foreach (var newChild in newChildren)
                childrenToApprove.Enqueue((newChild, child.deep + 1));
        }
        
        return (deep, cycle);
    }
}