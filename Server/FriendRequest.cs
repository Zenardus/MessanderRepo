//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Server
{
    using System;
    using System.Collections.Generic;
    
    public partial class FriendRequest
    {
        public int id { get; set; }
        public int fromID { get; set; }
        public int toID { get; set; }
    
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}
