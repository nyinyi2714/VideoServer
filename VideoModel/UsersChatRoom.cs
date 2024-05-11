using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VideoModel;

[Keyless]
public partial class UsersChatRoom
{
    [StringLength(50)]
    public string Username { get; set; } = null!;

    public int ChatRoomId { get; set; }

    [ForeignKey("Username")]
    public virtual RegisteredUser UsernameNavigation { get; set; } = null!;
}
