using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using N2.Security;
using N2.Persistence;
using N2.Engine;

// review: (JH) N2 documentation should receive an introduction to ItemBridge User class customization,
//              with description how to upgrade existing user records 
//              after extending User class.

// review: (JH) N2.Security.ContentMembershipProvider includes additional logics (unique UserName, Email etc).
//              How the logics should be migrated to N2 Aspnet.Identity?
//              Should we attach logics on Items.User to assure logics inplace for all user data changes? 

namespace N2.Security.AspNet.Identity
{
    /// <summary>
    /// AspNet.Identity UserStore implemented on N2 ItemBridge
    /// <seealso cref="http://msdn.microsoft.com/en-us/library/microsoft.aspnet.identity(v=vs.108).aspx"> Microsoft.AspNet.Identity Namespace </seealso>
    /// </summary>
    /// <remarks>
    /// TUser user type is registered with Bridge by UserStore, overriding any N2 configuration parameters.
    /// 
    /// Note: IdentityUserStore implementation is designed to be thread safe,
    ///       therefore a service may serve all requests.
    /// </remarks>
    [Service]
    public class IdentityUserStore<TUser>
        : IUserStore<TUser, int>, // todo: IQueryableUserStore<TUser,int> 
          IUserPasswordStore<TUser, int>, IUserSecurityStampStore<TUser, int>, 
          IUserLoginStore<TUser, int>, IUserRoleStore<TUser, int>, // todo: IUserClaimStore<TUser, int>
          IUserEmailStore<TUser, int>, IUserLockoutStore<TUser, int>, IUserPhoneNumberStore<TUser,int>,
          IUserTwoFactorStore<TUser, int>
          where TUser : ContentUser
    {
        private readonly N2.Engine.Logger<IdentityUserStore<TUser>> logger;
        private static readonly Task CompletedTask = Task.FromResult(0);

        public IdentityUserStore(ItemBridge bridge)
        {
            if (bridge == null)
                throw new ArgumentNullException("ItemBridge");
            Bridge = bridge;
            Bridge.SetUserType(typeof(TUser));
        }

        public ItemBridge Bridge { get; private set; }

        #region IDisposable

        public void Dispose()
        { // Nothing to dispose: UserStore is pure functional class
        }

        #endregion

        #region IUserStore

        public virtual Task CreateAsync(TUser user)
        {			
            return Task.FromResult(Create(user));
        }

        public virtual Task DeleteAsync(TUser user)
        {
            return Task.FromResult(Delete(user));
        }

        public virtual Task<TUser> FindByIdAsync(int userId)
        {
            var userList = Bridge.GetUserContainer(false);
            if (userList == null)
            {
                throw new InvalidOperationException("The user list does not exist.");
            }

            // TUser: see review questions on upgrading old users
            return Task.FromResult(Bridge.Repository.Find(Parameter.Equal("Parent", userList), ContentUser.UserIdQueryParameter(userId))
                .Select(u => ToApplicationUser(u))
                .Where(u => (u != null))
                .SingleOrDefault());
        }

        public virtual Task<TUser> FindByNameAsync(string userName)
        {
            return Task.FromResult(FindByName(userName));
        }

        public virtual Task UpdateAsync(TUser user)
        {
            return Task.FromResult(Update(user));
        }

        #endregion

        #region IQueryableUserStore (todo)

        // TODO: public IQueryable<TUser> Users { get; }

        #endregion

        #region private: Users implemented on ItemBridge
        
        ////private static string[] UserDefaultRoles = { "Everyone", "Members", "Writers" }; // TODO: should defaults be stored somewhere else?

        internal TUser FindByName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("Value cannot be null or empty.", "userName");
            }

            ContentItem user = Bridge.GetUser(userName);

