using Microsoft.AspNetCore.Html;
using ProjectShihab.Entities;
using System.Net;

namespace ProjectShihab.Web.Helpers
{
    // Small HTML snippets shared by many views.
    public static class Ui
    {
        public static IHtmlContent Stars(double value)
        {
            var html = "<span class=\"stars\">";
            for (var i = 1; i <= 5; i++)
                html += $"<i class=\"bi bi-star-fill{(i <= Math.Round(value) ? " on" : "")}\"></i>";
            return new HtmlString(html + "</span>");
        }

        public static IHtmlContent Pill(string text, string tone) =>
            new HtmlString($"<span class=\"pill pill-{tone}\">{WebUtility.HtmlEncode(text)}</span>");

        public static IHtmlContent Status(string status) => status switch
        {
            EnrollmentStatus.Completed => Pill("Completed", "success"),
            EnrollmentStatus.Enrolled => Pill("In progress", "info"),
            _ => Pill(status, "muted")
        };

        public static IHtmlContent Open(bool open, DateTime end) =>
            !open ? Pill("Closed", "muted") : end < DateTime.Today ? Pill("Ended", "warning") : Pill("Open", "success");

        public static string Initials(string? name) =>
            string.Concat((name ?? "?").Replace("Dr. ", "").Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2).Select(p => char.ToUpper(p[0])));

        public static IHtmlContent Person(string? name, string? sub = null) =>
            new HtmlString($"<div class=\"person\"><span class=\"avatar\">{Initials(name)}</span><div><div class=\"cell-title\">{WebUtility.HtmlEncode(name)}</div>"
                           + (sub != null ? $"<div class=\"cell-sub\">{WebUtility.HtmlEncode(sub)}</div>" : "") + "</div></div>");

        public static string Money(decimal value) => "৳ " + value.ToString("n0");

        public static (string Icon, string Tone) FileIcon(string type) => type switch
        {
            "Video" => ("bi-play-btn", "tone-danger"),
            "PDF" => ("bi-file-earmark-pdf", "tone-danger"),
            "PPT" => ("bi-file-earmark-slides", "tone-warning"),
            "Archive" => ("bi-file-earmark-zip", "tone-info"),
            _ => ("bi-file-earmark-text", "tone-brand")
        };

        public static string Ago(DateTime when)
        {
            var span = DateTime.Now - when;
            if (span.TotalMinutes < 1) return "just now";
            if (span.TotalHours < 1) return $"{(int)span.TotalMinutes}m ago";
            if (span.TotalDays < 1) return $"{(int)span.TotalHours}h ago";
            if (span.TotalDays < 30) return $"{(int)span.TotalDays}d ago";
            return when.ToString("dd MMM yyyy");
        }
    }
}
