using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VideoModel;

public partial class ChatRoom
{
    [Key]
    public int ChatRoomId { get; set; }

    [ForeignKey("ChatRoomId")]
    [InverseProperty("ChatRooms")]
    public virtual ICollection<RegisteredUser> RegisteredUsers { get; set; } = new List<RegisteredUser>();
}
