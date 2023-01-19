﻿using InnoGotchi.Application.Contracts.Repositories;
using InnoGotchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance.Repositories
{
    public class FarmRepository : RepositoryBase<Farm>, IFarmRepository
    {
        public FarmRepository(AppDbContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public async Task<IEnumerable<Farm>> GetFarmsAsync(bool trackChanges) =>
            await GetAll(trackChanges)
            .ToListAsync();

        public async Task<Farm> GetFarmByIdAsync(Guid id, bool trackChanges) =>
            await GetByCondition(f => f.Id == id, trackChanges)
            .SingleOrDefaultAsync();

        public async Task<Farm> GetFarmByUserIdAsync(Guid id, bool trackChanges) =>
            await GetByCondition(f => f.UserId == id, trackChanges)
            .SingleOrDefaultAsync();

        public async Task CreateFarm(Farm farm) =>
            await Create(farm);

        public void DelteFarm(Farm farm) =>
            Delete(farm);
    }
}