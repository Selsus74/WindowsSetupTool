using System.DirectoryServices.AccountManagement;
using WindowsSetupTool.Helpers;

namespace WindowsSetupTool.Setup
{
    public class UserSettings
    {
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
    }
}