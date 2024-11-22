using System;
using System.Collections.Generic;

namespace BLL.DAL;

public partial class BlogTag
{
    public int Id { get; set; }

    public int BlogId { get; set; }

    public int TagId { get; set; }

    public virtual Blog Blog { get; set; } = null!;

    public virtual Tag Tag { get; set; } = null!;
}
