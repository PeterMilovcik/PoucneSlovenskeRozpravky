using PoucneRozpravky.Catalog;
using PoucneRozpravky.Core.Models;

namespace PoucneRozpravky.Tests;

public class StatusTrackerTests
{
    [Theory]
    [InlineData(StoryStatus.OutlineDraft, StoryStatus.OutlineReady)]
    [InlineData(StoryStatus.OutlineReady, StoryStatus.TextGenerating)]
    [InlineData(StoryStatus.TextGenerating, StoryStatus.TextDraft)]
    [InlineData(StoryStatus.TextDraft, StoryStatus.GrammarChecked)]
    [InlineData(StoryStatus.GrammarChecked, StoryStatus.StyleChecked)]
    [InlineData(StoryStatus.StyleChecked, StoryStatus.ContentReviewed)]
    [InlineData(StoryStatus.ContentReviewed, StoryStatus.TextReady)]
    [InlineData(StoryStatus.TextReady, StoryStatus.AudioReady)]
    [InlineData(StoryStatus.AudioReady, StoryStatus.ImagesReady)]
    [InlineData(StoryStatus.ImagesReady, StoryStatus.VideoReady)]
    [InlineData(StoryStatus.VideoReady, StoryStatus.PublishedText)]
    [InlineData(StoryStatus.PublishedText, StoryStatus.PublishedAudio)]
    [InlineData(StoryStatus.PublishedAudio, StoryStatus.PublishedVideo)]
    [InlineData(StoryStatus.PublishedVideo, StoryStatus.FullyPublished)]
    public void ValidateTransition_ForwardStep_ReturnsTrue(StoryStatus current, StoryStatus next)
    {
        Assert.True(StatusTracker.ValidateTransition(current, next));
    }

    [Theory]
    [InlineData(StoryStatus.OutlineReady, StoryStatus.OutlineDraft)]
    [InlineData(StoryStatus.TextReady, StoryStatus.TextDraft)]
    [InlineData(StoryStatus.FullyPublished, StoryStatus.VideoReady)]
    [InlineData(StoryStatus.GrammarChecked, StoryStatus.OutlineDraft)]
    public void ValidateTransition_Backward_ReturnsFalse(StoryStatus current, StoryStatus previous)
    {
        Assert.False(StatusTracker.ValidateTransition(current, previous));
    }

    [Fact]
    public void ValidateTransition_SameStatus_ReturnsFalse()
    {
        Assert.False(StatusTracker.ValidateTransition(StoryStatus.TextDraft, StoryStatus.TextDraft));
    }

    [Theory]
    [InlineData(StoryStatus.TextDraft, StoryStatus.TextReady)]
    [InlineData(StoryStatus.OutlineDraft, StoryStatus.TextDraft)]
    [InlineData(StoryStatus.AudioReady, StoryStatus.FullyPublished)]
    public void ValidateTransition_SkipForward_ReturnsTrue(StoryStatus current, StoryStatus target)
    {
        Assert.True(StatusTracker.ValidateTransition(current, target));
    }

    [Theory]
    [InlineData(StoryStatus.OutlineDraft, StoryStatus.OutlineReady)]
    [InlineData(StoryStatus.TextDraft, StoryStatus.GrammarChecked)]
    [InlineData(StoryStatus.PublishedVideo, StoryStatus.FullyPublished)]
    public void GetNextStatus_ReturnsCorrectNextStep(StoryStatus current, StoryStatus expectedNext)
    {
        var next = StatusTracker.GetNextStatus(current);

        Assert.NotNull(next);
        Assert.Equal(expectedNext, next.Value);
    }

    [Fact]
    public void GetNextStatus_AtFullyPublished_ReturnsNull()
    {
        Assert.Null(StatusTracker.GetNextStatus(StoryStatus.FullyPublished));
    }

    [Theory]
    [InlineData(StoryStatus.OutlineDraft)]
    [InlineData(StoryStatus.TextReady)]
    [InlineData(StoryStatus.FullyPublished)]
    [InlineData(StoryStatus.AudioReady)]
    public void GetStatusDescription_ReturnsNonEmptySlovakDescription(StoryStatus status)
    {
        var description = StatusTracker.GetStatusDescription(status);

        Assert.False(string.IsNullOrWhiteSpace(description));
    }

    [Fact]
    public void GetStatusDescription_AllStatuses_HaveDescriptions()
    {
        foreach (var status in Enum.GetValues<StoryStatus>())
        {
            var description = StatusTracker.GetStatusDescription(status);
            Assert.False(string.IsNullOrWhiteSpace(description),
                $"Status {status} should have a non-empty description.");
        }
    }
}
