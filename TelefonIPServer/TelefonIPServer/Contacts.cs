//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TelefonIPServer
{
    using System;
    using System.Collections.Generic;
    
    public partial class Contacts
    {
        public int ContactID { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<int> ContactUserID { get; set; }
        public Nullable<int> ContactType { get; set; }
        public System.DateTime Timestamp { get; set; }
    
        public virtual Users Users { get; set; }
        public virtual ContactTypes ContactTypes { get; set; }
        public virtual Users Users1 { get; set; }
    }
}