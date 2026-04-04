namespace PoucneRozpravky.Review;

public class ContentReviewOptions
{
    public List<string> ForbiddenWords { get; set; } =
    [
        "násilie", "krv", "zabiť", "smrť", "vražda", "zabijem",
        "krvavý", "mŕtvy", "mŕtvola", "zomrieť", "zavraždiť"
    ];

    public List<string> FearWords { get; set; } =
    [
        "strašidelný", "hrozný", "desivý", "príšerný", "hrôza",
        "nočná mora", "démon", "príšera", "krvavý", "temný"
    ];

    public List<string> RequiredElements { get; set; } = ["poučenie", "morál"];

    public int MinCharacterMentions { get; set; } = 3;
}
