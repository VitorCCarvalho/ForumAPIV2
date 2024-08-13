﻿using System.ComponentModel.DataAnnotations;

namespace ForumCRUD.API.Data.Dtos.Forum;

public class CreateForumDto
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }
}
