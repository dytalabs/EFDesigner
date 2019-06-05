﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Core.Routing;
using Microsoft.Msagl.Layout.Incremental;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.Layout.MDS;
using Microsoft.Msagl.Prototype.Ranking;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Validation;
using Sawczyn.EFDesigner.EFModel.Extensions;

#pragma warning disable 1591

namespace Sawczyn.EFDesigner.EFModel
{
   [ValidationState(ValidationState.Enabled)]
   public partial class ModelRoot
   {
      public static readonly PluralizationService PluralizationService;

      public static Action ExecuteValidator { get; set; }

      public static readonly string[] IdentityBaseClasses = { "IdentityRole", "IdentityUser", "IdentityUserClaim", "IdentityUserLogin", "IdentityUserRole" };

      static ModelRoot()
      {
         try
         {
            PluralizationService = PluralizationService.CreateService(CultureInfo.CurrentCulture);
         }
         catch (NotImplementedException)
         {
            PluralizationService = null;
         }
      }

      public string FullName
      {
         get
         {
            return string.IsNullOrWhiteSpace(Namespace)
                      ? $"global::{EntityContainerName}"
                      : $"global::{Namespace}.{EntityContainerName}";
         }
      }

      [Obsolete("Use ModelRoot.Classes instead")]
      public LinkedElementCollection<ModelClass> Types
      {
         get
         {
            return Classes;
         }
      }

      #region Identity

      internal void SetIdentityKeyType(string keyType)
      {
         // set these no matter if we're IdentityDbContext or not
         // if not, they won't be used. But if (somehow) later this becomes IdentityDbContext, they'll be correct

         // Note that this is primarily for display purposes. We could simply look at ModelRoot to determine what the value should be,
         // but that would mean we'd have to change Type to a calculated property, and that's really messy.
         // Since Identity classes aren't user-editable, we're ok in assuming they'll be correct since this is the only way they can
         // be changed.

         SetKeyType("IdentityRole", "Id", keyType);
         SetKeyType("IdentityUser", "Id", keyType);
         SetKeyType("IdentityLogin", "UserId", keyType);
         SetKeyType("IdentityUserRole", "UserId", keyType);
         SetKeyType("IdentityUserRole", "RoleId", keyType);
         SetKeyType("IdentityUserClaim", "Id", keyType);
         SetKeyType("IdentityUserClaim", "UserId", keyType);

         void SetKeyType(string _className, string _attributeName, string _keyType)
         {
            ModelClass identityClass = Classes.Find(c => c.Name == _className);
            if (identityClass != null)
            {
               try
               {
                  identityClass.IsReadOnly = false;
                  ModelAttribute keyAttribute = identityClass.Attributes.Find(a => a.Name == _attributeName);
                  if (keyAttribute != null)
                     keyAttribute.Type = _keyType;
               }
               finally
               {
                  identityClass.IsReadOnly = true;
         
               }
            }
         }
      }

      public string IdentityNamespace
      {
         get
         {
            return EntityFrameworkVersion == EFVersion.EF6
                      ? "Microsoft.AspNet.Identity.EntityFramework"
                      : "Microsoft.AspNetCore.Identity.EntityFrameworkCore";
         }
      }

      internal void TargetIdentityAssociations()
      {
         if (!IsIdentityDbContext)
            return;

         ModelClass identityRole = Classes.Find(c => c.Name == "IdentityRole");
         ModelClass identityUserRole = Classes.Find(c => c.Name == "IdentityUserRole");
         ModelClass identityUser = Classes.Find(c => c.Name == "IdentityUser");
         ModelClass identityUserLogin = Classes.Find(c => c.Name == "IdentityUserLogin");
         ModelClass identityUserClaim = Classes.Find(c => c.Name == "IdentityUserClaim");

         Retarget(identityRole, identityUserRole, "Users");
         Retarget(identityUser, identityUserRole, "Roles");
         Retarget(identityUser, identityUserLogin, "Logins");
         Retarget(identityUser, identityUserClaim, "Claims");
      }

