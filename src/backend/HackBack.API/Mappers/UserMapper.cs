using HackBack.Contracts.ApiContracts;
using HackBack.Domain.Entities;

namespace HackBack.API.Mappers
{
    public static class UserMapper
    {
        public static UserPublic Map(this UserEntity user)
            => new(user.Id, user.Name, user.Email, user.Roles.Select(r => r.ToString()));
    }
}
