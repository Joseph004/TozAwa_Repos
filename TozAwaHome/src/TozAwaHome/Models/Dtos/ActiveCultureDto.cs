using System;
using System.Linq;

namespace TozAwaHome.Models.Dtos
{
public class ActiveCultureDto
{
    public Guid Id { get; init; }
    public string ShortName { get; set; } = "";
    public string Name { get; set; } = "";
    public string LongName { get; set; } = "";
    public bool IsDefault { get; set; }
    public string ToCookieString => Id + "|" + ShortName + "|" + Name + "|" + LongName;

    public ActiveCultureDto()
    {

    }

    public ActiveCultureDto(string cookieString)
    {
        var splitted = cookieString.Split("|");
        if (splitted.Any() && splitted.Length == 4)
        {
            Id = Guid.Parse(splitted[0]);
            ShortName = splitted[1];
            Name = splitted[2];
            LongName = splitted[3];
        }
    }
}

}

