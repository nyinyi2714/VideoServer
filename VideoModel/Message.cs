using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VideoModel;

[Keyless]
public partial class Message
{
    public int MessageId { get; set; }

    [StringLength(50)]
    public string Username { get; set; } = null!;

    [Unicode(false)]
    public string Text { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime Timestamp { get; set; }

    public int ChatRoomId { get; set; }

    [ForeignKey("ChatRoomId")]
    public virtual ChatRoom ChatRoom { get; set; } = null!;
}
