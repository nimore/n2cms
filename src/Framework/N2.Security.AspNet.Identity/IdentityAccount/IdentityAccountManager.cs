﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using N2.Engine;
using N2.Plugin;
using N2.Security;

namespace N2.Security.AspNet.Identity
{
    /// <summary>
    /// User accounts management, based on Aspnet.Identity (N2 Management UI)
    /// 
    /// <seealso cref="ContentUser"/>
    /// <seealso cref="UserStore"/>
    /// <seealso cref="IdentityRoleStore"/>
    /// <seealso cref="IdentityAccountResources"/>
    /// </summary>
    /// <remarks>
    /// <h2> How to register Aspnet.Identity to be used by N2 Management UI? </h2>
    /// <em>N2 Management UI are ASP.NET pages used by N2 backoffice to manage accounts. </em>
    /// <para>
    /// Two approaches are supported:
    /// <ul>
    ///  <li>Use existing N2 Management UI <br/>
    ///    <em>N2 Management UI is based on <see cref="MembershipAccountManager"/> service to manage accounts.
    ///        Service may be replaced by Aspnet.Itentity based service as described below.
    ///    </em>
    ///  </li>
    ///  <li>Replace N2 Management UI with custom implementation of account management pages <br/>
    ///    <em>Account management pages, used by N2 backoffice, are defined by <see cref="MembershipAccountResources"/> service. 
    ///        You may replace default <see cref="AspNetAccountResources"/> Aspnet.Identity account management pages
    ///        with your own definition.
    ///    </em>
    ///  </li>
    /// </ul>
    /// </para>
    /// 
    /// <h2>What an Aspnet.Identity N2 application should define</h2>
    /// <em>To use existing N2 Management account pages, application should define account manager to be used by the pages.
    ///     See also <see cref="AspNetAccountResources"/> resource registration.
    /// </em>
    /// <ul>
    ///  <li>TUser implementation, e.g. 
    /// <![CDATA[
    ///      /// <summary> Describes application user, local and external accounts and roles </summary>
    ///      /// <remarks> You can add profile data for the user by adding more properties
    ///      ///           to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    ///      ///           Some limitations apply:
    ///      ///             ApplicationUser is N2 ContentItem, therefore properties should be N2 compatible,
    ///      ///             optionally decorated with Editable attributes for better representation in N2 backend.
    ///      ///             See UserStore for additional info and limitations. 
    ///      /// </remarks>    
    ///      [PageDefinition("ApplicationUser", IconClass = "n2-icon-user")]
    ///      [RestrictParents(typeof(N2.Security.Items.UserList))]
    ///      [Throwable(AllowInTrash.No)]
    ///      [Versionable(AllowVersions.No)]
    ///      ApplicationUser : ContentUser 
    ///      {
    ///      } 
    /// ]]>
    ///  </li>
    ///  <li>AccountManager service, e.g.
    /// <![CDATA[
    ///      [Service(typeof(AccountManager), Replaces = typeof(MembershipAccountManager))]
    ///      public class ApplicationAccountManager : IdentityAccountManager<ApplicationUser> 
    ///      {
    ///        public ApplicationAccountManager(IdentityUserStore<ApplicationUser> userStore, IdentityRoleStore roleStore)
    ///          : base(userStore,roleStore)
    ///        {
    ///        }
    ///      }
    /// ]]>
    ///  </li>
    /// </ul>  
    /// </remarks>
    public abstract class IdentityAccountManager<TUser> : AccountManager where TUser : ContentUser
	{
        public IdentityAccountManager(
            IdentityUserStore<TUser> userStore, IdentityUserManager<TUser> userManager,
            IdentityRoleStore roleStore, IdentityRoleManager roleManager)
		{
            UserStore = userStore; UserManager = userManager;
            RoleStore = roleStore; RoleManager = roleManager;
		}

        protected IdentityUserStore<TUser> UserStore   { get; private set; }
        protected UserManager<TUser,int>   UserManager { get; private set; }
        protected IdentityRoleStore        RoleStore   { get; private set; }
        protected RoleManager<IRole>       RoleManager { get; private set; }
       
        // Note: (JH) temporary solution is to minimize NHibernate async problems 
        //            using UserStore methods instead of UserManager async methods 

        public override ManagerType AccountType { get { return ManagerType.ASPNET_IDENTITY; } }

        #region Users

        public override IAccountInfo FindUserByName(string userName)
        {            			
            TUser user = UserStore.FindByName(userName);

            return ToAccountInfo(user);
        }

        public override IList<IAccountInfo> GetUsers(int startIndex, int max)
        {
            return UserStore.GetUsers(startIndex, max).Select(u => ToAccountInfo(u)).OfType<IAccountInfo>().ToList();
        }

        public override int GetUsersCount()
        {
            return UserStore.GetUsersCount();
        }

