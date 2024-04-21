using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoModel
{
    public class VideoUser: IdentityUser
    {
        public virtual ICollection<Video> Videos { get; set; } = new List<Video>();
    }
}
