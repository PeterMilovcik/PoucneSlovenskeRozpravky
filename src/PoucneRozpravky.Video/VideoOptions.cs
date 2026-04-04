namespace PoucneRozpravky.Video;

public class VideoOptions
{
    public string FfmpegPath { get; set; } = "ffmpeg";
    public string Resolution { get; set; } = "1920x1080";
    public double TransitionDurationSeconds { get; set; } = 1.0;
    public double TitleCardDurationSeconds { get; set; } = 5.0;
    public double EndCardDurationSeconds { get; set; } = 8.0;
    public string VideoCodec { get; set; } = "libx264";
    public string AudioCodec { get; set; } = "aac";
    public string PixelFormat { get; set; } = "yuv420p";
}
