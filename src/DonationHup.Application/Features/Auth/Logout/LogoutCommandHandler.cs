using DonationHup.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Auth.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
    {
        private readonly IAuthService _auth;

        public LogoutCommandHandler(IAuthService auth)
        {
            _auth = auth;
        }

        public  Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)  
            =>  _auth.LogoutAsync(request , cancellationToken);
        
    }
}
