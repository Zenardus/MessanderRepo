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
    
    public partial class GroupMessages
    {
        public int id { get; set; }
        public int groupID { get; set; }
        public string message { get; set; }
        public System.DateTime time { get; set; }
        public int fromID { get; set; }
    
        public virtual Groups Groups { get; set; }
        public virtual User User { get; set; }
    }
}
