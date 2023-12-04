﻿// <auto-generated />
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using TozawaNGO.Attachment.Models;

#pragma warning disable 219, 612, 618
#nullable disable

namespace TozawaNGO.MyCompiledModels
{
    internal partial class OwnerFileAttachmentEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "TozawaNGO.Attachment.Models.OwnerFileAttachment",
                typeof(OwnerFileAttachment),
                baseEntityType);

            var ownerId = runtimeEntityType.AddProperty(
                "OwnerId",
                typeof(Guid),
                propertyInfo: typeof(OwnerFileAttachment).GetProperty("OwnerId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OwnerFileAttachment).GetField("<OwnerId>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw);
            ownerId.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var fileAttachmentId = runtimeEntityType.AddProperty(
                "FileAttachmentId",
                typeof(Guid),
                propertyInfo: typeof(OwnerFileAttachment).GetProperty("FileAttachmentId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OwnerFileAttachment).GetField("<FileAttachmentId>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw);
            fileAttachmentId.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var key = runtimeEntityType.AddKey(
                new[] { ownerId, fileAttachmentId });
            runtimeEntityType.SetPrimaryKey(key);

            var index = runtimeEntityType.AddIndex(
                new[] { fileAttachmentId });

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("FileAttachmentId") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("Id") }),
                principalEntityType,
                deleteBehavior: DeleteBehavior.Restrict,
                required: true);

            var fileAttachment = declaringEntityType.AddNavigation("FileAttachment",
                runtimeForeignKey,
                onDependent: true,
                typeof(FileAttachment),
                propertyInfo: typeof(OwnerFileAttachment).GetProperty("FileAttachment", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OwnerFileAttachment).GetField("<FileAttachment>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var owners = principalEntityType.AddNavigation("Owners",
                runtimeForeignKey,
                onDependent: false,
                typeof(List<OwnerFileAttachment>),
                propertyInfo: typeof(FileAttachment).GetProperty("Owners", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(FileAttachment).GetField("<Owners>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", "Authorization");
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "OwnerFileAttachments");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
