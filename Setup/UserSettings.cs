//---imports---
using System.DirectoryServices.AccountManagement;
using WindowsSetupTool.Helpers;
//---namespace---
namespace WindowsSetupTool.Setup
{
    //---class init---
    public class UserSettings
    {
        //---method for creating a user (optional with admin privileges)---
        public static void CreateLocalUser(string user, string password, bool isAdmin = false)
        {
            try
            {
                using PrincipalContext context = new(ContextType.Machine);

                //---check if already exists---
                UserPrincipal? existingUser = UserPrincipal.FindByIdentity(context, user);
                if (existingUser != null)
                {
                    Logger.Warning($"User:'{user}' already exists - skipped this task.");
                    return;
                }

                //---add new user---
                UserPrincipal newUser = new(context)
                {
                    Name = user,
                    Enabled = true,
                    PasswordNeverExpires = true
                };
                newUser.SetPassword(password);
                newUser.Save();

                Logger.Info($"User:'{user}' successfully created.");

                //---add to admin group if isAdmin=true---
                if (isAdmin)
                {
                    using GroupPrincipal? adminGroup = GroupPrincipal.FindByIdentity(context, "Administrators");
                    adminGroup?.Members.Add(newUser);
                    adminGroup?.Save();

                    Logger.Info($"User:'{user}' successfully added to administrator group.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error while creating User:'{user}': {ex.Message}");
            }
        }

        public static void ActivateLocalAdmin(string password)
        {
            using var context = new PrincipalContext(ContextType.Machine);

            // Well-known SID for local administrator: S-1-5-21-domain-500
            // Domain-ID is different on every system, therefor search by name
            // fallback on RID (500) if not found by name:
            var admin = UserPrincipal.FindByIdentity(context, IdentityType.Name, "Administrator");

            if (admin == null)
            {
                Logger.Warning(
                    "Standard Administrator Account could not be found by Name." +
                    "\n" +
                    "Fallback search for RID 500.");
                try
                {
                    // fallback search all users for RID 500
                    using var searcher = new PrincipalSearcher(new UserPrincipal(context));
                    admin = searcher.FindAll()
                        .OfType<UserPrincipal>()
                        .FirstOrDefault(u => u.Sid.Value.EndsWith("-500"));
                }
                catch (Exception ex)
                {
                    {
                        Logger.Error($"Error while searching for RID500: {ex}");
                        return;
                    }
                }
            }

            if (admin == null)
            {
                Logger.Error("Standard Administrator Account could not be found. Task skipped!");
                return;
            }

            if (password == null)
            {
                Logger.Error("No Password for Admin Account was specified. Task skipped!");
                return;
            }

            try
            {
                admin.Enabled = true;
                admin.SetPassword(password);
                admin.PasswordNeverExpires = true;
                admin.Save();
            }
            catch (Exception ex)
            {
                Logger.Error($"Error while setting Administrator Attributes: {ex}");
                return;
            }
        }
    }
}