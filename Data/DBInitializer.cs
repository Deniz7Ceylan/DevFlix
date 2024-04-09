using Microsoft.AspNetCore.Identity;
using DevFlix.Models;
using Microsoft.EntityFrameworkCore;

namespace DevFlix.Data;

public class DBInitializer
{
    public DBInitializer(DevFlixContext? context, RoleManager<DevFlixRole>? roleManager, UserManager<DevFlixUser>? userManager)
    {
        DevFlixRole devFlixRole;
        DevFlixUser devFlixUser;
        Restriction restriction;

        if (context != null)
        {
            context.Database.Migrate();
            
            if (!context.Restrictions.Any())
            {
                restriction = new Restriction();
                restriction.Name = "Genel İzleyici";
                restriction.Id = 0;
                context.Restrictions.Add(restriction);
                restriction = new Restriction();
                restriction.Name = "7 Yaş ve Üzeri";
                restriction.Id = 7;
                context.Restrictions.Add(restriction);
                restriction = new Restriction();
                restriction.Name = "13 Yaş ve Üzeri";
                restriction.Id = 13;
                context.Restrictions.Add(restriction);
                restriction = new Restriction();
                restriction.Name = "18 Yaş ve Üzeri";
                restriction.Id = 18;
                context.Restrictions.Add(restriction);
                restriction = new Restriction();
                restriction.Name = "Şiddet/Korku";
                restriction.Id = 19;
                context.Restrictions.Add(restriction);
                restriction = new Restriction();
                restriction.Name = "Olumsuz Örnek Oluşturabilecek Davranışlar";
                restriction.Id = 20;
                context.Restrictions.Add(restriction);
                restriction = new Restriction();
                restriction.Name = "Cinsellik";
                restriction.Id = 21;
                context.Restrictions.Add(restriction);
            }

            context.SaveChanges();
        }
        if (roleManager != null)
        {
            if (roleManager.Roles.Count() == 0)
            {
                devFlixRole = new DevFlixRole("Administrator");
                roleManager.CreateAsync(devFlixRole).Wait();
                devFlixRole = new DevFlixRole("ContentAdmin");
                roleManager.CreateAsync(devFlixRole).Wait();
            }
        }
        if (userManager != null)
        {
            if (userManager.Users.Count() == 0)
            {
                devFlixUser = new DevFlixUser();
                devFlixUser.UserName = "Administrator";
                devFlixUser.Email = "devadmin@sample.com";
                devFlixUser.Name = "Administrator";
                devFlixUser.PhoneNumber = "1234567890";
                devFlixUser.Passive = false;
                userManager.CreateAsync(devFlixUser, "Admin.123").Wait();
                userManager.AddToRoleAsync(devFlixUser, "Administrator").Wait();

                devFlixUser = new DevFlixUser();
                devFlixUser.UserName = "ContentAdmin";
                devFlixUser.Email = "devcontent@sample.com";
                devFlixUser.Name = "Content Admin";
                devFlixUser.PhoneNumber = "1234567890";
                devFlixUser.Passive = false;
                userManager.CreateAsync(devFlixUser, "Admin.123").Wait();
                userManager.AddToRoleAsync(devFlixUser, "ContentAdmin").Wait();
            }
        }
    }
}
