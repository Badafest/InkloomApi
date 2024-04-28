using Inkloom.Api.Data;
using Inkloom.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Inkloom.Api.Test;

public class TestDataContext(DbContextOptions<DataContext> options) : DataContext(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<User>().HasData(UserSeed.data);
    }
};