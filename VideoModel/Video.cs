using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VideoModel;

public partial class Video
{
    [Key]
    public int VideoId { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string Url { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime Timestamp { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Title { get; set; } = null!;

    [StringLength(200)]
    [Unicode(false)]
    public string Description { get; set; } = null!;

    public int UserId { get; set; }

    public int Views { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Videos")]
    public virtual User User { get; set; } = null!;
}
