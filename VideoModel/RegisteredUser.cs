using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VideoModel;

public partial class RegisteredUser
{
    [Key]
    [StringLength(50)]
    public string Username { get; set; } = null!;

    [InverseProperty("RegisteredUser")]
    public virtual ICollection<Video> Videos { get; set; } = new List<Video>();
}
