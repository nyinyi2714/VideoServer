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

    [InverseProperty("ChatRoom")]
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    [ForeignKey("ChatRoomId")]
    [InverseProperty("ChatRooms")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
