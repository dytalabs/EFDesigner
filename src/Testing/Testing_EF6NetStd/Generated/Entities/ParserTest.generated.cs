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
using System.Runtime.CompilerServices;
using System.Data.Entity.Spatial;

namespace Testing
{
   public partial class ParserTest
   {
      partial void Init();

      /// <summary>
      /// Default constructor. Protected due to required properties, but present because EF needs it.
      /// </summary>
      protected ParserTest()
      {
         Init();
      }

      /// <summary>
      /// Public constructor with required data
      /// </summary>
      /// <param name="foo"></param>
      public ParserTest(long foo)
      {
         this.foo = foo;
         Init();
      }

      /// <summary>
      /// Static create function (for use in LINQ queries, etc.)
      /// </summary>
      /// <param name="foo"></param>
      public static ParserTest Create(long foo)
      {
         return new ParserTest(foo);
      }

      /*************************************************************************
       * Persistent properties
       *************************************************************************/

      /// <summary>
      /// Identity, Required
      /// </summary>
      [Key]
      [Required]
      public int Id { get; set; }

      /// <summary>
      /// Required
      /// </summary>
      [Required]
      public long foo { get; set; }

      public string name1 { get; set; }

      public string name2 { get; protected set; }

      public int? name3 { get; set; }

      public int? name4 { get; protected set; }

      public int? name5 { get; set; }

      public int? name6 { get; protected set; }

      /// <summary>
      /// Max length = 6
      /// </summary>
      [MaxLength(6)]
      [StringLength(6)]
      public string name7 { get; set; }

      /// <summary>
      /// Max length = 6
      /// </summary>
      [MaxLength(6)]
      [StringLength(6)]
      public string name8 { get; protected set; }

      /// <summary>
      /// Max length = 6
      /// </summary>
      [MaxLength(6)]
      [StringLength(6)]
      public string name9 { get; set; }

      /// <summary>
      /// Max length = 6
      /// </summary>
      [MaxLength(6)]
      [StringLength(6)]
      public string name { get; protected set; }

      public int? name11 { get; set; }

      public int? name12 { get; protected set; }

      public int? name13 { get; set; }

      public int? name14 { get; protected set; }

      /// <summary>
      /// Max length = 6
      /// </summary>
      [MaxLength(6)]
      [StringLength(6)]
      public string name15 { get; set; }

      /// <summary>
      /// Max length = 6
      /// </summary>
      [MaxLength(6)]
      [StringLength(6)]
      public string name16 { get; protected set; }

      /// <summary>
      /// Max length = 6
      /// </summary>
      [MaxLength(6)]
      [StringLength(6)]
      public string name17 { get; set; }

      /// <summary>
      /// Max length = 6
      /// </summary>
      [MaxLength(6)]
      [StringLength(6)]
      public string name18 { get; protected set; }

   }
}

