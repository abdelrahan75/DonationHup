using DonationHup.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHub.Infrastructure.data.Interceptor;
public class SoftDeleteInterceptor : SaveChangesInterceptor
{
    private readonly TimeProvider _time;

    public SoftDeleteInterceptor(TimeProvider time)
    {
        _time = time;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, InterceptionResult<int> result)
    {
        SoftDelete(eventData.Context);
        return result;
    }
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        SoftDelete(eventData.Context);
        return ValueTask.FromResult(result);
    }

    private void SoftDelete(DbContext? context)
    {
        if (context is null) return;

        foreach (var entry in context.ChangeTracker.Entries<ISoftDeletable>().ToList())
        {
            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAtUtc = _time.GetUtcNow().UtcDateTime;
            }
        }
    }
}
    

