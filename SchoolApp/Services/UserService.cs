using AutoMapper;
using SchoolApp.Core;
using SchoolApp.Core.Filters;
using SchoolApp.DTO;
using SchoolApp.Models;
using SchoolApp.Repositories;
using SchoolApp.Security;

namespace SchoolApp.Services
{
    public class UserService : IUserService
    {
        private readonly IEncryptionUtil _encryptionUtil;
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(IEncryptionUtil encryptionUtil, IUnitOfWork unitOfWork, 
            IMapper mapper, ILogger<UserService> logger)
        {
            _encryptionUtil = encryptionUtil;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public Task<PaginatedResult<UserReadOnlyDTO>> GetPaginatedUsersFilteredAsync(int pageNumber, int pageSize, UserFiltersDTO userFiltersDTO)
        {
            throw new NotImplementedException();
        }

        public Task<UserReadOnlyDTO?> GetUserByUsernameAsync(string username)
        {
            throw new NotImplementedException();
        }

        public Task<User?> VerifyAndGetUserAsync(UserLoginDTO credentials)
        {
            throw new NotImplementedException();
        }
    }
}
