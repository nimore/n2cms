﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using N2.Security.Items;
using N2.Definitions;
using N2.Details;
using N2.Integrity;
using N2.Persistence;

// review: (JH) Is ID property good enough for UserId? Is it better to use Guid.NewGuid()?
//              pro: it's unique, indexed;  con: autogenerated, value may change at export/import 

// review: (JH) is UserName as Name good enough solution?  Name value restrictions are enforced by Name.

// review: (JH) is null password local account secure in all situations 
//              (e.g. when switching back to old-style membership)? 

namespace N2.Security.AspNet.Identity
{
    /// <summary> Aspnet.Identity N2 user base class 
    /// <seealso cref="IdentityAccountManager"/>
    /// <seealso cref="http://msdn.microsoft.com/en-us/library/dn613291(v=vs.108).aspx">IUser interface</seealso>
    /// </summary>
    public abstract class ContentUser : User, IUser<int>
    {
        private readonly N2.Engine.Logger<ContentUser> logger;

        public override string Title { get { return base.Title ?? base.Name; } set { base.Title = value; } }

        #region IUser, see IUserStore

        /// <summary> Unique user identifier <seealso cref="IUser.Id"/></summary>
        public int Id { get { return base.ID; } set { base.ID = value; } }
    
        /// <summary> Returns query parameter to select user by userId </summary>
        public static Parameter UserIdQueryParameter(int userId) 
        {
            return Parameter.Equal("ID", userId).Detail(false);
        }

        /// <summary> Unique User name <seealso cref="Name"/></summary>
        /// <remarks> Warning: not all characters are valid for Name property. See ??? 
        /// </remarks>
        public string UserName { get { return base.Name; } set { base.Name = value; } }

        /// <summary> Returns query parameter to select user by userName </summary>
        public static Parameter UserNameQueryParameter(string userName)
        {
            return Parameter.Equal("Name", userName).Detail(false);
        }

        #endregion

        #region see IUserPasswordStore

        /// <summary> PasswordHash and Password are synonyms </summary>
        /// <remarks> 
        /// No other stored password forms are supported beyond password hashing, 
        /// e.g. encyrpted passwords are not supported. 
        /// Password hashing scheme is defined by PasswordStore.
        /// </remarks>
        public string PasswordHash { get { return base.Password; } set { base.Password = value; } }

        #endregion

        #region see IUserSecurityStampStore

        /// <summary> Represent snapshot of user credentials. Guid value, re-generated on user property change. </summary>
        public string SecurityStamp { get { return GetDetail<string>("SecurityStamp", null); } set { base.SetDetail<string>("SecurityStamp", value, null); } }

        #endregion

        #region see IUserLockoutStore

        /// <summary> Used to record when an attempt to access the user has failed. </summary>
        /// <remarks> Note: The functionallity is similar to (but different from) classic membership IsLockedOut, LastLockoutDate. </remarks>
        public DateTimeOffset LockedOutEndDate { get { return GetDetail<DateTimeOffset>("LockedOutEndDate", DateTimeOffset.MinValue); } set { base.SetDetail<DateTimeOffset>("LockedOutEndDate", value, DateTimeOffset.MinValue); } }

        /// <summary> Used to record when an attempt to access the user has failed. </summary>
        public int AccessFailedCount { get { return GetDetail<int>("AccessFailedCount", 0); } set { base.SetDetail<int>("AccessFailedCount", value, 0); } }

        /// <summary> Used to record whether user lockout is enabled. </summary>
        public bool LockoutEnabled { get { return GetDetail<bool>("LockoutEnabled", true); } set { base.SetDetail<bool>("LockoutEnabled", value, true); } }

        #endregion

        #region see IUserPhoneNumberStore

        /// <summary> User phone number </summary>
        public string PhoneNumber { get { return GetDetail<string>("PhoneNumber", null); } set { base.SetDetail<string>("PhoneNumber", value, null); } }

        /// <summary> Used to record when an attempt to access the user has failed. </summary>
        public bool IsPhoneNumberConfirmed { get { return GetDetail<bool>("IsPhoneNumberConfirmed", false); } set { base.SetDetail<bool>("IsPhoneNumberConfirmed", value, false); } }

        #endregion

        [EditableChildren("External logins", "Sources", 20)]
        public virtual IList<UserExternalLoginInfo> ExternalLogins
        {
            get
            {
                try
                {
                    var childItems = Children.WhereAccessible();
                    return childItems.Cast<UserExternalLoginInfo>();
                }
                catch (Exception ex)
                {
                    logger.Error("External login list", ex);
                    return new List<UserExternalLoginInfo>();
                }
            }
        }
    }
}