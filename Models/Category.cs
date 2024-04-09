using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DevFlix.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DevFlix.Models;

public class Category
{
    public short Id { get; set; }
    [Column(TypeName = "nvarchar(50)")]
    [StringLength(50, MinimumLength = 2)]
    public string Name { get; set; } = "";
}