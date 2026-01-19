using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using SEM_Drahos.Data.entities;

namespace SEM_Drahos.Data;

public sealed class PotDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<VoxelModel> VoxelModels => Set<VoxelModel>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseMongoDB("mongodb://localhost:27017", "pot");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToCollection("voxel-render-users");
        modelBuilder.Entity<VoxelModel>().ToCollection("voxel-render-models");
    }
}