            return ToApplicationUser(user);
        }

        private bool Create(TUser user)
        {
            bool result = Update(user);
            ////if (result && (UserDefaultRoles.Length > 0))
            ////	AddUserToRoles(user, UserDefaultRoles);
            return result;
        }

        internal bool Update(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            var userList = Bridge.GetUserContainer(true);
            user.Parent = userList;
            Bridge.Save(user); 

            return true;
        }

        internal bool Delete(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            var u = Bridge.GetUser(user.UserName);
            if (u == null)
            {
                return false;
            }

            Bridge.Delete(user); 

            return true;
        }        

        private TUser ToApplicationUser(ContentItem user)
        {
            var userT = user as TUser;
            if ((user != null) && (userT == null))
            {
                logger.WarnFormat("Unexpected user type found. Null user returned! Found: {0}, Expected: {1}", user.GetType().AssemblyQualifiedName, typeof(TUser).AssemblyQualifiedName);
            }

            return userT; // will return null when not of required type (should be silently upgraded ?)
        }

        #endregion

        #region internal: Users (extended)

        public IEnumerable<TUser> GetUsers(int startIndex, int max)
        {
            return Bridge.GetUsers(startIndex, max).Select(u => ToApplicationUser(u)).Where(u => (u != null)); 
        }

        internal int GetUsersCount()
        {
            return Bridge.GetUsersCount();
        }

        #endregion

        #region IUserPasswordStore

        public virtual Task<string> GetPasswordHashAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.PasswordHash);
        }

        public virtual Task<bool> HasPasswordAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.PasswordHash != null);
        }

        public virtual Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.PasswordHash = passwordHash;

            return CompletedTask;
        }

        #endregion        

        #region IUserSecurityStampStore

        public virtual Task<string> GetSecurityStampAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.SecurityStamp);
        }

        public virtual Task SetSecurityStampAsync(TUser user, string stamp)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.SecurityStamp = stamp;

            return CompletedTask;
        }

        #endregion        

        #region IUserLoginStore

        public virtual Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            return Task.FromResult(Bridge.AddUserExternalLoginInfo(user, login.LoginProvider, login.ProviderKey));
        }

        public virtual Task<TUser> FindAsync(UserLoginInfo login)
        {
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            var loginItem = Bridge.FindUserExternalLoginInfo(login.LoginProvider, login.ProviderKey);			

            return Task.FromResult((loginItem != null ? loginItem.Parent as TUser : null));
        }

        public virtual Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            IList<UserLoginInfo> logins = Bridge.FindUserExternalLoginInfos(user)
                .Select(loginItem => new UserLoginInfo(loginItem.LoginProvider, loginItem.ProviderKey))
                .ToList();

            return Task.FromResult(logins);
        }

        public virtual Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            return Task.FromResult(Bridge.DeleteUserExternalLoginInfo(user, login.LoginProvider, login.ProviderKey));
        }

        #endregion        

        #region IUserRoleStore

        public virtual Task AddToRoleAsync(TUser user, string roleName)
        {
            AddToRole(user, roleName);
            return CompletedTask;
        }

        public virtual Task<IList<string>> GetRolesAsync(TUser user)
        {
            IList<string> roles = GetRoles(user);
            return Task.FromResult(roles);
        }

        public virtual Task<bool> IsInRoleAsync(TUser user, string roleName)
        {
            return Task.FromResult(IsInRole(user, roleName));
        }

        public virtual Task RemoveFromRoleAsync(TUser user, string roleName)
        {
            RemoveFromRole(user, roleName);
            return CompletedTask;            
        }

        #endregion

        #region private: UserRoles

        internal bool IsInRole(TUser user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("Value cannot be null or empty.", "role");
            }

            return user.Roles.Contains(roleName);
        }

        internal string[] GetRoles(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return user.GetRoles();
        }

        internal bool AddToRole(TUser user, string role)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrWhiteSpace(role))
            {
                throw new ArgumentException("Value cannot be null or empty.", "role");
            }

            var userList = Bridge.GetUserContainer(false);
            if (userList == null)
            {
                throw new InvalidOperationException("The user list does not exist.");
            }

            if (!userList.HasRole(role))
            {
                throw new InvalidOperationException(string.Format("The role {0} does not exist.", role));
            }

            if (!user.Roles.Contains(role))
            {
                user.Roles.Add(role);
                return true;
            }

            return false;
        }

        internal bool RemoveFromRole(TUser user, string role)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrWhiteSpace(role))
            {
                throw new ArgumentException("Value cannot be null or empty.", "role");
            }

            if (user.Roles.Contains(role))
            {
                user.Roles.Remove(role);
                return true;
            }

            return false;            
        }

        /// <summary> Exist one or more users in specified role? </summary>
        internal bool HasUsersInRole(string roleName)
        {
            return Bridge.HasUsersInRole(roleName);
        }

        /// <summary> Returns users (UserNames) in specified role </summary>
        internal string[] GetUsersInRole(string roleName)
        {
            return Bridge.GetUsersInRole(roleName, int.MaxValue);
        }        

        #endregion        

        #region IUserEmailStore

        /// <summary> Returns user associated with the email, null: not found </summary>
        public Task<TUser> FindByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Value cannor be null or empty.", "email");
            }

            return Task.FromResult(Bridge.FindUserByEmail(email) as TUser);
        }

        public Task<string> GetEmailAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            // review: IsApproved semantics is questionable. Should we introduce a new property IsEmailConfirmed? 
            return Task.FromResult(user.IsApproved); 
        }

        public Task SetEmailAsync(TUser user, string email)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.Email = email;

            return CompletedTask; 
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.IsApproved = confirmed;

            return CompletedTask; 
        }

        #endregion        

        #region IUserLockoutStore

        /// <summary> Returns whether the user can be locked out </summary>
        /// <remarks> It's a flag saying that user can (or can not) be locked in principle.
        ///  It is an opt-in flag for locking. 
        ///  To check if user is locked-out see UserManager.IsLockedOutAsync(user.Id). 
        /// </remarks>
        public Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            
            ////bool isEnabled;
            ////if (Bridge.IsAdmin(user.UserName))
            ////{
            ////	// admin cannot be locked out
            ////	isEnabled = false;
            ////}
            ////else
            ////{
            ////	isEnabled = user.LockoutEnabled;
            ////}

            ////return Task.FromResult(isEnabled);
            return Task.FromResult(user.LockoutEnabled);
        }

        /// <summary>  Sets whether the user can be locked out. </summary>
        public Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            ////if (!Bridge.IsAdmin(user.UserName))
            ////{
            ////	user.LockoutEnabled = enabled;
            ////}
            user.LockoutEnabled = enabled;

            return CompletedTask;
        }

        /// <summary> Returns the DateTimeOffset that represents the end of a user's lockout, any time in the past should be considered not locked out </summary>
        public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            // Note: The functionallity is similar to (but different from) classic membership IsLockedOut, LastLockoutDate
            return Task.FromResult(user.LockedOutEndDate);
        }

        /// <summary> Locks a user out until the specified end date (set to a past date, to unlock a user). </summary>
        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.LockedOutEndDate = new DateTimeOffset(DateTime.SpecifyKind(lockoutEnd.UtcDateTime, DateTimeKind.Utc));

            return CompletedTask;
        }

        /// <summary> Returns the current number of failed access attempts. This number usually will be reset whenever the password is verified or the account is locked out. </summary>
        public Task<int> GetAccessFailedCountAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.AccessFailedCount);
        }

        /// <summary> Incremented on an attempt to access the user has failed </summary>
        public Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.AccessFailedCount++;

            return Task.FromResult(user.AccessFailedCount);
        }

        /// <summary> Resets the access failed count, typically after the account is successfully accessed. </summary>
        public Task ResetAccessFailedCountAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.AccessFailedCount = 0;

            return CompletedTask;
        }

        #endregion

        #region IUserPhoneNumberStore

        public Task<string> GetPhoneNumberAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.IsPhoneNumberConfirmed);
        }

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.PhoneNumber = phoneNumber;

            return CompletedTask;            
        }

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.IsPhoneNumberConfirmed = confirmed;

            return CompletedTask;
        }

        #endregion        

        #region IUserTwoFactorStore
        
        public Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            return Task.FromResult(false);
        }

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
