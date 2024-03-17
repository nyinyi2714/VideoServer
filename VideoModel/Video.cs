using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VideoModel;

[Table("Video")]
public partial class Video
{
    [Key]
    public int VideoId { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string Url { get; set; } = null!;

    public int Likes { get; set; }

    public byte[] Timestamp { get; set; } = null!;

    [StringLength(200)]
    [Unicode(false)]
    public string Description { get; set; } = null!;

    public int UserId { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Videos")]
    public virtual User User { get; set; } = null!;
}
