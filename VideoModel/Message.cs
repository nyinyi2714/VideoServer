using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VideoModel;

public partial class Message
{
    [Key]
    public int MessageId { get; set; }

    public int SenderId { get; set; }

    [Unicode(false)]
    public string Text { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime Timestamp { get; set; }

    public int ChatRoomId { get; set; }

    [ForeignKey("ChatRoomId")]
    [InverseProperty("Messages")]
    public virtual ChatRoom ChatRoom { get; set; } = null!;

    [ForeignKey("SenderId")]
    [InverseProperty("Messages")]
    public virtual User Sender { get; set; } = null!;
}
