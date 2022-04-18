using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

using CloverleafTrack.Data;
using CloverleafTrack.Models;
using CloverleafTrack.Models.TrackEvents;

using Microsoft.EntityFrameworkCore;

namespace CloverleafTrack.Managers;

public interface IEventManager<TEventType>
{
    public Task<int> CountAsync();
    public Task<ImmutableList<TEventType>> GetEventsAsync(Gender gender, Environment environment);
    public ValueTask CreateAsync(TEventType trackEvent);
}

public class FieldEventManager : IEventManager<FieldEvent>
{
    private readonly CloverleafTrackDataContext db;
    private List<FieldEvent> cache = new();

    public FieldEventManager(CloverleafTrackDataContext db)
    {
        this.db = db;
    }

    public async Task<int> CountAsync()
    {
        if (!cache.Any())
        {
            await RefreshCacheAsync();
        }

        return cache.Count;
    }

    public async Task<ImmutableList<FieldEvent>> GetEventsAsync(Gender gender, Environment environment)
    {
        if (!cache.Any())
        {
            await RefreshCacheAsync();
        }

        return cache.Where(fe => fe.Gender == gender && fe.Environment == environment).ToImmutableList();
    }
    
    public async ValueTask CreateAsync(FieldEvent trackEvent)
    {
        await db.FieldEvents.AddAsync(trackEvent);
        await db.SaveChangesAsync();
        
        await RefreshCacheAsync();
    }

    private async Task RefreshCacheAsync()
    {
        cache.Clear();
        cache.TrimExcess();
        cache = await db.FieldEvents.Where(fe => !fe.Deleted).OrderBy(fe => fe.SortOrder).ToListAsync();
    }
}

public class FieldRelayEventManager : IEventManager<FieldRelayEvent>
{
    private readonly CloverleafTrackDataContext db;
    private List<FieldRelayEvent> cache = new();

    public FieldRelayEventManager(CloverleafTrackDataContext db)
    {
        this.db = db;
    }

    public async Task<int> CountAsync()
    {
        if (!cache.Any())
        {
            await RefreshCacheAsync();
        }

        return cache.Count;
    }

    public async Task<ImmutableList<FieldRelayEvent>> GetEventsAsync(Gender gender, Environment environment)
    {
        if (!cache.Any())
        {
            await RefreshCacheAsync();
        }

        return cache.Where(fe => fe.Gender == gender && fe.Environment == environment).ToImmutableList();
    }
    
    public async ValueTask CreateAsync(FieldRelayEvent trackEvent)
    {
        await db.FieldRelayEvents.AddAsync(trackEvent);
        await db.SaveChangesAsync();
        
        await RefreshCacheAsync();
    }

    private async Task RefreshCacheAsync()
    {
        cache.Clear();
        cache.TrimExcess();
        cache = await db.FieldRelayEvents.Where(fe => !fe.Deleted).OrderBy(fe => fe.SortOrder).ToListAsync();
    }
}

public class RunningEventManager : IEventManager<RunningEvent>
{
    private readonly CloverleafTrackDataContext db;
    private List<RunningEvent> cache = new();

    public RunningEventManager(CloverleafTrackDataContext db)
    {
        this.db = db;
    }

    public async Task<int> CountAsync()
    {
        if (!cache.Any())
        {
            await RefreshCacheAsync();
        }

        return cache.Count;
    }

    public async Task<ImmutableList<RunningEvent>> GetEventsAsync(Gender gender, Environment environment)
    {
        if (!cache.Any())
        {
            await RefreshCacheAsync();
        }

        return cache.Where(fe => fe.Gender == gender && fe.Environment == environment).ToImmutableList();
    }
    
    public async ValueTask CreateAsync(RunningEvent trackEvent)
    {
        await db.RunningEvents.AddAsync(trackEvent);
        await db.SaveChangesAsync();
        
        await RefreshCacheAsync();
    }

    private async Task RefreshCacheAsync()
    {
        cache.Clear();
        cache.TrimExcess();
        cache = await db.RunningEvents.Where(fe => !fe.Deleted).OrderBy(fe => fe.SortOrder).ToListAsync();
    }
}

public class RunningRelayEventManager : IEventManager<RunningRelayEvent>
{
    private readonly CloverleafTrackDataContext db;
    private List<RunningRelayEvent> cache = new();

    public RunningRelayEventManager(CloverleafTrackDataContext db)
    {
        this.db = db;
    }

    public async Task<int> CountAsync()
    {
        if (!cache.Any())
        {
            await RefreshCacheAsync();
        }

        return cache.Count;
    }

    public async Task<ImmutableList<RunningRelayEvent>> GetEventsAsync(Gender gender, Environment environment)
    {
        if (!cache.Any())
        {
            await RefreshCacheAsync();
        }

        return cache.Where(fe => fe.Gender == gender && fe.Environment == environment).ToImmutableList();
    }
    
    public async ValueTask CreateAsync(RunningRelayEvent trackEvent)
    {
        await db.RunningRelayEvents.AddAsync(trackEvent);
        await db.SaveChangesAsync();
        
        await RefreshCacheAsync();
    }

    private async Task RefreshCacheAsync()
    {
        cache.Clear();
        cache.TrimExcess();
        cache = await db.RunningRelayEvents.Where(fe => !fe.Deleted).OrderBy(fe => fe.SortOrder).ToListAsync();
    }
}
