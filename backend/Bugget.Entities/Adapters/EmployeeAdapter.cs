using Bugget.Entities.BO;
using Bugget.Entities.Views.Users;

namespace Bugget.Entities.Adapters;

public static class EmployeeAdapter
{    
    public static UserView ToUserView(Employee e)
    {
        return new UserView { Id = e.Id, Name = e.Name, TeamId = e.TeamId, PhotoUrl = e.PhotoUrl };
    }
    
    public static UserView ToUserView(string userId)
    {
        return new UserView { Id = userId, Name = string.Empty, TeamId = null, PhotoUrl = null };
    }
}