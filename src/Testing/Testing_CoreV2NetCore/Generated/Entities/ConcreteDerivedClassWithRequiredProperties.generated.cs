//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Testing_CoreV2NetCore
{
   public partial class ConcreteDerivedClassWithRequiredProperties : AbstractBaseClass
   {
      partial void Init();

      /// <summary>
      /// Default constructor. Protected due to required properties, but present because EF needs it.
      /// </summary>
      protected ConcreteDerivedClassWithRequiredProperties(): base()
      {
         Init();
      }

      /// <summary>
      /// Public constructor with required data
      /// </summary>
      /// <param name="_property1"></param>
      public ConcreteDerivedClassWithRequiredProperties(string _property1)
      {
         if (string.IsNullOrEmpty(_property1)) throw new ArgumentNullException(nameof(_property1));
         Property1 = _property1;
         Init();
      }

      /// <summary>
      /// Static create function (for use in LINQ queries, etc.)
      /// </summary>
      /// <param name="_property1"></param>
      public static ConcreteDerivedClassWithRequiredProperties Create(string _property1)
      {
         return new ConcreteDerivedClassWithRequiredProperties(_property1);
      }

      // Persistent properties

      /// <summary>
      /// Required
      /// </summary>
      [Required]
      public string Property1 { get; set; }

   }
}