      private void Retarget(ModelClass source, ModelClass target, string propertyName)
      {
         UnidirectionalAssociation association = Store.ElementDirectory.AllElements.OfType<UnidirectionalAssociation>()
                                                      .FirstOrDefault(a => a.Source == source &&
                                                                           a.TargetPropertyName == propertyName);

         ModelClass actualTarget = target.MostDerivedClasses().SingleOrDefault();


         if (association != null && actualTarget != null && association.Target != actualTarget)
            association.Target = actualTarget;
      }

      #endregion

      internal sealed partial class LayoutAlgorithmPropertyHandler
      {
         private static readonly Dictionary<LayoutAlgorithm, LayoutAlgorithmSettings> settingsCache = new Dictionary<LayoutAlgorithm, LayoutAlgorithmSettings>();

         protected override void OnValueChanged(ModelRoot element, LayoutAlgorithm oldValue, LayoutAlgorithm newValue)
         {
            base.OnValueChanged(element, oldValue, newValue);

            if (!element.Store.InUndoRedoOrRollback)
            {
               // if this is the first time we've been here, cache what's alread there
               if (oldValue != LayoutAlgorithm.Default && !settingsCache.ContainsKey(oldValue) && element.LayoutAlgorithmSettings != null)
                  settingsCache[oldValue] = element.LayoutAlgorithmSettings;

               // use the prior settings for this layout type if available
               if (newValue != LayoutAlgorithm.Default && settingsCache.ContainsKey(newValue))
                  element.LayoutAlgorithmSettings = settingsCache[newValue];
               else
               {
                  // if not, set some defaults that make sense in our context
                  switch (newValue)
                  {
                     case LayoutAlgorithm.Default:
                        element.LayoutAlgorithmSettings = null;
                        return;

                     case LayoutAlgorithm.FastIncremental:
                        FastIncrementalLayoutSettings fastIncrementalLayoutSettings = new FastIncrementalLayoutSettings();
                        element.LayoutAlgorithmSettings = fastIncrementalLayoutSettings;

                        break;

                     case LayoutAlgorithm.MDS:
                        MdsLayoutSettings mdsLayoutSettings = new MdsLayoutSettings();
                        mdsLayoutSettings.ScaleX = 1;
                        mdsLayoutSettings.ScaleY = 1;
                        mdsLayoutSettings.AdjustScale = true;
                        mdsLayoutSettings.RemoveOverlaps = true;
                        element.LayoutAlgorithmSettings = mdsLayoutSettings;

                        break;

                     case LayoutAlgorithm.Ranking:
                        RankingLayoutSettings rankingLayoutSettings = new RankingLayoutSettings();
                        rankingLayoutSettings.ScaleX = 1;
                        rankingLayoutSettings.ScaleY = 1;
                        element.LayoutAlgorithmSettings = rankingLayoutSettings;

                        break;

                     case LayoutAlgorithm.Sugiyama:
                        SugiyamaLayoutSettings sugiyamaLayoutSettings = new SugiyamaLayoutSettings
                        {
                           LayerSeparation = 1,
                           MinNodeHeight = 1,
                           MinNodeWidth = 1
                        };

                        element.LayoutAlgorithmSettings = sugiyamaLayoutSettings;

                        break;
                  }

                  element.LayoutAlgorithmSettings.ClusterMargin = 1;
                  element.LayoutAlgorithmSettings.NodeSeparation = 1;
                  element.LayoutAlgorithmSettings.EdgeRoutingSettings.Padding = .3;
                  element.LayoutAlgorithmSettings.EdgeRoutingSettings.PolylinePadding = 1.5;
                  element.LayoutAlgorithmSettings.EdgeRoutingSettings.EdgeRoutingMode = EdgeRoutingMode.StraightLine;

                  settingsCache[newValue] = element.LayoutAlgorithmSettings;
               }
            }
         }
      }

      #region Valid types based on EF version