        public override void UpdateUserEmail(string userName, string email)
        {            
            TUser user = UserStore.FindByName(userName);
			if (user == null)
			{
				throw new ArgumentException(string.Format("Unknown user {0}.", userName));
			}
            
            UserStore.SetEmailAsync(user, email).Wait();
			UserStore.SetEmailConfirmedAsync(user, false).Wait();
			UserStore.SetSecurityStampAsync(user, Guid.NewGuid().ToString());

			UserStore.Update(user);
        }

        public override void UnlockUser(string userName)
        {
			TUser user = UserStore.FindByName(userName);
			if (user == null)
			{
				return;
			}

			var lockoutEnabled = UserStore.GetLockoutEnabledAsync(user).Result;
			if (!lockoutEnabled)
			{
				throw new InvalidOperationException("Lockout is not enabled for this user.");
			}

			user.IsLockedOut = false;

			UserStore.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddSeconds(-1)).Wait();
			UserStore.ResetAccessFailedCountAsync(user).Wait();
			UserStore.Update(user);
        }

        public override bool ChangePassword(string userName, string newPassword)
        {
			TUser user = UserStore.FindByName(userName);
			if (user == null)
			{
				return false;
			}

			var validationResult = UserManager.PasswordValidator.ValidateAsync(newPassword).Result;
			if (!validationResult.Succeeded)
			{
				throw new ArgumentException("The password is not valid: " + string.Join("; ", validationResult.Errors));
			}

            String newHashed = UserManager.PasswordHasher.HashPassword(newPassword);
            UserStore.SetPasswordHashAsync(user, newHashed).Wait();  // assuring password is changed before leaving the method
			UserStore.Update(user);

            return true;
        }

        public override void DeleteUser(string userName)
        {
			TUser user = UserStore.FindByName(userName);
			if (user == null)
			{
				return;
			}

            UserStore.Delete(user);
        }

        protected AccountInfo ToAccountInfo(ContentUser muser)
        {
			if (muser == null)
			{
				return null;
			}

            return new AccountInfo()
            {
                UserName = muser.UserName,
                Email = muser.Email,
                CreationDate = muser.Created,
                Comment = muser.Comment,
                IsOnline = muser.IsOnline,
                LastLoginDate = muser.LastLoginDate,
                IsLockedOut = muser.IsLockedOut,
                LastLockoutDate = muser.LastLockoutDate.HasValue ? muser.LastLockoutDate.Value : DateTime.MinValue,
                IsApproved = muser.IsApproved
            };
        }

        #endregion

        #region Roles

        public override bool AreRolesEnabled()
        {
            return true;
        }

        /// <summary> Returns all known roles </summary>
        public override string[] GetAllRoles()
        {
            return RoleStore.GetAllRoleNames();
        }

        /// <summary> Add role to list of known roles <seealso cref="GetAllRoles"/></summary>
        public override void CreateRole(string roleName)
        {
            RoleStore.Create(roleName);
        }

        /// <summary> Delete role from list of known roles <seealso cref="GetAllRoles"/> </summary>
        public override bool DeleteRole(string roleName)
        {
            return RoleStore.Delete(roleName);
        }

        #endregion

        #region User roles

        /// <summary> Exist one or more users in specified role? </summary>
        public override bool HasUsersInRole(string roleName)
        {
            return UserStore.HasUsersInRole(roleName);
        }

        /// <summary> Returns users (UserNames) in specified role </summary>
        public override string[] GetUsersInRole(string roleName)
        {
            return UserStore.GetUsersInRole(roleName);
        }

        /// <summary> Is specified user in specified role? </summary>
        public override bool IsUserInRole(string userName, string roleName)
        {
			TUser user = UserStore.FindByName(userName);
			if (user == null)
			{
				throw new ArgumentException(string.Format("Unknown user {0}.", userName));
			}
            
            return UserStore.IsInRoleAsync(user, roleName).Result;
        }

        public override string[] GetRolesForUser(string userName)
        {
			TUser user = UserStore.FindByName(userName);
			if (user == null)
			{
				throw new ArgumentException(string.Format("Unknown user {0}.", userName));
			}

            return UserStore.GetRoles(user);
        }

        /// <summary> Add specified user in a role </summary>
        public override void AddUserToRole(string userName, string roleName)
        {            
			TUser user = UserStore.FindByName(userName);
			if (user == null)
			{
				throw new ArgumentException(string.Format("Unknown user {0}.", userName));
			}

            UserStore.AddToRole(user, roleName);
			UserStore.Update(user);
        }

        /// <summary> Removes specified user from role </summary>
        public override void RemoveUserFromRole(string userName, string roleName)
        {
			TUser user = UserStore.FindByName(userName);
			if (user == null)
			{
				throw new ArgumentException(string.Format("Unknown user {0}.", userName));
			}
            
            UserStore.RemoveFromRole(user, roleName);
			UserStore.Update(user);
        }

        #endregion
    }
}
