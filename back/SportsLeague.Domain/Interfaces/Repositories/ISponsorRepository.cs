using SportsLeague.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsLeague.Domain.Interfaces.Repositories
{
    public interface ISponsorRepository: IGenericRepository<Sponsor>
    {
        Task<bool> ExistsByNameAsync(string name);
    }
}