      public string[] SpatialTypes
      {
         get
         {
            return EntityFrameworkVersion == EFVersion.EF6 || GetEntityFrameworkPackageVersionNum() > 2.1
                      ? new[]
                        {
                           "Geography",
                           "GeographyCollection",
                           "GeographyLineString",
                           "GeographyMultiLineString",
                           "GeographyMultiPoint",
                           "GeographyMultiPolygon",
                           "GeographyPoint",
                           "GeographyPolygon",
                           "Geometry",
                           "GeometryCollection",
                           "GeometryLineString",
                           "GeometryMultiLineString",
                           "GeometryMultiPoint",
                           "GeometryMultiPolygon",
                           "GeometryPoint",
                           "GeometryPolygon"
                        }
                      : new string[0];
         }
      }

      public string[] ValidTypes
      {
         get
         {
            string[] validTypes = {
                                     "Binary",
                                     "Boolean",
                                     "Byte",
                                     "byte",
                                     "DateTime",
                                     "DateTimeOffset",
                                     "Decimal",
                                     "Double",
                                     "Guid",
                                     "Int16",
                                     "Int32",
                                     "Int64",
                                     "Single",
                                     "String",
                                     "Time"
                                  };

            return validTypes.Union(SpatialTypes).ToArray();
         }
      }

      public string[] ValidCLRTypes
      {
         get
         {
            string[] validClrTypes = {
                                        "Binary",
                                        "Boolean", "Boolean?", "Nullable<Boolean>",
                                        "Byte", "Byte?", "Nullable<Byte>",
                                        "DateTime", "DateTime?", "Nullable<DateTime>",
                                        "DateTimeOffset", "DateTimeOffset?", "Nullable<DateTimeOffset>",
                                        "DbGeography",
                                        "DbGeometry",
                                        "Decimal", "Decimal?", "Nullable<Decimal>",
                                        "Double", "Double?", "Nullable<Double>",
                                        "Guid", "Guid?", "Nullable<Guid>",
                                        "Int16", "Int16?", "Nullable<Int16>",
                                        "Int32", "Int32?", "Nullable<Int32>",
                                        "Int64", "Int64?", "Nullable<Int64>",
                                        "Single", "Single?", "Nullable<Single>",
                                        "String",
                                        "Time",
                                        "TimeSpan", "TimeSpan?", "Nullable<TimeSpan>",
                                        "bool", "bool?", "Nullable<bool>",
                                        "byte", "byte?", "Nullable<byte>",
                                        "byte[]",
                                        "decimal", "decimal?", "Nullable<decimal>",
                                        "double", "double?", "Nullable<double>",
                                        "int", "int?", "Nullable<int>",
                                        "long", "long?", "Nullable<long>",
                                        "short", "short?", "Nullable<short>",
                                        "string"
                                     };

            return validClrTypes.Union(SpatialTypes).ToArray();
         }
      }

      public string[] ValidIdentityAttributeTypes
      {
         get
         {
            List<string> baseResult = ValidIdentityTypeAttributesBaseList;

            baseResult.AddRange(Store.ElementDirectory
                                     .AllElements
                                     .OfType<ModelEnum>()
                                     .Where(e => baseResult.Contains(e.ValueType.ToString()))
                                     .Select(e => e.Name)
                                     .OrderBy(n => n));

            return baseResult.ToArray();
         }
      }

      internal static List<string> ValidIdentityTypeAttributesBaseList
      {
         get
         {
            return new List<string>
                   {
                      "Int16",
                      "Int32",
                      "Int64",
                      "Byte",
                      "String",
                      "Guid"
                   };
         }
      }

      public bool IsValidCLRType(string type)
      {
         return ValidCLRTypes.Contains(type);
      }

      #endregion

      #region Nuget

      public NuGetDisplay NuGetPackageVersion
      {
         get
         {
            return NuGetHelper.NuGetPackageDisplay.FirstOrDefault(x => x.EFVersion == EntityFrameworkVersion &&
                                                                           x.DisplayVersion == EntityFrameworkPackageVersion);
         }
      }

      /// <summary>
      /// DslPackage might set this to false depending on whether or not it can find the resources needed to load Nuget packages
      /// </summary>
      public static bool CanLoadNugetPackages { get; set; } = true;

