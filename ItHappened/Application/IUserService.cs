using System;
using ItHappened.Domain;
using LanguageExt;

namespace ItHappened.Application
{
    public interface IUserService
    {
        void CreateUser(UserForm form);
        void EditUser(Guid actorId, Guid userId, UserForm form);
        void DeleteUser(Guid actorId, Guid userId);
        Option<User> GetUserById(Guid actorId, Guid userId);
        Option<User> LogInByCredentials(string username, string password);
    }
}