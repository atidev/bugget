using System.Text;
using Bugget.Entities.BO;
using Bugget.Entities.Views;
using Bugget.Entities.Views.Users;

namespace Bugget.Entities.Adapters;

public static class EmployeeAdapter
{
    public static EmployeeView Transform(Employee e)
    {
        var sb = new StringBuilder()
            .AppendWithSeparator(e.Surname)
            .AppendWithSeparator(e.FirstName)
            .AppendWithSeparator(e.LastName);

        var fullName = sb.ToString().TrimEnd();
        
        if(string.IsNullOrEmpty(fullName))
            fullName = e.Id;
        
        return new EmployeeView { Id = e.Id, Name = fullName };
    }
    
    public static UserView ToUserView(Employee e)
    {
        var sb = new StringBuilder()
            .AppendWithSeparator(e.Surname)
            .AppendWithSeparator(e.FirstName)
            .AppendWithSeparator(e.LastName);
        
        return new UserView { Id = e.Id, Name = sb.ToString().TrimEnd() };
    }
    
    public static UserView ToUserView(string userId)
    {
        return new UserView { Id = userId, Name = string.Empty };
    }

    private static StringBuilder AppendWithSeparator(this StringBuilder sb, string? addValue)
    {
        const char separator = ' ';
        return string.IsNullOrWhiteSpace(addValue) ? sb : sb.Append(addValue).Append(separator);
    }
}