      // ReSharper disable once UnusedMember.Global
      public double GetEntityFrameworkPackageVersionNum()
      {
         string[] parts = EntityFrameworkPackageVersion.Split('.');

         string resultString = parts.Length > 1
                                  ? $"{parts[0]}.{parts[1]}"
                                  : parts.FirstOrDefault();

         return double.TryParse(resultString, out double result)
                   ? result
                   : 0;
      }

      #endregion Nuget

      #region Validation methods

      [ValidationMethod(ValidationCategories.Open | ValidationCategories.Save | ValidationCategories.Menu)]
      // ReSharper disable once UnusedMember.Local
      private void ConnectionStringMustExist(ValidationContext context)
      {
         if (!Classes.Any() && !Enums.Any())
            return;

         if (string.IsNullOrEmpty(ConnectionString) && string.IsNullOrEmpty(ConnectionStringName))
            context.LogWarning("Model: Default connection string missing", "MRWConnectionString", this);

         if (string.IsNullOrEmpty(EntityContainerName))
            context.LogError("Model: Entity container needs a name", "MREContainerNameEmpty", this);
      }

      [ValidationMethod(ValidationCategories.Open | ValidationCategories.Save | ValidationCategories.Menu)]
      // ReSharper disable once UnusedMember.Local
      private void SummaryDescriptionIsEmpty(ValidationContext context)
      {
         if (string.IsNullOrWhiteSpace(Summary) && WarnOnMissingDocumentation)
            context.LogWarning("Model: Summary documentation missing", "AWMissingSummary", this);
      }

      #endregion Validation methods

      #region DatabaseSchema tracking property

      protected virtual void OnDatabaseSchemaChanged(string oldValue, string newValue)
      {
         TrackingHelper.UpdateTrackingCollectionProperty(Store,
                                                         Classes,
                                                         ModelClass.DatabaseSchemaDomainPropertyId,
                                                         ModelClass.IsDatabaseSchemaTrackingDomainPropertyId);
      }

      internal sealed partial class DatabaseSchemaPropertyHandler
      {
         protected override void OnValueChanged(ModelRoot element, string oldValue, string newValue)
         {
            base.OnValueChanged(element, oldValue, newValue);

            if (!element.Store.InUndoRedoOrRollback)
               element.OnDatabaseSchemaChanged(oldValue, newValue);
         }
      }

      #endregion DatabaseSchema tracking property

      #region Namespace tracking property

      protected virtual void OnNamespaceChanged(string oldValue, string newValue)
      {
         TrackingHelper.UpdateTrackingCollectionProperty(Store, Classes, ModelClass.NamespaceDomainPropertyId, ModelClass.IsNamespaceTrackingDomainPropertyId);
         TrackingHelper.UpdateTrackingCollectionProperty(Store, Enums, ModelEnum.NamespaceDomainPropertyId, ModelEnum.IsNamespaceTrackingDomainPropertyId);
      }

      internal sealed partial class NamespacePropertyHandler
      {
         protected override void OnValueChanged(ModelRoot element, string oldValue, string newValue)
         {
            base.OnValueChanged(element, oldValue, newValue);

            if (!element.Store.InUndoRedoOrRollback)
               element.OnNamespaceChanged(oldValue, newValue);
         }
      }

      #endregion Namespace tracking property

      #region DefaultCollectionClass tracking property

      protected virtual void OnCollectionClassChanged(string oldValue, string newValue)
      {
         TrackingHelper.UpdateTrackingCollectionProperty(Store,
                                                         Store.Get<Association>().ToList(),
                                                         Association.CollectionClassDomainPropertyId,
                                                         Association.IsCollectionClassTrackingDomainPropertyId);
      }

      internal sealed partial class DefaultCollectionClassPropertyHandler
      {
         protected override void OnValueChanged(ModelRoot element, string oldValue, string newValue)
         {
            base.OnValueChanged(element, oldValue, newValue);

            if (!element.Store.InUndoRedoOrRollback)
               element.OnCollectionClassChanged(oldValue, newValue);
         }
      }

      #endregion DefaultCollectionClass tracking property

   }
}
