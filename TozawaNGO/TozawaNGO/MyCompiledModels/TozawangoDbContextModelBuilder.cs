﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

#pragma warning disable 219, 612, 618
#nullable disable

namespace TozawaNGO.MyCompiledModels
{
    public partial class TozawangoDbContextModel
    {
        partial void Initialize()
        {
            var identityRole = IdentityRoleEntityType.Create(this);
            var identityRoleClaimstring = IdentityRoleClaimstringEntityType.Create(this);
            var identityUserClaimstring = IdentityUserClaimstringEntityType.Create(this);
            var identityUserLoginstring = IdentityUserLoginstringEntityType.Create(this);
            var identityUserRolestring = IdentityUserRolestringEntityType.Create(this);
            var identityUserTokenstring = IdentityUserTokenstringEntityType.Create(this);
            var applicationUser = ApplicationUserEntityType.Create(this);
            var partner = PartnerEntityType.Create(this);
            var userHashPwd = UserHashPwdEntityType.Create(this);
            var userLog = UserLogEntityType.Create(this);
            var audit = AuditEntityType.Create(this);

            IdentityRoleClaimstringEntityType.CreateForeignKey1(identityRoleClaimstring, identityRole);
            IdentityUserClaimstringEntityType.CreateForeignKey1(identityUserClaimstring, applicationUser);
            IdentityUserLoginstringEntityType.CreateForeignKey1(identityUserLoginstring, applicationUser);
            IdentityUserRolestringEntityType.CreateForeignKey1(identityUserRolestring, identityRole);
            IdentityUserRolestringEntityType.CreateForeignKey2(identityUserRolestring, applicationUser);
            IdentityUserTokenstringEntityType.CreateForeignKey1(identityUserTokenstring, applicationUser);
            ApplicationUserEntityType.CreateForeignKey1(applicationUser, partner);
            UserHashPwdEntityType.CreateForeignKey1(userHashPwd, applicationUser);

            IdentityRoleEntityType.CreateAnnotations(identityRole);
            IdentityRoleClaimstringEntityType.CreateAnnotations(identityRoleClaimstring);
            IdentityUserClaimstringEntityType.CreateAnnotations(identityUserClaimstring);
            IdentityUserLoginstringEntityType.CreateAnnotations(identityUserLoginstring);
            IdentityUserRolestringEntityType.CreateAnnotations(identityUserRolestring);
            IdentityUserTokenstringEntityType.CreateAnnotations(identityUserTokenstring);
            ApplicationUserEntityType.CreateAnnotations(applicationUser);
            PartnerEntityType.CreateAnnotations(partner);
            UserHashPwdEntityType.CreateAnnotations(userHashPwd);
            UserLogEntityType.CreateAnnotations(userLog);
            AuditEntityType.CreateAnnotations(audit);

            AddAnnotation("ProductVersion", "6.0.25");
            AddAnnotation("Relational:DefaultSchema", "Authorization");
            AddAnnotation("Relational:MaxIdentifierLength", 128);
            AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);
        }
    }
}
