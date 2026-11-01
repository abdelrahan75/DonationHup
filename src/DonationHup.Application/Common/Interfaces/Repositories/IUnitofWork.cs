using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Common.Interfaces.Repositories
{
    public interface IUnitofWork
    {
        Task<int> CompleteAsync(CancellationToken cancellationToken = default);

    }
}
